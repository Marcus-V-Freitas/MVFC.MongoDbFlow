namespace MVFC.MongoDbFlow.Bootstrap;

/// <summary>
/// Inicializa e registra dependências principais do MongoDB.
/// </summary>
internal sealed class MongoBootstrap
{
    /// <summary>
    /// Obtém a instância do <see cref="MongoClient"/> utilizada para conexão com o MongoDB.
    /// </summary>
    internal MongoClient Client { get; }

    /// <summary>
    /// Obtém a instância do <see cref="IMongoDatabase"/> para acesso ao banco de dados MongoDB.
    /// </summary>
    internal IMongoDatabase Database { get; }

    /// <summary>
    /// Obtém o resolvedor de nomes de coleções para as entidades.
    /// </summary>
    internal ICollectionNameResolver Resolver { get; }

    /// <summary>
    /// Inicializa o bootstrap do MongoDB, configurando cliente, banco de dados, resolvedor de coleções,
    /// registrando serializadores customizados e mapeamentos de entidades.
    /// </summary>
    /// <param name="options">Opções de configuração do MongoDB.</param>
    /// <param name="serializers">Serializadores customizados a serem registrados.</param>
    /// <param name="maps">Mapeamentos de entidades a serem registrados.</param>
    internal MongoBootstrap(
        MongoOptions options,
        IEnumerable<ISerializerRegistration>? serializers,
        IEnumerable<IEntityMap>? maps)
    {
        Client = new MongoClient(options.ConnectionString);
        Database = Client.GetDatabase(options.DatabaseName);
        Resolver = new CollectionNameResolver(maps ?? []);

        MongoSerializerRegistry.RegisterAll(serializers ?? []);
        MongoMappingRegistry.RegisterAll(maps ?? []);
    }
}