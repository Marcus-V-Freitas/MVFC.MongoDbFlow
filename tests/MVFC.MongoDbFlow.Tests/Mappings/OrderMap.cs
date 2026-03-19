namespace MVFC.MongoDbFlow.Tests.Mappings;

public sealed class OrderMap : EntityMap<Order>
{
    public override string CollectionName => "orders";

    protected override void Configure(BsonClassMap<Order> cm)
    {
        ArgumentNullException.ThrowIfNull(cm);

        cm.AutoMap();
        cm.MapIdMember(x => x.Id).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        cm.MapMember(x => x.UserId).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        cm.MapMember(x => x.Status);
        cm.MapMember(x => x.TotalAmount);
        cm.MapMember(x => x.CreatedAt);
    }
}
