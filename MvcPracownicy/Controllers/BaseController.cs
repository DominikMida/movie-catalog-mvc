using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MvcPracownicy.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                context.Result = RedirectToAction("Logowanie", "Account");
            }

            base.OnActionExecuting(context);
        }
    }
}