using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public abstract class SiteController(ApiClient api) : Controller
{
    protected ApiClient Api { get; } = api;
    protected ISession Session => HttpContext.Session;
    protected virtual string? RequiredRole => null;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await Session.LoadAsync();
        var role = Session.GetString("Role");
        if (RequiredRole != null && role == null)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }
        if (RequiredRole != null && role != RequiredRole)
        {
            Response.StatusCode = 403;
            context.Result = View("Problem", "Your account cannot use this page.");
            return;
        }

        var result = await next();
        if (result.Exception is ApiError error && !result.ExceptionHandled)
        {
            result.ExceptionHandled = true;
            if (error.Status == 401 && role != null && this is not AccountController)
            {
                Session.Clear();
                TempData["Message"] = "Your session expired. Please log in again.";
                result.Result = RedirectToAction("Login", "Account");
                return;
            }

            Response.StatusCode = error.Status;
            result.Result = View("Problem", error.Message);
        }
    }
}
