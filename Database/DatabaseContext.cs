using System.Data.Common;
using System.Security.Claims;
using Microsoft.Data.Sqlite;

var connectionString = "Data Source=hyprdash.db";

using var connection = new SqliteConnection(connectionString);
connection.Open();

var cursor = connection.CreateCommand();
/*
cursor.CommandText = """
    CREATE TABLE IF NOT EXISTS todolist(
        Id          INTEGER PRIMARY KEY AUTOINCREMENT,
        Title       TEXT    NOT NULL DEFAULT '',
        IsCompleted INTEGER NOT NULL DEFAULT 0,
        CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now'))
    )
""";
cursor.ExecuteNonQuery();
*/

void AddTodo(string title)
{
    var cmd = connection.CreateCommand();
    cmd.CommandText = "INSERT INTO todolist(Title) VALUES($title)";
    cmd.Parameters.AddWithValue("$title", title);
    cmd.ExecuteNonQuery();
    Console.WriteLine($"Add : {title}");
}

void GetAllTodos()
{
    var cmd = connection.CreateCommand();
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


void CompleteTodo(int id)
{
    var cmd = connection.CreateCommand();
    cmd.CommandText = "UPDATE todolist SET IsCompleted = 1 WHERE Id = $id";
    cmd.Parameters.AddWithValue("$id", id);
    int affected = cmd.ExecuteNonQuery();
    Console.WriteLine(affected > 0 ? $"✓ Задачу #{id} виконано" : $"✗ Задачу #{id} не знайдено");
}

void UpdateTitle(int id, string newTitle)
{
    var cmd = connection.CreateCommand();
    cmd.CommandText = "UPDATE todolist SET Title = $title WHERE Id = $id";
    cmd.Parameters.AddWithValue("$title", newTitle);
    cmd.Parameters.AddWithValue("$id", id);
    cmd.ExecuteNonQuery();
    Console.WriteLine($"✓ Задачу #{id} оновлено");
}

void DeleteTodo(int id)
{
    var cmd = connection.CreateCommand();
    cmd.CommandText = "DELETE FROM todolist WHERE Id = $id";
    cmd.Parameters.AddWithValue("$id", id);
    int affected = cmd.ExecuteNonQuery();
    Console.WriteLine(affected > 0 ? $"✓ Задачу #{id} видалено" : $"✗ Задачу #{id} не знайдено");
}