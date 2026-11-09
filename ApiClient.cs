using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Services;

public class ApiError(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

public class ApiClient(HttpClient http)
{
    private const string CookieName = "RaceDay.Session";

    public async Task<T> Read<T>(ISession session, string path)
    {
        using var response = await Send(session, HttpMethod.Get, path);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    public async Task<T> Write<T>(ISession session, HttpMethod method, string path, object data)
    {
        using var response = await Send(session, method, path, JsonContent.Create(data));
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    public async Task Delete(ISession session, string path)
    {
        using var response = await Send(session, HttpMethod.Delete, path);
    }

    public async Task Post(ISession session, string path)
    {
        using var response = await Send(session, HttpMethod.Post, path);
    }

    public async Task Upload(ISession session, string path, IFormFile image)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StreamContent(image.OpenReadStream()), "image", "upload");
        using var response = await Send(session, HttpMethod.Post, path, form);
    }

    public async Task<(byte[] Bytes, string Type)> Image(ISession session, string path)
    {
        using var response = await Send(session, HttpMethod.Get, path);
        return (await response.Content.ReadAsByteArrayAsync(),
            response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
    }

    private async Task<HttpResponseMessage> Send(ISession session, HttpMethod method, string path, HttpContent? body = null)
    {
        using var request = new HttpRequestMessage(method, path) { Content = body };
        var cookie = session.GetString("ApiCookie");
        if (cookie != null) request.Headers.TryAddWithoutValidation("Cookie", cookie);

        HttpResponseMessage response;
        try { response = await http.SendAsync(request); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        { throw new ApiError(503, "RaceDay could not reach the API. Check that the API is running and try again."); }

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var value in cookies)
            {
                var pair = value.Split(';', 2)[0];
                if (pair.StartsWith(CookieName + "=", StringComparison.Ordinal))
                    session.SetString("ApiCookie", pair);
            }
        }

        if (response.IsSuccessStatusCode) return response;
        using (response)
        {
            string message = (int)response.StatusCode switch
            {
                401 => "Please log in again, or check your email and password.",
                403 => "Your account cannot use this feature.",
                404 => "That record could not be found.",
                413 => "Choose an image no larger than 5 MB.",
                >= 500 => "The API could not complete the request. Try again shortly.",
                _ => "Check the information you entered and try again."
            };

            // Show short API validation messages; never show server exception pages.
            if ((int)response.StatusCode is 400 or 409)
            {
                string error = await response.Content.ReadAsStringAsync();
                if (response.Content.Headers.ContentType?.MediaType == "text/plain" && error.Length < 300)
                    message = error;
                else
                {
                    try
                    {
                        using var json = JsonDocument.Parse(error);
                        if (json.RootElement.TryGetProperty("errors", out var errors))
                            message = string.Join(" ", errors.EnumerateObject().SelectMany(x =>
                                x.Value.EnumerateArray().Select(y => y.GetString())));
                        else if (json.RootElement.TryGetProperty("message", out var detail))
                            message = detail.GetString() ?? message;
                    }
                    catch (JsonException) { }
                }
            }
            throw new ApiError((int)response.StatusCode, message);
        }
    }
}
