// Тесты класса Tariff. Цена часа 50, первые 15 минут бесплатно
static class TariffTests
{
    static DateTime Start = new DateTime(2026, 10, 20, 10, 0, 0);

    static decimal Cost(int minutes, int discount)
    {
        return Tariff.Calc(Start, Start.AddMinutes(minutes), discount);
    }

    public static void Run()
    {
        Program.Test("Тариф: 0 минут - бесплатно", () => Program.Equal(0m, Cost(0, 0)));
        Program.Test("Тариф: 10 минут - бесплатно", () => Program.Equal(0m, Cost(10, 0)));
        Program.Test("Тариф: ровно 15 минут - бесплатно", () => Program.Equal(0m, Cost(15, 0)));
        Program.Test("Тариф: 16 минут - один час", () => Program.Equal(50m, Cost(16, 0)));
        Program.Test("Тариф: ровно 60 минут - один час", () => Program.Equal(50m, Cost(60, 0)));
        Program.Test("Тариф: 61 минута - два часа", () => Program.Equal(100m, Cost(61, 0)));
        Program.Test("Тариф: 3 часа", () => Program.Equal(150m, Cost(180, 0)));
        Program.Test("Тариф: сутки", () => Program.Equal(1200m, Cost(24 * 60, 0)));
        Program.Test("Тариф: сутки и минута", () => Program.Equal(1250m, Cost(24 * 60 + 1, 0)));

        Program.Test("Скидка 10% на 2 часа", () => Program.Equal(90m, Cost(120, 10)));
        Program.Test("Скидка 50% на 1 час", () => Program.Equal(25m, Cost(60, 50)));
        Program.Test("Скидка 100% - бесплатно", () => Program.Equal(0m, Cost(120, 100)));
        Program.Test("Скидка 0% - полная цена", () => Program.Equal(100m, Cost(120, 0)));

        Program.Test("Выезд раньше въезда - ошибка", () => Program.Throws(() => Cost(-5, 0)));
        Program.Test("Скидка -1 - ошибка", () => Program.Throws(() => Cost(60, -1)));
        Program.Test("Скидка 101 - ошибка", () => Program.Throws(() => Cost(60, 101)));
    }
}
