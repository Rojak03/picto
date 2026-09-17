using PictoSchedule.Data;
using PictoSchedule.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace PictoSchedule.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        //create user registration endpoint
        app.MapPost("/register", async (RegisterRequest request, AppDbContext db) =>
        {
            // Check if the email is already registered
            var existingParent = await db.Parents.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (existingParent != null)
            {
                return Results.BadRequest("Email is already registered.");
            }
            var newParent = new Parent
            {
                Name = request.Name,
                Email = request.Email
            };
            // Hash the password before saving
            var hasher = new PasswordHasher<Parent>();
            newParent.PasswordHash = hasher.HashPassword(newParent, request.Password);

            db.Parents.Add(newParent);
            await db.SaveChangesAsync();
            return Results.Created($"/api/parents/{newParent.Id}", newParent);
        });
        //create user login endpoint
        app.MapPost("/login", async (LoginRequest request, AppDbContext db, IConfiguration config) =>
        {
            var parent = await db.Parents.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (parent == null)
            {
                return Results.Unauthorized();
            }
            var hasher = new PasswordHasher<Parent>();
            var result = hasher.VerifyHashedPassword(parent, parent.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }
            var token = GenerateJwtToken(parent, config);
            return Results.Ok(new { Message = "Login successful", Token = token });
        });
    }
    private static string GenerateJwtToken(Parent parent, IConfiguration config)
    {
        var jwtSettings = config.GetSection("Jwt");

        var claims = new[]
        {
           new Claim(ClaimTypes.NameIdentifier, parent.Id.ToString()),
           new Claim(ClaimTypes.Email, parent.Email)
       };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);