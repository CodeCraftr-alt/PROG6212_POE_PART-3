using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class EventsController(ApiClient api) : SiteController(api)
{
    public async Task<IActionResult> Index(int? categoryId, string? type)
    {
        string path = "api/events?categoryId=" + categoryId + "&type=" + Uri.EscapeDataString(type ?? "");
        var events = await Api.Read<List<Race>>(Session, path);
        if (!string.IsNullOrWhiteSpace(type)) events = events.Where(e => e.EventType == type).ToList();
        var categories = await Api.Read<List<Category>>(Session, "api/categories");
        return View(new BrowsePage(events, categories, categoryId, type));
    }

    public async Task<IActionResult> Details(int id) =>
        View(await Api.Read<RaceDetails>(Session, $"api/events/{id}"));

    public async Task<IActionResult> Banner(int id)
    {
        try
        {
            var image = await Api.Image(Session, $"api/events/{id}/banner");
            return File(image.Bytes, image.Type);
        }
        catch (ApiError) { return NotFound(); }
    }

    public IActionResult Error() => View("Problem", "RaceDay could not complete the request. Please try again.");
}

