namespace MVFC.MongoDbFlow.UnitOfWork;

/// <summary>
/// Fábrica para criar instâncias de <see cref="IMongoUnitOfWork"/>.
/// </summary>
/// <remarks>
/// Inicializa uma nova instância de <see cref="MongoUnitOfWorkFactory"/> com as dependências necessárias.
/// </remarks>
/// <param name="client">Cliente MongoDB utilizado para iniciar sessões de transação.</param>
/// <param name="database">Instância do banco de dados MongoDB.</param>
/// <param name="resolver">Resolvedor de nomes de coleções.</param>
internal sealed class MongoUnitOfWorkFactory(
    MongoClient client,
    IMongoDatabase database,
    ICollectionNameResolver resolver) : IMongoUnitOfWorkFactory
{
    private readonly MongoClient _client = client;
    private readonly IMongoDatabase _database = database;
    private readonly ICollectionNameResolver _resolver = resolver;

    /// <summary>
    /// Cria uma nova instância de <see cref="IMongoUnitOfWork"/> para manipulação de transações no MongoDB.
    /// </summary>
    /// <returns>Instância de <see cref="IMongoUnitOfWork"/> pronta para uso.</returns>
    public IMongoUnitOfWork Create()
        => new MongoUnitOfWork(_client, _database, _resolver);
}