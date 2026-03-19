namespace MVFC.MongoDbFlow.Tests.Mappings;

public sealed class UserMap : EntityMap<User>
{
    public override string CollectionName => "users";

    protected override void Configure(BsonClassMap<User> cm)
    {
        ArgumentNullException.ThrowIfNull(cm);

        cm.AutoMap();
        cm.MapIdMember(x => x.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        cm.MapMember(x => x.Name).SetIsRequired(true);
        cm.MapMember(x => x.BirthDate);
    }
}
