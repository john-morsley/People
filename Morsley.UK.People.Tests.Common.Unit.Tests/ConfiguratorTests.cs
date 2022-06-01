using System.Collections.Generic;

namespace Morsley.UK.People.Tests.Common.Unit.Tests;

public class ConfiguratorTests
{
    [Test]
    [Category("Happy")]
    [TestCase("file-1a.json", "key-1,value-1a")]
    [TestCase("file-2a.json", "key-1,value-1a|key-2,value-2a")]
    [TestCase("file-3a.json", "key-1:key-2,value-3a")]
    [TestCase("file-4a.json", "key-1:key-2:key-3,value-4a")]
    [TestCase("file-5a.json", "key-1,value-1a|key-2,value-2a|key-3:key-4,value-3a|key-5:key-6,value-4a|key-7:key-8:key-9,value-5a|key-10:key-11:key-12,value-6a")]
    [TestCase("file-6a.json", "key-1:key-2,value-1a|key-1:key-3,value-2a")]
    public void SingleFile(string filename, string expected)
    {
        // Arrange...
        var appsettings = GetConfiguration(filename);
        var configurator = new Configurator();
        configurator.AddConfiguration(appsettings);

        // Act...
        var configuration = configurator.Build();

        // Assert...
        configuration.Should().NotBeNull();
        var pairs = expected.Split("|");
        foreach (var pair in pairs)
        {
            var kvp = pair.Split(",");
            var value = configuration[kvp[0]];
            value.Should().Be(kvp[1]);
        }
    }

    [Test]
    [Category("Happy")]
    [TestCase("file-1a.json|file-1b.json", "key-1,value-1b")]
    [TestCase("file-2a.json|file-2b.json", "key-1,value-1b|key-2,value-2b")]
    [TestCase("file-3a.json|file-3b.json", "key-1:key-2,value-3b")]
    [TestCase("file-4a.json|file-4b.json", "key-1:key-2:key-3,value-4b")]
    [TestCase("file-5a.json|file-5b.json", "key-1,value-1b|key-2,value-2b|key-3:key-4,value-3b|key-5:key-6,value-4b|key-7:key-8:key-9,value-5b|key-10:key-11:key-12,value-6b")]
    [TestCase("file-6a.json|file-6b.json", "key-1:key-2,value-1b|key-1:key-3,value-2b")]
    public void MultipleFiles(string filenames, string expected)
    {
        // Arrange...
        var configurator = new Configurator();
        foreach (var filename in filenames.Split("|"))
        {
            var appsettings = GetConfiguration(filename);
            configurator.AddConfiguration(appsettings);
        }

        // Act...
        var configuration = configurator.Build();

        // Assert...
        configuration.Should().NotBeNull();
        var pairs = expected.Split("|");
        foreach (var pair in pairs)
        {
            var kvp = pair.Split(",");
            var value = configuration[kvp[0]];
            value.Should().Be(kvp[1]);
        }
    }





    private IConfiguration GetConfiguration(string filename)
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFile(filename);

        var configuration = builder.Build();

        return configuration;
    }
}
