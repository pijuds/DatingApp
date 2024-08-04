using System.Security.Cryptography.X509Certificates;
using System.Text;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

//builder.Services.AddDbContext<DataContext>(opt=>{
  //opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("https://localhost:4200"));
});

builder.Services.AddScoped<ITokenService,TokenService>();

var app = builder.Build();

app.UseCors("CorsPolicy");


app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
//app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();
app.MapHub<PresenceHub>("/hubs/presence");
app.MapHub<MessageHub>("/hubs/message");
app.MapFallbackToController("Index", "Fallback");

using var scope=app.Services.CreateScope();
var services=scope.ServiceProvider;
try
{
  var context=services.GetRequiredService<DataContext>();
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
   var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

   await context.Database.MigrateAsync(); 
   await Seed.ClearConnectios(context); 
    
  await Seed.SeedUser(userManager,roleManager);
}
catch(Exception ex)
{
  var logger=services.GetServices<ILogger<Program>>();
  
  
  
}

app.Run();

