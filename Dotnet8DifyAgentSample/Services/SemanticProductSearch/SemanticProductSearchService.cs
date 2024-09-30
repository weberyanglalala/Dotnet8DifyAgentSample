using System.Diagnostics.CodeAnalysis;
using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.ProductService;
using Dotnet8DifyAgentSample.Services.SemanticProductSearch.Dtos;
using Dotnet8DifyAgentSample.Settings;
using Microsoft.SemanticKernel.Connectors.MongoDB;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using MongoDB.Driver;

namespace Dotnet8DifyAgentSample.Services.SemanticProductSearch;

[Experimental("SKEXP0020")]
public class SemanticProductSearchService
{
    private readonly ProductServiceByEFCore _productServiceByEFCore;
    private readonly MemoryBuilder _memoryBuilder;
    private readonly ISemanticTextMemory _semanticTextMemory;
    private readonly MongoDbVectorSettings _mongoDbVectorSettings;
    private readonly MongoDBMemoryStore _mongoDBMemoryStore;
    private readonly string _openAiApiKey;
    private readonly string _connectionString;
    private readonly string _searchIndexName;
    private readonly string _databaseName;
    private readonly string _collectionName;
    private readonly IMongoClient _mongoClient;
    private readonly ILogger<SemanticProductSearchService> _logger;

    public SemanticProductSearchService(MongoDbVectorSettings mongoDbVectorSettings, IMongoClient mongoClient,
        IConfiguration configuration, ILogger<SemanticProductSearchService> logger, ProductServiceByEFCore productServiceByEfCore)
    {
        _mongoDbVectorSettings = mongoDbVectorSettings;
        _connectionString = _mongoDbVectorSettings.ConnectionString;
        _searchIndexName = _mongoDbVectorSettings.SearchIndexName;
        _databaseName = _mongoDbVectorSettings.DatabaseName;
        _collectionName = _mongoDbVectorSettings.CollectionName;
        _openAiApiKey = configuration["OpenAIApiKey"];
        _mongoDBMemoryStore = new MongoDBMemoryStore(_connectionString, _databaseName, _searchIndexName);
        _memoryBuilder = new MemoryBuilder();
        _memoryBuilder.WithOpenAITextEmbeddingGeneration("text-embedding-ada-002", _openAiApiKey);
        _memoryBuilder.WithMemoryStore(_mongoDBMemoryStore);
        _semanticTextMemory = _memoryBuilder.Build();
        _mongoClient = mongoClient;
        _logger = logger;
        _productServiceByEFCore = productServiceByEfCore;
    }

    public async Task<List<ProductSearchResult>> GetRecommendationsAsync(string userInput)
    {
        var memories = _semanticTextMemory.SearchAsync(_collectionName, userInput, limit: 10, minRelevanceScore: 0.6);

        var result = new List<ProductSearchResult>();
        await foreach (var memory in memories)
        {
            var productSearchResult = new ProductSearchResult
            {
                Id = memory.Metadata.Id,
                Description = memory.Metadata.Description,
                Name = memory.Metadata.AdditionalMetadata,
                Relevance = memory.Relevance.ToString("0.00")
            };
            result.Add(productSearchResult);
        }

        return result;
    }

    public async Task FetchAndSaveProductDocumentsAsync(int startIndex, int limitSize)
    {
        await FetchAndSaveProductDocuments(_semanticTextMemory, startIndex, limitSize);
    }

    private async Task FetchAndSaveProductDocuments(ISemanticTextMemory memory, int startIndex, int limitSize)
    {
        
        List<Product> products = _productServiceByEFCore.GetProductsByPageAsQueryable(startIndex, limitSize).ToList();
        foreach (var product in products)
        {
            try
            {
                _logger.LogInformation($"Processing {product.Id}...");
                await memory.SaveInformationAsync(
                    collection: _collectionName,
                    text: product.Description,
                    id: product.Id.ToString(),
                    description: product.Description,
                    additionalMetadata: product.Name
                );
                _logger.LogInformation($"Done {product.Id}...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}