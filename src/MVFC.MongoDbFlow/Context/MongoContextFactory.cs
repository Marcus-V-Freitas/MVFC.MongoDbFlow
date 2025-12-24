namespace MVFC.MongoDbFlow.Context;

/// <summary>
/// Fábrica para criar instâncias de <see cref="IMongoContext"/>.
/// </summary>
/// <remarks>
/// Inicializa uma nova instância de <see cref="MongoContextFactory"/> com o banco de dados e o resolvedor de coleções informados.
/// </remarks>
/// <param name="database">Instância do banco de dados MongoDB.</param>
/// <param name="resolver">Resolvedor de nomes de coleções.</param>
internal sealed class MongoContextFactory(IMongoDatabase database, ICollectionNameResolver resolver) : IMongoContextFactory
{
    private readonly IMongoDatabase _database = database;
    private readonly ICollectionNameResolver _resolver = resolver;

    /// <summary>
    /// Cria uma nova instância de <see cref="IMongoContext"/> sem sessão associada.
    /// </summary>
    /// <returns>Instância de <see cref="IMongoContext"/> pronta para uso.</returns>
    public IMongoContext Create() => 
        new MongoContext(_database, _resolver, session: null);
}