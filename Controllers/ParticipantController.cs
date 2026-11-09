using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController(ApiClient api) : SiteController(api)
{
    protected override string RequiredRole => "Participant";

    public async Task<IActionResult> Entries() => View(await Api.Read<List<Entry>>(Session, "api/enrolments/me"));
    public async Task<IActionResult> Results() => View(await Api.Read<List<Result>>(Session, "api/results/me"));
    public async Task<IActionResult> Profile() => View(await Api.Read<Profile>(Session, "api/profile"));

    [HttpPost]
    public async Task<IActionResult> Enter(int id, int categoryId, int? bibNumber)
    {
        try
        {
            await Api.Write<Entry>(Session, HttpMethod.Post, $"api/events/{id}/enrolments", new { categoryId, bibNumber });
            TempData["Message"] = "You are entered in the event.";
            return RedirectToAction(nameof(Entries));
        }
        catch (ApiError error) when (error.Status != 401)
        {
            TempData["Message"] = error.Message;
            return RedirectToAction("Details", "Events", new { id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await Api.Delete(Session, $"api/enrolments/{id}");
            TempData["Message"] = "Your entry was cancelled.";
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Entries));
    }

    [HttpPost]
    public async Task<IActionResult> Profile(Profile form)
    {
        try
        {
            await Api.Write<Profile>(Session, HttpMethod.Put, "api/profile", new { form.FirstName, form.LastName, form.Phone });
            TempData["Message"] = "Profile saved.";
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> UploadPicture(IFormFile? image)
    {
        try
        {
            if (image == null) TempData["Message"] = "Choose an image first.";
            else
            {
                await Api.Upload(Session, "api/profile/picture", image);
                TempData["Message"] = "Profile picture saved.";
            }
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> Picture()
    {
        try
        {
            var image = await Api.Image(Session, "api/profile/picture");
            Response.Headers.CacheControl = "private, no-store";
            return File(image.Bytes, image.Type);
        }
        catch (ApiError) { return NotFound(); }
    }
}

