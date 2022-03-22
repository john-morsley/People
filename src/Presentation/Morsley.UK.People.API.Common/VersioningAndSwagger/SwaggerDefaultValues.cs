//namespace Morsley.UK.People.API.Common.VersioningAndSwagger
//{
//    public class SwaggerDefaultValues : IOperationFilter
//    {
//        public void Apply(
//            OpenApiOperation operation, 
//            OperationFilterContext context)
//        {
//            if (operation == null) throw new ArgumentNullException(nameof(operation));
//            if (context == null) throw new ArgumentNullException(nameof(context));

//            var apiDescription = context.ApiDescription;

//            operation.Deprecated |= apiDescription.IsDeprecated();

//            if (operation.Parameters == null) return;

//            foreach (var parameter in operation.Parameters)
//            {
//                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

//                if (parameter.Description == null)
//                {
//                    parameter.Description = description.ModelMetadata?.Description;
//                }

//                if (parameter.Schema.Default == null && description.DefaultValue != null)
//                {
//                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
//                }
//                parameter.Required |= description.IsRequired;
//            }
//        }
//    }
//}
