using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class AccountController(ApiClient api) : SiteController(api)
{
    [HttpGet]
    public IActionResult Login() => View("Account", new AccountForm());

    [HttpGet]
    public IActionResult Register() => View("Account", new AccountForm());

    [HttpPost]
    public Task<IActionResult> Login(AccountForm form) => SignIn("login", form);

    [HttpPost]
    public Task<IActionResult> Register(AccountForm form) => SignIn("register", form);

    private async Task<IActionResult> SignIn(string action, AccountForm form)
    {
        try
        {
            Session.Clear();
            var profile = await Api.Write<Profile>(Session, HttpMethod.Post, "api/auth/" + action, form);
            Session.SetInt32("UserId", profile.UserId);
            Session.SetString("Role", profile.Role);
            return profile.Role == "Organiser" ? RedirectToAction("Index", "Organiser") : RedirectToAction("Index", "Events");
        }
        catch (ApiError error)
        {
            TempData["Message"] = error.Message;
            return RedirectToAction(action == "login" ? nameof(Login) : nameof(Register));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        try { if (Session.GetString("ApiCookie") != null) await Api.Post(Session, "api/auth/logout"); }
        catch (ApiError) { }
        finally { Session.Clear(); }
        return RedirectToAction(nameof(Login));
    }
}
