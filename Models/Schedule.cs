namespace PictoSchedule.Models;

public class Schedule
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ChildId { get; set; }
    public Child? Child { get; set; }
    public List<ScheduleItem> ScheduleItems { get; set; } = new List<ScheduleItem>();
}