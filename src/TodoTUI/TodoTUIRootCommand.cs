using System.CommandLine;

namespace TodoTUI;

public class TodoTUIRootCommand : RootCommand
{
    public TodoTUIRootCommand()
        : base("TodoTUI is a terminal UI to keep your to do list!")
    { }

    public void RegisterHandler(TodoTUIClusterBuilder kindClusterBuilder) 
        => this.SetHandler((ctx) => kindClusterBuilder.Build());
}

