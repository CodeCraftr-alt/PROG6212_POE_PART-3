# RaceDay Part 3 walkthrough

Keep the API running while switching between Organiser and Participant accounts. Use fake emails and passwords on camera.

1. **Introduce the project.** Show the two projects in Visual Studio. Say: “The MVC project makes the pages. It asks the API for data, and only the API talks to the database.”
2. **Show organiser setup.** Register as an Organiser. Add a category called 10 km and a route with a distance of 10 km. From the dashboard, create a future event with a start time and a later end time. Assign the category and select a banner. Open the public page and show the banner and event details.
3. **Show a participant entry.** Log out and register a Participant. Filter the event list, open the event, choose a category and enter. Show the Active status in My events. Open Profile, edit the name and upload a profile picture.
4. **Publish a result.** Log back in as the Organiser. Show the count on the dashboard and the category and status on Entries & results. To demonstrate a finished event without waiting, change the demonstration event date to yesterday. Publish a result such as 01:02:34 in position 1. Log in as the Participant and open My results. Show the event name, date, category, time and position.
5. **Explain the code.** Open `RaceDay.Mvc/Controllers/OrganiserController.cs` at Edit, then its Razor form under `Views/Organiser/Edit.cshtml`. Show how the form goes to the controller, which sends the data to the API. In `RaceDay.Mvc/Services/ApiClient.cs`, show how each browser session keeps its own API cookie. In `RaceDay/Program.cs`, point to UseSqlServer. Explain that restarting the app keeps database records, but users must sign in again because sessions are in memory.
6. **Explain images and Docker.** Open `RaceDay/Controllers/ImagesController.cs` and `RaceDay/Storage/ImageStorage.cs`. Show the owner check, upload, the file in `RaceDay/Images`, and the filename saved in SQL. Then show the two Dockerfiles and `compose.yaml`. Run `docker compose up --build` before recording so the build can finish; show the containerised website and explain how the images volume keeps uploads when the container stops.
7. **Show checks and finish.** Show passing local tests and the actual GitHub Actions run. If GitHub reports a billing lock and never starts the job, explain that accurately; a local test result is not a GitHub green build.

Useful evidence: organiser dashboard with an enrolment count, event detail with a banner, participant profile picture, published result, the local images folder showing test uploads, the running Docker stack, and the successful GitHub Actions workflow. Do not show keys, connection strings, passwords or recovery codes.

Before submitting: place the Part 3 work in a clearly named branch or subfolder in your repository, complete the assignment's 20 meaningful commits, add the real CI screenshot and unlisted YouTube URL to the README, and submit the repository link on ARC.
