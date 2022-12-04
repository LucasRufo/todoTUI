using Spectre.Console;
using System.Dynamic;

namespace TodoTUI;

public class TodoTableHandler
{
    private readonly Table _todoTable;
    private readonly List<Todo> _todoList;
    private int _todoId;

    public TodoTableHandler()
    {
        _todoTable = new Table();

        _todoTable.AddColumn("Id");
        _todoTable.AddColumn("Done");
        _todoTable.AddColumn("Name");
        _todoTable.AddColumn("Created At");

        _todoList = new List<Todo>();
        _todoId = 1;
    }

    public List<Todo> GetTodoList() => _todoList;

    public void AddNewTodoInTable(string description)
    {
        var todo = new Todo(_todoId, description, _todoTable.Rows.Count);

        _todoList.Add(todo);

        _todoTable?.AddRow(
            new Markup($"{todo.Id}"),
            new Markup($"[red]-[/]"),
            new Markup($"[blue]{description}[/]"),
            new Markup($"[blue]{todo.CreatedAt}[/]"));

        IncrementTodoId();
    }

    public void ListTable()
    {
        AnsiConsole.Write(_todoTable);
    }

    public void RemoveTodo(List<Todo> todosToRemove)
    {
        foreach (var todoToRemove in todosToRemove)
        {
            _todoList.RemoveAll(todo => todo.Id == todoToRemove.Id);

            var todosToUpdateIndex = _todoList.Where(m => m.Id > todoToRemove.Id).ToList();

            foreach (var todo in todosToUpdateIndex)
            {
                todo.TableIndex--;
            }

            _todoTable.RemoveRow(todoToRemove.TableIndex);
        }
    }

    public void UpdateTodo(string newDescription, Todo todoToUpdate)
    {
        var todo = _todoList.Where(m => m.Id == todoToUpdate.Id).First();

        todo.Description = newDescription;

        _todoTable.UpdateCell(todo.TableIndex, 2, new Markup($"[blue]{todo.Description}[/]"));
    }

    public void MarkAsDone(List<Todo> todosToMarkAsDone)
    {
        foreach (var todoToMarkAsDone in todosToMarkAsDone)
        {
            var todoToUpdate = _todoList.Where(m => m.Id == todoToMarkAsDone.Id).First();

            todoToUpdate.Done = true;

            _todoTable.UpdateCell(todoToUpdate.TableIndex, 1, new Markup($"[green]X[/]"));
        }
    }

    private void IncrementTodoId()
    {
        _todoId++;
    }
}
