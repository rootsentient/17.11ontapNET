# Retail Sales System (Bài 6.1) - WinForms 3-Tier

## Cách chạy
1. Mở SSMS, chạy `SQL/SalesDB.sql` (tạo DB + 5 bảng + dữ liệu mẫu).
2. Sửa chuỗi kết nối trong `DAL/DbHelper.cs` (biến `ConnectionString`) cho đúng tên server của bạn.
3. Visual Studio 2022: File > Open > Project > chọn `SalesApp.csproj` > F5.
   (Cần .NET 8 SDK + workload ".NET desktop development". Dùng .NET Framework thì tạo project WinForms mới,
   copy 4 thư mục DTO/DAL/BUS/GUI + Program.cs vào, add reference System.Data.)

## Cấu trúc
- DTO: lớp dữ liệu | DAL: truy cập SQL (100% SqlParameter) | BUS: kiểm tra nghiệp vụ | GUI: giao diện
- Giao dịch thanh toán: `DAL/HoaDonDAL.cs` -> `ThanhToan()` (SqlTransaction, Rollback/Commit)
- Báo cáo: `DAL/ThongKeDAL.cs` (ExecuteScalar + SqlDataReader)
