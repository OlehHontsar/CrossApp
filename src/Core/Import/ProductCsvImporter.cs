using System;
using System.Collections.Generic;
using System.IO;
using Core.Dto;

namespace Core.Import;

public static class ProductCsvImporter 
{ 
    private const char Separator = ';'; 

    public static ImportResult<IDomainDto> Load(string path) 
    { 
        var items = new List<IDomainDto>(); 
        var errors = new List<string>(); 
        // Явно кажемо .NET зчитувати файл як UTF-8, ігноруючи системну локаль Windows
        string[] lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);


        for (int i = 0; i < lines.Length; i++) 
        { 
            int number = i + 1; 
            string line = lines[i].Trim(); 

            // Пропускаємо порожні рядки та текстові коментарі
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) 
                continue; 

            // Безпечний пропуск рядка технічних заголовків стовпців
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
        return new ImportResult<IDomainDto>(items, errors); 
    } 

    private static ParseOutcome ParseLine(string line) 
    { 
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries); 

        return parts switch 
        { 
            // 1. Патерн властивості: перевірка на критично невірну кількість стовпців у рядку
            { Length: < 6 } => new ParseFailed($"очікую 5 колонок, отримав {parts.Length - 1}"), 

            // 2. Патерн списку з фільтром when: валідація та збір ТОВАРУ за префіксом маркера типу "P"
            [var id, "P", var sku, var name, var unit, var qty] when !string.IsNullOrWhiteSpace(sku) && !string.IsNullOrWhiteSpace(name) => 
                int.TryParse(qty, out int q) && q >= 0 
                    ? new ParseOk(new ProductDto(id, sku, name, unit, q))
                    : new ParseFailed($"кількість '{qty}' не є невід'ємним числом"),

            // 3. Патерн списку з фільтром when: валідація та збір СКЛАДУ за префіксом маркера типу "W"
            [var id, "W", var sku, var name, var location, var capStr] when !string.IsNullOrWhiteSpace(sku) && !string.IsNullOrWhiteSpace(name) => 
                int.TryParse(capStr, out int c) && c >= 0 
                    ? new ParseOk(new WarehouseDto(id, sku, name, location, c))
                    : new ParseFailed($"місткість складу '{capStr}' не є числом"),

            // 4. Логічні та позиційні патерни: перевірка на пусті обов'язкові поля SKU або назви сутності
            [_, "P", "", _, _, _] or [_, "P", _, "", _, _] => new ParseFailed("SKU або назва порожні"),
            [_, "W", "", _, _, _] or [_, "W", _, "", _, _] => new ParseFailed("SKU або назва складу порожні"),

            // 5. Поліморфний патерн зрізу: обробка помилки при невідомому типі маркера всередині структури
            [_, var type, ..] => new ParseFailed($"невідомий тип префіксу сутності: '{type}'"),
            
            // 6. Дефолтний патерн
            _ => new ParseFailed($"невідома конфігурація структури: {parts.Length} колонок") 
        }; 
    } 

    private abstract record ParseOutcome; 
    private sealed record ParseOk(IDomainDto Value) : ParseOutcome; 
    private sealed record ParseFailed(string Reason) : ParseOutcome; 
}
