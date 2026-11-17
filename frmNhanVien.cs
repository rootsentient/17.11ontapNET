using System;
using System.Windows.Forms;
using SalesApp.BUS;
using SalesApp.DTO;

namespace SalesApp.GUI
{
    public class frmNhanVien : Form
    {
        readonly NhanVienBUS bus = new NhanVienBUS();
        TextBox txtMa, txtTen, txtSDT, txtCV;
        DataGridView dgv;

        public frmNhanVien()
        {
            Text = "Quản lý Nhân viên"; Width = 800; Height = 480;
            dgv = UiHelper.NewGrid();
            Controls.Add(dgv);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 130 };
            Controls.Add(pnl);
            txtMa = UiHelper.AddField(pnl, "Mã NV", 10, 10); txtMa.ReadOnly = true;
            txtTen = UiHelper.AddField(pnl, "Tên NV", 10, 45, 280);
            txtSDT = UiHelper.AddField(pnl, "Số điện thoại", 10, 80);
            txtCV = UiHelper.AddField(pnl, "Chức vụ", 320, 80);
            UiHelper.AddButton(pnl, "Thêm", 450, 10, (s, e) => Run(() => bus.Add(Read())));
            UiHelper.AddButton(pnl, "Sửa", 450, 45, (s, e) => Run(() => bus.Update(Read())));
            UiHelper.AddButton(pnl, "Xóa", 550, 45, (s, e) =>
            {
                if (UiHelper.Confirm("Xóa nhân viên này?")) Run(() => bus.Delete(ParseMa()));
            });
            UiHelper.AddButton(pnl, "Làm mới", 550, 10, (s, e) => { txtMa.Clear(); txtTen.Clear(); txtSDT.Clear(); txtCV.Clear(); });

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is NhanVienDTO n)
                {
                    txtMa.Text = n.MaNV.ToString(); txtTen.Text = n.TenNV;
                    txtSDT.Text = n.SoDienThoai; txtCV.Text = n.ChucVu;
                }
            };
            LoadData();
        }

        void LoadData() => dgv.DataSource = bus.GetAll();
        int ParseMa() => int.TryParse(txtMa.Text, out int m) ? m : 0;
        NhanVienDTO Read() => new NhanVienDTO { MaNV = ParseMa(), TenNV = txtTen.Text, SoDienThoai = txtSDT.Text, ChucVu = txtCV.Text };

        void Run(Action a)
        {
            try { a(); LoadData(); UiHelper.Info("Thao tác thành công."); }
            catch (Exception ex) { UiHelper.Error(ex.Message); }
        }
    }
}
