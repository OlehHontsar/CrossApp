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

// ПОВЕРТАЄМО НА МІСЦЕ: Новий рекорд складу, який шукають ваші імпортери
public record WarehouseDto(
    string Id, 
    string Sku, 
    string Name, 
    string Location, 
    int Capacity
) : IDomainDto;
