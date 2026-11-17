using System;
using System.Collections.Generic;
using SalesApp.DAL;
using SalesApp.DTO;

namespace SalesApp.BUS
{
    public class KhachHangBUS
    {
        readonly KhachHangDAL dal = new KhachHangDAL();

        public List<KhachHangDTO> GetAll() => dal.GetAll();
        public List<KhachHangDTO> Search(string kw) =>
            string.IsNullOrWhiteSpace(kw) ? dal.GetAll() : dal.Search(kw.Trim());

        /// <summary>Danh sách cho màn hình bán hàng: có thêm dòng "Khách lẻ" (MaKH = 0).</summary>
        public List<KhachHangDTO> GetForSale()
        {
            var list = dal.GetAll();
            list.Insert(0, new KhachHangDTO { MaKH = 0, TenKH = "-- Khách lẻ --" });
            return list;
        }

        static void Validate(KhachHangDTO k)
        {
            if (string.IsNullOrWhiteSpace(k.TenKH)) throw new Exception("Tên khách hàng không được để trống.");
            k.TenKH = k.TenKH.Trim();
        }

        public void Add(KhachHangDTO k) { Validate(k); dal.Insert(k); }
        public void Update(KhachHangDTO k)
        {
            Validate(k);
            if (dal.Update(k) == 0) throw new Exception("Không tìm thấy khách hàng để sửa.");
        }
        public void Delete(int ma)
        {
            if (ma <= 0) throw new Exception("Chưa chọn khách hàng.");
            dal.Delete(ma);
        }
    }
}
