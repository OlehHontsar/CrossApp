namespace Core.Tests;

using System.Collections.Generic;
using Core.Domain;

public static class TestData
{
    public static IReadOnlyList<Product> Items() => new List<Product>
    {
        Product.Create("id-big", "SKU-BIG", "Велика партія", "шт", 50),
        Product.Create("id-mid", "SKU-MID", "Середня партія", "шт", 5),
        Product.Create("id-low", "SKU-LOW", "Мала партія", "шт", 1) // Змінили з 2 на 1
    };
}