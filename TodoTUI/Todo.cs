namespace TodoTUI;

public class Todo
{
    public Todo(int id, string description, int tableIndex)
    {
        Id = id;
        Done = false;
        Description = description;
        CreatedAt = DateTime.Now.ToString("MM/dd/yyyy");
        TableIndex = tableIndex;
    }

    public int Id { get; set; }
    public bool Done { get; set; }
    public string Description { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public int TableIndex { get; set; }
}
