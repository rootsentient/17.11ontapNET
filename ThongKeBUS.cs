using System;
using System.Collections.Generic;
using SalesApp.DAL;
using SalesApp.DTO;

namespace SalesApp.BUS
{
    public class ThongKeBUS
    {
        readonly ThongKeDAL dal = new ThongKeDAL();

        public decimal DoanhThuNgay(DateTime ngay) => dal.DoanhThu(ngay.Date, ngay.Date.AddDays(1));

        public decimal DoanhThuThang(int thang, int nam)
        {
            var d = new DateTime(nam, thang, 1);
            return dal.DoanhThu(d, d.AddMonths(1));
        }

        public List<TopSanPhamDTO> Top5() => dal.TopSanPham(5);
    }
}
