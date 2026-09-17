namespace PictoSchedule.Models;

public class ScheduleItem
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public string Label { get; set; } = string.Empty;
    public string TimeOfDay { get; set; } = string.Empty;
    public string PictogramUrl { get; set; } = string.Empty;
}