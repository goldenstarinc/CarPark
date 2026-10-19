# Автостоянка (вариант 14)

Программа учёта автостоянки: владельцы, машины, въезд и выезд, стоимость со скидкой, долги.
C# (.NET 8), Windows Forms, SQLite.

## Запуск
Открыть папку в Visual Studio 2022 (нужна нагрузка «Разработка классических приложений .NET») и запустить проект `src/Parking`.
Или в консоли Windows:
```
dotnet run --project src/Parking
```
База `parking.db` создаётся сама рядом с программой.

## Тесты
```
dotnet run --project tests/Parking.Tests
```
