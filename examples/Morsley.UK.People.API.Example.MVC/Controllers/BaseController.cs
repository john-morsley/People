namespace Morsley.UK.People.API.Example.MVC.Controllers;

public abstract class BaseController : Controller
{
    protected const string ReadPersonAPIBaseURL = "https://localhost:5001";
    protected const string WritePersonAPIBaseURL = "https://localhost:5002";

    protected const string LoginPath = "/api/login";
    protected const string PersonPath = "/api/person";
    protected const string PeoplePath = "/api/people";

    protected const int DefaultClientTimeout = 100;

    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;

    public BaseController(ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private string AddParametersToURL(string url, GetPeople getPeople)
    {
        var parameters = new List<string>();
        if (getPeople.PageNumber > 0) parameters.Add($"pageNumber={getPeople.PageNumber}");
        if (getPeople.PageSize > 0) parameters.Add($"pageSize={getPeople.PageSize}");
        if (!string.IsNullOrWhiteSpace(getPeople.Fields)) parameters.Add($"fields={getPeople.Fields}");
        if (!string.IsNullOrWhiteSpace(getPeople.Filter)) parameters.Add($"filter={getPeople.Filter}");
        if (!string.IsNullOrWhiteSpace(getPeople.Search)) parameters.Add($"search={getPeople.Search}");
        if (!string.IsNullOrWhiteSpace(getPeople.Sort)) parameters.Add($"sort={getPeople.Sort}");
        if (parameters.Count > 0) url = $"{url}?{string.Join('&', parameters)}";
        return url;
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
            var client = GetHttpClient();

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
                            new PersonResourceJsonConverter(),
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
            var client = GetHttpClient();

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

    protected HttpClient GetHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = GetClientTimeout();
        return new HttpClient();
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

    protected async Task<PersonResource?> GetPeopleAsync(GetPeople getPeople)
    {
        try
        {
            var client = GetHttpClient();

            await AuthenticateReadAsync(client, GetUsername(), GetPassword());

            var url = $"{ReadPersonAPIBaseURL}{PeoplePath}";
            url = AddParametersToURL(url, getPeople);

            var result = await client.GetAsync(url);

            if (!result.IsSuccessStatusCode) return null;

            if (result.StatusCode != HttpStatusCode.OK)
            {
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new PersonResourceJsonConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            var personResource = JsonSerializer.Deserialize<PersonResource>(content, options);

            return personResource;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected async Task<PersonResource> GetPersonAsync(string path)
    {
        try
        {
            var client = GetHttpClient();
            //client.Timeout = GetClientTimeout();

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
                        new PersonResourceJsonConverter(),
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

    private TimeSpan GetClientTimeout()
    {
        var potentialSeconds = _configuration["Client:Timeout"];

        if (int.TryParse(potentialSeconds, out var seconds))
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.FromSeconds(DefaultClientTimeout);
    }

    private static Sex? GetSex(string? potentialSex)
    {
        if (potentialSex == null) return null;
        return Enum.Parse<Sex>(potentialSex);
    }

    protected int? GetDay(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;
        if (!IsValidDate(date, out var result)) return null;
        return result.Day;
    }

    private Guid? GetId(string potentialGuid)
    {
        if (string.IsNullOrWhiteSpace(potentialGuid)) return null;
        if (!Guid.TryParse(potentialGuid, out var guid)) return null;
        return guid;
    }

    protected int? GetMonth(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;
        if (!IsValidDate(date, out var result)) return null;
        return result.Month;
    }

    private string GetUsername()
    {
        return _configuration["Authentication:Username"];
    }

    protected int? GetYear(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;
        if (!IsValidDate(date, out var result)) return null;
        return result.Year;
    }

    private bool IsValidDate(string date, out DateOnly result)
    {
        if (string.IsNullOrWhiteSpace(date)) return false;
        return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    protected async Task<PersonResource?> UpdatePersonAsync(UpdatePerson person, string path)
    {
        var personId = GetId(person.Id);

        if (personId is null) return null;

        var request = new UpdatePersonRequest {
            Id = personId.Value,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Sex = GetSex(person.Sex),
            Gender = GetGender(person.Gender),
            DateOfBirth = GetDate(person.Year, person.Month, person.Day)
        };
        //request.Email = person.Email;
        //request.Mobile = person.Mobile;

        try
        {
            var client = GetHttpClient();

            await AuthenticateWriteAsync(client, GetUsername(), GetPassword());

            var url = $"{WritePersonAPIBaseURL}{path}";

            var json = JsonSerializer.Serialize(request);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await client.PutAsync(url, payload);

            if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.OK)
            {
                var content = await result.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        new PersonResourceJsonConverter(),
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };

                var resource = JsonSerializer.Deserialize<PersonResource>(content, options);

                ViewBag.Message = "Person updated.";

                return resource;
            }
            else
            {
                ViewBag.Message = "Person was not updated.";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }

    
}
