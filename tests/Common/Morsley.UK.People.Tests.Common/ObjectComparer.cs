namespace Morsley.UK.People.Tests.Common;

public class ObjectComparer
{
    public static bool PublicInstancePropertiesEqual<T1, T2>(T1 obj1, T2 obj2, params string[] ignore)
        where T1 : class
        where T2 : class
    {
        var ignoreList = new List<string>(ignore);

        var all = new List<string>();
        var matched = new List<string>();

        foreach (var obj1pi in typeof(T1).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if (!ignoreList.Contains(obj1pi.Name))
            {
                foreach (var obj2pi in typeof(T2).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(obj2pi.Name))
                    {
                        if (!all.Contains(obj1pi.Name)) all.Add(obj1pi.Name);
                        if (!all.Contains(obj2pi.Name)) all.Add(obj2pi.Name);

                        if (obj1pi.Name != obj2pi.Name) continue;

                        matched.Add(obj1pi.Name);

                        var value1 = typeof(T1).GetProperty(obj2pi.Name)?.GetValue(obj1, null);
                        var value2 = typeof(T2).GetProperty(obj2pi.Name)?.GetValue(obj2, null);

                        if (AreValuesEqual(value1, value2)) continue;

                        Assert.Fail($"{obj1pi.Name} - Expected: '{value1}', but found: '{value2}'");

                        return false;
                    }
                }
            }
        }

        var unmatched = new List<string>();
        foreach (var name in all)
        {
            if (!matched.Contains(name)) unmatched.Add(name);
        }

        if (unmatched.Count > 0)
        {
            Assert.Fail($"Properties that were not matched: {string.Join(" | ", unmatched)}");
            return false;
        }

        return true;
    }

    private static bool AreValuesEqual(object? obj1, object? obj2)
    {
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null && obj2 != null) return false;
        if (obj1 != null && obj2 == null) return false;
        if (obj1!.GetType().Name is nameof(Morsley.UK.People.Domain.Enumerations.Gender) or nameof(Morsley.UK.People.Domain.Enumerations.Sex))
        {
            obj1 = obj1.ToString();
        }
        if (obj2!.GetType().Name is nameof(Morsley.UK.People.Domain.Enumerations.Gender) or nameof(Morsley.UK.People.Domain.Enumerations.Sex))
        {
            obj2 = obj2.ToString();
        }
        // Is obj1 a string representation of an international date? e.g YYYY-MM-DD
        if (obj1!.GetType().Name is nameof(System.DateTime))
        {
            obj1 = ((DateTime)obj1).ToString("yyyy-MM-dd");
        }
        if (obj2!.GetType().Name is nameof(System.DateTime))
        {
            obj2 = ((DateTime)obj2).ToString("yyyy-MM-dd");
        }
        return Equals(obj1, obj2);
    }
}