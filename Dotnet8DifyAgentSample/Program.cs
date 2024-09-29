using Dotnet8DifyAgentSample.Filters;
using Dotnet8DifyAgentSample.Services.DifyWorkflow;
using Dotnet8DifyAgentSample.Services.OpenAI;
using Dotnet8DifyAgentSample.Services.SemanticKernel;
using Microsoft.SemanticKernel;
using Serilog;

namespace Dotnet8DifyAgentSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration));

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<DifyCreateProductService>();
        builder.Services.AddScoped<OpenAIService>();
        builder.Services.AddScoped<CustomExceptionFilter>();
        builder.Services.AddScoped<CustomValidationFilter>();
        builder.Services.AddControllers(options => { options.Filters.Add<CustomValidationFilter>(); })
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
        // Semantic Kernel Service
        builder.Services.AddSingleton<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddOpenAIChatCompletion("gpt-4o-2024-08-06", builder.Configuration["OpenAIApiKey"]);
            return kernelBuilder.Build();
        });
        builder.Services.AddScoped<ProductDetailGenerateClient>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}