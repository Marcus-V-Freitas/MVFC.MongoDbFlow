namespace MVFC.MongoDbFlow.Tests.Mappings;

public sealed class OrderMap : EntityMap<Order>
{
    public override string CollectionName => "orders";

    protected override void Configure(BsonClassMap<Order> cm)
    {
        cm.AutoMap();
        cm.MapIdMember(x => x.Id);
        cm.MapMember(x => x.Status);
        cm.MapMember(x => x.TotalAmount);
        cm.MapMember(x => x.CreatedAt);
    }
}