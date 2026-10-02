using System;
using System.IO;
using System.Linq;
using Core.Dto;
using Core.Import;
using Core.Domain; 

// ====================================================
// 📊 ШАР ЛАБОРАТОРНОЇ РОБОТИ №2 та №3 (Ваш відмовостійкий імпорт)
// ====================================================
string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv"); 

if (!File.Exists(path)) 
{ 
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}"); 
    return 1; 
} 

string extension = Path.GetExtension(path).ToLower();

ImportResult<IDomainDto> result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => throw new InvalidOperationException($"Непідтримуване розширення файлу: '{extension}'")
};

Console.WriteLine($"[ЛР3] Успішно завантажено DTO-записів з файлу: {result.Items.Count}"); 

// ====================================================
// 🧩 ШАР ЛАБОРАТОРНОЇ РОБОТИ №4 (Доменна модель та інваріанти)
// ====================================================
Console.WriteLine("\n====================================================");
Console.WriteLine("Лабораторна робота №4: Тестування доменних інваріантів");
Console.WriteLine("====================================================");

// ЗВ'ЯЗОК ТИЖНІВ: Пробуємо перетворити завантажені DTO на доменні сутності через FromDto
Console.WriteLine("\n--- Крок 1: Конвертація DTO-записів у сутності домену ---");
int validEntitiesCount = 0;
int domainErrorsCount = 0;

foreach (IDomainDto dto in result.Items)
{
    // Оскільки FromDto приймає лише ProductDto, фільтруємо через pattern matching
    if (dto is ProductDto prodDto)
    {
        try
        {
            // Метод FromDto автоматично запустить Create() та перевірить усі інваріанти!
            Product domainProd = Product.FromDto(prodDto);
            validEntitiesCount++;
        }
        catch (Exception ex)
        {
            domainErrorsCount++;
            Console.WriteLine($"  ! Бізнес-помилка для ID {prodDto.Id}: {ex.Message}");
        }
    }
}
Console.WriteLine($"Результат: Створено валідних сутностей: {validEntitiesCount}, відхилено інваріантами: {domainErrorsCount}");


// ДЕМОНСТРАЦІЯ СЦЕНАРІЇВ СТРОГО ЗА МЕТОДИЧКОЮ
Console.WriteLine("\n=== Сценарій 1: успіх ===");
try
{
    Product product = Product.Create("P-001", "sku-001", "Цемент М400 25кг", "шт", 100);
    Console.WriteLine(product);
    product.RegisterArrival(50);
    product.Issue(30);
    Console.WriteLine(product);
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine("\n=== Сценарій 2: порушення інваріантів ===");
// Локальний метод демонстрації відмов без падіння програми
static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" {title}: {ex.GetType().Name} — {ex.Message}");
    }
}

// Тестуємо інваріанти на свіжому об'єкті
Product testProduct = Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 120);
TryDo("видача більша за залишок", () => testProduct.Issue(1000));
TryDo("порожній SKU", () => Product.Create("P-002", " ", "Пісок", "т", 10));
TryDo("від'ємний залишок", () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

return 0;
