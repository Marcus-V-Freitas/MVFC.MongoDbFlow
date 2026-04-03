namespace MVFC.MongoDbFlow.Tests.Mappings;

public sealed class UserMap : EntityMap<User>
{
    public override string CollectionName => "users";

    protected override void Configure(BsonClassMap<User> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        map.AutoMap();
        map.MapIdMember(x => x.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        map.MapMember(x => x.Name).SetIsRequired(true);
        map.MapMember(x => x.BirthDate);
    }
}
