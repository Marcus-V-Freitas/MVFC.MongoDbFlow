namespace MVFC.MongoDbFlow.Tests.Mappings;

public sealed class OrderMap : EntityMap<Order>
{
    public override string CollectionName => "orders";

    protected override void Configure(BsonClassMap<Order> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        map.AutoMap();
        map.MapIdMember(x => x.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        map.MapMember(x => x.UserId).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        map.MapMember(x => x.Status);
        map.MapMember(x => x.TotalAmount);
        map.MapMember(x => x.CreatedAt);
    }
}
