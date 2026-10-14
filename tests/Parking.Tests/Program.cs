// Простой запуск тестов: "." - тест прошёл, "E" - тест упал
static class Program
{
    static int passed = 0;
    static int failed = 0;

    static int Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("CheckTests");
        CheckTests.Run();

        Console.WriteLine();
        Console.WriteLine("Прошло: " + passed + ", упало: " + failed);
        return failed == 0 ? 0 : 1;
    }

    public static void Test(string name, Action body)
    {
        try
        {
            body();
            passed++;
            Console.WriteLine("  .  " + name);
        }
        catch (Exception ex)
        {
            failed++;
            Console.WriteLine("  E  " + name + " -> " + ex.Message);
        }
    }

    public static void Equal(object expected, object actual)
    {
        if (!expected.Equals(actual))
            throw new Exception("ожидалось " + expected + ", получено " + actual);
    }

    public static void Throws(Action body)
    {
        try { body(); }
        catch (ArgumentException) { return; }
        throw new Exception("ожидалась ошибка, но её не было");
    }
}
