using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SalesApp.DTO;

namespace SalesApp.DAL
{
    public class NhanVienDAL
    {
        const string SELECT = "SELECT MaNV, TenNV, SoDienThoai, ChucVu FROM NhanVien ";

        List<NhanVienDTO> Map(DataTable dt)
        {
            var list = new List<NhanVienDTO>();
            foreach (DataRow r in dt.Rows)
                list.Add(new NhanVienDTO
                {
                    MaNV = (int)r["MaNV"],
                    TenNV = r["TenNV"].ToString(),
                    SoDienThoai = r["SoDienThoai"] as string,
                    ChucVu = r["ChucVu"] as string
                });
            return list;
        }

        public List<NhanVienDTO> GetAll() => Map(DbHelper.GetTable(SELECT + "ORDER BY TenNV"));

        public int Insert(NhanVienDTO n) => DbHelper.ExecuteNonQuery(
            "INSERT INTO NhanVien(TenNV,SoDienThoai,ChucVu) VALUES(@ten,@sdt,@cv)",
            new SqlParameter("@ten", n.TenNV), new SqlParameter("@sdt", DbHelper.Nz(n.SoDienThoai)),
            new SqlParameter("@cv", DbHelper.Nz(n.ChucVu)));

        public int Update(NhanVienDTO n) => DbHelper.ExecuteNonQuery(
            "UPDATE NhanVien SET TenNV=@ten, SoDienThoai=@sdt, ChucVu=@cv WHERE MaNV=@ma",
            new SqlParameter("@ma", n.MaNV), new SqlParameter("@ten", n.TenNV),
            new SqlParameter("@sdt", DbHelper.Nz(n.SoDienThoai)), new SqlParameter("@cv", DbHelper.Nz(n.ChucVu)));

        public int Delete(int ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM NhanVien WHERE MaNV=@ma", new SqlParameter("@ma", ma));
    }
}
