namespace Core.Tests;

using System;
using System.Collections.Generic;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;
using Xunit;

public class CatalogServiceTests
{
    [Fact]
    // 1) Інваріант: відмова і незмінний стан[cite: 6]
    public void Receive_WithNegativeQuantity_Throws_AndKeepsState()
    {
        var store = new InMemoryCatalogStore();
        var service = new CatalogService(store);
        var p = Product.Create("id-1", "SKU-1", "Кабель", "м", 10);
        store.Add(p);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Receive(p.Id, -5));
        Assert.Equal(10, store.GetById(p.Id)!.Quantity);
    }

    [Fact]
    // 2) Подія з тижня 6 як спосіб перевірки[cite: 6]
    public void Add_RaisesChangedExactlyOnce()
    {
        var service = new CatalogService(new InMemoryCatalogStore());
        var events = new List<CatalogChangedEventArgs>();
        service.Changed += (_, e) => events.Add(e);

        service.Add("SKU-2", "Гвинт", "шт", 7);

        Assert.Single(events);
        Assert.Equal(CatalogChangeKind.Added, events[0].Kind);
    }
}