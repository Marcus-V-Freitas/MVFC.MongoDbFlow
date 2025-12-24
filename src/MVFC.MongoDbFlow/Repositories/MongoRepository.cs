namespace MVFC.MongoDbFlow.Repositories;

/// <summary>
/// Implementação de repositório MongoDB para operações CRUD genéricas.
/// </summary>
internal sealed class MongoRepository<T, TId>(
    IMongoDatabase database,
    ICollectionNameResolver resolver,
    IClientSessionHandle? session = null) : IMongoRepository<T, TId>
{
    private readonly IMongoCollection<T> _collection = database.GetCollection<T>(resolver.Resolve<T>());
    private readonly IClientSessionHandle? _session = session;

    /// <summary>
    /// Insere uma nova entidade no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entity">Entidade a ser inserida.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task InsertAsync(
        T entity,
        CancellationToken ct = default)
    {
        if (_session is null)
            await _collection.InsertOneAsync(entity, default, ct);
        else
            await _collection.InsertOneAsync(_session, entity, cancellationToken: ct);
    }

    /// <summary>
    /// Insere múltiplas entidades no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entities">Coleção de entidades a serem inseridas.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task InsertManyAsync(
        IEnumerable<T> entities,
        CancellationToken ct = default)
    {
        if (_session is null)
            await _collection.InsertManyAsync(entities, cancellationToken: ct);
        else
            await _collection.InsertManyAsync(_session, entities, cancellationToken: ct);
    }

    /// <summary>
    /// Obtém uma entidade pelo identificador de forma assíncrona.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    public async Task<T?> GetByIdAsync(
        TId id,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);

        return _session is null
            ? await _collection.Find(filter).FirstOrDefaultAsync(ct)
            : await _collection.Find(_session, filter).FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Busca entidades que correspondam ao filtro especificado.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de entidades encontradas.</returns>
    public async Task<IReadOnlyList<T>> FindAsync(
        FilterDefinition<T> filter,
        FindOptions<T>? options = null,
        CancellationToken ct = default)
    {
        var cursor = _session is null
            ? await _collection.FindAsync(filter, options, ct)
            : await _collection.FindAsync(_session, filter, options, ct);

        return await cursor.ToListAsync(ct);
    }

    /// <summary>
    /// Busca entidades que correspondam ao filtro especificado e projeta o resultado.
    /// </summary>
    /// <typeparam name="TProjection">Tipo da projeção de retorno.</typeparam>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="projection">Definição da projeção.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de projeções encontradas.</returns>
    public async Task<IReadOnlyList<TProjection>> FindAsync<TProjection>(
        FilterDefinition<T> filter,
        ProjectionDefinition<T, TProjection> projection,
        FindOptions<T, TProjection>? options = null,
        CancellationToken ct = default)
    {
        options ??= new FindOptions<T, TProjection>();
        options.Projection = projection;

        var cursor = _session is null
            ? await _collection.FindAsync(filter, options, ct)
            : await _collection.FindAsync(_session, filter, options, ct);

        return await cursor.ToListAsync(ct);
    }

    /// <summary>
    /// Verifica se existe alguma entidade que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public async Task<bool> ExistsAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default)
    {
        var options = new FindOptions<T>
        {
            Limit = 1,
            Projection = Builders<T>.Projection.Include("_id")
        };

        var cursor = _session is null
            ? await _collection.FindAsync(filter, options, ct)
            : await _collection.FindAsync(_session, filter, options, ct);

        return await cursor.AnyAsync(ct);
    }

    /// <summary>
    /// Conta a quantidade de entidades que correspondem ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades encontradas.</returns>
    public async Task<long> CountAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default)
    {
        return _session is null
            ? await _collection.CountDocumentsAsync(filter, cancellationToken: ct)
            : await _collection.CountDocumentsAsync(_session, filter, cancellationToken: ct);
    }

    /// <summary>
    /// Atualiza uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a atualização foi realizada, caso contrário false.</returns>
    public async Task<bool> UpdateAsync(
        TId id,
        UpdateDefinition<T> update,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);

        var result = _session is null
            ? await _collection.UpdateOneAsync(filter, update, cancellationToken: ct)
            : await _collection.UpdateOneAsync(_session, filter, update, cancellationToken: ct);

        return result.MatchedCount > 0;
    }

    /// <summary>
    /// Atualiza múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades atualizadas.</returns>
    public async Task<long> UpdateManyAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default)
    {
        var result = _session is null
            ? await _collection.UpdateManyAsync(filter, update, cancellationToken: ct)
            : await _collection.UpdateManyAsync(_session, filter, update, cancellationToken: ct);

        return result.ModifiedCount;
    }

    /// <summary>
    /// Substitui uma entidade existente no banco de dados.
    /// </summary>
    /// <param name="entity">Entidade a ser substituída.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task ReplaceAsync(
        T entity,
        CancellationToken ct = default)
    {
        var id = GetEntityId(entity);
        var filter = Builders<T>.Filter.Eq("_id", id);

        if (_session is null)
            await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
        else
            await _collection.ReplaceOneAsync(_session, filter, entity, cancellationToken: ct);
    }

    /// <summary>
    /// Remove uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a exclusão foi realizada, caso contrário false.</returns>
    public async Task<bool> DeleteAsync(
        TId id,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);

        var result = _session is null
            ? await _collection.DeleteOneAsync(filter, ct)
            : await _collection.DeleteOneAsync(_session, filter, default, ct);

        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Remove múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades removidas.</returns>
    public async Task<long> DeleteManyAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default)
    {
        var result = _session is null
            ? await _collection.DeleteManyAsync(filter, ct)
            : await _collection.DeleteManyAsync(_session, filter, default, ct);

        return result.DeletedCount;
    }

    /// <summary>
    /// Obtém o valor da propriedade "Id" da entidade.
    /// </summary>
    /// <param name="entity">Entidade da qual o Id será extraído.</param>
    /// <returns>Valor do identificador da entidade.</returns>
    /// <exception cref="InvalidOperationException">Lançada se a entidade não expõe uma propriedade "Id".</exception>
    private static TId GetEntityId(T entity)
    {
        var prop = typeof(T).GetProperty("Id")
            ?? throw new InvalidOperationException(
                $"{typeof(T).Name} must expose an Id property");

        return (TId)prop.GetValue(entity)!;
    }
}