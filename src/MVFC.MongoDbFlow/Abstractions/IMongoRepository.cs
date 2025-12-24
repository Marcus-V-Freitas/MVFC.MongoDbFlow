namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface de repositório MongoDB para operações CRUD genéricas.
/// </summary>
public interface IMongoRepository<T, TId>
{
    /// <summary>
    /// Insere uma nova entidade no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entity">Entidade a ser inserida.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task InsertAsync(
        T entity,
        CancellationToken ct = default);

    /// <summary>
    /// Insere múltiplas entidades no banco de dados de forma assíncrona.
    /// </summary>
    /// <param name="entities">Coleção de entidades a serem inseridas.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task InsertManyAsync(
        IEnumerable<T> entities,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém uma entidade pelo identificador de forma assíncrona.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Entidade encontrada ou null.</returns>
    Task<T?> GetByIdAsync(
        TId id,
        CancellationToken ct = default);

    /// <summary>
    /// Busca entidades que correspondam ao filtro especificado.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de entidades encontradas.</returns>
    Task<IReadOnlyList<T>> FindAsync(
        FilterDefinition<T> filter,
        FindOptions<T>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Busca entidades que correspondam ao filtro especificado e projeta o resultado.
    /// </summary>
    /// <typeparam name="TProjection">Tipo da projeção de retorno.</typeparam>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="projection">Definição da projeção.</param>
    /// <param name="options">Opções de busca.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Lista somente leitura de projeções encontradas.</returns>
    Task<IReadOnlyList<TProjection>> FindAsync<TProjection>(
        FilterDefinition<T> filter, ProjectionDefinition<T, TProjection> projection,
        FindOptions<T, TProjection>? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe alguma entidade que corresponda ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se existir, caso contrário false.</returns>
    Task<bool> ExistsAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Conta a quantidade de entidades que correspondem ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades encontradas.</returns>
    Task<long> CountAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a atualização foi realizada, caso contrário false.</returns>
    Task<bool> UpdateAsync(
        TId id,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="update">Definição da atualização.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades atualizadas.</returns>
    Task<long> UpdateManyAsync(
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken ct = default);

    /// <summary>
    /// Substitui uma entidade existente no banco de dados.
    /// </summary>
    /// <param name="entity">Entidade a ser substituída.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task ReplaceAsync(
        T entity,
        CancellationToken ct = default);

    /// <summary>
    /// Remove uma entidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>True se a exclusão foi realizada, caso contrário false.</returns>
    Task<bool> DeleteAsync(
        TId id,
        CancellationToken ct = default);

    /// <summary>
    /// Remove múltiplas entidades que correspondam ao filtro.
    /// </summary>
    /// <param name="filter">Filtro de consulta.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    /// <returns>Número de entidades removidas.</returns>
    Task<long> DeleteManyAsync(
        FilterDefinition<T> filter,
        CancellationToken ct = default);
}