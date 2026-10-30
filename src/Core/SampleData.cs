using System.Collections.Generic;
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<Product> Products() => [
        Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 120),
        Product.Create("P-002", "SKU-002", "Пісок будівельний", "т", 18),
        Product.Create("P-003", "SKU-003", "Цегла червона", "шт", 4200),
        Product.Create("P-004", "SKU-004", "Фарба водоемульсійна 10л", "шт", 36),
        Product.Create("P-005", "SKU-005", "Шпаклівка фінішна", "кг", 250),
        Product.Create("P-006", "SKU-006", "Кабель UTP cat6", "м", 8),       // Дефіцит (< 10)
        Product.Create("P-007", "SKU-007", "Цвяхи будівельні 100мм", "кг", 45),
        Product.Create("P-008", "SKU-008", "Грунтовка глибокого проникнення 10л", "шт", 4), // Дефіцит (< 10)[cite: 9]
        Product.Create("P-009", "SKU-009", "Пінопласт 50мм", "м", 150),
        Product.Create("P-010", "SKU-010", "Монтажна піна", "шт", 65),
        Product.Create("P-011", "SKU-011", "Клей для плитки", "кг", 310),
        Product.Create("P-012", "SKU-012", "Арматура 12мм", "т", 5),         // Дефіцит (< 10)[cite: 9]
        Product.Create("P-013", "SKU-013", "Дюбель-цвях", "шт", 1200),
        Product.Create("P-014", "SKU-014", "Блок газобетонний", "шт", 850),
        Product.Create("P-015", "SKU-015", "Плита OSB-3 10мм", "шт", 40),
        Product.Create("P-016", "SKU-016", "Скоби будівельні", "шт", 3),    // Дефіцит (< 10)[cite: 9]
        Product.Create("P-017", "SKU-017", "Серпянка малярна", "м", 90)
    ];
}