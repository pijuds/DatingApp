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
         var uow=resultContext.HttpContext.RequestServices.GetRequiredService<IuintofWork>();
         var user=await uow.UserRepository.GetUserByIdAsync(Convert.ToInt32(userName));
         user.LastActive=DateTime.UtcNow;
         await uow.Complete();
    }
}