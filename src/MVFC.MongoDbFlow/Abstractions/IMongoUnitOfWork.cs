namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface de unidade de trabalho para transações MongoDB.
/// </summary>
public interface IMongoUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Obtém um repositório para a entidade e identificador especificados.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade.</typeparam>
    /// <typeparam name="TId">Tipo do identificador da entidade.</typeparam>
    /// <returns>Instância do repositório para a entidade informada.</returns>
    IMongoRepository<T, TId> GetRepository<T, TId>();

    /// <summary>
    /// Efetiva (commita) todas as operações realizadas na unidade de trabalho.
    /// </summary>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>
    /// Cancela (aborta) todas as operações realizadas na unidade de trabalho.
    /// </summary>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task AbortAsync(CancellationToken ct = default);
}