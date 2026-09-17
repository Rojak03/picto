using Microsoft.EntityFrameworkCore;
using PictoSchedule.Models;

namespace PictoSchedule.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ScheduleItem> ScheduleItems { get; set; } = null!;
    public DbSet<Child> Children { get; set; } = null!;
    public DbSet<Schedule> Schedules { get; set; } = null!;

    public DbSet<Parent> Parents { get; set; } = null!;

}