using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Domain;
using Core.Reports;
using Core.Events;
using Core.Configuration;
using Microsoft.Extensions.Configuration;

// 1. Побудова конфігурації (Лабораторна робота №9)
var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();

// Визначення шляху до файлу даних за замовчуванням з конфігурації
var defaultDataPath = options.Storage.ResolveFullPath(AppContext.BaseDirectory);

// Перевірка, чи передано кастомний шлях через аргумент --file
string dataPath = defaultDataPath;
var fileIndex = Array.IndexOf(args, "--file");
if (fileIndex >= 0 && fileIndex + 1 < args.Length)
{
    var passedPath = args[fileIndex + 1];
    dataPath = Path.IsPathRooted(passedPath) 
        ? passedPath 
        : Path.Combine(Directory.GetCurrentDirectory(), passedPath);
}

// Перевірка змінної середовища CROSSAPP_DATA_PATH (якщо задана — має пріоритет)
var legacyDataPath = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");
if (!string.IsNullOrWhiteSpace(legacyDataPath))
{
    dataPath = Path.IsPathRooted(legacyDataPath) 
        ? Path.Combine(legacyDataPath, options.Storage.FileName)
        : Path.Combine(Directory.GetCurrentDirectory(), legacyDataPath, options.Storage.FileName);
    Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);
}

// Читання секрету з конфігурації
var apiKey = config["Integration:ApiKey"];

Console.WriteLine($"Файл даних: {Path.GetFullPath(dataPath)}");
Console.WriteLine(apiKey is null ? "АріКеу: не заданий" : $"АріКеу: присутній, довжина {apiKey.Length}");

// Ініціалізація сховища та сервісу
bool useFile = true;
ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

// 2. Налаштування кооперативного скасування через Ctrl+C
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => 
{ 
    e.Cancel = true; 
    cts.Cancel(); 
};

// 3. Визначення команди
string command = "report-top"; 
var cleanArgs = args.Where(a => !a.StartsWith("--") && !a.EndsWith("catalog.json") && !a.EndsWith("items.json")).ToList();
if (cleanArgs.Count > 0)
{
    var firstArg = cleanArgs[0].ToLowerInvariant();
    if (firstArg == "import" || firstArg == "export" || firstArg.StartsWith("report") || firstArg == "demo-deferred")
    {
        command = firstArg;
    }
}
foreach (var arg in args)
{
    if (arg == "import" || arg == "export" || arg.StartsWith("report" ) || arg == "demo-deferred")
    {
        command = arg;
        break;
    }
}