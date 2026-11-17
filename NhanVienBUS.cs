using System;
using System.Collections.Generic;
using SalesApp.DAL;
using SalesApp.DTO;

namespace SalesApp.BUS
{
    public class NhanVienBUS
    {
        readonly NhanVienDAL dal = new NhanVienDAL();

        public List<NhanVienDTO> GetAll() => dal.GetAll();

        static void Validate(NhanVienDTO n)
        {
            if (string.IsNullOrWhiteSpace(n.TenNV)) throw new Exception("Tên nhân viên không được để trống.");
            n.TenNV = n.TenNV.Trim();
        }

        public void Add(NhanVienDTO n) { Validate(n); dal.Insert(n); }
        public void Update(NhanVienDTO n)
        {
            Validate(n);
            if (dal.Update(n) == 0) throw new Exception("Không tìm thấy nhân viên để sửa.");
        }
        public void Delete(int ma)
        {
            if (ma <= 0) throw new Exception("Chưa chọn nhân viên.");
            dal.Delete(ma);
        }
    }
}
