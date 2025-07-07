using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using webappproject.Services;

namespace webappproject.Controllers
{
    public class BaseController : Controller
    {
        protected readonly BanService _banService;

        public BaseController(BanService banService)
        {
            _banService = banService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check if user is authenticated and not on login/register/admin pages
            if (User?.Identity?.IsAuthenticated == true && !User.IsInRole("Admin"))
            {
                var userEmail = User.Identity.Name;
                var controllerName = context.RouteData.Values["controller"]?.ToString();
                var actionName = context.RouteData.Values["action"]?.ToString();

                // Skip ban check for login, register, and contact admin pages
                if (controllerName != "Login" && 
                    controllerName != "Register" && 
                    !(controllerName == "Home" && actionName == "Contact"))
                {
                    if (userEmail != null && _banService.IsBanned(userEmail))
                    {
                        ViewBag.IsBanned = true;
                        var banDetails = _banService.Get(x => x.Email == userEmail).FirstOrDefault();
                        ViewBag.BanReason = banDetails?.Reason ?? "Account suspended";
                        ViewBag.BanDate = banDetails?.BanDate.ToString("MMM dd, yyyy") ?? "";
                    }
                }
            }
        }
    }
}
