namespace Morsley.UK.People.API.Example.Console;

public class Tasks
{
    private const string AddPersonURL = "https://localhost:5002/api/person";

    public async static Task AddPeople(HttpClient client, int count)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));

        var random = new Random(DateTime.Now.Millisecond);
        var randomPeople = new RandomPeople(random);
        var people = randomPeople.GetRandomPeople(count);

        for (var i = 0; i < count; i++)
        {
            var person = people[i];

            System.Console.Write($"Adding Person: {person} - ");

            var addPersonRequest = new AddPersonRequest
            {
                FirstName = person.FirstName,
                LastName = person.LastName
            };

            var addPersonRequestJson = System.Text.Json.JsonSerializer.Serialize(addPersonRequest);
            var payload = new StringContent(addPersonRequestJson, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync(AddPersonURL, payload);

            System.Console.WriteLine($"Result: '{result.StatusCode}'");
        }
    }
}
