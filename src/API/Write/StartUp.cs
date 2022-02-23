using FluentValidation.AspNetCore;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Users.API.Write;

/// <summary>
/// 
/// </summary>
public class StartUp
{
    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get; }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddVersioningAndSwagger(Configuration, GetExecutingAssemblyName());

        services.AddLogging(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Trace));

        services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        })
        .AddNewtonsoftJson(setupAction =>
        {
            setupAction.SerializerSettings.Converters.Add(new StringEnumConverter());
        })
        .ConfigureApiBehaviorOptions(setupAction =>
        {
            setupAction.InvalidModelStateResponseFactory = context =>
            {
                var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var details = factory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                details.Detail = "See the errors field for details.";
                details.Instance = context.HttpContext.Request.Path;

                if (AreWeDealingWithValidationErrors(context))
                {
                    details.Title = "Validation error(s) occurred!";
                    details.Status = StatusCodes.Status422UnprocessableEntity;
                    // details.Type = ""; // ???

                    return new UnprocessableEntityObjectResult(details)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                }

                details.Title = "Error(s) occurred!";
                details.Status = StatusCodes.Status400BadRequest;

                return new BadRequestObjectResult(details)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        //.AddJsonOptions(options =>
        //{
        //    //options.JsonSerializerOptions.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
        //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        //    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //});

        //services.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
        //});

        services.AddFluentValidation(configuration =>
        {
            configuration.RegisterValidatorsFromAssemblyContaining<StartUp>();
        });

        services.AddModels();
        services.AddPersistence(Configuration);
        services.AddApplication();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="applicationBuilder"></param>
    /// <param name="webHostEnvironment"></param>
    /// <param name="apiVersionDescriptionProvider"></param>
    public void Configure(
        IApplicationBuilder applicationBuilder, 
        IWebHostEnvironment webHostEnvironment,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (webHostEnvironment.IsDevelopment())
        {
            applicationBuilder.UseDeveloperExceptionPage();
        }
        else
        {
            //applicationBuilder.UseExceptionHandler(applicationBuilder => {
            //    applicationBuilder.Run(async context => {
            //        context.Response.StatusCode = 500;
            //        await context.Response.WriteAsync("Oops, I didn't expect that to happen!");
            //    });
            //});

            applicationBuilder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    //var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;
                    context.Response.ContentType = "text/html";
                    var httpResponseFeature = context.Response.HttpContext.Features.Get<IHttpResponseFeature>();
                    if (httpResponseFeature != null) httpResponseFeature.ReasonPhrase = "Oops, I didn't expect that to happen! :-(";
                    // ToDo --> Log error
                    await Task.CompletedTask;
                });
            });
        }

        applicationBuilder.ConfigureVersioningAndSwagger(apiVersionDescriptionProvider);

        //applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        //applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private bool AreWeDealingWithValidationErrors(ActionContext context)
    {
        if (context?.ModelState.ErrorCount <= 0) return false;

        //var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
        //if (actionExecutingContext != null)
        //{
        //    return actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count;
        //}

        if (context is ActionExecutingContext actionExecutingContext)
        {
            return actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count;
        }

        if (context == null) return false;

        foreach (var value in context.ModelState.Values)
        {
            if (value.ValidationState == ModelValidationState.Invalid)
            {
                foreach (var error in value.Errors)
                {
                    if (error.GetType().FullName == "Microsoft.AspNetCore.Mvc.ModelBinding.ModelError") return true;
                }
            }
        }

        return false;
    }

    private string GetExecutingAssemblyName()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        if (executingAssembly == null) throw new InvalidOperationException("Did not expect Assembly.GetExecutingAssembly() to return null.");
        var name = executingAssembly.GetName().Name;
        if (name == null) throw new InvalidOperationException("Did not expect Assembly.GetExecutingAssembly().GetName().Name to return null.");
        return name;
    }
}
