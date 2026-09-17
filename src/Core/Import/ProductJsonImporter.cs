using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class ProductJsonImporter
{
    public static ImportResult<IDomainDto> Load(string path)
    {
        var items = new List<IDomainDto>();
        var errors = new List<string>();

        try
        {
            // Безпечне зчитування JSON тексту з підтримкою кирилиці
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            // Розбираємо масив як сирі JsonElement для гнучкого динамічного парсингу за типами
            var rawList = JsonSerializer.Deserialize<List<JsonElement>>(json, options) ?? [];

            foreach (var element in rawList)
            {
                if (element.TryGetProperty("id", out var idProp) && idProp.GetString()!.StartsWith("W"))
                {
                    var wh = JsonSerializer.Deserialize<WarehouseDto>(element.GetRawText(), options);
                    if (wh != null) items.Add(wh);
                }
                else
                {
                    var prod = JsonSerializer.Deserialize<ProductDto>(element.GetRawText(), options);
                    if (prod != null) items.Add(prod);
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Помилка десеріалізації JSON структури: {ex.Message}");
        }

        return new ImportResult<IDomainDto>(items, errors);
    }
}
