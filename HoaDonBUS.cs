using System;
using System.Collections.Generic;
using System.Linq;
using SalesApp.DAL;
using SalesApp.DTO;

namespace SalesApp.BUS
{
    public class HoaDonBUS
    {
        readonly HoaDonDAL dal = new HoaDonDAL();

        /// <summary>Kiểm tra giỏ hàng, gộp dòng trùng mã SP (tránh vi phạm PK), rồi gọi DAL thanh toán.</summary>
        public int ThanhToan(int maNV, int? maKH, List<ChiTietHoaDonDTO> gio)
        {
            if (maNV <= 0) throw new Exception("Vui lòng chọn nhân viên bán hàng.");
            if (gio == null || gio.Count == 0) throw new Exception("Giỏ hàng đang trống.");
            if (gio.Any(x => x.SoLuong <= 0)) throw new Exception("Số lượng phải lớn hơn 0.");

            var gop = gio.GroupBy(x => x.MaSP)
                         .Select(g => new ChiTietHoaDonDTO { MaSP = g.Key, SoLuong = g.Sum(x => x.SoLuong) })
                         .ToList();

            var hd = new HoaDonDTO { MaNV = maNV, MaKH = (maKH.HasValue && maKH.Value > 0) ? maKH : null };
            return dal.ThanhToan(hd, gop);
        }
    }
}
