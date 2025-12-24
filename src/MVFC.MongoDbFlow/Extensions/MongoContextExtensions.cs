namespace MVFC.MongoDbFlow.Extensions;

/// <summary>
/// Métodos de extensão para facilitar o uso de repositórios com <see cref="IMongoContext"/>.
/// </summary>
public static class MongoContextExtensions
{
    /// <summary>
    /// Cria uma instância de <see cref="IMongoRepository{T, TId}"/> utilizando o contexto informado.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade.</typeparam>
    /// <typeparam name="TId">Tipo do identificador da entidade.</typeparam>
    /// <param name="context">Contexto MongoDB utilizado para obter as dependências necessárias.</param>
    /// <returns>Instância de <see cref="IMongoRepository{T, TId}"/> pronta para uso.</returns>
    public static IMongoRepository<T, TId> GetRepository<T, TId>(
        this IMongoContext context)
        => new MongoRepository<T, TId>(
            context.Database,
            context.Resolver,
            context.Session);
}
