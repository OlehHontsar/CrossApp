using System;
using Core; 

EnvironmentReport report = EnvironmentInfo.Collect(); 

Console.WriteLine("CrossApp – інформація про середовище (Лабораторна 2)"); 
Console.WriteLine("Студент: Гонцар Олег, група ФЕІ-33");
Console.WriteLine(new string('-', 52)); 
Console.WriteLine($"ОС (OSDescription)   : {report.OsDescription}"); 
Console.WriteLine($"ОС (Environment)     : {report.OsVersion}"); 
Console.WriteLine($"Архітектура процесу  : {report.ProcessArchitecture}"); 
Console.WriteLine($"Версія .NET (CLR)    : {report.ClrVersion}"); 
Console.WriteLine($"Runtime              : {report.FrameworkDescription}"); 
Console.WriteLine($"RID (визначено)      : {report.DetectedRid}"); 
Console.WriteLine($"RID (від .NET)       : {report.ReportedRid}"); 
Console.WriteLine($"Каталог застосунку   : {report.BaseDirectory}"); 
Console.WriteLine($"Поточний каталог     : {report.CurrentDirectory}"); 
Console.WriteLine($"Примітка збірки (TFM): {report.BuildNote}"); // ДОДАНО
Console.WriteLine(new string('-', 52));
Console.WriteLine("Предметна область: Замовлення (Customer, Product, Order, OrderLine)");
