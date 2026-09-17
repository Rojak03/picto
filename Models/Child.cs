namespace PictoSchedule.Models;

public class Child
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;

    public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    public int ParentId { get; set; }
    public Parent? Parent { get; set; }
}