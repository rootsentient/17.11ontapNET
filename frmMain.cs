using System.Windows.Forms;

namespace SalesApp.GUI
{
    public class frmMain : Form
    {
        public frmMain()
        {
            Text = "Retail Sales System - Siêu thị mini";
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;

            var menu = new MenuStrip();
            var danhMuc = new ToolStripMenuItem("Danh mục");
            danhMuc.DropDownItems.Add("Sản phẩm", null, (s, e) => Open(new frmSanPham()));
            danhMuc.DropDownItems.Add("Khách hàng", null, (s, e) => Open(new frmKhachHang()));
            danhMuc.DropDownItems.Add("Nhân viên", null, (s, e) => Open(new frmNhanVien()));
            menu.Items.Add(danhMuc);
            menu.Items.Add(new ToolStripMenuItem("Bán hàng", null, (s, e) => Open(new frmBanHang())));
            menu.Items.Add(new ToolStripMenuItem("Báo cáo - Thống kê", null, (s, e) => Open(new frmThongKe())));
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        void Open(Form f) { f.MdiParent = this; f.Show(); }
    }
}
