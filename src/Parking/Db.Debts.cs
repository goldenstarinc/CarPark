using System.Data;

// Долги и оплата
public static partial class Db
{
    // завершённые стоянки, по которым не всё оплачено
    public static DataTable Debts()
    {
        return Select(@"SELECT c.Plate AS Номер, o.FullName AS Владелец, p.TimeOut AS Выезд,
                               p.Cost AS Стоимость, p.Paid AS Оплачено, p.Cost - p.Paid AS Долг, p.Id
                        FROM Parkings p JOIN Cars c ON c.Id = p.CarId JOIN Owners o ON o.Id = c.OwnerId
                        WHERE p.TimeOut IS NOT NULL AND p.Cost - p.Paid > 0 ORDER BY p.TimeOut");
    }

    // общая сумма долгов
    public static decimal TotalDebt()
    {
        var t = Select("SELECT IFNULL(SUM(Cost - Paid), 0) AS Total FROM Parkings WHERE TimeOut IS NOT NULL");
        return Convert.ToDecimal(t.Rows[0]["Total"]);
    }

    // принять оплату
    public static void Pay(int parkingId, decimal sum)
    {
        Exec("UPDATE Parkings SET Paid = Paid + @p0 WHERE Id = @p1", (double)sum, parkingId);
    }
}
