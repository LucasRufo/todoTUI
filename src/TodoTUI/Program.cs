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

TodoOptions AskForAction()
{
    TodoOptions[] todoChoices = GetTodoChoices();

    var selectionPrompt = 
        new SelectionPrompt<TodoOptions>()
            .Title("What do you want to do?")
            .PageSize(10)
            .UseConverter(todoOption =>
            {
                return todoOption switch
                {
                    TodoOptions.Add => "Add new Todo to my list.",
                    TodoOptions.MarkAsDone => "Mark Todos as done.",
                    TodoOptions.Update => "Update a Todo from my list.",
                    TodoOptions.Remove => "Remove Todos from my list.",
                    TodoOptions.Exit => "Exit.",
                    _ => throw new InvalidOperationException(),
                };
            })
            .HighlightStyle(new Style(Color.Green))
            .AddChoices(todoChoices);

    return AnsiConsole.Prompt(selectionPrompt);
}

TodoOptions[] GetTodoChoices()
{
    if (tableHandler.GetTodoList().Count == 0)
    {
        return new TodoOptions[]
        {
            TodoOptions.Add,
            TodoOptions.Exit
        };
    }
    else
    {
        return new TodoOptions[]
        {
            TodoOptions.Add,
            TodoOptions.MarkAsDone,
            TodoOptions.Update,
            TodoOptions.Remove,
            TodoOptions.Exit
        };
    }
}

void AskForNewTodo()
{
    var description = AskForTaskName("What's your next [underline green]task[/]?");

    tableHandler.AddNewTodoInTable(description);
}

void AskForTodoToBeUpdated()
{
    var todoToUpdate = AskForSingleTodoToAction("[underline green]update[/]");

    if (todoToUpdate == null) return;

    AnsiConsole.MarkupLine($"[deepskyblue1]{TodoConverter(todoToUpdate, todoToUpdate.Description.Length)}[/]");
    AnsiConsole.WriteLine();

    var newDescription = AskForTaskName("What's the [underline green]new description[/] for this todo?");

    tableHandler.UpdateTodo(newDescription, todoToUpdate);
}

void AskForTodosToBeMarkedAsDone()
{
    var todosToMarkAsDone = AskForTodosToAction("[underline green]mark as done[/]");

    if (todosToMarkAsDone.Count == 0) return;

    tableHandler.MarkAsDone(todosToMarkAsDone);
}

void AskForTodosToBeRemoved()
{
    var todosToRemove = AskForTodosToAction("[underline red]remove[/]");

    if (todosToRemove.Count == 0) return;

    AnsiConsole.MarkupLine("The following todos are going to be [underline red]removed[/]?");
    AnsiConsole.MarkupLine("This operation is irreversible.");
    AnsiConsole.WriteLine();

    string todosToRemoveMarkup = string.Empty;
    var maxLength = todosToRemove.Max(m => m.Description.Length);

    foreach (var todoToRemove in todosToRemove)
    {
        todosToRemoveMarkup += $"[red]{TodoConverter(todoToRemove, maxLength)}[/]\n";
    }

    todosToRemoveMarkup = todosToRemoveMarkup.TrimEnd('\n');

    var panel = new Panel(todosToRemoveMarkup)
    {
        Border = BoxBorder.Rounded
    };

    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();

    if (AnsiConsole.Confirm("Are you sure?"))
    {
        tableHandler.RemoveTodo(todosToRemove);
    }
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
    return TodoConverter(todo, maxLength);
}

string TodoConverter(Todo todo, int maxLength)
{
    var done = todo.Done ? "[green]X[/]" : "[red]-[/]";
    var message = string.Format("{0,-2} | {1,1} | {2,-" + maxLength + "} | {3} | {4}",
        todo.Id, done, todo.Description, todo.CreatedAt, todo.CompletedAt);
    return message;
}
