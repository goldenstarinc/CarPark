using System.Data;

// Вкладка "Долги"
public partial class MainForm
{
    TabPage DebtsTab()
    {
        var page = new TabPage("Долги");
        var grid = Grid();
        var sum = Box(60, 12, 100);
        var total = Lbl("", 400, 15);

        void Reload()
        {
            Fill(grid, Db.Debts());
            total.Text = "Общий долг: " + Db.TotalDebt() + " руб.";
        }

        var top = TopPanel(90);
        top.Controls.AddRange(new Control[]
        {
            Lbl("Сумма", 10, 15), sum, total,
            Btn("Оплатить", 10, 50, (s, e) => Run(() =>
            {
                int id = SelectedId(grid);
                decimal debt = Convert.ToDecimal(grid.CurrentRow.Cells["Долг"].Value);
                Db.Pay(id, Debt.CheckPayment(sum.Text, debt));
                sum.Text = "";
                Reload();
            }))
        });

        page.Enter += (s, e) => Reload();
        page.Controls.Add(grid);
        page.Controls.Add(top);
        Reload();
        return page;
    }
}
