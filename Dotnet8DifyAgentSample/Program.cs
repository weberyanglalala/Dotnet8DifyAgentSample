using System.Diagnostics.CodeAnalysis;
using Dotnet8DifyAgentSample.Filters;
using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Models.MongoDB;
using Dotnet8DifyAgentSample.Services.DifyWorkflow;
using Dotnet8DifyAgentSample.Services.OpenAI;
using Dotnet8DifyAgentSample.Services.ProductService;
using Dotnet8DifyAgentSample.Services.SemanticKernel;
using Dotnet8DifyAgentSample.Services.SemanticProductSearch;
using Dotnet8DifyAgentSample.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using MongoDB.Driver;
using Serilog;

namespace Dotnet8DifyAgentSample;

public class Program
{
    [Experimental("SKEXP0020")]
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
        builder.Services.AddScoped<ProductDetailGenerateService>();

        // Product Create Service
        builder.Services.AddDbContext<SkDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        builder.Services.AddScoped<ProductServiceByEFCore>();
        builder.Services.AddScoped<ProductEFCorePlugin>();

        builder.Services.AddScoped<ProductChatService>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddOpenAIChatCompletion("gpt-4o-2024-08-06", builder.Configuration["OpenAIApiKey"]);
            var productService = sp.GetRequiredService<ProductServiceByEFCore>();
            kernelBuilder.Plugins.AddFromObject(new ProductEFCorePlugin(productService));
            return new ProductChatService(kernelBuilder.Build());
        });

        // Product Semantic Search Service
        builder.Services
            .Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<MongoDbSettings>();
            return new MongoClient(settings.ConnectionString);
        });
        
        builder.Services.AddScoped<SemanticProductSearchService>();
        
        builder.Services.AddScoped<MongoRepository>();

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