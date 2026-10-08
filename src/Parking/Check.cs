// Проверка данных, которые вводит пользователь
public static class Check
{
    // обязательное текстовое поле
    public static string Required(string text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Заполните поле: " + field);
        return text.Trim();
    }

    // госномер: 6-9 букв или цифр, возвращает номер в верхнем регистре
    public static string Plate(string text)
    {
        string plate = Required(text, "Госномер").ToUpper();
        if (plate.Length < 6 || plate.Length > 9)
            throw new ArgumentException("Госномер: от 6 до 9 символов");
        foreach (char ch in plate)
            if (!char.IsLetterOrDigit(ch))
                throw new ArgumentException("Госномер: только буквы и цифры");
        return plate;
    }

    // скидка: целое число от 0 до 100
    public static int Discount(string text)
    {
        if (!int.TryParse(text.Trim(), out int d))
            throw new ArgumentException("Скидка должна быть целым числом");
        if (d < 0 || d > 100)
            throw new ArgumentException("Скидка: от 0 до 100");
        return d;
    }
}
