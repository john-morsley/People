namespace Morsley.UK.People.Application.Models;

public class PagedListOfPersonJsonConverter : JsonConverter<Morsley.UK.People.Application.Models.PagedList<Morsley.UK.People.Domain.Models.Person>>
{
    public override bool CanConvert(Type typeToConvert)
    {

        var isCovertable = typeof(Morsley.UK.People.Application.Models.PagedList<Morsley.UK.People.Domain.Models.Person>).IsAssignableFrom(typeToConvert);
        return isCovertable;
    }

    public override Morsley.UK.People.Application.Models.PagedList<Morsley.UK.People.Domain.Models.Person> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

        //var pagedList = PagedList<Morsley.UK.People.Domain.Models.Person>.Create();

        var people = new List<Morsley.UK.People.Domain.Models.Person>();
        //uint pageNumber = 0;
        uint pageSize = 0;
        uint currentPage = 0; // ToDo --> We need a SetCurrentPage method.
        uint totalPages = 0;
        uint totalCount = 0;
        //uint count = 0;

        while (reader.Read())
        {
            //if (reader.TokenType == JsonTokenType.EndObject) return pagedList;
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                var pagedList = PagedList<Morsley.UK.People.Domain.Models.Person>.Create(people);

                //pagedList.Count = people.Count;

                return pagedList;
            }

            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:

                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "HasNext":
                        case "HasPrevious":
                            break;
                        //case "Count":
                        //    count = reader.GetUInt32();
                        //    break;
                        case "CurrentPage":
                            currentPage = reader.GetUInt32();
                            break;
                        case "TotalPages":
                            totalPages = reader.GetUInt32();
                            break;
                        case "PageSize":
                            pageSize = reader.GetUInt32();
                            break;
                        case "TotalCount":
                            totalCount = reader.GetUInt32();
                            break;
                        case "Items":
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonTokenType.EndArray) break;
                                try
                                {
                                    var person = JsonSerializer.Deserialize<Morsley.UK.People.Domain.Models.Person>(ref reader, options);
                                    if (person != null) people.Add(person);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }
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

    public override void Write(Utf8JsonWriter writer, Morsley.UK.People.Application.Models.PagedList<Morsley.UK.People.Domain.Models.Person> value, JsonSerializerOptions options)
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