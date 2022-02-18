using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Users.API.Models.Response.v1;

//using Users.API.Models.Response.v1;

namespace Users.API.Models.Shared;

//public class UserResourceConverter : JsonConverter<UserResource>
//{
//    public override bool CanConvert(Type typeToConvert)
//    {
//        return typeof(UserResource).IsAssignableFrom(typeToConvert);
//    }

//    public override UserResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        if (reader.TokenType != JsonTokenType.StartObject)  throw new JsonException("Expected StartObject token");

//        var userData = new UserResource();
//        var user = new UserResponse();

//        while (reader.Read())
//        {
//            //if (reader.TokenType == JsonTokenType.EndObject) return users;

//            switch (reader.TokenType)
//            {
//                case JsonTokenType.PropertyName:

//                    var propertyName = reader.GetString();
//                    reader.Read();
//                    switch (propertyName)
//                    {
//                        case "Id":
//                            user.Id = reader.GetGuid();
//                            break;
//                        case "FirstName":
//                            user.FirstName = reader.GetString();
//                            break;
//                        case "LastName":
//                            user.LastName = reader.GetString();
//                            break;
//                        case "DateOfBirth":
//                            var potentialDate = reader.GetString();
//                            if (string.IsNullOrWhiteSpace(potentialDate)) break;
//                            if (potentialDate.Length != 10) break;
//                            if (!DateTime.TryParseExact(potentialDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
//                            {
//                                throw new InvalidOperationException($"DateOfBirth is not valid: Expected format is 'yyyy-MM-dd', actual value: '{potentialDate}'");
//                            }
//                            user.DateOfBirth = dt.ToString("yyyy-MM-dd");
//                            break;
//                        case "Sex":

//                            //user.Sex = reader.GetString();
//                            break;
//                        case "Gender":
//                            //user.Gender = reader.GetString();
//                            break;
//                        case "_links":
//                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

//                            while (reader.Read())
//                            {
//                                if (reader.TokenType == JsonTokenType.EndArray) break;
//                                //var user = JsonSerializer.Deserialize<ExpandoObject>(ref reader, options);
//                                //users.Add(user);
//                            }

//                            break;
//                        case "_embedded":
//                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected StartArray token");

//                            while (reader.Read())
//                            {
//                                if (reader.TokenType == JsonTokenType.EndArray) break;
//                                //var user = JsonSerializer.Deserialize<ExpandoObject>(ref reader, options);
//                                //users.Add(user);
//                            }

//                            break;
//                        default: break;
//                    }

//                    break;

//                default: break;
//            }
//        }

//        if (user.Id != Guid.Empty) userData.User = user;
//        return userData;

//        //throw new JsonException("Expected EndObject token");
//    }

//    public override void Write(Utf8JsonWriter writer, UserResource value, JsonSerializerOptions options)
//    {
//        throw new NotImplementedException();

//        //writer.WriteStartObject();

//        //writer.WriteNumber("CurrentPage", value.CurrentPage);
//        //writer.WriteNumber("PageSize", value.PageSize);
//        //writer.WriteNumber("TotalPages", value.TotalPages);
//        //writer.WriteNumber("TotalCount", value.TotalCount);
//        //writer.WriteBoolean("HasPrevious", value.HasPrevious);
//        //writer.WriteBoolean("HasNext", value.HasNext);

//        //writer.WriteStartArray("Items");
//        //foreach(var item in value)
//        //{
//        //    var json = JsonSerializer.Serialize(item, options).Replace("\\", "");
//        //    writer.WriteRawValue(json);
//        //}
//        //writer.WriteEndArray();

//        //writer.WriteEndObject();
//    }
//}
