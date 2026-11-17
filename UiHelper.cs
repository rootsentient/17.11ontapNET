using System;
using System.Windows.Forms;

namespace SalesApp.GUI
{
    public static class UiHelper
    {
        public static TextBox AddField(Control parent, string label, int x, int y, int width = 180)
        {
            parent.Controls.Add(new Label { Text = label, Left = x, Top = y + 3, Width = 95 });
            var t = new TextBox { Left = x + 100, Top = y, Width = width };
            parent.Controls.Add(t);
            return t;
        }

        public static Button AddButton(Control parent, string text, int x, int y, EventHandler onClick, int width = 90)
        {
            var b = new Button { Text = text, Left = x, Top = y, Width = width, Height = 30 };
            b.Click += onClick;
            parent.Controls.Add(b);
            return b;
        }

        public static DataGridView NewGrid() => new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        public static void Error(string msg) =>
            MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void Info(string msg) =>
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static bool Confirm(string msg) =>
            MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
