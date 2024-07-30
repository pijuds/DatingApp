using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUser(UserManager<AppUser> userManager ,RoleManager<AppRole> roleManager
        )
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        var roles = new List<AppRole>
    {
        new AppRole { Name = "Member" },
        new AppRole { Name = "Admin" },
        new AppRole { Name = "Moderator" }
    };

    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role.Name);
        if (!roleExists)
        {
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                // Handle the errors
                Console.WriteLine($"Failed to create role {role.Name}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
        }
        else
        {
            Console.WriteLine($"Role {role.Name} already exists.");
        }
    }

        foreach (var user in users)
        {
            user.Photos.First().IsApproved = true;
            user.UserName = user.UserName.ToLower();
            
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new AppUser
        {
            UserName = "admin"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
       
    }
}