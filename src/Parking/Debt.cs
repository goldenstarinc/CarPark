using System.Globalization;

// Задолженность по оплате
public static class Debt
{
    // долг = стоимость - оплачено
    public static decimal Of(decimal cost, decimal paid)
    {
        return cost - paid;
    }

    // проверка суммы оплаты, возвращает сумму числом
    public static decimal CheckPayment(string text, decimal debt)
    {
        string t = text.Trim().Replace(',', '.');
        if (!decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sum))
            throw new ArgumentException("Сумма должна быть числом");
        if (sum <= 0)
            throw new ArgumentException("Сумма должна быть больше нуля");
        if (sum > debt)
            throw new ArgumentException("Сумма больше долга");
        return sum;
    }
}
