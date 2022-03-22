using MediatR;
using Morsley.UK.People.Application.Models;

namespace Morsley.UK.People.API.Contracts.Shared
{
    public class PagedListJsonConverter : JsonConverter<Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>).IsAssignableFrom(typeToConvert);
        }

        public override Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

            
            //var pagedList = PagedList<PersonResponse>.Create();

            var people = new List<PersonResponse>();
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
                    var pagedList = new Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse>(people);

                    pagedList.Count = people.Count;

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
                                        var person = JsonSerializer.Deserialize<PersonResponse>(ref reader, options);
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

        public override void Write(Utf8JsonWriter writer, Morsley.UK.People.API.Contracts.Responses.PagedList<PersonResponse> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("CurrentPage", value.CurrentPage);
            writer.WriteNumber("PageSize", value.PageSize);
            writer.WriteNumber("TotalPages", value.TotalPages);
            writer.WriteNumber("TotalCount", value.TotalCount);
            writer.WriteBoolean("HasPrevious", value.HasPrevious);
            writer.WriteBoolean("HasNext", value.HasNext);

            writer.WriteStartArray("Items");
            //foreach(var item in value)
            //{
            //    var json = JsonSerializer.Serialize(item, options).Replace("\\", "");
            //    writer.WriteRawValue(json);
            //}
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
