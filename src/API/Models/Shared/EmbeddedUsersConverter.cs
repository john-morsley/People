using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
//using Users.API.Models.Response.v1;

namespace Users.API.Models.Shared;

public class EmbeddedUsersConverter : JsonConverter<IEnumerable<Users.API.Models.Response.v1.UserResponse>>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IEnumerable<Users.API.Models.Response.v1.UserResponse>).IsAssignableFrom(typeToConvert);
    }

    public override IEnumerable<Users.API.Models.Response.v1.UserResponse> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        var users = new List<Users.API.Models.Response.v1.UserResponse>();

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
                        case "_embedded":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var user = JsonSerializer.Deserialize<Users.API.Models.Response.v1.UserResponse>(ref reader, options);
                                users.Add(user);
                            }

                            break;
                        default: break;
                    }

                    break;

                default: break;
            }
        }

        return users;

        //throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<Users.API.Models.Response.v1.UserResponse> value, JsonSerializerOptions options)
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
