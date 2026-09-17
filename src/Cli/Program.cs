using System;
using System.IO;
using System.Linq;
using Core;
using Core.Dto;
using Core.Import;

// ====================================================
// 📊 ШАР ЛАБОРАТОРНОЇ РОБОТИ №2: СИСТЕМНИЙ ЗВІТ
// ====================================================
EnvironmentReport report = EnvironmentInfo.Collect(); 

Console.WriteLine("====================================================");
Console.WriteLine("CrossApp – інформація про середовище (Лабораторна 2)"); 
Console.WriteLine("====================================================");
Console.WriteLine("Студент: Гонцар Олег, група ФЕІ-33");
Console.WriteLine($"ОС (OSDescription)   : {report.OsDescription}"); 
Console.WriteLine($"ОС (Environment)     : {report.OsVersion}"); 
Console.WriteLine($"Архітектура процесу  : {report.ProcessArchitecture}"); 
Console.WriteLine($"Версія .NET (CLR)    : {report.ClrVersion}"); 
Console.WriteLine($"Runtime              : {report.FrameworkDescription}"); 
Console.WriteLine($"RID (визначено)      : {report.DetectedRid}"); 
Console.WriteLine($"RID (від .NET)       : {report.ReportedRid}"); 
Console.WriteLine($"Каталог застосунку   : {report.BaseDirectory}"); 
Console.WriteLine($"Поточний каталог     : {report.CurrentDirectory}"); 
Console.WriteLine($"Примітка збірки (TFM): {report.BuildNote}");
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

ImportResult<ProductDto> result = ProductCsvImporter.Load(path); 

Console.WriteLine($"Завантажено записів: {result.Items.Count}"); 

// Демонстрація перших 5 записів із табличним форматуванням
foreach (ProductDto p in result.Items.Take(5)) 
    Console.WriteLine($"  {p.Id,-6} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}"); 

// Виведення пропущених рядків з номерами та причинами
if (result.Errors.Count > 0) 
{ 
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}"); 
    foreach (string e in result.Errors) 
        Console.WriteLine($"  ! {e}"); 
} 

return 0;
