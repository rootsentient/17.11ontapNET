using System;
using System.Windows.Forms;
using SalesApp.BUS;
using SalesApp.DTO;

namespace SalesApp.GUI
{
    public class frmKhachHang : Form
    {
        readonly KhachHangBUS bus = new KhachHangBUS();
        TextBox txtMa, txtTen, txtDT, txtDiem, txtTim;
        DataGridView dgv;

        public frmKhachHang()
        {
            Text = "Quản lý Khách hàng"; Width = 800; Height = 500;
            dgv = UiHelper.NewGrid();
            Controls.Add(dgv);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 150 };
            Controls.Add(pnl);
            txtMa = UiHelper.AddField(pnl, "Mã KH", 10, 10); txtMa.ReadOnly = true;
            txtTen = UiHelper.AddField(pnl, "Tên KH", 10, 45, 280);
            txtDT = UiHelper.AddField(pnl, "Điện thoại", 10, 80);
            txtDiem = UiHelper.AddField(pnl, "Điểm tích lũy", 320, 10, 100); txtDiem.ReadOnly = true;
            txtTim = UiHelper.AddField(pnl, "Tìm (tên/SĐT)", 10, 115);
            UiHelper.AddButton(pnl, "Thêm", 450, 10, (s, e) => Run(() => bus.Add(Read())));
            UiHelper.AddButton(pnl, "Sửa", 450, 45, (s, e) => Run(() => bus.Update(Read())));
            UiHelper.AddButton(pnl, "Xóa", 450, 80, (s, e) =>
            {
                if (UiHelper.Confirm("Xóa khách hàng này?")) Run(() => bus.Delete(ParseMa()));
            });
            UiHelper.AddButton(pnl, "Làm mới", 550, 10, (s, e) => { txtMa.Clear(); txtTen.Clear(); txtDT.Clear(); txtDiem.Clear(); });
            UiHelper.AddButton(pnl, "Tìm", 300, 112, (s, e) => LoadData());

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is KhachHangDTO k)
                {
                    txtMa.Text = k.MaKH.ToString(); txtTen.Text = k.TenKH;
                    txtDT.Text = k.DienThoai; txtDiem.Text = k.DiemTichLuy.ToString();
                }
            };
            LoadData();
        }

        void LoadData() => dgv.DataSource = bus.Search(txtTim.Text);
        int ParseMa() => int.TryParse(txtMa.Text, out int m) ? m : 0;
        KhachHangDTO Read() => new KhachHangDTO { MaKH = ParseMa(), TenKH = txtTen.Text, DienThoai = txtDT.Text };

        void Run(Action a)
        {
            try { a(); LoadData(); UiHelper.Info("Thao tác thành công."); }
            catch (Exception ex) { UiHelper.Error(ex.Message); }
        }
    }
}
