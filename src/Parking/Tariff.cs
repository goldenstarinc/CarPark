// Расчёт стоимости стоянки
public static class Tariff
{
    public const decimal HourPrice = 50;
    public const int FreeMinutes = 15;

    // стоимость: первые 15 минут бесплатно, потом каждый начатый час, минус скидка в процентах
    public static decimal Calc(DateTime timeIn, DateTime timeOut, int discount)
    {
        if (timeOut < timeIn)
            throw new ArgumentException("Время выезда раньше времени въезда");
        if (discount < 0 || discount > 100)
            throw new ArgumentException("Скидка: от 0 до 100");

        double minutes = (timeOut - timeIn).TotalMinutes;
        if (minutes <= FreeMinutes)
            return 0;

        int hours = (int)Math.Ceiling(minutes / 60);
        decimal sum = hours * HourPrice;
        return sum - sum * discount / 100;
    }
}
