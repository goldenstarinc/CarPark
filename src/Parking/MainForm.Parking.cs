using System.Data;

// Вкладки "Стоянка" и "История"
public partial class MainForm
{
    TabPage ParkingTab()
    {
        var page = new TabPage("Стоянка");
        var grid = Grid();
        var car = new ComboBox { Left = 60, Top = 12, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        var when = new DateTimePicker { Left = 400, Top = 12, Width = 160, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };

        void Reload() { Fill(grid, Db.Inside()); }

        void ReloadCars()
        {
            car.DataSource = Db.CarsList();
            car.DisplayMember = "Name";
            car.ValueMember = "Id";
        }

        var top = TopPanel(90);
        top.Controls.AddRange(new Control[]
        {
            Lbl("Машина", 10, 15), car,
            Lbl("Время", 340, 15), when,
            Btn("Въезд", 10, 50, (s, e) => Run(() =>
            {
                if (car.SelectedValue == null)
                    throw new ArgumentException("Сначала добавьте машину");
                Db.Enter(Convert.ToInt32(car.SelectedValue), when.Value);
                Reload();
            })),
            Btn("Выезд", 120, 50, (s, e) => Run(() =>
            {
                int id = SelectedId(grid);
                decimal cost = Db.Exit(id, when.Value);
                if (cost > 0 && MessageBox.Show("К оплате: " + cost + " руб. Оплатить сейчас?", "Выезд", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Db.Pay(id, cost);
                Reload();
            }))
        });

        page.Enter += (s, e) => { ReloadCars(); Reload(); };
        page.Controls.Add(grid);
        page.Controls.Add(top);
        ReloadCars();
        Reload();
        return page;
    }

    TabPage HistoryTab()
    {
        var page = new TabPage("История");
        var grid = Grid();
        page.Enter += (s, e) => Fill(grid, Db.History());
        page.Controls.Add(grid);
        Fill(grid, Db.History());
        return page;
    }
}
