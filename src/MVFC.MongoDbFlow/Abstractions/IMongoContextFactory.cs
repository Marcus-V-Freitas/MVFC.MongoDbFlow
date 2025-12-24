namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Fábrica para criar instâncias de <see cref="IMongoContext"/>.
/// </summary>
public interface IMongoContextFactory
{
    /// <summary>
    /// Cria uma nova instância de <see cref="IMongoContext"/> para acesso ao banco de dados MongoDB.
    /// </summary>
    /// <returns>Instância de <see cref="IMongoContext"/> pronta para uso.</returns>
    IMongoContext Create();
}