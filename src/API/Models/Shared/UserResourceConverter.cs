namespace Users.API.Models.Shared;

public class UserResourceConverter : JsonConverter<UserResource>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(UserResource).IsAssignableFrom(typeToConvert);
    }

    public override UserResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        var userResource = new UserResource();
        var id = Guid.Empty;
        string? firstName = null;
        string? lastName = null;
        Sex? sex = null;
        Gender? gender = null;
        string? dateOfBirth = null;
        var links = new List<Link>();
        var embedded = new List<UserResource>();

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
                            id = reader.GetGuid();
                            break;
                        case "FirstName":
                            firstName = reader.GetString();
                            //if (!string.IsNullOrWhiteSpace(firstName)) data.FirstName = firstName;
                            break;
                        case "LastName":
                            lastName = reader.GetString();
                            //if (!string.IsNullOrWhiteSpace(lastName)) data.LastName = lastName;
                            break;
                        case "Sex":
                            var potentialSex = reader.GetString();
                            if (string.IsNullOrEmpty(potentialSex)) break;
                            if (!Enum.TryParse<Sex>(potentialSex, ignoreCase: true, out var resultSex))
                            {
                                throw new InvalidOperationException($"Sex is not valid: Actual value: '{potentialSex}'");
                            }
                            sex = resultSex;
                            break;
                        case "Gender":
                            var potentialGender = reader.GetString();
                            if (string.IsNullOrEmpty(potentialGender)) break;
                            if (!Enum.TryParse<Gender>(potentialGender, ignoreCase: true, out var resultGender))
                            {
                                throw new InvalidOperationException($"Gender is not valid: Actual value: '{potentialGender}'");
                            }
                            gender = resultGender;
                            break;
                        case "DateOfBirth":
                            var potentialDate = reader.GetString();
                            if (string.IsNullOrEmpty(potentialDate)) break;
                            if (potentialDate.Length != 10 || 
                                !DateTime.TryParseExact(potentialDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
                            {
                                throw new InvalidOperationException($"DateOfBirth is not valid: Expected format is 'YYYY-MM-DD', actual value: '{potentialDate}'");
                            }
                            dateOfBirth = dt.ToString("yyyy-MM-dd");
                            break;
                        case "_links":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var link = JsonSerializer.Deserialize<Link>(ref reader, options);
                                if (link != null) links.Add(link);
                            }

                            break;
                        case "_embedded":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                var embeddedUserResource = JsonSerializer.Deserialize<UserResource>(ref reader, options);
                                if (embeddedUserResource != null) embedded.Add(embeddedUserResource);
                            }

                            break;
                        default: break;
                    }

                    break;

                default: break;
            }
        }

        if (id != Guid.Empty)
        {
            var data = new UserResponse(id)
            {
                FirstName = firstName, 
                LastName = lastName,
                Sex = sex,
                Gender = gender,
                DateOfBirth = dateOfBirth
            };
            if (data.Id != Guid.Empty) userResource.AddData(data);
        }

        if (links.Count > 0) userResource.AddLinks(links);
        if (embedded.Count > 0) userResource.AddEmbedded(embedded);

        return userResource;
    }

    public override void Write(Utf8JsonWriter writer, UserResource value, JsonSerializerOptions options)
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
