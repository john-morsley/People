﻿namespace Morsley.UK.People.API.Example.MVC.Controllers;

public abstract class BaseController : Controller
{
    protected const string ReadPersonAPIBaseURL = "https://localhost:5001";
    protected const string WritePersonAPIBaseURL = "https://localhost:5002";

    protected const string LoginPath = "/api/login";
    protected const string PersonPath = "/api/person";
    protected const string PeoplePath = "/api/people";

    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;

    public BaseController(ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private async Task AuthenticateAsync(HttpClient client, string username, string password, string url)
    {
        var token = await GetJwtTokenAsync(client, username, password, url);
        if (token == null) return;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    private async Task AuthenticateReadAsync(HttpClient client, string username, string password)
    {
        const string url = $"{ReadPersonAPIBaseURL}{LoginPath}";
        await AuthenticateAsync(client, username, password, url);
    }

    private async Task AuthenticateWriteAsync(HttpClient client, string username, string password)
    {
        const string url = $"{WritePersonAPIBaseURL}{LoginPath}";
        await AuthenticateAsync(client, username, password, url);
    }

    protected async Task<PersonResource?> CreatePersonAsync(Person person)
    {
        var request = new AddPersonRequest();
        request.FirstName = person.FirstName;
        request.LastName = person.LastName;
        request.Sex = GetSex(person.Sex);
        request.Gender = GetGender(person.Gender);
        request.DateOfBirth = GetDate(person.Year, person.Month, person.Day);
        //request.Email = person.Email;
        //request.Mobile = person.Mobile;

        try
        {
            var client = new HttpClient();

            await AuthenticateWriteAsync(client, GetUsername(), GetPassword());

            var url = $"{WritePersonAPIBaseURL}{PersonPath}";

            var json = JsonSerializer.Serialize(request);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(url, payload);

            if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.Created)
            {
                var content = await result.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                        {
                            new PersonResourceConverter(),
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        }
                };

                var personResource = JsonSerializer.Deserialize<PersonResource>(content, options);

                ViewBag.Message = "Person created.";

                return personResource;
            }
            else
            {
                ViewBag.Message = "Person was not created.";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }

    protected async Task DeletePersonAsync(string path)
    {
        try
        {
            var client = new HttpClient();

            await AuthenticateWriteAsync(client, GetUsername(), GetPassword());

            var url = $"{WritePersonAPIBaseURL}{path}";

            var result = await client.DeleteAsync(url);

            if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.Message = "Person deleted.";
            }
            else
            {
                ViewBag.Message = "Person was not deleted.";
            }
        }
        catch (Exception e)
        {
            ViewBag.Message = "An unexpected error occurred attempting to delete.";
        }
    }

    private static string? GetDate(string? potentialYear, string? potentialMonth, string? potentialDay)
    {
        if (potentialYear is null) return null;
        if (potentialMonth is null) return null;
        if (potentialDay is null) return null;
        var year = int.Parse(potentialYear);
        var month = int.Parse(potentialMonth);
        var day = int.Parse(potentialDay);
        var date = $"{year:000#}-{month:0#}-{day:0#}";
        return date;
    }

    private static Gender? GetGender(string? potentialGender)
    {
        if (potentialGender == null) return null;
        return Enum.Parse<Gender>(potentialGender);
    }

    private async Task<string?> GetJwtTokenAsync(HttpClient client, string username, string password, string url)
    {
        var request = new LoginRequest(username, password);
        var result = await client!.PostAsJsonAsync(url, request);
        if (!result.IsSuccessStatusCode) return null;
        if (result.StatusCode != HttpStatusCode.OK) return null;
        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
        if (response == null) return null;
        return response.Token;
    }

    private string GetPassword()
    {
        return _configuration["Authentication:Password"];
    }

    protected async Task<PersonResource> GetPeopleAsync(GetPeople getPeople)
    {
        try
        {
            var client = new HttpClient();

            await AuthenticateReadAsync(client, GetUsername(), GetPassword());

            var url = $"{ReadPersonAPIBaseURL}{PeoplePath}";

            var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        new PersonResourceConverter(),
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                var personResource = JsonSerializer.Deserialize<PersonResource>(content, options);

                return personResource;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        throw new NotImplementedException();
    }

    protected async Task<PersonResource> GetPersonAsync(string path)
    {
        try
        {
            var client = new HttpClient();

            await AuthenticateReadAsync(client, GetUsername(), GetPassword());

            var url = $"{ReadPersonAPIBaseURL}{path}";

            var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        new PersonResourceConverter(),
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                var personResource = JsonSerializer.Deserialize<PersonResource>(content, options);

                return personResource;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        throw new NotImplementedException();
    }

    private static Sex? GetSex(string? potentialSex)
    {
        if (potentialSex == null) return null;
        return Enum.Parse<Sex>(potentialSex);
    }

    private string GetUsername()
    {
        return _configuration["Authentication:Username"];
    }

}
