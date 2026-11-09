# PROG6212_POE_PART-3
# RaceDay Part 3

RaceDay is a web app for running, walking and cycling events. Organisers create events, assign categories and publish results. Participants browse events, enter a category and view their results.

The browser opens **RaceDay.Mvc**. MVC calls the **RaceDay API**, which is the only project that talks to SQL Server and manages uploaded images. SQL Server keeps records after both apps close. EF InMemory is used only by the isolated tests.

## Run in Visual Studio or PowerShell

Requirements: .NET 8 SDK and SQL Server LocalDB on Windows. Open `RaceDay.sln` in Visual Studio. Choose **RaceDay Website and API** from the Visual Studio toolbar and press F5 to start both projects. If the profile is not visible, reopen the solution. Alternatively, run the two commands in separate terminals from the repository root:

```powershell
dotnet run --project RaceDay --launch-profile http
dotnet run --project RaceDay.Mvc --launch-profile "RaceDay MVC"
```

Open [RaceDay MVC](http://localhost:5073) or [API Swagger](http://localhost:5024/swagger). There are no preset accounts. Start by registering an Organiser, creating a category and a route, then creating an event.

The API creates a fresh **RaceDayPart3** database on first startup. It does not edit or import the Part 1 or Part 2 databases. To use a different SQL Server, set `ConnectionStrings__RaceDay` before starting the API. Use a dedicated empty database for initial setup. The coursework uses EF Core's `EnsureCreatedAsync`; it creates a fresh schema and does not upgrade an existing schema. A database login used on first startup needs permission to create the database and its tables. Do not commit production credentials.

## Image uploads

Upload JPEG or PNG images up to 5 MB from the event edit page or the participant profile page. MVC sends the upload to the API. The API writes the file to `RaceDay/Images` and stores the generated filename in SQL Server. MVC asks the API for the image when displaying the page. Profile pictures still require a participant session.

The API creates the folder if it is missing. No cloud account, emulator or storage key is needed. Files in this folder are ignored by Git. If you need a different folder, set the API environment variable `ImageStorage__Folder` to a writable path. Back up the folder together with the database. Images previously uploaded to cloud storage must be uploaded again; their old references do not point to local files.

## Run with Docker

Install Docker Desktop, start its Linux containers engine, and run this from the repository root after creating an untracked `.env` file:

```text
RACEDAY_SQL_PASSWORD=RaceDay@123!
```

Choose a strong local password that includes uppercase, lowercase, numbers and symbols; avoid semicolons and compose interpolation characters in this example. Then start the full stack with one command:

```powershell
docker compose up --build
```

Open [RaceDay MVC](http://localhost:5073). Compose starts MVC, the API and SQL Server Developer. Named volumes retain the database and uploaded images when you stop the stack with `docker compose down`. The two Dockerfiles publish with the .NET SDK and run in the smaller ASP.NET runtime image.

To build and run **only the MVC image** against a Windows API already running on port 5024:

```powershell
docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc .
docker run --rm -p 127.0.0.1:5073:8080 -e RaceDayApi__BaseUrl=http://host.docker.internal:5024/ raceday-mvc
```

For this option, start the host API with `dotnet run --project RaceDay --no-launch-profile --urls http://0.0.0.0:5024` so the container can reach it. Use this binding only on a trusted development network. Compose is simpler for running the full stack.

## Where to look in the code

| File or folder | What it does |
| --- | --- |
| `RaceDay.Mvc/Controllers` | Handles website pages and forms. Calls the API for all changes. |
| `RaceDay.Mvc/Views` | Razor pages for accounts, event browsing, the organiser dashboard and participant pages. |
| `RaceDay.Mvc/Services/ApiClient.cs` | Sends HTTP requests and forwards the API cookie from the current MVC session. |
| `RaceDay/Program.cs` and `Data/RaceDayContext.cs` | Connect EF Core to SQL Server and define keys and relationships. |
| `RaceDay/Controllers/ImagesController.cs` and `Storage/ImageStorage.cs` | Check upload permissions, save and read files in the API images folder. |
| `RaceDay.Tests` | Tests for controller behaviour, roles, authentication and the API. |

## Tests and submission

```powershell
dotnet test RaceDay.sln
```

The GitHub Actions workflow restores and builds the full solution, runs the tests and builds both Docker images. Keep it in the GitHub repository's root `.github/workflows` folder. The workflow runs on pushes and pull requests.

See [the Part 3 demo and submission guide](docs/PART3-DEMO.md) for the recording order, evidence, and remaining steps. The older [Part 2 README](docs/PART2-README.md) is kept for reference and describes the earlier InMemory version.

## YouTube Link 
[https://www.youtube.com/watch?v=JP7DAAq8Ba8]
