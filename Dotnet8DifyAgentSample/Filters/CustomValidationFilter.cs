using System.Reflection;
using System.Text.Json.Serialization;
using Dotnet8DifyAgentSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dotnet8DifyAgentSample.Filters
{
    public class CustomValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => GetJsonPropertyName(context.ActionDescriptor.Parameters.FirstOrDefault()?.ParameterType, kvp.Key),
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var apiResponse = new ApiResponse
                {
                    IsSuccess = false,
                    Code = ApiStatusCode.BadRequest,
                    Body = new
                    {
                        title = "One or more validation errors occurred.",
                        errors = errors
                    }
                };

                context.Result = new BadRequestObjectResult(apiResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method is not needed for validation, but must be implemented
        }

        private string GetJsonPropertyName(Type type, string propertyName)
        {
            if (type == null)
                return propertyName;

            var property = type.GetProperty(propertyName);
            if (property == null)
                return propertyName;

            var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            return attribute?.Name ?? propertyName;
        }
    }
}