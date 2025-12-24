namespace MVFC.MongoDbFlow.Mapping;

/// <summary>
/// Mapeamento base para entidades do MongoDB.
/// </summary>
public abstract class EntityMap<T> : IEntityMap
{
    /// <summary>
    /// Obtém o tipo da entidade mapeada.
    /// </summary>
    public Type EntityType => typeof(T);

    /// <summary>
    /// Obtém o nome da coleção associada à entidade.
    /// </summary>
    public abstract string CollectionName { get; }

    /// <summary>
    /// Realiza o registro do mapeamento da entidade no MongoDB.
    /// Se o mapeamento já estiver registrado, não faz nada.
    /// </summary>
    public void Register()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(T)))
            return;

        BsonClassMap.TryRegisterClassMap<T>(Configure);
    }

    /// <summary>
    /// Configura o mapeamento da entidade, definindo propriedades, serializadores e regras específicas.
    /// Deve ser implementado nas classes derivadas para customizar o mapeamento.
    /// </summary>
    /// <param name="map">Instância de <see cref="BsonClassMap{T}"/> para configuração.</param>
    protected abstract void Configure(BsonClassMap<T> map);
}