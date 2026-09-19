using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PictoSchedule.Data;
using PictoSchedule.Models;

namespace PictoSchedule.Endpoints;

public static class ScheduleItemEndpoints
{
    public static void MapScheduleItemEndpoints(this WebApplication app)
    {
        app.MapGet("/schedules/{scheduleId}/items", async (ClaimsPrincipal user, int scheduleId, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == scheduleId && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }

            var scheduleItems = await db.ScheduleItems.Where(x => x.ScheduleId == scheduleId).OrderBy(x => x.TimeOfDay).ToListAsync();

            return Results.Ok(scheduleItems);

        }).RequireAuthorization();

        app.MapPost("/schedules/{scheduleId}/items", async (ClaimsPrincipal user, int scheduleId, ScheduleItem newItem, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == scheduleId && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }

            newItem.ScheduleId = scheduleId;
            db.ScheduleItems.Add(newItem);
            await db.SaveChangesAsync();
            return Results.Created($"/schedules/{newItem.ScheduleId}/items/{newItem.Id}", newItem);
        }).RequireAuthorization();

        app.MapPut("/schedules/{scheduleId}/items/{id}", async (ClaimsPrincipal user, int scheduleId, int id, ScheduleItem updateItem, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == scheduleId && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }

            var selectedItem = await db.ScheduleItems.Where(s => s.Id == id && s.ScheduleId == scheduleId).FirstOrDefaultAsync();
            if (selectedItem is null)
            {
                return Results.NotFound("Could not find schedule item");
            }
            selectedItem.Label = updateItem.Label;
            selectedItem.TimeOfDay = updateItem.TimeOfDay;
            selectedItem.PictogramUrl = updateItem.PictogramUrl;

            await db.SaveChangesAsync();
            return Results.Ok(selectedItem);
        }).RequireAuthorization();

        app.MapDelete("/schedules/{scheduleId}/items/{id}", async (ClaimsPrincipal user, int scheduleId, int id, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!await db.Schedules.AnyAsync(s => s.Id == scheduleId && s.Child != null && s.Child.ParentId == parentId))
            {
                return Results.Forbid();
            }
            var selectedItem = await db.ScheduleItems.Where(s => s.Id == id && s.ScheduleId == scheduleId).FirstOrDefaultAsync();
            if (selectedItem is null)
            {
                return Results.NotFound("Could not find schedule item");
            }
            db.ScheduleItems.Remove(selectedItem);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
