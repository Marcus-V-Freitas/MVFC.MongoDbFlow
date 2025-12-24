namespace MVFC.MongoDbFlow.Context;

/// <summary>
/// Implementação de contexto MongoDB para acesso ao banco de dados, sessão e resolução de coleções.
/// </summary>
internal sealed class MongoContext : IMongoContext
{
    /// <summary>
    /// Obtém a instância do banco de dados MongoDB.
    /// </summary>
    public IMongoDatabase Database { get; }

    /// <summary>
    /// Obtém a sessão atual do cliente MongoDB, se houver.
    /// </summary>
    public IClientSessionHandle? Session { get; }

    /// <summary>
    /// Obtém o resolvedor de nomes de coleções para as entidades.
    /// </summary>
    public ICollectionNameResolver Resolver { get; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="MongoContext"/> com o banco de dados, resolvedor de coleções e sessão informados.
    /// </summary>
    /// <param name="database">Instância do banco de dados MongoDB.</param>
    /// <param name="resolver">Resolvedor de nomes de coleções.</param>
    /// <param name="session">Sessão do cliente MongoDB (opcional).</param>
    internal MongoContext(
        IMongoDatabase database,
        ICollectionNameResolver resolver,
        IClientSessionHandle? session)
    {
        Database = database;
        Resolver = resolver;
        Session = session;
    }
}