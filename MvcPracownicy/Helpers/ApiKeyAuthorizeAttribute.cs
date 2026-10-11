using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcPracownicy.Data;

namespace MvcPracownicy.Helpers;

public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var user = request.Headers["X-Api-User"].FirstOrDefault();
        var token = request.Headers["X-Api-Token"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(token) || !Db.ValidateApiToken(user, token))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Brak autoryzacji API" });
            return;
        }

        await next();
    }
}
