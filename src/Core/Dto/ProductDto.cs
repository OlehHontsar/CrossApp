using System.Collections.Generic;

namespace Core.Dto;

// Спільний інтерфейс для поліморфної обробки різнорідних даних домену
public interface IDomainDto 
{ 
    string Id { get; } 
    string Name { get; } 
}

// Імутабельний рекорд товару (сумісний із CSV-варіантом «Склад» та JSON)
public record ProductDto(
    string Id, 
    string Sku, 
    string Name, 
    string Unit, 
    int Quantity, 
    string? Note = null
) : IDomainDto;

// Новий рекорд складу, доданий у межах поліморфного розбору
public record WarehouseDto(
    string Id, 
    string Sku, 
    string Name, 
    string Location, 
    int Capacity
) : IDomainDto;

// Універсальний узагальнений контейнер для відмовостійкої передачі результатів
public sealed record ImportResult<T>(
    IReadOnlyList<T> Items, 
    IReadOnlyList<string> Errors
);
