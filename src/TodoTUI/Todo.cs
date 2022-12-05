namespace TodoTUI;

public class Todo
{
    public Todo(int id, string description, int tableIndex)
    {
        Id = id;
        Done = false;
        Description = description;
        CreatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        TableIndex = tableIndex;
        CompletedAt = "-";
    }

    public int Id { get; set; }
    public bool Done { get; set; }
    public string Description { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public string CompletedAt { get; set; } = "";
    public int TableIndex { get; set; }
}
