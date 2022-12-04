using Spectre.Console;

AnsiConsole.Markup("[underline red]Hello from Todo TUI. A terminal UI to keep your to do list![/]");
AnsiConsole.WriteLine();

var id = 1;

var table = new Table();

table.AddColumn("Id");
table.AddColumn("Done");
table.AddColumn("Name");
table.AddColumn("Created At");

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
            throw new NotImplementedException();
        case TodoOptions.Update:
            throw new NotImplementedException();
        case TodoOptions.Exit:
            throw new NotImplementedException();
    } 
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
    AnsiConsole.Write(table);
}

void AddNewTodoInTable(string description)
{
    table?.AddRow(new Markup($"{id}"), new Markup($""), new Markup($"[blue]{description}[/]"), new Markup($"[blue]{DateTime.Now.ToString("MM/dd/yyyy")}[/]"));
    id++;
}

enum TodoOptions
{
    Exit = 0,
    List = 1,
    Add = 2,
    Remove = 3,
    Update = 4
}
