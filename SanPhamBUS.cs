using System;
using System.Collections.Generic;
using SalesApp.DAL;
using SalesApp.DTO;

namespace SalesApp.BUS
{
    public class SanPhamBUS
    {
        readonly SanPhamDAL dal = new SanPhamDAL();

        public List<SanPhamDTO> GetAll() => dal.GetAll();
        public List<SanPhamDTO> Search(string kw) =>
            string.IsNullOrWhiteSpace(kw) ? dal.GetAll() : dal.Search(kw.Trim());

        static void Validate(SanPhamDTO s)
        {
            if (string.IsNullOrWhiteSpace(s.MaSP)) throw new Exception("Mã sản phẩm không được để trống.");
            if (string.IsNullOrWhiteSpace(s.TenSP)) throw new Exception("Tên sản phẩm không được để trống.");
            if (s.GiaBan <= 0) throw new Exception("Giá bán phải lớn hơn 0.");
            if (s.SoLuongTon < 0) throw new Exception("Số lượng tồn không được âm.");
            s.MaSP = s.MaSP.Trim(); s.TenSP = s.TenSP.Trim();
        }

        public void Add(SanPhamDTO s)
        {
            Validate(s);
            if (dal.Exists(s.MaSP)) throw new Exception("Mã sản phẩm đã tồn tại.");
            dal.Insert(s);
        }

        public void Update(SanPhamDTO s)
        {
            Validate(s);
            if (dal.Update(s) == 0) throw new Exception("Không tìm thấy sản phẩm để sửa.");
        }

        public void Delete(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) throw new Exception("Chưa chọn sản phẩm.");
            dal.Delete(ma);
        }
    }
}
