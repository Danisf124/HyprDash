using Microsoft.Data.Sqlite;
using System;

namespace HyprDash
{
    public class TodoDb
    {
        private readonly SqliteConnection _connection;

        public TodoDb(string datasource = "tododb")
        {
            _connection = new SqliteConnection(datasource);
            _connection.Open();
            InitDb();
        }

        private void InitDb()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS todolist(
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title       TEXT    NOT NULL DEFAULT '',
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now'))
                )
            """;
            cmd.ExecuteNonQuery();
        }

        public void AddTodo(string title)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "INSERT INTO todolist(Title) VALUES($title)";
            cmd.Parameters.AddWithValue("$title", title);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Add : {title}");
        }

        public void GetAllTodos()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM todolist ORDER BY CreatedAt DESC";

            using var reader = cmd.ExecuteReader();
            Console.WriteLine("Todo list: ");

            while(reader.Read())
            {
                var status = reader.GetInt32(2) == 1 ? "✅" : "⬜";
                Console.WriteLine($"  [{reader.GetInt32(0)}] {status} {reader.GetString(1)}  ({reader.GetString(3)})");
            }
            Console.WriteLine();
        }


        public void CompleteTodo(int id)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE todolist SET IsCompleted = 1 WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            int affected = cmd.ExecuteNonQuery();
            Console.WriteLine(affected > 0 ? $"✓ Задачу #{id} виконано" : $"✗ Задачу #{id} не знайдено");
        }

        public void UpdateTitle(int id, string newTitle)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE todolist SET Title = $title WHERE Id = $id";
            cmd.Parameters.AddWithValue("$title", newTitle);
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"✓ Задачу #{id} оновлено");
        }

        public void DeleteTodo(int id)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM todolist WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            int affected = cmd.ExecuteNonQuery();
            Console.WriteLine(affected > 0 ? $"✓ Задачу #{id} видалено" : $"✗ Задачу #{id} не знайдено");
        }
    }
}