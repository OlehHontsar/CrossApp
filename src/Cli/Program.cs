using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Domain;
using Core.Events; // НОВЕ: простір імен для подій та аргументів ЛР6

// ====================================================
// 🏢 КОМПОЗИТОР ЗАЛЕЖНОСТЕЙ (Composition Root - ЛР5)
// ====================================================
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

// Ручне впровадження залежностей (Manual DI) залежно від аргументів командного рядка
ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

Console.WriteLine($"[ЛР5/ЛР6] Сховище: {store.GetType().Name}");
Console.WriteLine(new string('-', 50));

// ====================================================
// 🔔 ЛАБОРАТОРНА РОБОТА №6: Налаштування подій та підписок
// ====================================================

// 1. Налаштування папки та файлу логів (створення каталогу кодом)
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

// 2. Перший підписник: локальний метод для виведення в консоль і запису у файл[cite: 5]
void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} залишок {e.QuantityAfter}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

// Підписуємося на подію першим обробником[cite: 5]
service.Changed += OnCatalogChanged;

// 3. Другий підписник: лямбда-вираз із замиканням над зовнішньою змінною-лічильником[cite: 3, 6]
int changes = 0;
service.Changed += (_, _) => changes++;

try
{
    // 1. Успішний сценарій: додавання товару та реєстрація приходу через сервіс
    var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
    Console.WriteLine($"Додано товар через сервіс: {created}");

    service.Receive(created.Id, 25);
    Console.WriteLine($"Після приходу (+25): {service.Find(created.Id)}");

    // Вивід кількості подій, зафіксованих лямбдою
    Console.WriteLine($"[ЛР6] Отримано подій лічильником: {changes}");

    // 2. Вивід списку всіх записів
    Console.WriteLine("\nПовний список товарів у сховищі:");
    foreach (var p in service.All())
    {
        Console.WriteLine($" - {p}");
    }

    // ====================================================
    // 🔄 СЦЕНАРІЙ ВІДПИСКИ (ЛР6)
    // ====================================================
    Console.WriteLine("\n=== Тестування відписки ( -= ) ===");
    service.Changed -= OnCatalogChanged; // Відписуємо локальний метод

    // Робимо ще одну зміну: консоль і файл уже мовчать, але лямбда-лічильник продовжує працювати[cite: 6]
    service.Receive(created.Id, 10);
    Console.WriteLine($"[ЛР6] Подій після відписки (консоль мовчить, лічильник = {changes}): {changes}");
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка виконання бізнес-операції: {ex.Message}");
}

// 3. Сценарій відмови: спроба оновити неіснуючий запис
Console.WriteLine("\n=== Сценарій відмови (перевірка винятків та відсутності події) ====");
try
{
    // Некоректна операція викличе виняток, і подія ПРИ ЦЬОМУ НЕ ПІДНІМЕТЬСЯ[cite: 6]
    service.Receive("invalid-id-999", 10);
}
catch (Exception ex)
{
    Console.WriteLine($"Перехоплено очікуваний виняток: {ex.GetType().Name} — {ex.Message}");
}

// ====================================================
// 🔎 ВИКОРИСТАННЯ Func (ЛР6, Крок 7)
// ====================================================
Console.WriteLine("\n=== Використання делегата Func (FindAll) ===");
// Використовуємо Func для фільтрації сутностей за умовою
var filtered = service.FindAll(p => p.Quantity < 100);
Console.WriteLine($"Знайдено товарів із залишком < 100: {filtered.Count}");

return 0;