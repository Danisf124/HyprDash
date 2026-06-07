using System;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace HyprDash
{
    public class TodoList
    {
        private readonly TodoDb _todoDb = new TodoDb();

        public int Id;

        public string Title;

        public bool IsCompleted;

        public DateTime CreatedAt;

        public List<TodoList> TodoLists {get; set;}

        public TodoList(string title)
        {
           Title = title;
           IsCompleted = false;
           CreatedAt = DateTime.Now;
        }

        public TodoList()
        {
            Title = String.Empty;
            IsCompleted = false;
            CreatedAt = DateTime.MinValue;
        }

        public void CreateNewTodo(string title)
        {
            TodoList newTodo = new TodoList(title);

            _todoDb.AddTodo(newTodo);
        }

        public void GetAllTodosFromDB()
        {
            TodoLists = _todoDb.GetAllTodos();
        }

        public void CompleteTodo(int id)
        {
            _todoDb.CompleteTodo(id);
        }

        public void DeleteTodo(int id)
        {
            _todoDb.DeleteTodo(id);
        }

        public void ClearAllTodo()
        {
            _todoDb.ClearAllTodo();
        }

    }
}