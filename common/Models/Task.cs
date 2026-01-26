namespace common.Models;

public class Task
{
    public int Id { get; set; }
    public int IdCategory { get; set; }
    public int CreatedBy { get; set; }
    public required int[] Assignees { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}
