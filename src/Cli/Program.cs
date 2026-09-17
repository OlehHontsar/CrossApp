using System;
using System.IO;
using System.Linq;
using Core;
using Core.Dto;
using Core.Import;

// ====================================================
// 📊 ШАР ЛАБОРАТОРНОЇ РОБОТИ №2: СИСТЕМНИЙ ЗВІТ
// ====================================================
EnvironmentReport envReport = EnvironmentInfo.Collect(); 

Console.WriteLine("====================================================");
Console.WriteLine("CrossApp – інформація про середовище (Лабораторна 2)"); 
Console.WriteLine("====================================================");
Console.WriteLine("Студент: Гонцар Олег, група ФЕІ-33");
Console.WriteLine($"ОС (OSDescription)   : {envReport.OsDescription}"); 
Console.WriteLine($"ОС (Environment)     : {envReport.OsVersion}"); 
Console.WriteLine($"Архітектура процесу  : {envReport.ProcessArchitecture}"); 
Console.WriteLine($"Версія .NET (CLR)    : {envReport.ClrVersion}"); 
Console.WriteLine($"Runtime              : {envReport.FrameworkDescription}"); 
Console.WriteLine($"RID (визначено)      : {envReport.DetectedRid}"); 
Console.WriteLine($"RID (від .NET)       : {envReport.ReportedRid}"); 
Console.WriteLine($"Каталог застосунку   : {envReport.BaseDirectory}"); 
Console.WriteLine($"Поточний каталог     : {envReport.CurrentDirectory}"); 
Console.WriteLine($"Примітка збірки (TFM): {envReport.BuildNote}");
Console.WriteLine(new string('-', 52)); 
Console.WriteLine("Предметна область: Замовлення (Customer, Product, Order, OrderLine)");
Console.WriteLine("====================================================\n");

// ====================================================
// 📂 ШАР ЛАБОРАТОРНОЇ РОБОТИ №3: ВІДМОВОСТІЙКИЙ ІМПОРТ
// ====================================================
string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv"); 

if (!File.Exists(path)) 
{ 
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}"); 
    return 1; 
} 

string extension = Path.GetExtension(path).ToLower();

// ДОДАТКОВЕ ЗАВДАННЯ 1: Вибір стратегії імпорту за розширенням файлу через switch expression
ImportResult<IDomainDto> result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => throw new InvalidOperationException($"Непідтримуване розширення файлу: '{extension}'")
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}"); 

// ДОДАТКОВЕ ЗАВДАННЯ 2: Роздільне табличне відображення різнорідних сутностей через Pattern Matching
foreach (IDomainDto p in result.Items.Take(5)) 
{
    switch (p)
    {
        case ProductDto prod:
            Console.WriteLine($"  {prod.Id,-6} {prod.Sku,-10} {prod.Name,-26} {prod.Quantity,5} {prod.Unit}");
            break;
        case WarehouseDto wh:
            Console.WriteLine($"  {wh.Id,-6} {wh.Sku,-10} {wh.Name,-26} {wh.Capacity,5} {wh.Location}");
            break;
    }
}

// Виведення списку пропущених рядків із номерами
if (result.Errors.Count > 0) 
{ 
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}"); 
    foreach (string e in result.Errors) 
        Console.WriteLine($"  ! {e}"); 
} 

// ДОДАТКОВЕ ЗАВДАННЯ 3: Розрахунок аналітичної статистики одним фінальним рядком
int totalRows = result.Items.Count + result.Errors.Count;
double errorRate = totalRows > 0 ? ((double)result.Errors.Count / totalRows) * 100 : 0;

Console.WriteLine(new string('=', 52));
Console.WriteLine($"СТАТИСТИКА ІМПОРТУ: Усього: {totalRows} | Прийнято: {result.Items.Count} | Пропущено: {result.Errors.Count} | % помилок: {errorRate:F1}%");
Console.WriteLine("====================================================");

return 0;
