using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Users.API.Models.Response.v1;

//using Users.API.Models.Response.v1;

namespace Users.API.Models.Shared;

public class UserDataConverter : JsonConverter<UserData>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(UserData).IsAssignableFrom(typeToConvert);
    }

    public override UserData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        var userData = new UserData();
        var user = new UserResponse();
        var links = new List<Link>();
        var embedded = new List<UserData>();

        while (reader.Read())
        {
            //if (reader.TokenType == JsonTokenType.EndObject) return users;

            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:

                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Id":
                            user.Id = reader.GetGuid();
                            break;
                        case "FirstName":
                            user.FirstName = reader.GetString();
                            break;
                        case "LastName":
                            user.LastName = reader.GetString();
                            break;
                        case "DateOfBirth":
                            var potentialDate = reader.GetString();
                            if (string.IsNullOrEmpty(potentialDate)) break;
                            if (potentialDate.Length != 10 || 
                                !DateTime.TryParseExact(potentialDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
                            {
                                throw new InvalidOperationException($"DateOfBirth is not valid: Expected format is 'YYYY-MM-DD', actual value: '{potentialDate}'");
                            }
                            user.DateOfBirth = dt.ToString("yyyy-MM-dd");
                            break;
                        case "Sex":
                            var potentialSex = reader.GetString();
                            if (string.IsNullOrEmpty(potentialSex)) break;
                            if (!Enum.TryParse<Sex>(potentialSex, ignoreCase: true, out var sex))
                            {
                                throw new InvalidOperationException($"Sex is not valid: Actual value: '{potentialSex}'");
                            }
                            user.Sex = sex;
                            break;
                        case "Gender":
                            var potentialGender = reader.GetString();
                            if (string.IsNullOrEmpty(potentialGender)) break;
                            if (!Enum.TryParse<Gender>(potentialGender, ignoreCase: true, out var gender))
                            {
                                throw new InvalidOperationException($"Gender is not valid: Actual value: '{potentialGender}'");
                            }
                            user.Gender = gender;
                            break;
                        case "_links":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var link = JsonSerializer.Deserialize<Link>(ref reader, options);
                                links.Add(link);
                            }

                            break;
                        case "_embedded":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var userDatum = JsonSerializer.Deserialize<UserData>(ref reader, options);
                                embedded.Add(userDatum);
                            }

                            break;
                        default: break;
                    }

                    break;

                default: break;
            }
        }

        if (user.Id != Guid.Empty) userData.User = user;
        if (links.Count > 0) userData.Links = links;
        if (embedded.Count > 0) userData.Embedded = embedded;
        return userData;

        //throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, UserData value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();

        //writer.WriteStartObject();

        //writer.WriteNumber("CurrentPage", value.CurrentPage);
        //writer.WriteNumber("PageSize", value.PageSize);
        //writer.WriteNumber("TotalPages", value.TotalPages);
        //writer.WriteNumber("TotalCount", value.TotalCount);
        //writer.WriteBoolean("HasPrevious", value.HasPrevious);
        //writer.WriteBoolean("HasNext", value.HasNext);

        //writer.WriteStartArray("Items");
        //foreach(var item in value)
        //{
        //    var json = JsonSerializer.Serialize(item, options).Replace("\\", "");
        //    writer.WriteRawValue(json);
        //}
        //writer.WriteEndArray();

        //writer.WriteEndObject();
    }
}
