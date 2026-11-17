using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SalesApp.DTO;

namespace SalesApp.DAL
{
    public class SanPhamDAL
    {
        const string SELECT = "SELECT MaSP, TenSP, GiaBan, SoLuongTon FROM SanPham ";

        List<SanPhamDTO> Map(DataTable dt)
        {
            var list = new List<SanPhamDTO>();
            foreach (DataRow r in dt.Rows)
                list.Add(new SanPhamDTO
                {
                    MaSP = r["MaSP"].ToString(),
                    TenSP = r["TenSP"].ToString(),
                    GiaBan = (decimal)r["GiaBan"],
                    SoLuongTon = (int)r["SoLuongTon"]
                });
            return list;
        }

        public List<SanPhamDTO> GetAll() => Map(DbHelper.GetTable(SELECT + "ORDER BY TenSP"));

        // Tìm theo tên hoặc mã vạch với ký tự đại diện %
        public List<SanPhamDTO> Search(string keyword) =>
            Map(DbHelper.GetTable(SELECT + "WHERE TenSP LIKE @kw OR MaSP LIKE @kw ORDER BY TenSP",
                new SqlParameter("@kw", SqlDbType.NVarChar, 150) { Value = "%" + keyword + "%" }));

        public bool Exists(string ma) =>
            (int)DbHelper.ExecuteScalar("SELECT COUNT(*) FROM SanPham WHERE MaSP=@ma",
                new SqlParameter("@ma", ma)) > 0;

        public int Insert(SanPhamDTO s) => DbHelper.ExecuteNonQuery(
            "INSERT INTO SanPham(MaSP,TenSP,GiaBan,SoLuongTon) VALUES(@ma,@ten,@gia,@sl)",
            new SqlParameter("@ma", s.MaSP), new SqlParameter("@ten", s.TenSP),
            new SqlParameter("@gia", s.GiaBan), new SqlParameter("@sl", s.SoLuongTon));

        public int Update(SanPhamDTO s) => DbHelper.ExecuteNonQuery(
            "UPDATE SanPham SET TenSP=@ten, GiaBan=@gia, SoLuongTon=@sl WHERE MaSP=@ma",
            new SqlParameter("@ma", s.MaSP), new SqlParameter("@ten", s.TenSP),
            new SqlParameter("@gia", s.GiaBan), new SqlParameter("@sl", s.SoLuongTon));

        public int Delete(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM SanPham WHERE MaSP=@ma", new SqlParameter("@ma", ma));
    }
}
