using Microsoft.AspNetCore.Mvc.Infrastructure;
using Users.API.Models.IoC;

namespace Users.API.Write;

public class StartUp
{
    public IConfiguration Configuration { get; }
        
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddVersioningAndSwagger(Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        services.AddLogging(builder => builder.AddConsole().AddFilter(level => level >= LogLevel.Debug));

        services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        }).ConfigureApiBehaviorOptions(setupAction =>
        {
            setupAction.InvalidModelStateResponseFactory = context =>
            {
                var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var details = factory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                details.Detail = "See the errors field for deatils.";
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

        //services.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.Converters.Add(new Users.API.Models.Shared.PagedListJsonConverter());
        //});

        //services.AddFluentValidation(configuration => 
        //{
        //    configuration.RegisterValidatorsFromAssemblyContaining<StartUp>();
        //});

        services.AddModelValidation();
        services.AddPersistence(Configuration);
        services.AddApplication();
    }

    private bool AreWeDealingWithValidationErrors(ActionContext context)
    {
        if (context?.ModelState.ErrorCount <= 0) return false;
        var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
        if (actionExecutingContext?.ActionArguments.Count != context.ActionDescriptor.Parameters.Count) return false;
        return true;
    }

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
                    context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Oops, I didn't expect that to happen! :-(";
                    // ToDo --> Log error
                });
            });
        }

        applicationBuilder.ConfigureVersioningAndSwagger(apiVersionDescriptionProvider);

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
