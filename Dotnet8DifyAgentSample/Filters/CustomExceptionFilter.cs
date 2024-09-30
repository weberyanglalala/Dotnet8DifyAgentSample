using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using Dotnet8DifyAgentSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dotnet8DifyAgentSample.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var request = context.HttpContext.Request;
            var userId = context.HttpContext.User.Identity?.Name ?? "Anonymous";
	 
            request.Body.Position = 0;
            var requestDetails = new Dictionary<string, object>
            {
                ["UserId"] = userId,
                ["Path"] = request.Path.Value,
                ["Method"] = request.Method,
                ["Headers"] = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                ["QueryString"] = HttpUtility.UrlDecode(request.QueryString.Value, Encoding.UTF8)
            };

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var requestDetailsJson = JsonSerializer.Serialize(requestDetails, options);

            _logger.LogError(context.Exception,
                "An error occurred while processing the request. Request Details: {RequestDetails}",
                requestDetailsJson);

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Code = ApiStatusCode.Error,
                Body = "An error occurred while processing the request."
            };

            context.Result = new OkObjectResult(apiResponse);
            context.ExceptionHandled = true;
        }
    }
}