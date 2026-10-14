// Тесты класса Check
static class CheckTests
{
    public static void Run()
    {
        Program.Test("Номер: нормальный номер принимается", () => Program.Equal("А123ВС77", Check.Plate("А123ВС77")));
        Program.Test("Номер: маленькие буквы становятся большими", () => Program.Equal("А123ВС77", Check.Plate("а123вс77")));
        Program.Test("Номер: пробелы по краям убираются", () => Program.Equal("А123ВС77", Check.Plate("  А123ВС77 ")));
        Program.Test("Номер: ровно 6 символов", () => Program.Equal("A12345", Check.Plate("A12345")));
        Program.Test("Номер: ровно 9 символов", () => Program.Equal("A12345678", Check.Plate("A12345678")));
        Program.Test("Номер: пустой - ошибка", () => Program.Throws(() => Check.Plate("")));
        Program.Test("Номер: только пробелы - ошибка", () => Program.Throws(() => Check.Plate("   ")));
        Program.Test("Номер: 5 символов - ошибка", () => Program.Throws(() => Check.Plate("A1234")));
        Program.Test("Номер: 10 символов - ошибка", () => Program.Throws(() => Check.Plate("A123456789")));
        Program.Test("Номер: спецсимвол - ошибка", () => Program.Throws(() => Check.Plate("A123-BC")));

        Program.Test("Скидка: 0 принимается", () => Program.Equal(0, Check.Discount("0")));
        Program.Test("Скидка: 100 принимается", () => Program.Equal(100, Check.Discount("100")));
        Program.Test("Скидка: 15 принимается", () => Program.Equal(15, Check.Discount("15")));
        Program.Test("Скидка: -1 - ошибка", () => Program.Throws(() => Check.Discount("-1")));
        Program.Test("Скидка: 101 - ошибка", () => Program.Throws(() => Check.Discount("101")));
        Program.Test("Скидка: буквы - ошибка", () => Program.Throws(() => Check.Discount("abc")));
        Program.Test("Скидка: пусто - ошибка", () => Program.Throws(() => Check.Discount("")));
        Program.Test("Скидка: дробное число - ошибка", () => Program.Throws(() => Check.Discount("5.5")));

        Program.Test("Поле: пробелы по краям убираются", () => Program.Equal("Иван", Check.Required(" Иван ", "ФИО")));
        Program.Test("Поле: пустое - ошибка", () => Program.Throws(() => Check.Required("", "ФИО")));
    }
}
