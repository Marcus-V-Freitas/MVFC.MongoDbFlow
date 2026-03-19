namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Contexto MongoDB para acesso ao banco de dados, resolução de nomes de coleções e manipulação de sessão.
/// </summary>
public interface IMongoContext
{
    /// <summary>
    /// Obtém a instância do banco de dados MongoDB.
    /// </summary>
    public IMongoDatabase Database { get; }

    /// <summary>
    /// Obtém o resolvedor de nomes de coleções para entidades.
    /// </summary>
    public ICollectionNameResolver Resolver { get; }

    /// <summary>
    /// Obtém a sessão atual do cliente MongoDB, se houver.
    /// </summary>
    public IClientSessionHandle? Session { get; }
}
