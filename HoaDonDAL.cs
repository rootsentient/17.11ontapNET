using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SalesApp.DTO;

namespace SalesApp.DAL
{
    public class HoaDonDAL
    {
        /// <summary>
        /// Thanh toán trong MỘT SqlTransaction (ACID):
        /// 1) Insert HoaDon  2) Insert ChiTietHoaDon  3) Trừ tồn kho (thiếu hàng => Rollback)
        /// 4) Cộng điểm tích lũy  5) Commit. Lỗi bất kỳ => Rollback toàn bộ.
        /// Đơn giá được đọc từ DB trong transaction (không tin dữ liệu từ giao diện).
        /// </summary>
        public int ThanhToan(HoaDonDTO hd, List<ChiTietHoaDonDTO> items)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    // 1. Chèn hóa đơn (tổng tiền cập nhật sau khi tính xong)
                    int maHD;
                    using (var cmd = new SqlCommand(
                        "INSERT INTO HoaDon(NgayLap,MaNV,MaKH,TongTien) OUTPUT INSERTED.MaHD " +
                        "VALUES(GETDATE(),@nv,@kh,0)", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@nv", hd.MaNV);
                        cmd.Parameters.AddWithValue("@kh", (object)hd.MaKH ?? DBNull.Value);
                        maHD = (int)cmd.ExecuteScalar();
                    }

                    decimal tong = 0;
                    foreach (var it in items)
                    {
                        // Khóa dòng sản phẩm, đọc giá + tồn hiện tại
                        decimal gia; int ton; string ten;
                        using (var cmd = new SqlCommand(
                            "SELECT TenSP, GiaBan, SoLuongTon FROM SanPham WITH (UPDLOCK, ROWLOCK) WHERE MaSP=@ma",
                            conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@ma", it.MaSP);
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (!rd.Read()) throw new Exception("Sản phẩm " + it.MaSP + " không tồn tại.");
                                ten = rd.GetString(0); gia = rd.GetDecimal(1); ton = rd.GetInt32(2);
                            }
                        }

                        // 3. Không đủ hàng => Rollback + báo lỗi
                        if (ton < it.SoLuong)
                        {
                            trans.Rollback();
                            throw new InvalidOperationException(
                                $"Sản phẩm \"{ten}\" không đủ tồn kho (còn {ton}, cần {it.SoLuong}). Giao dịch đã được hủy.");
                        }

                        // 2. Chi tiết hóa đơn
                        using (var cmd = new SqlCommand(
                            "INSERT INTO ChiTietHoaDon(MaHD,MaSP,SoLuong,DonGia) VALUES(@hd,@sp,@sl,@gia)", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@hd", maHD);
                            cmd.Parameters.AddWithValue("@sp", it.MaSP);
                            cmd.Parameters.AddWithValue("@sl", it.SoLuong);
                            cmd.Parameters.AddWithValue("@gia", gia);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Trừ tồn kho
                        using (var cmd = new SqlCommand(
                            "UPDATE SanPham SET SoLuongTon = SoLuongTon - @sl WHERE MaSP=@sp AND SoLuongTon >= @sl",
                            conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@sl", it.SoLuong);
                            cmd.Parameters.AddWithValue("@sp", it.MaSP);
                            if (cmd.ExecuteNonQuery() == 0)
                                throw new InvalidOperationException("Trừ tồn kho thất bại cho " + ten);
                        }
                        tong += gia * it.SoLuong;
                    }

                    // Cập nhật tổng tiền hóa đơn
                    using (var cmd = new SqlCommand("UPDATE HoaDon SET TongTien=@t WHERE MaHD=@hd", conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@t", tong);
                        cmd.Parameters.AddWithValue("@hd", maHD);
                        cmd.ExecuteNonQuery();
                    }

                    // 4. Cộng điểm tích lũy: TongTien / 100000
                    if (hd.MaKH.HasValue)
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE KhachHang SET DiemTichLuy = DiemTichLuy + @diem WHERE MaKH=@kh", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@diem", (int)(tong / 100000m));
                            cmd.Parameters.AddWithValue("@kh", hd.MaKH.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 5. Hoàn tất
                    trans.Commit();
                    return maHD;
                }
                catch
                {
                    try { trans.Rollback(); } catch { /* đã rollback ở trên */ }
                    throw;
                }
            }
        }
    }
}
