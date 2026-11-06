namespace Core.Tests;

using System.Linq;
using Core.Reports;
using Xunit;

public class ReportServiceTests
{
    [Theory]
    [InlineData(10, 2)] 
    [InlineData(2, 1)]
    [InlineData(0, 0)]
    // 3) Звіт на кількох наборах[cite: 6]
    public void LowStock_CountsOnlyBelowThreshold(int threshold, int expected) =>
        Assert.Equal(expected, ReportService.LowStock(TestData.Items(), threshold).Count);

    [Fact]
    // 4) Детермінований топ- з тижня 7
    public void TopByQuantity_IsDeterministic() =>
        Assert.Equal(new[] { "SKU-BIG", "SKU-MID" },
            ReportService.TopByQuantity(TestData.Items(), 2).Select(l => l.Sku));
}