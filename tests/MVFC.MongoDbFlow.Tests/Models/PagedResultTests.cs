namespace MVFC.MongoDbFlow.Tests.Models;

public sealed class PagedResultTests
{
    [Fact]
    public void Should_Calculate_PageCount_With_Remainder()
    {
        var result = new PagedResult<int>([1, 2, 3], 10, 0, 3);

        result.PageCount.Should().Be(4); // 10 / 3 = 3.33 -> 4
    }

    [Fact]
    public void Should_Calculate_PageCount_Exact()
    {
        var result = new PagedResult<int>([1, 2, 3], 9, 0, 3);

        result.PageCount.Should().Be(3);
    }

    [Fact]
    public void Should_Return_Zero_Pages_When_Empty()
    {
        var result = new PagedResult<int>([], 0, 0, 10);

        result.PageCount.Should().Be(0);
    }
}
