using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController(ApiClient api) : SiteController(api)
{
    protected override string RequiredRole => "Organiser";

    public async Task<IActionResult> Index() =>
        View(await Api.Read<List<DashboardRow>>(Session, "api/events/owned"));

    public async Task<IActionResult> Edit(int id = 0)
    {
        var race = new Race { EventDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(11, 0), Capacity = 50 };
        if (id > 0)
        {
            var details = await Api.Read<RaceDetails>(Session, $"api/events/{id}");
            race = details.Race;
            if (race.OrganizerID != Session.GetInt32("UserId")) return StatusCode(403);
            race.AdditionalCategoryIds = details.Categories.Select(c => c.CategoryID).Where(c => c != race.CategoryID).ToList();
        }
        return View(await LoadForm(race));
    }

    [HttpPost]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public async Task<IActionResult> Edit(int id, Race race, IFormFile? image)
    {
        race.EventID = id;
        if (image != null && image.Length > 5 * 1024 * 1024)
        {
            ViewData["Error"] = "The banner is too large. Choose a JPEG or PNG up to 5 MB, or save without a banner.";
            return View(await LoadForm(race));
        }
        if (!ModelState.IsValid)
        {
            ViewData["Error"] = "Check the dates, times and numbers you entered. An end time is required.";
            return View(await LoadForm(race));
        }
        try
        {
            var saved = await Api.Write<Race>(Session, id == 0 ? HttpMethod.Post : HttpMethod.Put,
                id == 0 ? "api/events" : $"api/events/{id}", race);
            if (image != null)
            {
                try { await Api.Upload(Session, $"api/events/{saved.EventID}/banner", image); }
                catch (ApiError error) when (error.Status != 401)
                {
                    TempData["Message"] = "The event was saved, but its banner was not. " + error.Message;
                    return RedirectToAction(nameof(Edit), new { id = saved.EventID });
                }
            }
            TempData["Message"] = "Event saved.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiError error) when (error.Status != 401)
        {
            ViewData["Error"] = error.Message;
            return View(await LoadForm(race));
        }
    }

    private async Task<RaceForm> LoadForm(Race race) => new(race,
        await Api.Read<List<Category>>(Session, "api/categories"),
        await Api.Read<List<Course>>(Session, "api/routes"));

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await Api.Delete(Session, $"api/events/{id}");
        TempData["Message"] = "Event deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Categories() => View(await Api.Read<List<Category>>(Session, "api/categories"));

    [HttpPost]
    public async Task<IActionResult> SaveCategory(int id, string categoryName, string? description)
    {
        try
        {
            await Api.Write<Category>(Session, id == 0 ? HttpMethod.Post : HttpMethod.Put,
                id == 0 ? "api/categories" : $"api/categories/{id}", new { categoryName, description });
            TempData["Message"] = "Category saved. Assign it from an event's edit page.";
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try { await Api.Delete(Session, $"api/categories/{id}"); }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Routes() => View(await Api.Read<List<Course>>(Session, "api/routes"));

    [HttpPost]
    public async Task<IActionResult> SaveRoute(Course course)
    {
        try
        {
            await Api.Write<Course>(Session, HttpMethod.Post, "api/routes", course);
            TempData["Message"] = "Route saved.";
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Routes));
    }

    public async Task<IActionResult> Entries(int id)
    {
        var entries = await Api.Read<List<Entry>>(Session, $"api/events/{id}/enrolments");
        var race = await Api.Read<RaceDetails>(Session, $"api/events/{id}");
        var results = await Api.Read<List<Result>>(Session, $"api/events/{id}/results");
        return View(new EntriesPage(race, entries, results));
    }

    [HttpPost]
    public async Task<IActionResult> Publish(int id, int enrollmentId, int resultId, TimeOnly finishTime, int position)
    {
        try
        {
            if (!ModelState.IsValid) throw new ApiError(400, "Enter a valid finish time and position.");
            var data = new { finishTime, position };
            await Api.Write<Result>(Session, resultId == 0 ? HttpMethod.Post : HttpMethod.Put,
                resultId == 0 ? $"api/enrolments/{enrollmentId}/result" : $"api/results/{resultId}", data);
            TempData["Message"] = "Result published.";
        }
        catch (ApiError error) when (error.Status != 401) { TempData["Message"] = error.Message; }
        return RedirectToAction(nameof(Entries), new { id });
    }
}


