using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
     public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
     {
    // Configure Identity with custom settings
     services.AddIdentityCore<AppUser>(opt =>
     {
        opt.Password.RequireNonAlphanumeric = false; // Customize password requirements
     })
    .AddRoles<AppRole>() // Add role support
    .AddRoleManager<RoleManager<AppRole>>() // Add role manager
    .AddEntityFrameworkStores<DataContext>(); // Use Entity Framework for storage

    // Configure JWT authentication
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])), // Provide the key for token validation
            ValidateAudience = false, // Disable audience validation
            ValidateIssuer = false // Disable issuer validation
        };

        // Handle JWT tokens in query string for SignalR hubs
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for the SignalR hub, read the token from the query string
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

    // Configure authorization policies
    services.AddAuthorization(opt =>
    {
        opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
    });

        return services;
      }
    }

}