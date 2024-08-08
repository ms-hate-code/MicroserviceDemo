using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MicroserviceDemo.BuildingBlock.Swagger;

public class SwaggerSecurityRequirements: IOperationFilter
{
    private const string AuthType = "Bearer";

    private static readonly OpenApiSecurityRequirement Requirement = new()
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = AuthType
            },
            Scheme = "oauth2",
            Name = AuthType,
            In = ParameterLocation.Header
        },
        Array.Empty<string>()
    }};

    private static bool HasAttribute(MethodInfo methodInfo, Type type, bool inherit)
    {
        var actionAttributes = methodInfo.GetCustomAttributes(inherit);
        var controllerAttributes = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(inherit);
        var actionAndControllerAttributes = actionAttributes.Union(controllerAttributes);

        return actionAndControllerAttributes.Any(attr => attr.GetType() == type);
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(AuthorizeAttribute), true);
        bool hasAnonymousAttribute = HasAttribute(context.MethodInfo, typeof(AllowAnonymousAttribute), true);

        bool isAuthorized = hasAuthorizeAttribute && !hasAnonymousAttribute;
        if (isAuthorized)
        {
            operation.Security = [Requirement];
        }
    }
}