using Spectre.Console;
using TodoTUI;

var tableHandler = new TodoTableHandler();

while (true)
{
    AnsiConsole.Markup("[bold cyan1]Hello from [underline]Todo TUI[/]. A terminal UI to keep your to do list![/]");
    AnsiConsole.WriteLine();

    tableHandler.ListTable();
    AnsiConsole.WriteLine();

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
        case TodoOptions.MarkAsDone:
            AskForTodosToBeMarkedAsDone();
            break;
        case TodoOptions.Exit:
            Environment.Exit(0);
            break;
    }

    AnsiConsole.Clear();
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
                    TodoOptions.Remove => "Remove Todos from my list.",
                    TodoOptions.MarkAsDone => "Mark Todos as done.",
                    TodoOptions.Exit => "Exit.",
                    _ => throw new InvalidOperationException(),
                };
            })
            .HighlightStyle(new Style(Color.Green))
            .AddChoices(new[]
            {
                TodoOptions.Add,
                TodoOptions.Update,
                TodoOptions.Remove,
                TodoOptions.MarkAsDone,
                TodoOptions.Exit
            }));
}

void AskForNewTodo()
{
    var description = AskForTaskName("What's your next [underline green]task[/]?");

    tableHandler.AddNewTodoInTable(description);
}

string AskForTaskName(string prompt)
{
    return AnsiConsole.Prompt(
        new TextPrompt<string>(prompt)
            .ValidationErrorMessage("[red]That's not a valid task![/]")
            .Validate(task =>
            {
                if (task.Length > 50)
                    return ValidationResult.Error("[red]The task length need to be lower than 50 caracthers.[/]");

                return ValidationResult.Success();
            }));
}

void AskForTodoToBeUpdated()
{
    var todoToUpdate = AskForSingleTodoToAction("[underline green]update[/]");

    var newDescription = AskForTaskName("What's the [underline green]new description[/] for this todo?");

    tableHandler.UpdateTodo(newDescription, todoToUpdate);
}

void AskForTodosToBeMarkedAsDone()
{
    var todosToMarkAsDone = AskForTodosToAction("[underline green]mark as done[/]");

    tableHandler.MarkAsDone(todosToMarkAsDone);
}

void AskForTodosToBeRemoved()
{
    var todosToRemove = AskForTodosToAction("[underline red]remove[/]");

    tableHandler.RemoveTodo(todosToRemove);
}

List<Todo> AskForTodosToAction(string action)
{
    return AnsiConsole.Prompt(
        new MultiSelectionPrompt<Todo>()
            .Title($"Select the todos that you want to {action}.")
            .PageSize(10)
            .NotRequired()
            .MoreChoicesText("[silver](Move up and down to reveal more todos)[/]")
            .InstructionsText(
                "[silver](Press [blue]<space>[/] to toggle a todo, " +
                "[green]<enter>[/] to accept)[/]")
            .UseConverter(ChoiceConverter)
            .HighlightStyle(new Style(Color.DeepSkyBlue1))
            .AddChoices(tableHandler!.GetTodoList().ToArray()));
}

Todo AskForSingleTodoToAction(string action)
{
    return AnsiConsole.Prompt(
        new SelectionPrompt<Todo>()
            .Title($"Select the todo that you want to {action}.")
            .PageSize(10)
            .MoreChoicesText("[silver](Move up and down to reveal more todos)[/]")
            .UseConverter(ChoiceConverter)
            .HighlightStyle(new Style(Color.DeepSkyBlue1))
            .AddChoices(tableHandler!.GetTodoList().ToArray()));
}

string ChoiceConverter(Todo todo)
{
    var maxLength = tableHandler!.GetTodoList()!.Max(m => m.Description.Length);
    var done = todo.Done ? "[green]X[/]" : "[red]-[/]";
    var message = string.Format("{0,-2} | {1,1} | {2,-" + maxLength + "} | {3} | {4}", 
        todo.Id, done, todo.Description, todo.CreatedAt, todo.CompletedAt);
    return message;
}

