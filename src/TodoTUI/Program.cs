using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using TodoTUI;

var todoTUIRootCommand = new TodoTUIRootCommand();

var todoTUIClusterBuilder = new TodoTUIClusterBuilder();

todoTUIRootCommand.SetHandler(() =>
{
    todoTUIClusterBuilder.Build();
});

await new CommandLineBuilder(todoTUIRootCommand)
    .UseDefaults()
    .Build()
    .InvokeAsync(args);

