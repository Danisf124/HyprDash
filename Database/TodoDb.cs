using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace HyprDash
{
    public class TodoDb
    {
        private readonly SqliteConnection _connection;

        public TodoDb(string datasource = "Data Source=todo.db")
        {
            _connection = new SqliteConnection(datasource);
            _connection.Open();
            InitDb();
            _connection.Close();
        }

        private void InitDb()
        {
                        
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TodoLists (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL
                );";
    
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = createTableQuery;
                command.ExecuteNonQuery();
            }
            
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        public void AddTodo(TodoList todo)
        {
            _connection.Open();
            // SQL-запит на додавання. 
            // @Title, @IsCompleted, @CreatedAt — це параметри (заглушки), які ми заповнимо нижче.
            // SELECT last_insert_rowid(); одразу повертає Id, який база щойно згенерувала.
            string insertQuery = @"
                INSERT INTO TodoLists (Title, IsCompleted, CreatedAt) 
                VALUES (@Title, @IsCompleted, @CreatedAt);
                SELECT last_insert_rowid();";

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = insertQuery;
                
                // Підставляємо значення з нашого об'єкта у параметри
                command.Parameters.AddWithValue("@Title", todo.Title);
                
                // Конвертуємо bool у int (true = 1, false = 0)
                command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
                
                // Форматуємо дату у стандартний текстовий формат (ISO 8601)
                command.Parameters.AddWithValue("@CreatedAt", todo.CreatedAt.ToString("O"));

                // ExecuteScalar виконує запит і повертає перше значення першого рядка (наш згенерований Id)
                long newId = (long)command.ExecuteScalar()!;
                
                // Оновлюємо Id у нашому C# об'єкті, щоб він відповідав базі!
                todo.Id = (int)newId;
            }

            _connection.Close();
        }


        public List<TodoList> GetAllTodos()
        {
            var todos = new List<TodoList>();

            _connection.Open();

            string selectQuery = "SELECT Id, Title, IsCompleted, CreatedAt FROM TodoLists;";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = selectQuery;

                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var todo = new TodoList();

                        todo.Id = reader.GetInt32(0);

                        todo.Title = reader.GetString(1);

                        todo.IsCompleted = reader.GetInt32(2) == 1;

                        todo.CreatedAt = DateTime.Parse(reader.GetString(3));

                        todos.Add(todo);
                    }
                }
            }
            
            _connection.Close();
    
            return todos;
        }

    }
}