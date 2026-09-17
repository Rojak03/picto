using Microsoft.EntityFrameworkCore;
using PictoSchedule.Data;
using PictoSchedule.Models;

namespace PictoSchedule.Endpoints;

public static class ScheduleItemEndpoints
{
    public static void MapScheduleItemEndpoints(this WebApplication app)
    {
        app.MapGet("/schedules/{scheduleId}/items", async (int scheduleId, AppDbContext db) =>
            await db.ScheduleItems.Where(x => x.ScheduleId == scheduleId).OrderBy(x => x.TimeOfDay).ToListAsync());

        app.MapPost("/schedules/{scheduleId}/items", async (int scheduleId, ScheduleItem newItem, AppDbContext db) =>
        {
            newItem.ScheduleId = scheduleId;
            db.ScheduleItems.Add(newItem);
            await db.SaveChangesAsync();
            return Results.Created($"/schedules/{newItem.ScheduleId}/items/{newItem.Id}", newItem);
        });

        app.MapPut("/schedules/{scheduleId}/items/{id}", async (int scheduleId, int id, ScheduleItem updateItem, AppDbContext db) =>
        {
            var selectedItem = await db.ScheduleItems.FindAsync(id);
            if (selectedItem is null)
            {
                return Results.NotFound("Could not find schedule item");
            }
            selectedItem.Label = updateItem.Label;
            selectedItem.TimeOfDay = updateItem.TimeOfDay;
            selectedItem.PictogramUrl = updateItem.PictogramUrl;

            await db.SaveChangesAsync();
            return Results.Ok(selectedItem);
        });

        app.MapDelete("/schedules/{scheduleId}/items/{id}", async (int scheduleId, int id, AppDbContext db) =>
        {
            var selectedItem = await db.ScheduleItems.FindAsync(id);
            if (selectedItem is null)
            {
                return Results.NotFound("Could not find schedule item");
            }
            db.ScheduleItems.Remove(selectedItem);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
