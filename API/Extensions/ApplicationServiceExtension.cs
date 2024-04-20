using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration config)
        {
            Services.AddDbContext<DataContext>(opt=>{
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
});

             Services.AddCors();
             Services.AddScoped<ITokenService,TokenService>();
             Services.AddScoped<IUserRepositoty,UserRepository>();
             Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
             return Services;
        }
    }
}