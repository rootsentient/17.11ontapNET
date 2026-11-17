namespace SalesApp.DTO
{
    public class KhachHangDTO
    {
        public int MaKH { get; set; }          // 0 = khách lẻ (không lưu DB)
        public string TenKH { get; set; }
        public string DienThoai { get; set; }
        public int DiemTichLuy { get; set; }
    }
}
