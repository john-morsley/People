namespace Morsley.UK.People.API.Common.IoC;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ConfigureVersioningAndSwagger(
        this IApplicationBuilder applicationBuilder,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (applicationBuilder == null) throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder.UseSwagger();
    
        applicationBuilder.UseSwaggerUI(options =>
        {
            // Build a Swagger endpoint for each discovered API version...
            //foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            //{
            //    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            //}
    
            options.RoutePrefix = "";
        });
            
        return applicationBuilder;
    }
}