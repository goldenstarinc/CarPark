// Тесты класса Debt
static class DebtTests
{
    public static void Run()
    {
        Program.Test("Долг: 100 стоимость, 30 оплачено", () => Program.Equal(70m, Debt.Of(100, 30)));
        Program.Test("Долг: всё оплачено", () => Program.Equal(0m, Debt.Of(100, 100)));
        Program.Test("Долг: ничего не оплачено", () => Program.Equal(100m, Debt.Of(100, 0)));

        Program.Test("Оплата: 50 при долге 100", () => Program.Equal(50m, Debt.CheckPayment("50", 100)));
        Program.Test("Оплата: ровно весь долг", () => Program.Equal(100m, Debt.CheckPayment("100", 100)));
        Program.Test("Оплата: запятая как разделитель", () => Program.Equal(25.5m, Debt.CheckPayment("25,5", 100)));
        Program.Test("Оплата: 0 - ошибка", () => Program.Throws(() => Debt.CheckPayment("0", 100)));
        Program.Test("Оплата: отрицательная - ошибка", () => Program.Throws(() => Debt.CheckPayment("-5", 100)));
        Program.Test("Оплата: больше долга - ошибка", () => Program.Throws(() => Debt.CheckPayment("150", 100)));
        Program.Test("Оплата: не число - ошибка", () => Program.Throws(() => Debt.CheckPayment("abc", 100)));
        Program.Test("Оплата: пусто - ошибка", () => Program.Throws(() => Debt.CheckPayment("", 100)));
    }
}
