using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Domain;
using Core.Reports;
using Core.Events;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);
var items = service.All();

// Визначаємо команду (ігноруємо прапорець --file)[cite: 8]
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.TopByQuantity(items, 5));
        break;

    case "report-low":
        Print(ReportService.LowStock(items, 10));
        break;

    case "report-groups":
        foreach (var g in ReportService.ByUnit(items))
            Console.WriteLine($"{g.Key,-6} | позицій: {g.Count,3} | сумарно: {g.TotalQuantity,6}");
        break;

    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"позицій {t.Items}, всього {t.Total}, середнє {t.Average:F2}, макс {t.Max}");
        break;

    case "demo-deferred":
        RunDeferredDemo(service);
        break;

    default:
        Console.WriteLine("Доступні команди: report-top | report-low | report-groups | report-sum | demo-deferred");
        break;
}

return 0;

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($" {l.Sku,-10} {l.Name,-22} {l.Quantity,6} {l.Unit}");
}

static void RunDeferredDemo(CatalogService service)
{
    Console.WriteLine("=== ДЕМОНСТРАЦІЯ ВІДКЛАДЕНОГО ВИКОНАННЯ ===");
    var source = service.All().ToList();
    var query = source.Where(p => p.Quantity < 10); // Запит побудовано, але НЕ виконано

    Console.WriteLine($"Дефіцитних до змін: {query.Count()}");
    
    // Змінюємо джерело даних після побудови запиту
    source.Add(Product.Create("tmp00001", "SKU-999", "Скоба", "шт", 3));
    
    Console.WriteLine($"Дефіцитних після змін: {query.Count()}"); // Кількість інша, бо запит виконується на момент виклику Count()
    
    var snapshot = query.ToList(); // Матеріалізація результату
    source.Clear();
    
    Console.WriteLine($"У знімку (після очищення джерела) лишилось: {snapshot.Count}");
}