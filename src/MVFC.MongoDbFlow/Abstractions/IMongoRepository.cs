namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface de repositório MongoDB para operações CRUD genéricas.
/// </summary>
public interface IMongoRepository<T, TId>
{
    private const string DELETED_FIELD = "IsDeleted";

    /// <summary>
    /// Insere uma nova entidade no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entity">Entidade a ser inserida.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task InsertAsync(
        T entity,
        CancellationToken ct = default);

    /// <summary>
    /// Insere múltiplas entidades no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entities">Coleção de entidades a serem inseridas.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task InsertManyAsync(
        IEnumerable<T> entities,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém uma entidade pelo identificador de forma assíncrona.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    public Task<T?> GetOneAsync(
        TId id,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém uma entidade que corresponda ao filtro especificado de forma assíncrona.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    public Task<T?> GetOneAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Busca entidades que correspondam ao filtro especificado.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de entidades encontradas.</returns>
    public Task<IReadOnlyList<T>> FindAsync(
        FilterDefinition<T> filter,
        FindOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Busca entidades que corresponda ao filtro especificado e projeta o resultado.
    /// </summary>
    /// <typeparam name="TProjection">Tipo da projeção de retorno.</typeparam>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="projection">Definição da projeção.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de projeções encontradas.</returns>
    public Task<IReadOnlyList<TProjection>> FindAsync<TProjection>(
        FilterDefinition<T> filter, ProjectionDefinition<T, TProjection> projection,
        FindOptions<T, TProjection>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe alguma entidade que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    public Task<bool> ExistsAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém os valores distintos de um campo para as entidades que correspondam ao filtro.
    /// </summary>
    /// <typeparam name="TField">Tipo do campo.</typeparam>
    /// <param name="field">Campo para obter valores distintos.</param>
    /// <param name="filter">Filtro de consulta opcional.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de valores distintos encontrados.</returns>
    public Task<IReadOnlyList<TField>> DistinctAsync<TField>(
        FieldDefinition<T, TField> field,
        FilterDefinition<T>? filter = null,
        CancellationToken ct = default);

    /// <summary>
    /// Busca entidades paginadas que correspondam ao filtro especificado.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="pageIndex">Índice da página (base 0).</param>
    /// <param name="pageSize">Tamanho da página.</param>
    /// <param name="sort">Opção de ordenação opcional.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Resultado paginado contendo as entidades encontradas.</returns>
    public Task<PagedResult<T>> FindPagedAsync(
       FilterDefinition<T> filter,
       int pageIndex,
       int pageSize,
       SortDefinition<T>? sort = null,
       CancellationToken ct = default);

    /// <summary>
    /// Conta a quantidade de entidades que correspondem ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades encontradas.</returns>
    public Task<long> CountAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a atualização foi realizada, caso contrário false.</returns>
    public Task<bool> UpdateAsync(
        TId id,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza a primeira entidade que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a atualização foi realizada, caso contrário false.</returns>
    public Task<bool> UpdateAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades atualizadas.</returns>
    public Task<long> UpdateManyAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Substitui uma entidade existente no banco de dados.
    /// </summary>
    /// <param name="entity">Entidade a ser substituída.</param>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task ReplaceAsync(
        T entity,
        TId id,
        CancellationToken ct = default);

    /// <summary>
    /// Substitui uma entidade existente que corresponda ao filtro.
    /// </summary>
    /// <param name="entity">Entidade a ser substituída.</param>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task ReplaceAsync(
        T entity,
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Remove uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a exclusão foi realizada, caso contrário false.</returns>
    public Task<bool> DeleteAsync(
        TId id,
        CancellationToken ct = default);

    /// <summary>
    /// Remove uma entidade pelo filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a exclusão foi realizada, caso contrário false.</returns>
    public Task<bool> DeleteAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Remove múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades removidas.</returns>
    public Task<long> DeleteManyAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza e retorna a entidade encontrada que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade atualizada ou null.</returns>
    public Task<T?> FindOneAndUpdateAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        FindOneAndUpdateOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza e retorna a entidade encontrada pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade atualizada ou null.</returns>
    public Task<T?> FindOneAndUpdateAsync(
        TId id,
        UpdateDefinition<T> update,
        FindOneAndUpdateOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Remove e retorna a entidade encontrada pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade removida ou null.</returns>
    public Task<T?> FindOneAndDeleteAsync(
        TId id,
        FindOneAndDeleteOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Remove e retorna a entidade encontrada que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="options">Opções da operação.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade removida ou null.</returns>
    public Task<T?> FindOneAndDeleteAsync(
        FilterDefinition<T> filter,
        FindOneAndDeleteOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Realiza a exclusão lógica (soft delete) das entidades que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task SoftDeleteAsync(
        FilterDefinition<T> filter,
        string fieldName = DELETED_FIELD,
        CancellationToken ct = default);

    /// <summary>
    /// Realiza a exclusão lógica (soft delete) da entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task SoftDeleteAsync(
        TId id,
        string fieldName = DELETED_FIELD,
        CancellationToken ct = default);

    /// <summary>
    /// Restaura uma entidade logicamente excluída pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task RestoreAsync(
       TId id,
       string fieldName = DELETED_FIELD,
       CancellationToken ct = default);

    /// <summary>
    /// Restaura entidades logicamente excluídas que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="fieldName">Nome do campo de exclusão lógica.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task RestoreAsync(
        FilterDefinition<T> filter,
        string fieldName = DELETED_FIELD,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza campos específicos das entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task UpdateFieldsAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Executa operações em lote (bulk) no banco de dados.
    /// </summary>
    /// <param name="models">Coleção de operações a serem executadas.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    public Task BulkWriteAsync(
        IEnumerable<WriteModel<T>> models,
        CancellationToken ct = default);
}
