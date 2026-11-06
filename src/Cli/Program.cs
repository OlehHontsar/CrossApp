using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Domain;
using Core.Reports;
using Core.Events;

bool useFile = args.Contains("--file");

// Динамічно визначаємо шлях до файлу, якщо передано прапорець --file <path>
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
var fileIndex = Array.IndexOf(args, "--file");
if (fileIndex >= 0 && fileIndex + 1 < args.Length)
{
    var passedPath = args[fileIndex + 1];
    dataPath = Path.IsPathRooted(passedPath) 
        ? passedPath 
        : Path.Combine(Directory.GetCurrentDirectory(), passedPath);
}

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

// Налаштування кооперативного скасування через Ctrl+C або таймаут
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => 
{ 
    e.Cancel = true; 
    cts.Cancel(); 
};

// Для демонстрації таймауту за бажанням можна розкоментувати рядок нижче:
// cts.CancelAfter(TimeSpan.FromSeconds(2));

// Визначаємо команду (ігноруємо прапорець --file та значення шляху)
var command = args.FirstOrDefault(a => !a.StartsWith("--") && a != Path.GetFileName(dataPath) && !Directory.Exists(a) && !File.Exists(a)) ?? "report-top";
// Або простіший пошук першої команди, що не є прапорцем і не є шляхом файлу:
var cleanArgs = args.Where(a => !a.StartsWith("--")).ToList();
// Якщо передано команду типу "import", візьмемо її, інакше за замовчуванням "report-top"
if (cleanArgs.Count > 0 && (cleanArgs[0] == "import" || cleanArgs[0].StartsWith("report") || cleanArgs[0] == "demo-deferred"))
{
    command = cleanArgs[0];
}

try
{
    switch (command)
    {
        case "import":
            Console.WriteLine($"Розпочато імпорт з файлу: {dataPath}...");
            var addedCount = await service.ImportAsync(new JsonCatalogImporter(), dataPath, cts.Token);
            Console.WriteLine($"Імпортовано записів: {addedCount}");
            break;

        case "report-top":
            Print(ReportService.TopByQuantity(service.All(), 5));
            break;

        case "report-low":
            Print(ReportService.LowStock(service.All(), 10));
            break;

        case "report-groups":
            foreach (var g in ReportService.ByUnit(service.All()))
                Console.WriteLine($"{g.Key,-6} | позицій: {g.Count,3} | сумарно: {g.TotalQuantity,6}");
            break;

        case "report-sum":
            var t = ReportService.Totals(service.All());
            Console.WriteLine($"позицій {t.Items}, всього {t.Total}, середнє {t.Average:F2}, макс {t.Max}");
            break;

        case "demo-deferred":
            RunDeferredDemo(service);
            break;

        default:
            Console.WriteLine("Доступні команди: import | report-top | report-low | report-groups | report-sum | demo-deferred");
            break;
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Імпорт скасовано.");
}
catch (DomainException ex)
{
    Console.WriteLine($"Дані відхилено: {ex.Message}");
}
catch (FileNotFoundException)
{
    Console.WriteLine($"Файл не знайдено: {dataPath}");
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
    var query = source.Where(p => p.Quantity < 10);

    Console.WriteLine($"Дефіцитних до змін: {query.Count()}");
    
    source.Add(Product.Create("tmp00001", "SKU-999", "Скоба", "шт", 3));
    
    Console.WriteLine($"Дефіцитних після змін: {query.Count()}");
    
    var snapshot = query.ToList();
    source.Clear();
    
    Console.WriteLine($"У знімку (після очищення джерела) лишилось: {snapshot.Count}");
}