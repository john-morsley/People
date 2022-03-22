using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Morsley.UK.People.API.Common.Helpers;

public class ContentHelper
{
    public static long? CalculateLength<T>(T obj) where T : class
    {
        var encoderSettings = new TextEncoderSettings();
        encoderSettings.AllowRange(UnicodeRanges.All);
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(encoderSettings),
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(obj, options);

        json = System.Text.RegularExpressions.Regex.Unescape(json);

        return json.Length;
    }
}