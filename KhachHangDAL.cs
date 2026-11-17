using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SalesApp.DTO;

namespace SalesApp.DAL
{
    public class KhachHangDAL
    {
        const string SELECT = "SELECT MaKH, TenKH, DienThoai, DiemTichLuy FROM KhachHang ";

        List<KhachHangDTO> Map(DataTable dt)
        {
            var list = new List<KhachHangDTO>();
            foreach (DataRow r in dt.Rows)
                list.Add(new KhachHangDTO
                {
                    MaKH = (int)r["MaKH"],
                    TenKH = r["TenKH"].ToString(),
                    DienThoai = r["DienThoai"] as string,
                    DiemTichLuy = (int)r["DiemTichLuy"]
                });
            return list;
        }

        public List<KhachHangDTO> GetAll() => Map(DbHelper.GetTable(SELECT + "ORDER BY TenKH"));

        public List<KhachHangDTO> Search(string kw) =>
            Map(DbHelper.GetTable(SELECT + "WHERE TenKH LIKE @kw OR DienThoai LIKE @kw ORDER BY TenKH",
                new SqlParameter("@kw", SqlDbType.NVarChar, 100) { Value = "%" + kw + "%" }));

        public int Insert(KhachHangDTO k) => DbHelper.ExecuteNonQuery(
            "INSERT INTO KhachHang(TenKH,DienThoai) VALUES(@ten,@dt)",
            new SqlParameter("@ten", k.TenKH), new SqlParameter("@dt", DbHelper.Nz(k.DienThoai)));

        public int Update(KhachHangDTO k) => DbHelper.ExecuteNonQuery(
            "UPDATE KhachHang SET TenKH=@ten, DienThoai=@dt WHERE MaKH=@ma",
            new SqlParameter("@ma", k.MaKH), new SqlParameter("@ten", k.TenKH),
            new SqlParameter("@dt", DbHelper.Nz(k.DienThoai)));

        public int Delete(int ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM KhachHang WHERE MaKH=@ma", new SqlParameter("@ma", ma));
    }
}
