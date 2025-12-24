namespace MVFC.MongoDbFlow.Abstractions;

/// <summary>
/// Interface para registro de serializadores customizados no MongoDB.
/// </summary>
public interface ISerializerRegistration
{
    /// <summary>
    /// Realiza o registro dos serializadores customizados no MongoDB.
    /// Deve ser chamado durante a inicialização da aplicação para garantir que os tipos personalizados sejam corretamente serializados e desserializados.
    /// </summary>
    void Register();
}