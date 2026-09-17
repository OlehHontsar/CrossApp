using System;
using System.Collections.Generic;
using System.IO;
using Core.Dto;

namespace Core.Import;

public static class ProductCsvImporter 
{ 
    private const char Separator = ';'; 

    public static ImportResult<ProductDto> Load(string path) 
    { 
        var items = new List<ProductDto>(); 
        var errors = new List<string>(); 
        string[] lines = File.ReadAllLines(path); 

        for (int i = 0; i < lines.Length; i++) 
        { 
            int number = i + 1; 
            string line = lines[i].Trim(); 

            if (string.IsNullOrWhiteSpace(line)) 
                continue; 

            // Пропуск першого рядка, якщо це заголовок стовпців
            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase)) 
                continue;                       

            switch (ParseLine(line)) 
            { 
                case ParseOk ok: 
                    items.Add(ok.Value); 
                    break; 
                case ParseFailed failed: 
                    errors.Add($"рядок {number}: {failed.Reason}"); 
                    break; 
            } 
        } 
        return new ImportResult<ProductDto>(items, errors); 
    } 

    private static ParseOutcome ParseLine(string line) 
    { 
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries); 

        return parts switch 
        { 
            // 1. Патерн властивості для перевірки недостатньої кількості колонок
            { Length: < 5 } => new ParseFailed($"очікую 5 колонок, отримав {parts.Length}"), 

            // 2. Списковий патерн для перевірки порожніх полів SKU або Назви
            [_, "", _, _, _] or [_, _, "", _, _] => new ParseFailed("SKU або назва порожні"), 

            // 3. Патерн списку з guard-виразом when для перевірки типу int та його знаку
            [_, _, _, _, var qty] when !int.TryParse(qty, out int q) || q < 0 
                => new ParseFailed($"кількість '{qty}' не є невід'ємним числом"), 

            // 4. Патерн списку для успішного збору об'єкта
            [var id, var sku, var name, var unit, var qty] 
                => new ParseOk(new ProductDto(id, sku, name, unit, int.Parse(qty))), 

            // 5. Дефолтний патерн для обробки надлишкових колонок
            _ => new ParseFailed($"занадто багато колонок: {parts.Length}") 
        }; 
    } 

    // Внутрішня ієрархія для повернення результатів парсингу рядка
    private abstract record ParseOutcome; 
    private sealed record ParseOk(ProductDto Value) : ParseOutcome; 
    private sealed record ParseFailed(string Reason) : ParseOutcome; 
}
