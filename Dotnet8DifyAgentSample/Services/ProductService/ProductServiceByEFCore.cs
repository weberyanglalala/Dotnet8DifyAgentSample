using System.Linq.Expressions;
using Dotnet8DifyAgentSample.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet8DifyAgentSample.Services.ProductService;

public class ProductServiceByEFCore
{
    private readonly SkDbContext _dbContext;

    public ProductServiceByEFCore(SkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _dbContext.Products.FindAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetFilteredAsync(
        string nameFilter = "",
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1, // Default page is 1
        int pageSize = 10) // Default page size is 10
    {
        var query = _dbContext.Products.AsQueryable();

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(p => p.Name.Contains(nameFilter));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.SalePrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.SalePrice <= maxPrice.Value);
        }

        // Apply pagination
        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        // Execute the query and return the result
        return await query.ToListAsync();
    }

    public async Task<int> GetFilteredProductCount(string nameFilter = "",
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var query = _dbContext.Products.AsQueryable();

        // Apply filters if provided
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(p => p.Name.Contains(nameFilter));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.SalePrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.SalePrice <= maxPrice.Value);
        }

        // Execute the query and return the result
        return await query.CountAsync();
    }

    public IQueryable<Product> GetProductsByPageAsQueryable(int skipIndex, int count)
    {
        return _dbContext.Products
            .OrderBy(p => p.Id)
            .Skip(skipIndex - 1)
            .Take(count);
    }
}