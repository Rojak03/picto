using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PictoSchedule.Data;
using PictoSchedule.Models;

namespace PictoSchedule.Endpoints;

public static class ChildEndpoints
{
    public static void MapChildEndpoints(this WebApplication app)
    {
        app.MapGet("/api/children", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var children = await db.Children.Where(c => c.ParentId == parentId).ToListAsync();
            return Results.Ok(children);
        }).RequireAuthorization();
        app.MapPost("/api/children", async (ClaimsPrincipal user, Child newChild, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            newChild.ParentId = parentId;
            db.Children.Add(newChild);
            await db.SaveChangesAsync();
            return Results.Created($"/api/children/{newChild.Id}", newChild);
        }).RequireAuthorization();
        app.MapPut("/api/children/{id}", async (ClaimsPrincipal user, int id, Child updateChild, AppDbContext db) =>
                {
                    var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var selectedChild = await db.Children.FindAsync(id);
                    if (selectedChild is null)
                    {
                        return Results.NotFound("Could not find child");
                    }
                    if (selectedChild.ParentId != parentId)
                    {
                        return Results.Forbid();
                    }
                    selectedChild.Name = updateChild.Name;
                    selectedChild.ProfilePictureUrl = updateChild.ProfilePictureUrl;

                    await db.SaveChangesAsync();
                    return Results.Ok(selectedChild);
                }).RequireAuthorization();
        app.MapDelete("/api/children/{id}", async (ClaimsPrincipal user, int id, AppDbContext db) =>
        {
            var parentId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var selectedChild = await db.Children.FindAsync(id);
            if (selectedChild is null)
            {
                return Results.NotFound("Could not find child");
            }
            if (selectedChild.ParentId != parentId)
            {
                return Results.Forbid();
            }
            db.Children.Remove(selectedChild);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}