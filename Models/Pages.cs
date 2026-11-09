namespace RaceDay.Mvc.Models;

public class Profile
{
    public int UserId { get; set; }
    public string Role { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public bool HasPicture { get; set; }
}

public class AccountForm
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Participant";
    public string? Phone { get; set; }
}

public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public string? Description { get; set; }
}

public class Course
{
    public int RouteID { get; set; }
    public string RouteName { get; set; } = "";
    public string StartLocation { get; set; } = "";
    public string FinishLocation { get; set; } = "";
    public decimal DistanceKm { get; set; }
}

public class Race
{
    public int EventID { get; set; }
    public int OrganizerID { get; set; }
    public int CategoryID { get; set; }
    public int RouteID { get; set; }
    public string EventName { get; set; } = "";
    public string Description { get; set; } = "";
    public string EventType { get; set; } = "Run";
    public DateOnly EventDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string Location { get; set; } = "";
    public int Capacity { get; set; }
    public string Status { get; set; } = "Open";
    public string? BannerUrl { get; set; }
    public List<int> AdditionalCategoryIds { get; set; } = new();
}

public class RaceDetails
{
    public Race Race { get; set; } = new();
    public Course? Route { get; set; }
    public List<Category> Categories { get; set; } = new();
}

public class Entry
{
    public int EnrollmentID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public string? EventName { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Status { get; set; } = "";
    public int? BibNumber { get; set; }
}

public class Result
{
    public int ResultID { get; set; }
    public int EnrollmentID { get; set; }
    public string? EventName { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? CategoryName { get; set; }
    public TimeOnly FinishTime { get; set; }
    public int Position { get; set; }
    public int Finishers { get; set; }
}

public record BrowsePage(List<Race> Events, List<Category> Categories, int? CategoryId, string? Type);
public record DashboardRow(Race Race, int Entries);
public record RaceForm(Race Race, List<Category> Categories, List<Course> Routes);
public record EntriesPage(RaceDetails Event, List<Entry> Entries, List<Result> Results);
