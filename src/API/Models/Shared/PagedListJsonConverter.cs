namespace Users.API.Models.Shared;

public class PagedListJsonConverter : JsonConverter<Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Users.API.Models.Shared.PagedList<Users.API.Models.Response.v1.UserResponse>).IsAssignableFrom(typeToConvert);
    }

    public override PagedList<UserResponse> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        var pagedList = new PagedList<UserResponse>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return pagedList;

            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:

                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case nameof(pagedList.HasNext):
                        case nameof(pagedList.HasPrevious):
                            break;
                        case nameof(pagedList.CurrentPage):
                            pagedList.CurrentPage = reader.GetUInt32();
                            break;
                        case nameof(pagedList.TotalPages):
                            pagedList.TotalPages = reader.GetUInt32();
                            break;
                        case nameof(pagedList.PageSize):
                            pagedList.PageSize = reader.GetUInt32();
                            break;
                        case nameof(pagedList.TotalCount):
                            pagedList.TotalCount = reader.GetUInt32();
                            break;
                        case "Items":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var user = JsonSerializer.Deserialize<UserResponse>(ref reader, options);
                                if (user != null) pagedList.Add(user);
                            }

                            break;
                        default:

                            break;
                    }
                    break;

                default:

                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, PagedList<UserResponse> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("CurrentPage", value.CurrentPage);
        writer.WriteNumber("PageSize", value.PageSize);
        writer.WriteNumber("TotalPages", value.TotalPages);
        writer.WriteNumber("TotalCount", value.TotalCount);
        writer.WriteBoolean("HasPrevious", value.HasPrevious);
        writer.WriteBoolean("HasNext", value.HasNext);

        writer.WriteStartArray("Items");
        foreach(var item in value)
        {
            var json = JsonSerializer.Serialize(item, options).Replace("\\", "");
            writer.WriteRawValue(json);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
