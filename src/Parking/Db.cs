using System.Data;
using Microsoft.Data.Sqlite;

// Работа с базой SQLite
public static partial class Db
{
    const string ConnStr = "Data Source=parking.db";

    static SqliteConnection Open()
    {
        var c = new SqliteConnection(ConnStr);
        c.Open();
        using var pragma = c.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON";
        pragma.ExecuteNonQuery();
        return c;
    }

    static SqliteCommand Make(SqliteConnection c, string sql, object[] args)
    {
        var cmd = c.CreateCommand();
        cmd.CommandText = sql;
        for (int i = 0; i < args.Length; i++)
            cmd.Parameters.AddWithValue("@p" + i, args[i] ?? DBNull.Value);
        return cmd;
    }

    // запрос без результата (INSERT, UPDATE, DELETE)
    static void Exec(string sql, params object[] args)
    {
        using var c = Open();
        using var cmd = Make(c, sql, args);
        cmd.ExecuteNonQuery();
    }

    // запрос с результатом (SELECT)
    static DataTable Select(string sql, params object[] args)
    {
        using var c = Open();
        using var cmd = Make(c, sql, args);
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        for (int i = 0; i < reader.FieldCount; i++)
            table.Columns.Add(reader.GetName(i), typeof(object));
        while (reader.Read())
        {
            var row = table.NewRow();
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i);
            table.Rows.Add(row);
        }
        return table;
    }

    // создание таблиц
    public static void Init()
    {
        Exec(@"CREATE TABLE IF NOT EXISTS Owners (
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 FullName TEXT NOT NULL,
                 Phone TEXT,
                 Discount INTEGER NOT NULL DEFAULT 0 CHECK (Discount BETWEEN 0 AND 100))");
        Exec(@"CREATE TABLE IF NOT EXISTS Cars (
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 Plate TEXT NOT NULL UNIQUE,
                 Brand TEXT NOT NULL,
                 OwnerId INTEGER NOT NULL REFERENCES Owners(Id))");
        InitParkings();
    }

    // ----- владельцы -----

    public static DataTable Owners()
    {
        return Select("SELECT FullName AS ФИО, Phone AS Телефон, Discount AS Скидка, Id FROM Owners ORDER BY FullName");
    }

    public static void AddOwner(string name, string phone, int discount)
    {
        Exec("INSERT INTO Owners (FullName, Phone, Discount) VALUES (@p0, @p1, @p2)", name, phone, discount);
    }

    public static void UpdateOwner(int id, string name, string phone, int discount)
    {
        Exec("UPDATE Owners SET FullName = @p0, Phone = @p1, Discount = @p2 WHERE Id = @p3", name, phone, discount, id);
    }

    public static void DeleteOwner(int id)
    {
        Exec("DELETE FROM Owners WHERE Id = @p0", id);
    }

    // ----- машины -----

    public static DataTable Cars()
    {
        return Select(@"SELECT c.Plate AS Номер, c.Brand AS Марка, o.FullName AS Владелец, c.OwnerId, c.Id
                        FROM Cars c JOIN Owners o ON o.Id = c.OwnerId ORDER BY c.Plate");
    }

    public static void AddCar(string plate, string brand, int ownerId)
    {
        Exec("INSERT INTO Cars (Plate, Brand, OwnerId) VALUES (@p0, @p1, @p2)", plate, brand, ownerId);
    }

    public static void UpdateCar(int id, string plate, string brand, int ownerId)
    {
        Exec("UPDATE Cars SET Plate = @p0, Brand = @p1, OwnerId = @p2 WHERE Id = @p3", plate, brand, ownerId, id);
    }

    public static void DeleteCar(int id)
    {
        Exec("DELETE FROM Cars WHERE Id = @p0", id);
    }
}
