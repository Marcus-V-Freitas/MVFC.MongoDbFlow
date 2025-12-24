namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Fábrica para criar instâncias de <see cref="IMongoUnitOfWork"/>.
/// </summary>
public interface IMongoUnitOfWorkFactory
{
    /// <summary>
    /// Cria uma nova instância de <see cref="IMongoUnitOfWork"/> para manipulação de transações no MongoDB.
    /// </summary>
    /// <returns>Instância de <see cref="IMongoUnitOfWork"/> pronta para uso.</returns>
    IMongoUnitOfWork Create();
}