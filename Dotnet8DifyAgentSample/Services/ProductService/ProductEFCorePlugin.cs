using System.ComponentModel;
using Dotnet8DifyAgentSample.Models;
using Microsoft.SemanticKernel;

namespace Dotnet8DifyAgentSample.Services.ProductService;

public class ProductEFCorePlugin
{
    private readonly ProductServiceByEFCore _productService;

    public ProductEFCorePlugin(ProductServiceByEFCore productService)
    {
        _productService = productService;
    }

    [KernelFunction("CreateNewProduct")]
    [Description("Create a new product in the database")]
    public async Task<Product> CreateProduct(
        [Description("The name of the product")]
        string name,
        [Description("The sale price of the product")]
        decimal salePrice,
        [Description("The desciption of the product")]
        string description)
    {
        var product = new Product()
        {
            Name = name,
            SalePrice = salePrice,
            Description = description,
            SaleCount = 0
        };
        return await _productService.CreateAsync(product);
    }

    [KernelFunction("UpdateProductById")]
    [Description("Update a product in the database")]
    public async Task<Product> UpdateProduct(
        [Description("The id of the product")] int id,
        [Description("The name of the product")]
        string name,
        [Description("The sale price of the product")]
        decimal salePrice,
        [Description("The desciption of the product")]
        string description)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            throw new Exception("Product not found");
        }

        product.Name = name;
        product.SalePrice = salePrice;
        product.Description = description;

        return await _productService.UpdateAsync(product);
    }
    
    [KernelFunction("GetProductFilteredResultCount")]
    [Description("Get the count of products that match the filter criteria")]
    public async Task<int> GetFilteredProductCount(
        [Description("The name filter")] string nameFilter,
        [Description("The minimum sale price filter")] decimal? minPrice,
        [Description("The maximum sale price filter")] decimal? maxPrice)
    {
        var productsCount = await _productService.GetFilteredProductCount(nameFilter, minPrice, maxPrice);
        return productsCount;
    }
    
    [KernelFunction("GetProductFilteredResult")]
    [Description("Get the products that match the filter criteria")]
    public async Task<IEnumerable<Product>> GetFilteredProducts(
        [Description("The name filter")] string nameFilter,
        [Description("The minimum sale price filter could be null")] decimal? minPrice,
        [Description("The maximum sale price filter could be null")] decimal? maxPrice,
        [Description("The page number default page 1")] int page,
        [Description("The page size default is 10")] int pageSize)
    {
        var products = await _productService.GetFilteredAsync(nameFilter, minPrice, maxPrice, page, pageSize);
        return products;
    }
}