using System;
using System.Windows.Forms;
using SalesApp.BUS;
using SalesApp.DTO;

namespace SalesApp.GUI
{
    public class frmSanPham : Form
    {
        readonly SanPhamBUS bus = new SanPhamBUS();
        TextBox txtMa, txtTen, txtGia, txtSL, txtTim;
        DataGridView dgv;

        public frmSanPham()
        {
            Text = "Quản lý Sản phẩm"; Width = 850; Height = 550;
            dgv = UiHelper.NewGrid();
            Controls.Add(dgv);                       // Fill thêm trước, Top thêm sau

            var pnl = new Panel { Dock = DockStyle.Top, Height = 150 };
            Controls.Add(pnl);
            txtMa = UiHelper.AddField(pnl, "Mã SP/mã vạch", 10, 10);
            txtTen = UiHelper.AddField(pnl, "Tên SP", 10, 45, 300);
            txtGia = UiHelper.AddField(pnl, "Giá bán", 10, 80);
            txtSL = UiHelper.AddField(pnl, "Tồn kho", 320, 80, 100);
            UiHelper.AddButton(pnl, "Thêm", 450, 10, (s, e) => Run(() => bus.Add(Read())));
            UiHelper.AddButton(pnl, "Sửa", 450, 45, (s, e) => Run(() => bus.Update(Read())));
            UiHelper.AddButton(pnl, "Xóa", 450, 80, (s, e) =>
            {
                if (UiHelper.Confirm("Xóa sản phẩm này?")) Run(() => bus.Delete(txtMa.Text.Trim()));
            });
            UiHelper.AddButton(pnl, "Làm mới", 550, 10, (s, e) => ClearInput());
            txtTim = UiHelper.AddField(pnl, "Tìm (tên/mã)", 10, 115);
            UiHelper.AddButton(pnl, "Tìm", 300, 112, (s, e) => LoadData());

            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is SanPhamDTO sp)
                {
                    txtMa.Text = sp.MaSP; txtMa.ReadOnly = true;
                    txtTen.Text = sp.TenSP; txtGia.Text = sp.GiaBan.ToString("0.##"); txtSL.Text = sp.SoLuongTon.ToString();
                }
            };
            LoadData();
        }

        void LoadData() => dgv.DataSource = bus.Search(txtTim.Text);

        void ClearInput()
        {
            txtMa.ReadOnly = false;
            txtMa.Clear(); txtTen.Clear(); txtGia.Clear(); txtSL.Clear(); txtMa.Focus();
        }

        SanPhamDTO Read()
        {
            if (!decimal.TryParse(txtGia.Text, out decimal gia)) throw new Exception("Giá bán không hợp lệ.");
            if (!int.TryParse(txtSL.Text, out int sl)) throw new Exception("Số lượng tồn không hợp lệ.");
            return new SanPhamDTO { MaSP = txtMa.Text, TenSP = txtTen.Text, GiaBan = gia, SoLuongTon = sl };
        }

        void Run(Action a)
        {
            try { a(); LoadData(); UiHelper.Info("Thao tác thành công."); }
            catch (Exception ex) { UiHelper.Error(ex.Message); }
        }
    }
}
