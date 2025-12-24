namespace MVFC.MongoDbFlow.Config;

/// <summary>
/// Opções de configuração para conexão com o MongoDB.
/// </summary>
public sealed record MongoOptions(
    string ConnectionString, 
    string DatabaseName);