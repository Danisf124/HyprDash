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
                            layout["Content"].Update(UiBuilder.BuildTodoTable(todoList));
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

                                var title = await GetTitle(layout, ctx);

                                todoList.CreateNewTodo(title);

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

                                int id = await GetId(layout, ctx, existingIds);

                                todoList.DeleteTodo(id);

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
        
            // functions

            static async Task<string> GetTitle(Layout layout, LiveDisplayContext ctx)
            {
                bool isTyping = true;

                string userInput = String.Empty;

                var inputPanel = new Panel($"[yellow]Введіть назву:[/] {Markup.Escape(userInput)}[blink]_[/]");

                while(isTyping)
                {
                    inputPanel = new Panel($"[yellow]Введіть назву:[/] {Markup.Escape(userInput)}[blink]_[/]");
                    layout["Content"].Update(inputPanel).Size(15);
                    ctx.Refresh();
                    while(Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey(intercept: true);

                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            isTyping = false; // Виходимо з головного циклу
                            break;
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace)
                        {
                            if (userInput.Length > 0)
                            {
                                userInput = userInput.Substring(0, userInput.Length - 1);
                            }
                        }
                        // Додаємо символ, якщо він не є системним (наприклад, стрілки чи F1)
                        else if (!char.IsControl(keyInfo.KeyChar))
                        {
                            userInput += keyInfo.KeyChar;
                        }

                        await Task.Delay(30);
                    }
                    
                }

                var finalPanel = new Panel($"[yellow]Введіть назву:[/] [green]{Markup.Escape(userInput)}[/]");
                layout["Content"].Update(finalPanel);
                ctx.Refresh();

                return userInput;
            }   

            static async Task<int> GetId(Layout layout, LiveDisplayContext ctx, List<int> validIds)
            {
                bool isTyping = true;
                string userInput = String.Empty;
                string errorMessage = String.Empty; // Стан для виводу повідомлень про помилку
                var panelText = $"[yellow]Введіть id:[/] {Markup.Escape(userInput)}[blink]_[/]";

                while(isTyping)
                {
                    // Формуємо текст панелі. Якщо є помилка — додаємо її червоним кольором
                    panelText = $"[yellow]Введіть id:[/] {Markup.Escape(userInput)}[blink]_[/]";
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        panelText += $"\n[red]{errorMessage}[/]";
                    }

                    var inputPanel = new Panel(panelText);
                    layout["Content"].Update(inputPanel).Size(15);
                    ctx.Refresh();

                    while(Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey(intercept: true);

                        // Якщо користувач почав щось виправляти, прибираємо помилку
                        if (errorMessage != String.Empty) 
                        {
                            errorMessage = String.Empty;
                        }

                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            // Перевіряємо, чи це коректне число і чи введено хоч щось
                            if (int.TryParse(userInput, out int parsedId))
                            {
                                // Перевіряємо, чи є такий ID у нашому списку з бази
                                if (validIds.Contains(parsedId))
                                {
                                    isTyping = false; // Усе добре, виходимо
                                    break;
                                }
                                else
                                {
                                    errorMessage = $"Завдання з ID {parsedId} не існує!";
                                    userInput = String.Empty; // Скидаємо ввід, щоб користувач спробував знову
                                }
                            }
                            else
                            {
                                errorMessage = "Ввід не може бути порожнім!";
                                userInput = String.Empty;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace)
                        {
                            if (userInput.Length > 0)
                            {
                                userInput = userInput.Substring(0, userInput.Length - 1);
                            }
                        }
                        // ЗАМІНА: Замість !char.IsControl використовуємо char.IsDigit
                        else if (char.IsDigit(keyInfo.KeyChar)) 
                        {
                            // Захист від переповнення типу int (максимум 9 символів)
                            if (userInput.Length < 9) 
                            {
                                userInput += keyInfo.KeyChar;
                            }
                        }
                    }
                    
                    await Task.Delay(30);
                }

                var finalPanel = new Panel($"[yellow]Введіть id:[/] [green]{Markup.Escape(userInput)}[/]");
                layout["Content"].Update(finalPanel);
                ctx.Refresh();

                // Тут Parse вже безпечний, бо ми перевірили його через TryParse під час вводу
                return int.Parse(userInput);
            }

        }
    }
}

