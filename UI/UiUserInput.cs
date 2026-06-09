using Spectre.Console;

namespace HyprDash
{
    public class UiUserInput
    {
        static public async Task<string> GetTitle(Layout layout, LiveDisplayContext ctx)
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
                    else if(keyInfo.Key == ConsoleKey.Escape)
                    {
                        isTyping = false;
                    }
    
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
    
        static public async Task<int> GetId(Layout layout, LiveDisplayContext ctx, List<int> validIds)
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
                    else if(keyInfo.Key == ConsoleKey.Escape)
                    {
                        isTyping = false;
                        return 0;
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
    
        static public async Task<string> GetConfirm(Layout layout, LiveDisplayContext ctx)
        {
            bool isTyping = true;

            string userInput = String.Empty;

            string errorMessage = String.Empty;

            string planeText = $"[red]Ви дійсно хочете видалити всі завдання?[/]y/n: {Markup.Escape(userInput)}[blink]_[/]";

            while(isTyping)
            {
                planeText = $"[red]Ви дійсно хочете видалити всі завдання?[/]y/n: {Markup.Escape(userInput)}[blink]_[/]";

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    planeText += $"\n[red]{errorMessage}[/]";
                }

                var inputPanel = new Panel(planeText);
                layout["Content"].Update(inputPanel).Size(15);
                ctx.Refresh();

                while(Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(intercept: true);

                    if(errorMessage != String.Empty) 
                    {
                        errorMessage = String.Empty;
                    }

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        if(userInput != "y" && userInput != "n")
                        {
                            errorMessage = "Можна ввести тільки y або n ";
                            userInput = string.Empty;
                        }
                        else
                        {
                            isTyping = false;
                            break;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (userInput.Length > 0)
                        {
                            userInput = userInput.Substring(0, userInput.Length - 1);
                        }
                    }
                    else if(keyInfo.Key == ConsoleKey.Escape)
                    {
                        isTyping = false;
                        
                    }

                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        userInput += keyInfo.KeyChar;
                    }

                }

                await Task.Delay(30);
            }
            var finalPanel = new Panel($"[red]Ви дійсно хочете видалити всі завдання?[/]y/n:");
            layout["Content"].Update(finalPanel);
            ctx.Refresh();

            // Тут Parse вже безпечний, бо ми перевірили його через TryParse під час вводу
            return userInput;
        }
    }

}