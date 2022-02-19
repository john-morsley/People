namespace Users.API.Models.Request.v1;

[ModelBinder(BinderType = typeof(GetUserRequestBinder))]
public class GetUserRequest
{
    public Guid Id { get; set; }

    public string? Fields { get; set; }
}

public class GetUserRequestBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // To read from body
        var memoryStream = new MemoryStream();
        var body = bindingContext.HttpContext.Request.Body;
        var reader = new StreamReader(body, Encoding.UTF8);
        var text = reader.ReadToEnd();
        //var cat = JsonConvert.DeserializeObject<Cat>(text);

        var vp = bindingContext.ValueProvider.GetValue("userId");

        //Guid userId;
        if (!Guid.TryParse(vp.FirstValue, out var userId)) return;

        var getUserRequest = new Users.API.Models.Request.v1.GetUserRequest();
        getUserRequest.Id = userId;

        var properties = typeof(Users.API.Models.Request.v1.GetUserRequest).GetProperties();
        foreach (var property in properties)
        {
            if (property.Name.ToLower() == "id") continue;
            var valueProvider = bindingContext.ValueProvider.GetValue(property.Name);
            property.SetValue(getUserRequest, valueProvider.FirstValue);
        }

        bindingContext.Result = ModelBindingResult.Success(getUserRequest);

        await Task.CompletedTask;
    }
}

