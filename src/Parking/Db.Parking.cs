using System.Data;
using System.Globalization;

// Въезд, выезд и история стоянки
public static partial class Db
{
    const string TimeFormat = "yyyy-MM-dd HH:mm";

    static string Fmt(DateTime t)
    {
        return t.ToString(TimeFormat, CultureInfo.InvariantCulture);
    }

    static DateTime Parse(string s)
    {
        return DateTime.ParseExact(s, TimeFormat, CultureInfo.InvariantCulture);
    }

    static void InitParkings()
    {
        Exec(@"CREATE TABLE IF NOT EXISTS Parkings (
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 CarId INTEGER NOT NULL REFERENCES Cars(Id),
                 TimeIn TEXT NOT NULL,
                 TimeOut TEXT,
                 Cost REAL NOT NULL DEFAULT 0,
                 Paid REAL NOT NULL DEFAULT 0)");
    }

    // машины для выпадающего списка
    public static DataTable CarsList()
    {
        return Select("SELECT Id, Plate || ' ' || Brand AS Name FROM Cars ORDER BY Plate");
    }

    // машины, которые сейчас на стоянке
    public static DataTable Inside()
    {
        return Select(@"SELECT c.Plate AS Номер, c.Brand AS Марка, o.FullName AS Владелец, p.TimeIn AS Въезд, p.Id
                        FROM Parkings p JOIN Cars c ON c.Id = p.CarId JOIN Owners o ON o.Id = c.OwnerId
                        WHERE p.TimeOut IS NULL ORDER BY p.TimeIn");
    }

    // все записи стоянки
    public static DataTable History()
    {
        return Select(@"SELECT c.Plate AS Номер, c.Brand AS Марка, o.FullName AS Владелец,
                               p.TimeIn AS Въезд, p.TimeOut AS Выезд, p.Cost AS Стоимость, p.Paid AS Оплачено, p.Id
                        FROM Parkings p JOIN Cars c ON c.Id = p.CarId JOIN Owners o ON o.Id = c.OwnerId
                        ORDER BY p.TimeIn DESC");
    }

    // въезд машины
    public static void Enter(int carId, DateTime time)
    {
        var busy = Select("SELECT Id FROM Parkings WHERE CarId = @p0 AND TimeOut IS NULL", carId);
        if (busy.Rows.Count > 0)
            throw new ArgumentException("Эта машина уже на стоянке");
        Exec("INSERT INTO Parkings (CarId, TimeIn) VALUES (@p0, @p1)", carId, Fmt(time));
    }

    // выезд машины, возвращает стоимость
    public static decimal Exit(int parkingId, DateTime time)
    {
        var t = Select(@"SELECT p.TimeIn, o.Discount
                         FROM Parkings p JOIN Cars c ON c.Id = p.CarId JOIN Owners o ON o.Id = c.OwnerId
                         WHERE p.Id = @p0 AND p.TimeOut IS NULL", parkingId);
        if (t.Rows.Count == 0)
            throw new ArgumentException("Эта машина уже уехала");

        DateTime timeIn = Parse((string)t.Rows[0]["TimeIn"]);
        int discount = Convert.ToInt32(t.Rows[0]["Discount"]);
        decimal cost = Tariff.Calc(timeIn, Parse(Fmt(time)), discount);

        Exec("UPDATE Parkings SET TimeOut = @p0, Cost = @p1 WHERE Id = @p2", Fmt(time), (double)cost, parkingId);
        return cost;
    }
}
