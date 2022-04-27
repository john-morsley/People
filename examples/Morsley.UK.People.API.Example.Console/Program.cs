using Morsley.UK.People.API.Example.Console;

var count = 0;
do
{
    Console.Write("Enter the number of people to add (0 to quit): ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out count) && count > 0)
    {
        Console.WriteLine("--------------------------------------------------");

        var what = "person";
        if (count > 1) what = "people";
        Console.WriteLine($"OK, Adding {count} {what}, stand by...");

        Console.WriteLine("--------------------------------------------------");

        var client = new HttpClient();
        var isAuthenticated = await Security.AuthenticateAsync(client, "johnmorsley", "P@$$w0rd!");

        Console.WriteLine("--------------------------------------------------");

        if (isAuthenticated) await Tasks.AddPeople(client, count);

        Console.WriteLine("--------------------------------------------------");
    }
}
while (count != 0);


//if (!int.TryParse(input, out var count))
//{
//    Console.WriteLine("Not a number!");
//}
//else
//{
//    var url = "https://localhost:5002/api/person";
//    var client = new HttpClient();

//    await Security.AuthenticateAsync(client, "johnmorsley", "P@$$w0rd!");

//    var random = new Random(DateTime.Now.Millisecond);
//    var randomPeople = new RandomPeople(random);
//    var people = randomPeople.GetRandomPeople(count);

//    for (var i = 0; i < count; i++)
//    {
//        var person = people[i];

//        Console.WriteLine($"Adding: {person}");

//        var addPersonRequest = new AddPersonRequest
//        {
//            FirstName = person.FirstName,
//            LastName = person.LastName
//        };

//        var addPersonRequestJson = System.Text.Json.JsonSerializer.Serialize(addPersonRequest);
//        var payload = new StringContent(addPersonRequestJson, System.Text.Encoding.UTF8, "application/json");

//        var result = await client.PostAsync(url, payload);
        
//        Console.WriteLine($"Result: {result.StatusCode}");
//    }
//}

//Console.WriteLine("Press any key to quit.");
//Console.ReadLine();