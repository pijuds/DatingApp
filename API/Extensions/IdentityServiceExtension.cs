using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection Services,IConfiguration config)
        {
            Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            options =>{
            options.TokenValidationParameters = new TokenValidationParameters
            {
        // Here you can provide parameters for token validation, such as issuer, audience, key, etc.
        // For example:
        
             ValidateIssuerSigningKey = true,
        // You need to provide appropriate values for the following properties based on your JWT configuration
        
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
        
            ValidateAudience=false,
            ValidateIssuer=false
            
           };
       });
        return Services;

        }
    }
}