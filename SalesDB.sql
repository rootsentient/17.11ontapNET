-- ===== SalesDB: chạy toàn bộ script này trong SSMS =====
IF DB_ID('SalesDB') IS NULL CREATE DATABASE SalesDB;
GO
USE SalesDB;
GO
DROP TABLE IF EXISTS ChiTietHoaDon;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS SanPham;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS NhanVien;
GO
CREATE TABLE NhanVien(
    MaNV         int IDENTITY(1,1) PRIMARY KEY,
    TenNV        nvarchar(100) NOT NULL,
    SoDienThoai  varchar(15) NULL,
    ChucVu       nvarchar(50) NULL
);
-- Unique nhưng cho phép nhiều NULL (UNIQUE thường chỉ cho 1 NULL)
CREATE UNIQUE INDEX UX_NhanVien_SDT ON NhanVien(SoDienThoai) WHERE SoDienThoai IS NOT NULL;

CREATE TABLE KhachHang(
    MaKH         int IDENTITY(1,1) PRIMARY KEY,
    TenKH        nvarchar(100) NOT NULL,
    DienThoai    varchar(15) NULL,
    DiemTichLuy  int NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UX_KhachHang_DT ON KhachHang(DienThoai) WHERE DienThoai IS NOT NULL;

CREATE TABLE SanPham(
    MaSP         varchar(20) PRIMARY KEY,
    TenSP        nvarchar(150) NOT NULL,
    GiaBan       decimal(18,2) NOT NULL CHECK (GiaBan > 0),
    SoLuongTon   int NOT NULL CHECK (SoLuongTon >= 0)
);

CREATE TABLE HoaDon(
    MaHD      int IDENTITY(1,1) PRIMARY KEY,
    NgayLap   datetime NOT NULL DEFAULT GETDATE(),
    MaNV      int NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),
    MaKH      int NULL FOREIGN KEY REFERENCES KhachHang(MaKH),
    TongTien  decimal(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE ChiTietHoaDon(
    MaHD     int NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD),
    MaSP     varchar(20) NOT NULL FOREIGN KEY REFERENCES SanPham(MaSP),
    SoLuong  int NOT NULL CHECK (SoLuong > 0),
    DonGia   decimal(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP)
);
GO
-- ===== Dữ liệu mẫu =====
INSERT NhanVien(TenNV,SoDienThoai,ChucVu) VALUES
(N'Nguyễn Văn An','0901000001',N'Thu ngân'),
(N'Trần Thị Bình','0901000002',N'Quản lý');
INSERT KhachHang(TenKH,DienThoai) VALUES
(N'Lê Minh Châu','0912000001'),
(N'Phạm Quốc Dũng','0912000002');
INSERT SanPham(MaSP,TenSP,GiaBan,SoLuongTon) VALUES
('8934563138',N'Mì Hảo Hảo tôm chua cay',4500,200),
('8934588012',N'Sữa tươi Vinamilk 1L',32000,80),
('8935001234',N'Nước suối Aquafina 500ml',6000,300),
('8936012345',N'Dầu ăn Simply 1L',58000,50),
('8937023456',N'Gạo ST25 5kg',185000,40),
('8938034567',N'Bánh Oreo',15000,120);
GO
