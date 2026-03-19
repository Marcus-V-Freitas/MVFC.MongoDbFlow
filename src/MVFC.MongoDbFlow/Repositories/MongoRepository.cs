namespace MVFC.MongoDbFlow.Repositories;

/// <summary>
/// Implementação de repositório MongoDB para operações CRUD genéricas.
/// </summary>
internal sealed class MongoRepository<T, TId>(
    IMongoDatabase database,
    ICollectionNameResolver resolver,
    IClientSessionHandle? session = null) : IMongoRepository<T, TId>
{
    private const string OBJECT_ID_FIELD = "_id";
    private const string DELETED_FIELD = "IsDeleted";

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
        await (_session is null
            ? _collection.InsertOneAsync(entity, default, ct)
            : _collection.InsertOneAsync(_session, entity, cancellationToken: ct));
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
        await (_session is null
            ? _collection.InsertManyAsync(entities, cancellationToken: ct)
            : _collection.InsertManyAsync(_session, entities, cancellationToken: ct));
    }

    /// <summary>
    /// Obtém uma entidade pelo identificador de forma assíncrona.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    public async Task<T?> GetOneAsync(
        TId id,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        return await GetOneAsync(filter, ct);
    }

    /// <summary>
    /// Obtém uma entidade que corresponda ao filtro especificado de forma assíncrona.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    public async Task<T?> GetOneAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default) =>
            _session is null
                ? await _collection.Find(filter).FirstOrDefaultAsync(ct)
                : await _collection.Find(_session, filter).FirstOrDefaultAsync(ct);

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
        var options = new CountOptions 
        { 
            Limit = 1,
        };

        var count = _session is null
            ? await _collection.CountDocumentsAsync(filter, options, ct)
            : await _collection.CountDocumentsAsync(_session, filter, options, ct);

        return count > 0;
    }

    /// <summary>
    /// Obtém os valores distintos de um campo para as entidades que correspondam ao filtro.
    /// </summary>
    /// <typeparam name="TField">Tipo do campo.</typeparam>
    /// <param name="field">Campo para obter valores distintos.</param>
    /// <param name="filter">Filtro de consulta opcional.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de valores distintos encontrados.</returns>
    public async Task<IReadOnlyList<TField>> DistinctAsync<TField>(
        FieldDefinition<T, TField> field,
        FilterDefinition<T>? filter = null,
        CancellationToken ct = default)
    {
        filter ??= Builders<T>.Filter.Empty;

        var cursor = _session is null
            ? await _collection.DistinctAsync(field, filter, cancellationToken: ct)
            : await _collection.DistinctAsync(_session, field, filter, cancellationToken: ct);

        return await cursor.ToListAsync(ct);
    }

    /// <summary>
    /// Busca entidades paginadas que correspondam ao filtro especificado.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="pageIndex">Índice da página (base 0).</param>
    /// <param name="pageSize">Tamanho da página.</param>
    /// <param name="sort">Opção de ordenação opcional.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Resultado paginado contendo as entidades encontradas.</returns>
    public async Task<PagedResult<T>> FindPagedAsync(
        FilterDefinition<T> filter,
        int pageIndex,
        int pageSize,
        SortDefinition<T>? sort = null,
        CancellationToken ct = default)
    {
        var query = _collection.Find(filter);

        if (sort != null)
            query = query.Sort(sort);

        var totalCount = await query.CountDocumentsAsync(ct);
        var items = await query
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>(items, totalCount, pageIndex, pageSize);
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
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        return await UpdateAsync(filter, update, ct);
    }

    /// <summary>
    /// Atualiza a primeira entidade que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a atualização foi realizada, caso contrário false.</returns>
    public async Task<bool> UpdateAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default)
    {
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
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task ReplaceAsync(
        T entity,
        TId id,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        await (_session is null
            ? _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct)
            : _collection.ReplaceOneAsync(_session, filter, entity, cancellationToken: ct));
    }

    /// <summary>
    /// Substitui uma entidade existente que corresponda ao filtro.
    /// </summary>
    /// <param name="entity">Entidade a ser substituída.</param>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task ReplaceAsync(
        T entity,
        FilterDefinition<T> filter,
        CancellationToken ct = default) =>
        await (_session is null
            ? _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct)
            : _collection.ReplaceOneAsync(_session, filter, entity, cancellationToken: ct));

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
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        return await DeleteAsync(filter, ct);
    }

    /// <summary>
    /// Remove uma entidade pelo filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a exclusão foi realizada, caso contrário false.</returns>
    public async Task<bool> DeleteAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default)
    {
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
    /// Atualiza e retorna a entidade encontrada que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade atualizada ou null.</returns>
    public async Task<T?> FindOneAndUpdateAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        FindOneAndUpdateOptions<T>? options = null,
        CancellationToken ct = default)
    {
        return _session is null
            ? await _collection.FindOneAndUpdateAsync(filter, update, options, ct)
            : await _collection.FindOneAndUpdateAsync(_session, filter, update, options, ct);
    }

    /// <summary>
    /// Atualiza e retorna a entidade encontrada pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade atualizada ou null.</returns>
    public async Task<T?> FindOneAndUpdateAsync(
        TId id,
        UpdateDefinition<T> update,
        FindOneAndUpdateOptions<T>? options = null,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        return await FindOneAndUpdateAsync(filter, update, options, ct);
    }

    /// <summary>
    /// Remove e retorna a entidade encontrada que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade removida ou null.</returns>
    public async Task<T?> FindOneAndDeleteAsync(
        FilterDefinition<T> filter,
        FindOneAndDeleteOptions<T>? options = null,
        CancellationToken ct = default)
    {
        return _session is null
            ? await _collection.FindOneAndDeleteAsync(filter, options, ct)
            : await _collection.FindOneAndDeleteAsync(_session, filter, options, ct);
    }

    /// <summary>
    /// Remove e retorna a entidade encontrada pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade removida ou null.</returns>
    public async Task<T?> FindOneAndDeleteAsync(
        TId id,
        FindOneAndDeleteOptions<T>? options = null,
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);

        return await FindOneAndDeleteAsync(filter, options, ct);
    }

    /// <summary>
    /// Realiza a exclusão lógica (soft delete) das entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task SoftDeleteAsync(
        FilterDefinition<T> filter, 
        string fieldName = DELETED_FIELD, 
        CancellationToken ct = default)
    {
        var update = Builders<T>.Update.Set(fieldName, true);
        await UpdateFieldsAsync(filter, update, ct);
    }

    /// <summary>
    /// Realiza a exclusão lógica (soft delete) da entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task SoftDeleteAsync(
        TId id, 
        string fieldName = DELETED_FIELD, 
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);
        var update = Builders<T>.Update.Set(fieldName, true);

        await UpdateFieldsAsync(filter, update, ct);
    }

    /// <summary>
    /// Restaura uma entidade logicamente excluída pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task RestoreAsync(
        TId id, 
        string fieldName = DELETED_FIELD, 
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(OBJECT_ID_FIELD, id);
        var update = Builders<T>.Update.Set(fieldName, false);

        await UpdateFieldsAsync(filter, update, ct);
    }

    /// <summary>
    /// Restaura entidades logicamente excluídas que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task RestoreAsync(
        FilterDefinition<T> filter, 
        string fieldName = DELETED_FIELD, 
        CancellationToken ct = default)
    {
        var update = Builders<T>.Update.Set(fieldName, false);

        await UpdateFieldsAsync(filter, update, ct);
    }

    /// <summary>
    /// Atualiza campos específicos das entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task UpdateFieldsAsync(
        FilterDefinition<T> filter, 
        UpdateDefinition<T> update, 
        CancellationToken ct = default)
    {
        await (_session is null ?
            _collection.UpdateOneAsync(filter, update, cancellationToken: ct) :
            _collection.UpdateOneAsync(_session, filter, update, cancellationToken: ct));
    }

    /// <summary>
    /// Atualiza campos específicos da entidade identificada pelo id.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task UpdateFieldsAsync(
        TId id, 
        UpdateDefinition<T> update, 
        CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq("_id", id);

        await (_session is null ?
            _collection.UpdateOneAsync(filter, update, cancellationToken: ct) :
            _collection.UpdateOneAsync(_session, filter, update, cancellationToken: ct));
    }

    /// <summary>
    /// Executa operações em lote (bulk) no banco de dados.
    /// </summary>
    /// <param name="models">Coleção de operações a serem executadas.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public async Task BulkWriteAsync(
        IEnumerable<WriteModel<T>> models, 
        CancellationToken ct = default) =>
            await (_session is null ?
                _collection.BulkWriteAsync(models, cancellationToken: ct) :
                _collection.BulkWriteAsync(_session, models, cancellationToken: ct));
}