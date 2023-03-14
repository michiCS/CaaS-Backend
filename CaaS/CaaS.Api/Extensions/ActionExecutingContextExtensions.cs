using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Extensions;

public static class ActionExecutingContextExtensions
{
    public static object? GetValueByString(this ActionExecutingContext context, string propertyString)
    {
        object? result = null;

        if (propertyString.Contains("."))
        {
            var arr = propertyString.Split(".");
            var objectName = arr[0];
            var propertyName = arr[1];
            var typeName = context.ActionArguments[objectName];
            result = typeName?.GetType()?.GetProperty(propertyName)?.GetValue(typeName);
        }
        else
        {
            result = context.ActionArguments[propertyString];
        }

        return result;
    }
}
