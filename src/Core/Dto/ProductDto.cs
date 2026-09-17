using System.Collections.Generic;

namespace Core.Dto;

// Позиційний рекорд товару, адаптований під ваш домен "Замовлення" та "Склад"
public record ProductDto(
    string Id, 
    string Sku, 
    string Name, 
    string Unit, 
    int Quantity, 
    string? Note = null
);

// Узагальнений контейнер результату відмовостійкого імпорту
public sealed record ImportResult<T>(IReadOnlyList<T> Items, IReadOnlyList<string> Errors);
