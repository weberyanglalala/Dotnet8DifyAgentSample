namespace Dotnet8DifyAgentSample.Settings;

public class MongoDbVectorSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
    public string SearchIndexName { get; set; }
}