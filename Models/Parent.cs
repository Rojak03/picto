namespace PictoSchedule.Models;

public class Parent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public List<Child> Children { get; set; } = new List<Child>();
}