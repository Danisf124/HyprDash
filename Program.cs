using System.Drawing;
using System.Globalization;
using System.Numerics;
using Spectre.Console;



namespace HyprDash
{
    internal class Program
    {
           
        enum ScreenType
        {
            WeatherForDay, // Welcome, Weather for day
            WeatherForWeek, // Weather for week
            TodoList
        }

        

        static async Task Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var cache = new CachedWeatherService(); // Weather service
            var culture = new CultureInfo("uk-UA"); // for Ukraine region
            var todoList = new TodoList();
            var uiBuilder = new UiBuilder();
            //var uiUserInput = new UiUserInput();

            todoList.GetAllTodosFromDB();
            List<int> existingIds = todoList.TodoLists.Select(t => t.Id).ToList();
           
            DateTime currentDateTime = DateTime.Now;
            await cache.RefreshIfNeed();
            
            // Welcome panel and date&time panel
            var welcomePanel = new Panel(
                    new Rows(
                        new Markup($"Привіт [blue]Danisf[/] :3"),
                        new Markup($"Зараз: {currentDateTime.ToString("d MMMM yyyy, dddd, HH:mm", culture)}")))
                        .Border(BoxBorder.Rounded);

            var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header").Size(5),
                new Layout("Content"),
                new Layout("Footer").Size(6)
                .SplitRows(
                    new Layout("TopFooter").Size(1),
                    new Layout("BottomFooter").Size(2)
                )
            );

            await AnsiConsole.Live(layout)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .StartAsync(async ctx =>
                {
                    bool isRunning = true;
                    ScreenType currentScreen = ScreenType.WeatherForDay;

                   
                    while(isRunning)
                    {
                        //Updating data
                        await cache.RefreshIfNeed();
                        currentDateTime = DateTime.Now;                        

                        layout["Header"].Update(UiBuilder.BuildWelcomePanel(currentDateTime, culture));

                        layout["TopFooter"].Update(new Panel(new Markup("[grey]Використовуйте [yellow]стрілки ← →[/] для перемикання екранів | [red]ESC[/] для виходу[/]")).Border(BoxBorder.None));
                        
                        layout["BottomFooter"].Update(new Text("")); // Empty

                        // Chancing screens

                        if(currentScreen == ScreenType.WeatherForDay)
                        {
                            layout["Content"].Update(UiBuilder.BuildWeatherForDay(cache.Day)).Size(29);
                            
                        }    
                        else if(currentScreen == ScreenType.WeatherForWeek)
                        {
                            layout["Content"].Update(UiBuilder.BuildWeatherForWeek(cache.Week)).Size(15);
                        }
                        else if(currentScreen == ScreenType.TodoList)
                        {
                            layout["Content"].Update(UiBuilder.BuildTodoTable(todoList, culture));
                            layout["BottomFooter"].Update(new Panel(new Markup("[yellow]A[/] - Щоб додати нове завдання, C - виконати, D - Видалити, F - очистити;")).Border(BoxBorder.None));
                        }

                        //ctx.Refresh();

                        if(Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(intercept: true).Key;

                            if(key == ConsoleKey.RightArrow)
                            {
                                currentScreen = (ScreenType)(((int)currentScreen + 1) % Enum.GetNames(typeof(ScreenType)).Length);
                            }
                            else if(key == ConsoleKey.LeftArrow)
                            {
                                int totalScreens = Enum.GetNames(typeof(ScreenType)).Length;
                                currentScreen = (ScreenType)(((int)currentScreen - 1 + totalScreens) % totalScreens);
                            }
                            else if(key == ConsoleKey.Escape)
                            {
                                // Exit
                                isRunning = false;
                            }
                            else if(currentScreen == ScreenType.TodoList && key == ConsoleKey.A)
                            {  
                                // Clearing layout
                                layout["Content"].Update(new Text("")).Size(1);
                                layout["TopFooter"].Update(new Text("")).Size(1);
                                layout["BottomFooter"].Update(new Text("")).Size(1);

                                var title = await UiUserInput.GetTitle(layout, ctx);

                                if(title == "")
                                {
                                    continue;
                                }
                                else
                                {
                                    todoList.CreateNewTodo(title);
                                }

                                // Updating data
                                todoList.GetAllTodosFromDB(); 
                                existingIds = todoList.TodoLists.Select(t => t.Id).ToList();
                            }
                            else if(currentScreen == ScreenType.TodoList && key == ConsoleKey.C)
                            {
                                layout["TopFooter"].Update(new Text("")).Size(1);
                                layout["BottomFooter"].Update(new Text("")).Size(1);

                                int id = await UiUserInput.GetId(layout, ctx, existingIds);

                                todoList.CompleteTodo(id);

                                // Updating data
                                todoList.GetAllTodosFromDB(); 
                                existingIds = todoList.TodoLists.Select(t => t.Id).ToList();
                            }
                            else if(currentScreen == ScreenType.TodoList && key == ConsoleKey.D)
                            {
                                layout["TopFooter"].Update(new Text("")).Size(1);
                                layout["BottomFooter"].Update(new Text("")).Size(1);

                                int id = await UiUserInput.GetId(layout, ctx, existingIds);

                                if(id == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    todoList.DeleteTodo(id);
                                }

                                // Updating data
                                todoList.GetAllTodosFromDB(); 
                                existingIds = todoList.TodoLists.Select(t => t.Id).ToList();
                            }
                            else if(currentScreen == ScreenType.TodoList && key == ConsoleKey.F)
                            {

                                var confirm = await UiUserInput.GetConfirm(layout, ctx);   
                                
                                if(confirm == "y")
                                {
                                    todoList.ClearAllTodo();
                                }
                                else
                                {
                                    layout["BottomFooter"].Update(new Text("Всі завдання видалені"));
                                }
                                // Updating data
                                todoList.GetAllTodosFromDB(); 
                                existingIds = todoList.TodoLists.Select(t => t.Id).ToList();
                            }
                            
                        }

                        ctx.Refresh();

                        await Task.Delay(100);
                    }
                });
        
        }

    }
}

