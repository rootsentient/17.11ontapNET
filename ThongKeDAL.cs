using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SalesApp.DTO;

namespace SalesApp.DAL
{
    public class ThongKeDAL
    {
        // ExecuteScalar: tổng doanh thu trong khoảng [from, to)
        public decimal DoanhThu(DateTime from, DateTime to)
        {
            object o = DbHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(TongTien),0) FROM HoaDon WHERE NgayLap >= @f AND NgayLap < @t",
                new SqlParameter("@f", from), new SqlParameter("@t", to));
            return Convert.ToDecimal(o);
        }

        // SqlDataReader: Top N sản phẩm bán chạy nhất
        public List<TopSanPhamDTO> TopSanPham(int top)
        {
            var list = new List<TopSanPhamDTO>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT TOP (@top) sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS Tong " +
                "FROM ChiTietHoaDon ct JOIN SanPham sp ON sp.MaSP = ct.MaSP " +
                "GROUP BY sp.MaSP, sp.TenSP ORDER BY Tong DESC", conn))
            {
                cmd.Parameters.AddWithValue("@top", top);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read())
                        list.Add(new TopSanPhamDTO
                        {
                            MaSP = rd.GetString(0),
                            TenSP = rd.GetString(1),
                            TongSoLuongBan = Convert.ToInt32(rd.GetValue(2))
                        });
            }
            return list;
        }
    }
}
