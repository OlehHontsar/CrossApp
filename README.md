# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine. 
Призначення: оформлення замовлень і підрахунок сум. 

## Запуск 
```bash
# Збірка всього рішення
dotnet build 

# Локальний запуск консольного інтерфейсу
dotnet run --project src/Cli 
```

## Середовище 
.NET SDK 10.0, Windows 11 x64

---

## Лабораторна робота 2: Бібліотека Core + Cli, multi-targeting, публікація

### Структура проєкту та конвенція каталогів `Core`
Для забезпечення чистої архітектури всю системну логіку відокремлено від консольної точки входу (`Cli`) у бібліотеку класів (`Core`). Згідно з домовленістю на семестр, у проєкті `Core` закладено структуру каталогів для нашої предметної області:
* `Core/Dto/` — record-типи формату даних (тиждень 3): `CustomerDto`, `ProductDto`, `OrderDto`, `OrderLineDto`
* `Core/Domain/` — сутності з поведінкою та інваріантами (тиждень 4): `Customer`, `Product`, `Order`, `OrderLine`
* `Core/Storage/` — реалізації сховищ даних (тиждень 5)

*Примітка: для збереження порожніх папок у Git до них додано файли-заглушки `.gitkeep`.*

### Поточна структура каталогів рішення після рефакторингу
```text
CrossApp/
├── CrossApp.slnx
├── README.md
├── .gitignore
└── src/
    ├── Core/
    │   ├── Core.csproj
    │   ├── EnvironmentInfo.cs   # Збір даних середовища (record + static class)
    │   ├── Dto/                 # [Планування]
    │   ├── Domain/              # [Планування]
    │   └── Storage/             # [Планування]
    └── Cli/
        ├── Cli.csproj           # Містить ProjectReference на Core
        └── Program.cs           # Консольне виведення звіту
```

### Результати порівняння режимів публікації

Збірка та вимірювання розмірів здійснювалися на ОС **Windows 11 x64** за допомогою команд:
* `dotnet publish src/Cli -c Release -r win-x64 --self-contained true`
* `dotnet publish src/Cli -c Release -r win-x64 --self-contained false`

| RID | Режим | Розмір publish | Потрібен runtime |
| :--- | :--- | :--- | :--- |
| **win-x64** | self-contained | ~76.68 МБ | ні (середовище виконання .NET інтегровано в бінарник) |
| **win-x64** | framework-dependent | ~0.19 МБ | так (.NET 10 має бути встановлений в системі) |

### Самоперевірка (Definition of Done)
1. Проєкти `Cli` та `Core` зв'язані односпрямованим посиланням `Cli -> Core`.
2. У `Program.cs` повністю відсутня бізнес-логіка та прямі виклики `RuntimeInformation` / `Environment`.
3. Застосунок успішно збирається та запускається безпосередньо з каталогу публікації (`publish/Cli.exe`).
4. Технічні каталоги `bin/`, `obj/` та `publish/` заігнорені й не потрапляють у коміт.
