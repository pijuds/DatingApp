using API.Interfaces;
using Extensions.API;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext=await next();
        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
         var userName=resultContext.HttpContext.User.GetUserId();
         var repo=resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepositoty>();
         var user=await repo.GetUserByIdAsync(Convert.ToInt32(userName));
         user.LastActive=DateTime.UtcNow;
         await repo.SaveAllAsync();
    }
}