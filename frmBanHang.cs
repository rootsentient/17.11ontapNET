using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SalesApp.BUS;
using SalesApp.DTO;

namespace SalesApp.GUI
{
    public class frmBanHang : Form
    {
        readonly SanPhamBUS spBus = new SanPhamBUS();
        readonly KhachHangBUS khBus = new KhachHangBUS();
        readonly NhanVienBUS nvBus = new NhanVienBUS();
        readonly HoaDonBUS hdBus = new HoaDonBUS();

        ComboBox cboNV, cboKH, cboSP;
        TextBox txtMaVach;
        NumericUpDown nudSL;
        DataGridView dgv;
        Label lblTong, lblDiem;
        DataTable cart;
        List<SanPhamDTO> dsSP = new List<SanPhamDTO>();

        public frmBanHang()
        {
            Text = "Bán hàng / Lập hóa đơn"; Width = 980; Height = 650;
            BuildUI();
            LoadData();
        }

        void BuildUI()
        {
            // Giỏ hàng: cột ThanhTien tự tính bằng Expression
            cart = new DataTable();
            cart.Columns.Add("MaSP", typeof(string));
            cart.Columns.Add("TenSP", typeof(string));
            cart.Columns.Add("SoLuong", typeof(int));
            cart.Columns.Add("DonGia", typeof(decimal));
            cart.Columns.Add("ThanhTien", typeof(decimal), "SoLuong * DonGia");

            dgv = UiHelper.NewGrid();
            dgv.DataSource = cart;
            dgv.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            dgv.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            Controls.Add(dgv);

            var top = new Panel { Dock = DockStyle.Top, Height = 110 };
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 80 };
            Controls.Add(top);
            Controls.Add(bottom);

            top.Controls.Add(new Label { Text = "Nhân viên", Left = 10, Top = 15, Width = 80 });
            cboNV = new ComboBox { Left = 95, Top = 12, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            top.Controls.Add(cboNV);

            top.Controls.Add(new Label { Text = "Khách hàng", Left = 400, Top = 15, Width = 80 });
            cboKH = new ComboBox { Left = 485, Top = 12, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cboKH.SelectedIndexChanged += (s, e) => RefreshTotal();
            top.Controls.Add(cboKH);

            top.Controls.Add(new Label { Text = "Mã vạch", Left = 10, Top = 65, Width = 80 });
            txtMaVach = new TextBox { Left = 95, Top = 62, Width = 150 };
            txtMaVach.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;                 // máy quét thường gửi mã + Enter
                var sp = dsSP.FirstOrDefault(x => x.MaSP.Equals(txtMaVach.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                if (sp == null) { UiHelper.Error("Không tìm thấy mã vạch."); return; }
                cboSP.SelectedValue = sp.MaSP;
                AddToCart(sp, 1);
                txtMaVach.Clear();
            };
            top.Controls.Add(txtMaVach);

            top.Controls.Add(new Label { Text = "Sản phẩm", Left = 270, Top = 65, Width = 70 });
            cboSP = new ComboBox { Left = 345, Top = 62, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            top.Controls.Add(cboSP);

            top.Controls.Add(new Label { Text = "SL", Left = 640, Top = 65, Width = 25 });
            nudSL = new NumericUpDown { Left = 665, Top = 62, Width = 60, Minimum = 1, Maximum = 10000, Value = 1 };
            top.Controls.Add(nudSL);

            UiHelper.AddButton(top, "Thêm giỏ", 740, 58, (s, e) =>
            {
                if (cboSP.SelectedItem is SanPhamDTO sp) AddToCart(sp, (int)nudSL.Value);
            });
            UiHelper.AddButton(top, "Xóa dòng", 840, 58, (s, e) =>
            {
                if (dgv.CurrentRow?.DataBoundItem is DataRowView v) { cart.Rows.Remove(v.Row); RefreshTotal(); }
            });

            lblTong = new Label { Left = 10, Top = 10, Width = 400, Height = 30, Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold) };
            lblDiem = new Label { Left = 10, Top = 45, Width = 400 };
            bottom.Controls.Add(lblTong);
            bottom.Controls.Add(lblDiem);
            UiHelper.AddButton(bottom, "THANH TOÁN", 700, 15, ThanhToan_Click, 200).Height = 45;
        }

        void LoadData()
        {
            dsSP = spBus.GetAll();
            cboSP.DataSource = null;
            cboSP.DisplayMember = "TenSP"; cboSP.ValueMember = "MaSP"; cboSP.DataSource = dsSP;

            if (cboNV.DataSource == null)
            {
                cboNV.DisplayMember = "TenNV"; cboNV.ValueMember = "MaNV"; cboNV.DataSource = nvBus.GetAll();
            }

            int selKH = cboKH.SelectedValue is int v ? v : 0;
            cboKH.DataSource = null;
            cboKH.DisplayMember = "TenKH"; cboKH.ValueMember = "MaKH"; cboKH.DataSource = khBus.GetForSale();
            cboKH.SelectedValue = selKH;
            RefreshTotal();
        }

        void AddToCart(SanPhamDTO sp, int sl)
        {
            DataRow row = cart.AsEnumerable().FirstOrDefault(r => r.Field<string>("MaSP") == sp.MaSP);
            int daCo = row?.Field<int>("SoLuong") ?? 0;
            if (daCo + sl > sp.SoLuongTon)
            {
                UiHelper.Error($"\"{sp.TenSP}\" chỉ còn {sp.SoLuongTon} trong kho (giỏ đã có {daCo}).");
                return;
            }
            if (row != null) row["SoLuong"] = daCo + sl;
            else cart.Rows.Add(sp.MaSP, sp.TenSP, sl, sp.GiaBan);
            RefreshTotal();
        }

        decimal Tong() => cart.Rows.Count == 0 ? 0 : Convert.ToDecimal(cart.Compute("SUM(ThanhTien)", ""));

        void RefreshTotal()
        {
            decimal tong = Tong();
            int maKH = cboKH.SelectedValue is int v ? v : 0;
            int diem = maKH > 0 ? (int)(tong / 100000m) : 0;
            lblTong.Text = "Tổng tiền: " + tong.ToString("N0") + " đ";
            lblDiem.Text = maKH > 0 ? "Điểm tích lũy sẽ cộng: " + diem : "Khách lẻ - không tích điểm";
        }

        void ThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (cart.Rows.Count == 0) throw new Exception("Giỏ hàng đang trống.");
                if (!UiHelper.Confirm("Xác nhận thanh toán " + Tong().ToString("N0") + " đ?")) return;

                var gio = cart.AsEnumerable().Select(r => new ChiTietHoaDonDTO
                {
                    MaSP = r.Field<string>("MaSP"),
                    SoLuong = r.Field<int>("SoLuong")
                }).ToList();

                int maNV = cboNV.SelectedValue is int n ? n : 0;
                int? maKH = cboKH.SelectedValue is int k && k > 0 ? k : (int?)null;

                int maHD = hdBus.ThanhToan(maNV, maKH, gio);
                UiHelper.Info("Thanh toán thành công! Mã hóa đơn: " + maHD);
                cart.Clear();
                LoadData();                         // cập nhật tồn kho + điểm mới
            }
            catch (Exception ex)
            {
                UiHelper.Error(ex.Message);
                LoadData();                         // tồn kho có thể đã thay đổi
            }
        }
    }
}
