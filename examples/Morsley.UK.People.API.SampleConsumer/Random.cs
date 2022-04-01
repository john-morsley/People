namespace Morsley.UK.People.API.SampleConsumer;

public class RandomPeople
{
    class NameList
    {
        public string[] boys { get; set; }
        public string[] girls { get; set; }
        public string[] last { get; set; }

        public NameList()
        {
            boys = new string[] { };
            girls = new string[] { };
            last = new string[] { };
        }
    }

    private Random _random;
    private List<string> _males;
    private List<string> _females;
    private List<string> _last;

    public RandomPeople(Random random)
    {
        _random = random;

        var json = File.ReadAllText("names.json");
        var names = JsonConvert.DeserializeObject<NameList>(json);

        _males = new List<string>(names.boys);
        _females = new List<string>(names.girls);
        _last = new List<string>(names.last);
    }

    public (string FirstName, string LastName) Generate(Sex sex)
    {
        var first = sex == Sex.Male ? _males[_random.Next(_males.Count)] : _females[_random.Next(_females.Count)];
        var last = _last[_random.Next(_last.Count)];

        return (first, last);
    }

    public List<Person> GetRandomPeople(int number)
    {
        var people = new List<Person>();

        for (var  i = 0; i < number; i++)
        {
            var sex = (Sex)_random.Next(0, 2);
            var name = Generate(sex);
            var person = new Person
            {
                FirstName = name.FirstName,
                LastName = name.LastName
            };
            people.Add(person);
        }

        return people;
    }
}
