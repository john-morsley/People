using Microsoft.Extensions.Configuration.Memory;

namespace Morsley.UK.People.Tests.Common;

public class Configurator
{
    private IList<IConfiguration> _configurations;

    public Configurator()
    {
        _configurations = new List<IConfiguration>();
    }

    public Configurator AddConfiguration(IConfiguration configuration)
    {
        _configurations.Add(configuration);
        return this;
    }

    public Configurator AddConfiguration(IDictionary<string, string> values)
    {
        var cb = new ConfigurationBuilder();
        var cs = new MemoryConfigurationSource();
        var data = new List<KeyValuePair<string, string>>();
        data.AddRange(values);
        cs.InitialData = data;
        cb.Add(cs);
        var c = cb.Build();
        AddConfiguration(c);
        return this;
    }

    public IConfiguration Build()
    {
        var cb = new ConfigurationBuilder();
        var cs = new MemoryConfigurationSource();
        var data = new List<KeyValuePair<string, string>>();
        foreach (var configuration in _configurations)
        {
            foreach (var child in configuration.GetChildren())
            {
                var key = child.Key;
                var items = GetValuesForKey(child, key);
                RemoveOldKeyValuePairs(data, items);
                data.AddRange(items);
            }
        }
        cs.InitialData = data;
        cb.Add(cs);
        var c = cb.Build();
        return c;
    }

    private IList<KeyValuePair<string, string>> GetValuesForKey(IConfigurationSection? section, string key)
    {
        var value = section.Value;
        var children = section.GetChildren();
        if (!children.Any())
        {
            var kvp = new KeyValuePair<string, string>(key, value);
            return new List<KeyValuePair<string, string>>{ kvp };
        }

        var result = new List<KeyValuePair<string, string>>();
        foreach (var child in children)
        {
            var childKey = key + ":" + child.Key;
            var kvp = GetValuesForKey(child, childKey);
            result.AddRange(kvp);
        }

        return result;
    }

    private void RemoveOldKeyValuePairs(IList<KeyValuePair<string, string>> oldItems, IList<KeyValuePair<string, string>> newItems)
    {
        var remove = new List<KeyValuePair<string, string>>();

        foreach (var oldItem in oldItems)
        {
            foreach (var newItem in newItems)
            {
                if (oldItem.Key == newItem.Key) remove.Add(oldItem);
            }
        }

        var keep = new List<KeyValuePair<string, string>>();

        foreach (var kvp in remove)
        {
            oldItems.Remove(kvp);
        }

        //return oldItems;
    }
}
