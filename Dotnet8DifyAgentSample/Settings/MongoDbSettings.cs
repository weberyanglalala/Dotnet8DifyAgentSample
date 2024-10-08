namespace Dotnet8DifyAgentSample.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string VectorDatabaseName { get; set; }
    public string VectorCollectionName { get; set; }
    public string SearchIndexName { get; set; }
    public string LineMessageDatabaseName { get; set; }
}