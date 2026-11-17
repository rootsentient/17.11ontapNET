using System;
using System.Windows.Forms;
using SalesApp.GUI;

namespace SalesApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}
