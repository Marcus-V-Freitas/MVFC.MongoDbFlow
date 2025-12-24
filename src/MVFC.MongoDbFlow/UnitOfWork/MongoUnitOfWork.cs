namespace MVFC.MongoDbFlow.UnitOfWork;

/// <summary>
/// Implementação de unidade de trabalho para transações MongoDB.
/// </summary>
internal sealed class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly IClientSessionHandle _session;
    private readonly IMongoDatabase _database;
    private readonly ICollectionNameResolver _resolver;
    private TransactionState _state = TransactionState.Active;

    /// <summary>
    /// Indica se a transação está ativa.
    /// </summary>
    internal bool IsActive => _state == TransactionState.Active;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="MongoUnitOfWork"/>, iniciando uma sessão e transação no MongoDB.
    /// </summary>
    /// <param name="client">Cliente MongoDB utilizado para iniciar a sessão.</param>
    /// <param name="database">Instância do banco de dados MongoDB.</param>
    /// <param name="resolver">Resolvedor de nomes de coleções.</param>
    public MongoUnitOfWork(MongoClient client, IMongoDatabase database, ICollectionNameResolver resolver)
    {
        _session = client.StartSession();
        _session.StartTransaction();
        _database = database;
        _resolver = resolver;
    }

    /// <summary>
    /// Obtém um repositório para a entidade e identificador especificados, utilizando a sessão da transação.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade.</typeparam>
    /// <typeparam name="TId">Tipo do identificador da entidade.</typeparam>
    /// <returns>Instância do repositório para a entidade informada.</returns>
    public IMongoRepository<T, TId> GetRepository<T, TId>() => 
        new MongoRepository<T, TId>(_database, _resolver, _session);

    /// <summary>
    /// Efetiva (commita) todas as operações realizadas na unidade de trabalho.
    /// </summary>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_state != TransactionState.Active)
            return;

        await _session.CommitTransactionAsync(ct);
        _state = TransactionState.Committed;
    }

    /// <summary>
    /// Cancela (aborta) todas as operações realizadas na unidade de trabalho.
    /// </summary>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task AbortAsync(CancellationToken ct = default)
    {
        if (_state != TransactionState.Active)
            return;

        await _session.AbortTransactionAsync(ct);
        _state = TransactionState.Aborted;
    }

    /// <summary>
    /// Libera os recursos utilizados pela unidade de trabalho, abortando a transação se ainda estiver ativa.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_state == TransactionState.Active)
        {
            await _session.AbortTransactionAsync();
            _state = TransactionState.Aborted;
        }

        _session.Dispose();
    }
}