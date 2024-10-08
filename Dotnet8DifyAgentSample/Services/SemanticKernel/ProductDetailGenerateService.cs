using Microsoft.SemanticKernel;

namespace Dotnet8DifyAgentSample.Services.SemanticKernel;

public class ProductDetailGenerateService
{
    private readonly Kernel _kernel;

    public ProductDetailGenerateService(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> CreateProductDetail(string productName)
    {
        var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        var plugin = _kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginDirectory, "ProductCreatePlugin"));
        var productDetailFunction = plugin["ProductDetail"];
        var response = await _kernel.InvokeAsync(productDetailFunction, arguments: new()
        {
            { "product_name", productName }
        });
        return response.ToString();
    }
}