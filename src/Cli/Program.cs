using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Domain;

// ====================================================
// 🏢 ЛАБОРАТОРНА РОБОТА №5: Composition Root та Сервісний шар
// ====================================================
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

// Ручне впровадження залежностей (Manual DI) залежно від аргументів командного рядка
ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

Console.WriteLine($"[ЛР5] Сховище: {store.GetType().Name}");
Console.WriteLine(new string('-', 50));

try
{
    // 1. Успішний сценарій: додавання товару та реєстрація приходу через сервіс
    var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
    Console.WriteLine($"Додано товар через сервіс: {created}");

    service.Receive(created.Id, 25);
    Console.WriteLine($"Після приходу (+25): {service.Find(created.Id)}");

    // 2. Вивід списку всіх записів
    Console.WriteLine("\nПовний список товарів у сховищі:");
    foreach (var p in service.All())
    {
        Console.WriteLine($" - {p}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка виконання бізнес-операції: {ex.Message}");
}

// 3. Сценарій відмови: спроба оновити неіснуючий запис
Console.WriteLine("\n=== Сценарій відмови (перевірка винятків) ====");
try
{
    service.Receive("invalid-id-999", 10);
}
catch (Exception ex)
{
    Console.WriteLine($"Перехоплено очікуваний виняток: {ex.GetType().Name} — {ex.Message}");
}

return 0;