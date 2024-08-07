using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Services.API;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration config)
        {

             Services.AddDbContext<DataContext>(opt => {
              opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
             });

            

             Services.AddCors();
             Services.AddScoped<ITokenService,TokenService>();
             //Services.AddScoped<IUserRepositoty,UserRepository>();
             Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
             Services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
             Services.AddScoped<IPhotoService,PhotoService>();
             Services.AddScoped<LogUserActivity>();
             //Services.AddScoped<ILikesRepository,LikesRepository>();
             //Services.AddScoped<IMessageRepository,MessageRepository>();
             Services.AddScoped<IuintofWork,UnitofWork>();
             Services.AddSignalR();
             Services.AddSingleton<PresenceTracker>();            
             return Services;
        }
    }
}