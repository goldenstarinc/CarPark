# Лабораторная работа №2. UML-модель проекта
Вариант 14. Модуль «Автостоянка».

**Цель:** разработать систему требований и UML-модель проекта (диаграммы использования, последовательности, активности и классов) и связать её с кодом на C#.

> Диаграммы написаны в формате Mermaid: GitHub и Visual Studio Code показывают их как картинки. В методичке для этого используется Modeling Project в Visual Studio 2013; в новых версиях Visual Studio нет проекта моделирования, поэтому модель оформлена текстом.

## 1. Система требований
По группам из методички (ITILv3).

| Группа | Требование |
|---|---|
| Функциональные | Вести владельцев (ФИО, телефон, скидка) и машины (госномер, марка, владелец) |
| Функциональные | Отмечать въезд и выезд, фиксировать время |
| Функциональные | Считать стоимость: 50 руб. за начатый час, 15 минут бесплатно, минус скидка владельца |
| Функциональные | Вести долги, принимать оплату, показывать общую сумму долгов |
| Функциональные | Показывать историю всех стоянок |
| Управленческие | Данные хранятся в одном файле `parking.db`; копия файла = резервная копия |
| Эргономические | Пять вкладок, русский язык, понятные сообщения об ошибках |
| Архитектурные | Расчёты (`Tariff`, `Debt`, `Check`) не зависят от окна и базы данных |
| Взаимодействия | Внешних систем нет, обмен только с SQLite |
| Сервисного уровня | Любое действие не дольше 1 секунды; неверный ввод не записывается в БД |

## 2. Диаграмма использования (Use Case)
```mermaid
flowchart LR
    A([Администратор])
    subgraph Автостоянка
        U1(Вести владельцев)
        U2(Вести машины)
        U3(Отметить въезд)
        U4(Отметить выезд)
        U5(Рассчитать стоимость)
        U6(Принять оплату)
        U7(Смотреть историю)
        U8(Смотреть долги)
    end
    A --> U1
    A --> U2
    A --> U3
    A --> U4
    A --> U6
    A --> U7
    A --> U8
    U4 -. include .-> U5
    U6 -. extend .-> U8
```

## 3. Диаграмма последовательности: выезд машины
```mermaid
sequenceDiagram
    actor Админ as Администратор
    participant F as MainForm
    participant D as Db
    participant T as Tariff
    participant S as SQLite
    Админ->>F: выбрать машину, «Выезд»
    F->>D: Exit(parkingId, время)
    D->>S: SELECT время въезда и скидка владельца
    S-->>D: TimeIn, Discount
    D->>T: Calc(TimeIn, TimeOut, Discount)
    T-->>D: стоимость
    D->>S: UPDATE Parkings (TimeOut, Cost)
    D-->>F: стоимость
    F-->>Админ: сообщение со стоимостью, обновление таблиц
```

## 4. Диаграмма активности: приём оплаты
```mermaid
flowchart TD
    Start([Начало]) --> Sel[Выбрать строку на вкладке «Долги»]
    Sel --> Inp[Ввести сумму]
    Inp --> Num{Это число?}
    Num -- нет --> Err[Сообщение об ошибке]
    Num -- да --> Pos{Больше 0?}
    Pos -- нет --> Err
    Pos -- да --> Lim{Не больше долга?}
    Lim -- нет --> Err
    Lim -- да --> Save[Debt.CheckPayment -> Db.Pay]
    Save --> Upd[Обновить таблицу и общую сумму долгов]
    Err --> Inp
    Upd --> End([Конец])
```

## 5. Диаграмма классов
```mermaid
classDiagram
    class MainForm {
        +OwnersTab()
        +CarsTab()
        +ParkingTab()
        +HistoryTab()
        +DebtsTab()
    }
    class Db {
        <<static>>
        +Init()
        +Owners() DataTable
        +AddOwner(name, phone, discount)
        +UpdateOwner(id, name, phone, discount)
        +DeleteOwner(id)
        +Cars() DataTable
        +AddCar(plate, brand, ownerId)
        +UpdateCar(id, plate, brand, ownerId)
        +DeleteCar(id)
        +Enter(carId, time)
        +Exit(parkingId, time) decimal
        +History() DataTable
        +Debts() DataTable
        +TotalDebt() decimal
        +Pay(parkingId, sum)
    }
    class Tariff {
        <<static>>
        +HourPrice = 50
        +FreeMinutes = 15
        +Calc(timeIn, timeOut, discount) decimal
    }
    class Debt {
        <<static>>
        +Of(cost, paid) decimal
        +CheckPayment(text, debt) decimal
    }
    class Check {
        <<static>>
        +Required(text, field) string
        +Plate(text) string
        +Discount(text) int
    }
    MainForm ..> Db : вызывает
    MainForm ..> Check : проверяет ввод
    MainForm ..> Debt : проверяет оплату
    Db ..> Tariff : считает стоимость
```
Класс `Db` разбит на три файла (`Db.cs`, `Db.Parking.cs`, `Db.Debts.cs`) через `partial`, а `MainForm` — на `MainForm.cs`, `MainForm.Parking.cs`, `MainForm.Debts.cs`.

Диаграмма данных (таблицы из `docs/schema.sql`):
```mermaid
erDiagram
    Owners ||--o{ Cars : владеет
    Cars ||--o{ Parkings : стоит
    Owners {
        int Id
        string FullName
        string Phone
        int Discount
    }
    Cars {
        int Id
        string Plate
        string Brand
        int OwnerId
    }
    Parkings {
        int Id
        int CarId
        string TimeIn
        string TimeOut
        real Cost
        real Paid
    }
```

## 6. Связь модели с кодом
Код программы написан по этой модели вручную: автогенерации кода из модели в Visual Studio 2022 нет. Каждый класс диаграммы соответствует файлу в `src/Parking`; методы совпадают по названиям. Доработка кода после генерации (из методички) заменена обычной разработкой, рефакторинг — выделением расчётов `Tariff`, `Debt`, `Check` из окна в отдельные классы.

---

## Ответы на вопросы
1. **Какие этапы жизненного цикла поддерживает Visual Studio?** Требования, спецификацию, проектирование, реализацию, верификацию (тестирование), выпуск и сопровождение.
2. **Какие UML-диаграммы можно построить?** Диаграммы классов, последовательности, использования (прецедентов), активности, компонентов; в версиях с проектом моделирования также слоёв и схемы зависимостей.
3. **Возможности автоматической генерации кода?** По диаграмме классов создаются классы, поля, методы с заглушками и связи между ними. Это экономит время на описании структуры, но готовой программой код не является: тела методов нужно дописать.
4. **Рефакторинг, возможности и ограничения?** Переименование, выделение метода (Extract Method), изменение сигнатуры и др. без изменения поведения программы. Ограничение: рефакторинг не исправляет логику и требует проверки тестами.
