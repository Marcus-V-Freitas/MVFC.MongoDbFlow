namespace MVFC.MongoDbFlow.UnitOfWork;

/// <summary>
/// Gerencia o escopo de uma transação MongoDB, garantindo commit ou dispose.
/// </summary>
/// <remarks>
/// Inicializa uma nova instância de <see cref="MongoTransactionScope"/> utilizando a fábrica de unidade de trabalho informada.
/// </remarks>
/// <param name="factory">Fábrica responsável por criar a unidade de trabalho para a transação.</param>
public sealed class MongoTransactionScope(IMongoUnitOfWorkFactory factory) : IAsyncDisposable
{
    private readonly IMongoUnitOfWork _uow = factory.Create();

    /// <summary>
    /// Obtém a unidade de trabalho associada ao escopo da transação.
    /// </summary>
    public IMongoUnitOfWork Uow => _uow;

    /// <summary>
    /// Efetiva (commita) todas as operações realizadas dentro do escopo da transação.
    /// </summary>
    public async Task CommitAsync() =>
        await _uow.CommitAsync();

    /// <summary>
    /// Libera os recursos utilizados pelo escopo da transação, descartando a unidade de trabalho.
    /// </summary>
    public async ValueTask DisposeAsync() =>
        await _uow.DisposeAsync();
}