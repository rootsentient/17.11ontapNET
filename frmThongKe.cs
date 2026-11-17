using System;
using System.Windows.Forms;
using SalesApp.BUS;

namespace SalesApp.GUI
{
    public class frmThongKe : Form
    {
        readonly ThongKeBUS bus = new ThongKeBUS();
        DateTimePicker dtp;
        NumericUpDown nudThang, nudNam;
        Label lblNgay, lblThang;
        DataGridView dgv;

        public frmThongKe()
        {
            Text = "Báo cáo - Thống kê"; Width = 800; Height = 520;
            dgv = UiHelper.NewGrid();
            Controls.Add(dgv);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 140 };
            Controls.Add(pnl);

            pnl.Controls.Add(new Label { Text = "Ngày", Left = 10, Top = 18, Width = 50 });
            dtp = new DateTimePicker { Left = 65, Top = 14, Width = 130, Format = DateTimePickerFormat.Short };
            pnl.Controls.Add(dtp);
            UiHelper.AddButton(pnl, "DT ngày", 205, 10, (s, e) => Run(() =>
                lblNgay.Text = "Doanh thu ngày: " + bus.DoanhThuNgay(dtp.Value).ToString("N0") + " đ"));
            lblNgay = new Label { Left = 310, Top = 18, Width = 400 };
            pnl.Controls.Add(lblNgay);

            pnl.Controls.Add(new Label { Text = "Tháng/Năm", Left = 10, Top = 58, Width = 70 });
            nudThang = new NumericUpDown { Left = 85, Top = 54, Width = 45, Minimum = 1, Maximum = 12, Value = DateTime.Now.Month };
            nudNam = new NumericUpDown { Left = 135, Top = 54, Width = 65, Minimum = 2000, Maximum = 2100, Value = DateTime.Now.Year };
            pnl.Controls.Add(nudThang); pnl.Controls.Add(nudNam);
            UiHelper.AddButton(pnl, "DT tháng", 205, 50, (s, e) => Run(() =>
                lblThang.Text = "Doanh thu tháng: " + bus.DoanhThuThang((int)nudThang.Value, (int)nudNam.Value).ToString("N0") + " đ"));
            lblThang = new Label { Left = 310, Top = 58, Width = 400 };
            pnl.Controls.Add(lblThang);

            UiHelper.AddButton(pnl, "Top 5 bán chạy", 10, 95, (s, e) => Run(() => dgv.DataSource = bus.Top5()), 140);
        }

        void Run(Action a)
        {
            try { a(); }
            catch (Exception ex) { UiHelper.Error(ex.Message); }
        }
    }
}
