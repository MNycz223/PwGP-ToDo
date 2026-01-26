namespace common.Models;

public class Task
{
    public int Id { get; set; }
    public int IdCategory { get; set; }
    public int CreatedBy { get; set; }
    public int[] Assignees { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
