using Spectre.Console;
using TodoTUI;

AnsiConsole.Markup("[underline red]Hello from Todo TUI. A terminal UI to keep your to do list![/]");
AnsiConsole.WriteLine();

var tableHandler = new TodoTableHandler();

tableHandler.ListTable();

while (true)
{
    TodoOptions choice = AskForAction();

    switch (choice)
    {
        case TodoOptions.Add:
            AskForNewTodo();
            break;
        case TodoOptions.Remove:
            AskForTodosToBeRemoved();
            break;
        case TodoOptions.Update:
            AskForTodoToBeUpdated();
            break;
        case TodoOptions.Exit:
            Environment.Exit(0);
            break;
    }
}

static TodoOptions AskForAction()
{
    return AnsiConsole.Prompt(
        new SelectionPrompt<TodoOptions>()
            .Title("What do you want to do?")
            .PageSize(10)
            .UseConverter(todoOption =>
            {
                return todoOption switch
                {
                    TodoOptions.Add => "Add new Todo to my list.",
                    TodoOptions.Update => "Update a Todo from my list.",
                    TodoOptions.Remove => "Remove a Todo from my list.",
                    TodoOptions.Exit => "Exit.",
                    _ => throw new InvalidOperationException(),
                };
            })
            .AddChoices(new[]
            {
                TodoOptions.Add,
                TodoOptions.Update,
                TodoOptions.Remove,
                TodoOptions.Exit
            }));
}

void AskForNewTodo()
{
    var description = AnsiConsole.Ask<string>("What's your next [green]task[/]?");

    tableHandler.AddNewTodoInTable(description);

    tableHandler.ListTable();
}

void AskForTodoToBeUpdated()
{
    var todoToUpdate = AskForSingleTodoToAction("update");

    var newDescription = AnsiConsole.Ask<string>("What's the new description for this todo?");

    tableHandler.UpdateTodo(newDescription, todoToUpdate);

    tableHandler.ListTable();
}

void AskForTodosToBeRemoved()
{
    var todosToRemove = AskForTodosToAction("remove");

    tableHandler.RemoveTodo(todosToRemove);

    tableHandler.ListTable();
}

List<Todo> AskForTodosToAction(string action)
{
    return AnsiConsole.Prompt(
        new MultiSelectionPrompt<Todo>()
            .Title($"Select the todos that you want to {action}")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more todos)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle a todo, " +
                "[green]<enter>[/] to accept)[/]")
            .UseConverter(ChoiceConverter)
            .AddChoices(tableHandler!.GetTodoList().ToArray()));
}

Todo AskForSingleTodoToAction(string action)
{
    return AnsiConsole.Prompt(
        new SelectionPrompt<Todo>()
            .Title($"Select the todo that you want to {action}")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more todos)[/]")
            .UseConverter(ChoiceConverter)
            .AddChoices(tableHandler!.GetTodoList().ToArray()));
}

string ChoiceConverter(Todo todo)
{
    var maxLength = tableHandler!.GetTodoList()!.Max(m => m.Description.Length);
    var done = todo.Done ? "X" : "-";
    var message = string.Format("{0,-2} | {1,1} | {2,-" + maxLength + "} | {3}", todo.Id, done, todo.Description, todo.CreatedAt);
    return message;
}

