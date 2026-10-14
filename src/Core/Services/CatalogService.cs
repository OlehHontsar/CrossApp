using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService(ICatalogStore store)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));

    // Оголошення події за стандартами .NET (.NET event з використанням EventHandler<T>)[cite: 3, 5]
    public event EventHandler<CatalogChangedEventArgs>? Changed;

    // Безпечне підняття події через оператор ?.Invoke[cite: 3, 5]
    private void OnChanged(Product p, CatalogChangeKind kind, int delta) =>
        Changed?.Invoke(this, new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity));

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
        _store.Add(product);
        
        // Подія викликається після успішного додавання[cite: 5]
        OnChanged(product, CatalogChangeKind.Added, quantity); 
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        
        // Якщо інваріант RegisterArrival кине виняток, рядок нижче не виконається, і подія не підніметься[cite: 5]
        product.RegisterArrival(quantity);
        _store.Update(product);
        
        // Подія викликається після успішної зміни стану[cite: 5]
        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    // Використання делегата Func для фільтрації (коли потрібен результат, а не просте сповіщення)[cite: 3, 5, 7]
    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}