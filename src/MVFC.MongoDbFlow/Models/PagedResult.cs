namespace MVFC.MongoDbFlow.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int PageIndex,
    int PageSize)
{
    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);
}
