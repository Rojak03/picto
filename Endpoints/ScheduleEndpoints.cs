using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PictoSchedule.Data;
using PictoSchedule.Models;

namespace PictoSchedule.Endpoints;

public static class ScheduleEndpoints
{
    public static void MapScheduleEndpoints(this WebApplication app)
    {
        app.MapGet("/children/{childId}/schedules", async (ClaimsPrincipal user, int childId, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Children.AnyAsync(c => c.Id == childId && c.ParentId == parentId))
            {
                return Results.Forbid();
            }
            var schedules = await db.Schedules.Where(s => s.ChildId == childId).ToListAsync();
            return Results.Ok(schedules);
        }).RequireAuthorization();

        app.MapPost("/children/{childId}/schedules", async (ClaimsPrincipal user, int childId, Schedule newSchedule, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Children.AnyAsync(c => c.Id == childId && c.ParentId == parentId))
            {
                return Results.Forbid();
            }
            newSchedule.ChildId = childId;
            db.Schedules.Add(newSchedule);
            await db.SaveChangesAsync();
            return Results.Created($"/children/{childId}/schedules/{newSchedule.Id}", newSchedule);
        }).RequireAuthorization();

        app.MapPut("/schedules/{id}", async (ClaimsPrincipal user, int id, Schedule updateSchedule, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == id && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }
            var selectedSchedule = await db.Schedules.FindAsync(id);
            if (selectedSchedule is null)
            {
                return Results.NotFound("Could not find schedule");
            }
            selectedSchedule.Title = updateSchedule.Title;

            await db.SaveChangesAsync();
            return Results.Ok(selectedSchedule);
        }).RequireAuthorization();
        app.MapDelete("/schedules/{id}", async (ClaimsPrincipal user, int id, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == id && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }
            var selectedSchedule = await db.Schedules.FindAsync(id);
            if (selectedSchedule is null)
            {
                return Results.NotFound("Could not find schedule");
            }
            db.Schedules.Remove(selectedSchedule);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
