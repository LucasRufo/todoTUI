using Spectre.Console;
using TodoTUI;

AnsiConsole.Markup("[underline red]Hello from Todo TUI. A terminal UI to keep your to do list![/]");
AnsiConsole.WriteLine();

var id = 1;
var todoList = new List<Todo>();
var todoTable = new Table();

todoTable.AddColumn("Id");
todoTable.AddColumn("Done");
todoTable.AddColumn("Name");
todoTable.AddColumn("Created At");

AddNewTodoInTable("teste");
AddNewTodoInTable("teste2");
AddNewTodoInTable("teste3");
AddNewTodoInTable("teste4");

ListTable();

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<TodoOptions>()
            .Title("What do you want to do?")
            .PageSize(10)
            .UseConverter(x =>
            {
                return x switch
                {
                    TodoOptions.Exit => "Exit.",
                    TodoOptions.List => "List all my Todos.",
                    TodoOptions.Add => "Add new Todo to my list.",
                    TodoOptions.Remove => "Remove a Todo from my list.",
                    TodoOptions.Update => "Update a Todo from my list.",
                    _ => throw new InvalidOperationException(),
                };
            })
            .AddChoices(new[]
            {
                TodoOptions.List,
                TodoOptions.Add,
                TodoOptions.Remove,
                TodoOptions.Update,
                TodoOptions.Exit
            }));

    switch (choice)
    {
        case TodoOptions.List:
            ListTable();
            break;
        case TodoOptions.Add:
            AskForNewTodo();
            break;
        case TodoOptions.Remove:
            AskForTodosToBeRemoved();
            break;
        case TodoOptions.Update:
            throw new NotImplementedException();
        case TodoOptions.Exit:
            Environment.Exit(0);
            break;
    }
}

List<Todo> AskForChoiceOfTodos(string action)
{
    return AnsiConsole.Prompt(
        new MultiSelectionPrompt<Todo>()
            .Title($"Select the todos that you want to {action}")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more todos)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle a todo, " +
                "[green]<enter>[/] to accept)[/]")
            .UseConverter(todo =>
            {
                var done = todo.Done ? "X" : "-";
                //string.Format("|{0,5}|{1,5}|{2,5}|{3,5}|", todo.Id, done, todo.Description, todo.CreatedAt);
                return $"{todo.Id} {done} {todo.Description} {todo.CreatedAt}";
            })
            .AddChoices(todoList.ToArray()));
}

void AskForTodosToBeRemoved()
{
    var todosToRemove = AskForChoiceOfTodos("remove");

    foreach (var todoToRemove in todosToRemove)
    {
        todoList.RemoveAll(todo => todo.Id == todoToRemove.Id);

        var todosToUpdateIndex = todoList.Where(m => m.Id > todoToRemove.Id).ToList();

        foreach (var todo in todosToUpdateIndex)
        {
            todo.TableIndex--;
        }

        todoTable.RemoveRow(todoToRemove.TableIndex);
    }

    ListTable();
}

void AskForNewTodo()
{
    var description = AnsiConsole.Ask<string>("What's your next [green]task[/]?");

    AddNewTodoInTable(description);

    ListTable();
}

void ListTable()
{
    AnsiConsole.Clear();
    AnsiConsole.Write(todoTable);
}

void AddNewTodoInTable(string description)
{
    var todo = new Todo(id, description, todoTable.Rows.Count);

    todoList.Add(todo);

    todoTable?.AddRow(
        new Markup($"{todo.Id}"),
        new Markup($"-"),
        new Markup($"[blue]{description}[/]"),
        new Markup($"[blue]{todo.CreatedAt}[/]"));

    id++;
}
