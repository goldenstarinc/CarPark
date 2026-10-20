using System.Data;
using Microsoft.Data.Sqlite;

// Главное окно: вкладки
public partial class MainForm : Form
{
    public MainForm()
    {
        Text = "Автостоянка";
        Width = 900;
        Height = 600;
        BackColor = Color.White;
        ForeColor = Color.Black;
        AutoScaleMode = AutoScaleMode.Dpi;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(OwnersTab());
        tabs.TabPages.Add(CarsTab());
        tabs.TabPages.Add(ParkingTab());
        tabs.TabPages.Add(HistoryTab());
        tabs.TabPages.Add(DebtsTab());
        Controls.Add(tabs);
    }

    // ----- вспомогательные методы для создания элементов -----

    static Label Lbl(string text, int x, int y)
    {
        return new Label { Text = text, Left = x, Top = y, AutoSize = true };
    }

    static TextBox Box(int x, int y, int width)
    {
        return new TextBox { Left = x, Top = y, Width = width, BorderStyle = BorderStyle.FixedSingle };
    }

    static Button Btn(string text, int x, int y, EventHandler click)
    {
        var b = new Button { Text = text, Left = x, Top = y, Width = 100, Height = 28, FlatStyle = FlatStyle.Flat, BackColor = Color.White };
        b.FlatAppearance.BorderColor = Color.Black;
        b.Click += click;
        return b;
    }

    static DataGridView Grid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            EnableHeadersVisualStyles = false
        };
        g.DefaultCellStyle.SelectionBackColor = Color.Black;
        g.DefaultCellStyle.SelectionForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        return g;
    }

    static Panel TopPanel(int height)
    {
        return new Panel { Dock = DockStyle.Top, Height = height };
    }

    // показать таблицу в сетке, служебные колонки скрыть
    static void Fill(DataGridView g, DataTable t)
    {
        g.DataSource = t;
        foreach (DataGridViewColumn c in g.Columns)
            if (c.Name == "Id" || c.Name == "OwnerId")
                c.Visible = false;
    }

    // Id выбранной строки
    static int SelectedId(DataGridView g)
    {
        if (g.CurrentRow == null)
            throw new ArgumentException("Выберите строку в таблице");
        return Convert.ToInt32(g.CurrentRow.Cells["Id"].Value);
    }

    // выполнить действие и показать ошибку, если она есть
    static void Run(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (SqliteException)
        {
            MessageBox.Show("Ошибка базы: такая запись уже есть или она связана с другими данными");
        }
    }

    // ----- вкладка "Владельцы" -----

    TabPage OwnersTab()
    {
        var page = new TabPage("Владельцы");
        var grid = Grid();
        var name = Box(50, 12, 220);
        var phone = Box(350, 12, 140);
        var discount = Box(590, 12, 50);
        discount.Text = "0";

        void Reload() { Fill(grid, Db.Owners()); }

        var top = TopPanel(90);
        top.Controls.AddRange(new Control[]
        {
            Lbl("ФИО", 10, 15), name,
            Lbl("Телефон", 285, 15), phone,
            Lbl("Скидка, %", 510, 15), discount,
            Btn("Добавить", 10, 50, (s, e) => Run(() =>
            {
                Db.AddOwner(Check.Required(name.Text, "ФИО"), phone.Text.Trim(), Check.Discount(discount.Text));
                Reload();
            })),
            Btn("Изменить", 120, 50, (s, e) => Run(() =>
            {
                Db.UpdateOwner(SelectedId(grid), Check.Required(name.Text, "ФИО"), phone.Text.Trim(), Check.Discount(discount.Text));
                Reload();
            })),
            Btn("Удалить", 230, 50, (s, e) => Run(() =>
            {
                Db.DeleteOwner(SelectedId(grid));
                Reload();
            }))
        });

        grid.SelectionChanged += (s, e) =>
        {
            if (grid.CurrentRow == null) return;
            name.Text = grid.CurrentRow.Cells["ФИО"].Value.ToString();
            phone.Text = grid.CurrentRow.Cells["Телефон"].Value.ToString();
            discount.Text = grid.CurrentRow.Cells["Скидка"].Value.ToString();
        };

        page.Controls.Add(grid);
        page.Controls.Add(top);
        Reload();
        return page;
    }

    // ----- вкладка "Машины" -----

    TabPage CarsTab()
    {
        var page = new TabPage("Машины");
        var grid = Grid();
        var plate = Box(70, 12, 120);
        var brand = Box(270, 12, 140);
        var owner = new ComboBox { Left = 500, Top = 12, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

        void Reload() { Fill(grid, Db.Cars()); }

        void ReloadOwners()
        {
            owner.DataSource = Db.Owners();
            owner.DisplayMember = "ФИО";
            owner.ValueMember = "Id";
        }

        int OwnerId()
        {
            if (owner.SelectedValue == null)
                throw new ArgumentException("Сначала добавьте владельца");
            return Convert.ToInt32(owner.SelectedValue);
        }

        var top = TopPanel(90);
        top.Controls.AddRange(new Control[]
        {
            Lbl("Госномер", 10, 15), plate,
            Lbl("Марка", 205, 15), brand,
            Lbl("Владелец", 425, 15), owner,
            Btn("Добавить", 10, 50, (s, e) => Run(() =>
            {
                Db.AddCar(Check.Plate(plate.Text), Check.Required(brand.Text, "Марка"), OwnerId());
                Reload();
            })),
            Btn("Изменить", 120, 50, (s, e) => Run(() =>
            {
                Db.UpdateCar(SelectedId(grid), Check.Plate(plate.Text), Check.Required(brand.Text, "Марка"), OwnerId());
                Reload();
            })),
            Btn("Удалить", 230, 50, (s, e) => Run(() =>
            {
                Db.DeleteCar(SelectedId(grid));
                Reload();
            }))
        });

        grid.SelectionChanged += (s, e) =>
        {
            if (grid.CurrentRow == null) return;
            plate.Text = grid.CurrentRow.Cells["Номер"].Value.ToString();
            brand.Text = grid.CurrentRow.Cells["Марка"].Value.ToString();
            owner.SelectedValue = grid.CurrentRow.Cells["OwnerId"].Value;
        };

        page.Enter += (s, e) => ReloadOwners();
        page.Controls.Add(grid);
        page.Controls.Add(top);
        ReloadOwners();
        Reload();
        return page;
    }
}
