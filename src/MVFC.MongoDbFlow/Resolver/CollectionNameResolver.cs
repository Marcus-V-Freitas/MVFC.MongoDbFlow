namespace MVFC.MongoDbFlow.Resolver;

/// <summary>
/// Resolve o nome da coleção para um tipo de entidade.
/// </summary>
internal sealed class CollectionNameResolver(IEnumerable<IEntityMap> maps) : ICollectionNameResolver
{
    private readonly Dictionary<Type, string> _map = maps.ToDictionary(
            m => m.EntityType,
            m => m.CollectionName);

    /// <summary>
    /// Resolve o nome da coleção correspondente ao tipo de entidade informado.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade para a qual o nome da coleção será resolvido.</typeparam>
    /// <returns>Nome da coleção no MongoDB.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando não há mapeamento registrado para o tipo informado.</exception>
    public string Resolve<T>()
    {
        var type = typeof(T);

        if (_map.TryGetValue(type, out var name))
            return name;

        throw new InvalidOperationException($"No collection mapping registered for {type.Name}");
    }
}