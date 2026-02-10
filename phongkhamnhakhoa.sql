CREATE DATABASE PhongKhamNhaKhoa;
USE PhongKhamNhaKhoa;

CREATE TABLE TaiKhoanNguoiDung (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255),
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100),
    NgayTao DATETIME DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    LyDoKhoa NVARCHAR(500),
    OTP VARCHAR(6),
    ThoiGianOTP DATETIME,    
    CONSTRAINT CHK_TaiKhoanNguoiDung_SoDienThoai_Format
    CHECK (SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),    
    CONSTRAINT CHK_TaiKhoanNguoiDung_TrangThai_Valid
    CHECK (TrangThai IN (N'Hoạt động', N'Bị khóa', N'Chờ xác thực'))
);

CREATE TABLE BenhNhan (
    MaBenhNhan INT IDENTITY(10000,1) PRIMARY KEY,
    MaTaiKhoan INT NULL,
    
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),
    
    TienSuBenh NVARCHAR(MAX),
    DiUng NVARCHAR(MAX),
    GhiChuBacSi NVARCHAR(MAX),
    
    LoaiBenhNhan NVARCHAR(20) DEFAULT N'Khách mới',
    NgayDangKy DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_BenhNhan_TaiKhoanNguoiDung FOREIGN KEY (MaTaiKhoan) 
    REFERENCES TaiKhoanNguoiDung(MaTaiKhoan) ON DELETE SET NULL,
    
    CONSTRAINT CHK_BenhNhan_SoDienThoai_Format
    CHECK (SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    
    CONSTRAINT CHK_BenhNhan_GioiTinh_Valid
    CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác'))
);

CREATE TABLE QuanHeBenhNhan (
    MaQuanHe INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT NOT NULL,
    MaBenhNhan INT NOT NULL,
    QuanHe NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    NgayTao DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_QuanHeBenhNhan_TaiKhoan FOREIGN KEY (MaTaiKhoan) 
    REFERENCES TaiKhoanNguoiDung(MaTaiKhoan) ON DELETE CASCADE,
    
    CONSTRAINT FK_QuanHeBenhNhan_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan) ON DELETE CASCADE,
    
    CONSTRAINT UQ_QuanHeBenhNhan_TaiKhoan_BenhNhan UNIQUE (MaTaiKhoan, MaBenhNhan),
    
    CONSTRAINT CHK_QuanHeBenhNhan_QuanHe_Valid
    CHECK (QuanHe IN (
        N'Bản thân', N'Vợ', N'Chồng', N'Con', N'Cha', N'Mẹ', 
        N'Anh', N'Chị', N'Em', N'Ông', N'Bà', N'Cháu',
        N'Họ hàng', N'Bạn bè', N'Khác'
    ))
);

CREATE TABLE LoaiNhanVien (
    MaLoaiNV INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiNV NVARCHAR(50) NOT NULL UNIQUE,
    MoTa NVARCHAR(255)
);
CREATE TABLE TaiKhoanNhanVien (
    MaTaiKhoanNV INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    NgayTao DATETIME DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME
);

CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY(1000,1) PRIMARY KEY,
    MaTaiKhoanNV INT NOT NULL,
    MaLoaiNV INT NOT NULL,
    
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),
    
    ThoiGianBatDauLam DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Đang làm việc',
    
    ChuyenKhoa NVARCHAR(255),
    BangCap NVARCHAR(MAX),
    SoNamKinhNghiem INT DEFAULT 0,
    GioiThieu NVARCHAR(MAX),
    
    CONSTRAINT FK_NhanVien_TaiKhoanNhanVien FOREIGN KEY (MaTaiKhoanNV) 
    REFERENCES TaiKhoanNhanVien(MaTaiKhoanNV),
    
    CONSTRAINT FK_NhanVien_LoaiNhanVien FOREIGN KEY (MaLoaiNV) 
    REFERENCES LoaiNhanVien(MaLoaiNV)
);

-- 7. PHÂN QUYỀN
CREATE TABLE PhanQuyen (
    MaPhanQuyen INT IDENTITY(1,1) PRIMARY KEY,
    MaLoaiNV INT NOT NULL,
    XemLich BIT DEFAULT 0,
    SuaLich BIT DEFAULT 0,
    XemHoSo BIT DEFAULT 0,
    SuaHoSo BIT DEFAULT 0,
    XemDoanhThu BIT DEFAULT 0,
    QuanLyKho BIT DEFAULT 0,
    QuanLyNhanSu BIT DEFAULT 0,
    
    CONSTRAINT FK_PhanQuyen_LoaiNhanVien FOREIGN KEY (MaLoaiNV) 
    REFERENCES LoaiNhanVien(MaLoaiNV),
    
    CONSTRAINT UQ_PhanQuyen_MaLoaiNV UNIQUE (MaLoaiNV)
);


CREATE TABLE DichVu (
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(200) NOT NULL,
    LoaiDichVu NVARCHAR(50),
    MoTa NVARCHAR(MAX),
    ThoiGianThucHien INT,
    TrangThai NVARCHAR(20) DEFAULT N'Khả dụng',
    
    CONSTRAINT CHK_DichVu_TenDichVu_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenDichVu))) > 0),
    
    CONSTRAINT CHK_DichVu_LoaiDichVu_Valid
    CHECK (LoaiDichVu IN (N'Tổng quát', N'Thẩm mỹ', N'Chỉnh nha', N'Implant', N'Trẻ em'))
);

CREATE TABLE GiaDichVu (
    MaGiaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    MaDichVu INT NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL,
    NgayApDung DATE NOT NULL,
    NgayKetThuc DATE,
    GhiChu NVARCHAR(255),
    
    CONSTRAINT FK_GiaDichVu_DichVu FOREIGN KEY (MaDichVu) 
    REFERENCES DichVu(MaDichVu) ON DELETE CASCADE,
    
    CONSTRAINT CHK_GiaDichVu_DonGia_Valid
    CHECK (DonGia > 0)
);
CREATE TABLE GheNhaKhoa (
    MaGhe INT IDENTITY(1,1) PRIMARY KEY,
    TenGhe NVARCHAR(100) NOT NULL,
    ViTri NVARCHAR(255),
    TrangThai NVARCHAR(20) DEFAULT N'Trống',
    ThoiGianBaoTri DATE,
    MoTa NVARCHAR(500)
);

-- 11. LỊCH HẸN
CREATE TABLE LichHen (
    MaLichHen INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoanDatLich INT NULL,
    MaBenhNhan INT NOT NULL,
    MaBacSi INT,
    MaDichVu INT NOT NULL,
    MaGhe INT,
    
    NgayDat DATETIME DEFAULT GETDATE(),
    NgayHen DATE NOT NULL,
    GioHen TIME NOT NULL,
    LyDoKham NVARCHAR(MAX),
    QuanHe NVARCHAR(50),
    KenhDatLich NVARCHAR(50),
    
    TrangThai NVARCHAR(20) DEFAULT N'Chờ xác nhận',
    MaNhanVienXacNhan INT,
    ThoiGianXacNhan DATETIME,
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_LichHen_TaiKhoanNguoiDung FOREIGN KEY (MaTaiKhoanDatLich) 
    REFERENCES TaiKhoanNguoiDung(MaTaiKhoan) ON DELETE SET NULL,
    
    CONSTRAINT FK_LichHen_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_LichHen_NhanVien_BacSi FOREIGN KEY (MaBacSi) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_LichHen_DichVu FOREIGN KEY (MaDichVu) 
    REFERENCES DichVu(MaDichVu),
    
    CONSTRAINT FK_LichHen_GheNhaKhoa FOREIGN KEY (MaGhe) 
    REFERENCES GheNhaKhoa(MaGhe),
    
    CONSTRAINT FK_LichHen_NhanVien_XacNhan FOREIGN KEY (MaNhanVienXacNhan) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_LichHen_KenhDatLich_Valid
    CHECK (KenhDatLich IN (N'Điện thoại', N'Zalo', N'Facebook', N'Website', N'Đến trực tiếp')),
    
CONSTRAINT CHK_LichHen_TrangThai_Valid
CHECK (TrangThai IN (N'Chờ xác nhận', N'Đã xác nhận')),

CONSTRAINT CHK_LichHen_NgayHen_Valid
CHECK (NgayHen >= CAST(GETDATE() AS DATE)),

CONSTRAINT CHK_LichHen_GioHen_Valid
    CHECK (
        (GioHen >= '07:30:00' AND GioHen <= '12:00:00') OR
        (GioHen >= '14:00:00' AND GioHen <= '19:30:00')
    )
);

-- 12. LỊCH LÀM VIỆC
CREATE TABLE LichLamViec (
    MaLich INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT NOT NULL,
    Thu INT,
    CaLam NVARCHAR(50) NOT NULL,
    GhiChu NVARCHAR(255),
    
    CONSTRAINT FK_LichLamViec_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,
    
    CONSTRAINT UQ_LichLamViec_NhanVien_Thu_CaLam UNIQUE (MaNhanVien, Thu, CaLam),
    
    CONSTRAINT CHK_LichLamViec_Thu_Valid
    CHECK (Thu BETWEEN 2 AND 8),
    
    CONSTRAINT CHK_LichLamViec_CaLam_Valid
    CHECK (CaLam IN (N'Sáng', N'Chiều', N'Cả ngày'))
);

CREATE TABLE HoSoBenhAn (
    MaHoSo INT IDENTITY(1,1) PRIMARY KEY,
    MaBenhNhan INT NOT NULL,
    MaBacSi INT NOT NULL,
    MaLichHen INT,
    MaGhe INT,
    
    NgayKham DATETIME DEFAULT GETDATE(),
    TrieuChung NVARCHAR(MAX),
    ChanDoan NVARCHAR(MAX),
    PhuongPhapDieuTri NVARCHAR(MAX),
    DonThuoc NVARCHAR(MAX),
    LoiDan NVARCHAR(MAX),
    HenTaiKham DATE,
    
    TongTien DECIMAL(15,2) DEFAULT 0,
    DaThanhToan BIT DEFAULT 0,
    
    CONSTRAINT FK_HoSoBenhAn_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_HoSoBenhAn_NhanVien_BacSi FOREIGN KEY (MaBacSi) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_HoSoBenhAn_LichHen FOREIGN KEY (MaLichHen) 
    REFERENCES LichHen(MaLichHen),
    
    CONSTRAINT FK_HoSoBenhAn_GheNhaKhoa FOREIGN KEY (MaGhe) 
    REFERENCES GheNhaKhoa(MaGhe)
);


CREATE TABLE ChiTietHoSo (
    MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo INT NOT NULL,
    MaDichVu INT NOT NULL,
    SoLuong INT DEFAULT 1,
    DonGia DECIMAL(15,2),
    ThanhTien DECIMAL(15,2),
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_ChiTietHoSo_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo) ON DELETE CASCADE,
    
    CONSTRAINT FK_ChiTietHoSo_DichVu FOREIGN KEY (MaDichVu) 
    REFERENCES DichVu(MaDichVu),
    
    CONSTRAINT CHK_ChiTietHoSo_SoLuong_Valid
    CHECK (SoLuong > 0)
);

CREATE TABLE HinhAnhXQuang (
    MaHinhAnh INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo INT NOT NULL,
    TenFile NVARCHAR(255) NOT NULL,
    DuongDan VARCHAR(500) NOT NULL,
    LoaiAnh NVARCHAR(50),
    MoTa NVARCHAR(500),
    NgayUpload DATETIME DEFAULT GETDATE(),
    NguoiUpload INT,
    
    CONSTRAINT FK_HinhAnhXQuang_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo) ON DELETE CASCADE,
    
    CONSTRAINT FK_HinhAnhXQuang_NhanVien FOREIGN KEY (NguoiUpload) 
    REFERENCES NhanVien(MaNhanVien)
);


CREATE TABLE VatTu (
    MaVatTu INT IDENTITY(1,1) PRIMARY KEY,
    TenVatTu NVARCHAR(200) NOT NULL,
    LoaiVatTu NVARCHAR(50),
    DonViTinh NVARCHAR(50),
    SoLuongTon INT DEFAULT 0,
    SoLuongToiThieu INT DEFAULT 10,
    HanSuDung DATE,
    NhaCungCap NVARCHAR(200),
    GhiChu NVARCHAR(500),
    
CONSTRAINT CHK_VatTu_TenVatTu_NotEmpty
CHECK (LEN(RTRIM(LTRIM(TenVatTu))) > 0),

CONSTRAINT CHK_VatTu_LoaiVatTu_Valid
CHECK (LoaiVatTu IN (N'Thuốc men', N'Dụng cụ', N'Vật tư tiêu hao', N'Khác'))
);

CREATE TABLE NhapKho (
    MaNhapKho INT IDENTITY(1,1) PRIMARY KEY,
    MaVatTu INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    DonGiaNhap DECIMAL(15,2),
    ThanhTien DECIMAL(15,2),
    NgayNhap DATETIME DEFAULT GETDATE(),
    NguoiNhap INT,
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_NhapKho_VatTu FOREIGN KEY (MaVatTu) 
    REFERENCES VatTu(MaVatTu) ON DELETE CASCADE,
    
    CONSTRAINT FK_NhapKho_NhanVien FOREIGN KEY (NguoiNhap) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_NhapKho_SoLuongNhap_Valid
    CHECK (SoLuongNhap > 0)
);


CREATE TABLE XuatKho (
    MaXuatKho INT IDENTITY(1,1) PRIMARY KEY,
    MaVatTu INT NOT NULL,
    SoLuongXuat INT NOT NULL,
    LyDoXuat NVARCHAR(255),
    NgayXuat DATETIME DEFAULT GETDATE(),
    NguoiXuat INT,
    MaHoSo INT,
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_XuatKho_VatTu FOREIGN KEY (MaVatTu) 
    REFERENCES VatTu(MaVatTu) ON DELETE CASCADE,
    
    CONSTRAINT FK_XuatKho_NhanVien FOREIGN KEY (NguoiXuat) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_XuatKho_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo),
    
    CONSTRAINT CHK_XuatKho_SoLuongXuat_Valid
    CHECK (SoLuongXuat > 0)
);



CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo INT NOT NULL,
    SoTien DECIMAL(15,2) NOT NULL,
    HinhThucThanhToan NVARCHAR(50),
    CoTraGop BIT DEFAULT 0,
    KeHoachTraGop NVARCHAR(500),
    NgayThanhToan DATETIME DEFAULT GETDATE(),
    NguoiThu INT,
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_ThanhToan_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo) ON DELETE CASCADE,
    
    CONSTRAINT FK_ThanhToan_NhanVien FOREIGN KEY (NguoiThu) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_ThanhToan_HinhThucThanhToan_Valid
    CHECK (HinhThucThanhToan IN (N'Tiền mặt', N'Chuyển khoản', N'Online'))
);


CREATE TABLE Luong (
    MaLuong INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    LuongCoBan DECIMAL(15,2),
    PhanTramHoaHong DECIMAL(5,2),
    SoTienHoaHong DECIMAL(15,2),
    Thuong DECIMAL(15,2),
    KhauTru DECIMAL(15,2),
    TongLuong DECIMAL(15,2),
    NgayTinhLuong DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Chờ thanh toán',
    
    CONSTRAINT FK_Luong_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT UQ_Luong_NhanVien_Thang_Nam UNIQUE (MaNhanVien, Thang, Nam)
);


CREATE TABLE ThongBao (
    MaThongBao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiThongBao NVARCHAR(50),
    MaBenhNhan INT,
    MaLichHen INT,
    SoDienThoaiNhan VARCHAR(15) NOT NULL,
    
    NoiDung NVARCHAR(MAX) NOT NULL,
    HinhThuc NVARCHAR(50),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa gửi',
    ThoiGianGui DATETIME,
    ThoiGianHenGui DATETIME,
    KetQua NVARCHAR(500),
    
    CONSTRAINT FK_ThongBao_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_ThongBao_LichHen FOREIGN KEY (MaLichHen) 
    REFERENCES LichHen(MaLichHen),
    
    CONSTRAINT CHK_ThongBao_LoaiThongBao_Valid
    CHECK (LoaiThongBao IN (
        N'Xác nhận lịch', N'Nhắc lịch', N'Nhắc tái khám', 
        N'Chúc mừng sinh nhật', N'Cảnh báo', N'Khác'
    )),
    
    CONSTRAINT CHK_ThongBao_HinhThuc_Valid
    CHECK (HinhThuc IN (N'SMS', N'Zalo', N'Email', N'Trong hệ thống'))
);


CREATE TABLE ChamCong (
    MaChamCong INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT NOT NULL,
    NgayLamViec DATE NOT NULL,
    GioVao TIME,
    GioRa TIME,
    SoGioLam DECIMAL(5,2),
    TrangThai NVARCHAR(20) DEFAULT N'Đúng giờ',
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_ChamCong_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT UQ_ChamCong_NhanVien_Ngay UNIQUE (MaNhanVien, NgayLamViec)
);

CREATE TABLE BaoCao (
    MaBaoCao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiBaoCao NVARCHAR(50),
    TenBaoCao NVARCHAR(100) NOT NULL,
    ThoiGianBatDau DATE,
    ThoiGianKetThuc DATE,
    NguoiTao INT,
    NgayTao DATETIME DEFAULT GETDATE(),
    DuongDanFile VARCHAR(500),
    TrangThai NVARCHAR(20) DEFAULT N'Đã tạo',
    
    CONSTRAINT FK_BaoCao_NhanVien FOREIGN KEY (NguoiTao) 
    REFERENCES NhanVien(MaNhanVien)
);


CREATE TABLE CauHinhHeThong (
    MaCauHinh INT IDENTITY(1,1) PRIMARY KEY,
    TenCauHinh NVARCHAR(100) NOT NULL UNIQUE,
    GiaTri NVARCHAR(MAX),
    MoTa NVARCHAR(255),
    LoaiCauHinh NVARCHAR(50),
    NguoiCapNhat INT,
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_CauHinhHeThong_NhanVien FOREIGN KEY (NguoiCapNhat) 
    REFERENCES NhanVien(MaNhanVien)
);

CREATE INDEX IX_BenhNhan_SoDienThoai ON BenhNhan(SoDienThoai);
CREATE INDEX IX_LichHen_NgayHen_TrangThai ON LichHen(NgayHen, TrangThai);
CREATE INDEX IX_HoSoBenhAn_MaBenhNhan ON HoSoBenhAn(MaBenhNhan);
CREATE INDEX IX_VatTu_SoLuongTon ON VatTu(SoLuongTon);
CREATE INDEX IX_ThongBao_TrangThai_ThoiGianHenGui ON ThongBao(TrangThai, ThoiGianHenGui);


CREATE TRIGGER trg_BenhNhan_TaoQuanHeBanThan
ON BenhNhan
AFTER INSERT
AS
BEGIN
    INSERT INTO QuanHeBenhNhan (MaTaiKhoan, MaBenhNhan, QuanHe)
    SELECT i.MaTaiKhoan, i.MaBenhNhan, N'Bản thân'
    FROM inserted i
    WHERE i.MaTaiKhoan IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM QuanHeBenhNhan qh 
            WHERE qh.MaTaiKhoan = i.MaTaiKhoan 
                AND qh.MaBenhNhan = i.MaBenhNhan
        );
END;
GO

CREATE TRIGGER trg_CapNhatTonKhoNhap
ON NhapKho
AFTER INSERT
AS
BEGIN
    UPDATE v
    SET v.SoLuongTon = v.SoLuongTon + i.SoLuongNhap
    FROM VatTu v
    INNER JOIN inserted i ON v.MaVatTu = i.MaVatTu;
END;
GO

CREATE TRIGGER trg_CapNhatTonKhoXuat
ON XuatKho
AFTER INSERT
AS
BEGIN
    UPDATE v
    SET v.SoLuongTon = v.SoLuongTon - i.SoLuongXuat
    FROM VatTu v
    INNER JOIN inserted i ON v.MaVatTu = i.MaVatTu;
END;
GO


INSERT INTO LoaiNhanVien (TenLoaiNV, MoTa) VALUES
(N'Admin', N'Quản trị viên'),
(N'Bác sĩ', N'Bác sĩ điều trị'),
(N'Lễ tân', N'Nhân viên tiếp đón'),
(N'Phụ tá', N'Phụ tá nha khoa');


INSERT INTO PhanQuyen (MaLoaiNV, XemLich, SuaLich, XemHoSo, SuaHoSo, XemDoanhThu, QuanLyKho, QuanLyNhanSu) VALUES
(1, 1, 1, 1, 1, 1, 1, 1), -- Admin
(2, 1, 0, 1, 1, 0, 0, 0), -- Bác sĩ
(3, 1, 1, 1, 0, 0, 0, 0), -- Lễ tân
(4, 1, 0, 1, 0, 0, 1, 0); -- Phụ tá



INSERT INTO TaiKhoanNhanVien (TenDangNhap, MatKhau) VALUES
('admin', '12345678'),
('dr.minh', '12345678'),
('reception', '12345678'),
('assistant', '12345678');

INSERT INTO NhanVien (MaTaiKhoanNV, MaLoaiNV, HoTen, SoDienThoai, Email, ChuyenKhoa) VALUES
(1, 1, N'Nguyễn Văn Admin', '0901234567', 'admin@phongkham.com', N'Quản lý'),
(2, 2, N'Trần Thị Minh', '0912345678', 'dr.minh@phongkham.com', N'Nha khoa tổng quát'),
(3, 3, N'Lê Thị Lan', '0923456789', 'reception@phongkham.com', NULL),
(4, 4, N'Phạm Văn Tú', '0934567890', 'assistant@phongkham.com', NULL);

INSERT INTO DichVu (TenDichVu, LoaiDichVu, ThoiGianThucHien) VALUES
(N'Khám tổng quát', N'Tổng quát', 30),
(N'Cạo vôi răng', N'Tổng quát', 45),
(N'Trám răng', N'Tổng quát', 60),
(N'Niềng răng', N'Chỉnh nha', 120);

INSERT INTO GheNhaKhoa (TenGhe, ViTri) VALUES
(N'Ghế 1', N'Phòng 1'),
(N'Ghế 2', N'Phòng 1'),
(N'Ghế 3', N'Phòng 2');
DECLARE @AdminId INT;

SELECT @AdminId = MaNhanVien
FROM NhanVien
WHERE HoTen = N'Nguyễn Văn Admin';

INSERT INTO CauHinhHeThong
(TenCauHinh, GiaTri, MoTa, LoaiCauHinh, NguoiCapNhat)
VALUES
(N'ThoiGianTuDongDuyet', '20', N'Thời gian tự động duyệt lịch (phút)', N'Lịch hẹn', @AdminId),
(N'GioNhanLichCoDinh', '06:00', N'Giờ nhắc lịch cố định', N'Thông báo', @AdminId),
(N'SoLichToiDaTrongNgay', '10', N'Số lịch tối đa/ngày', N'Lịch hẹn', @AdminId);

INSERT INTO CauHinhHeThong
(TenCauHinh, GiaTri, MoTa, LoaiCauHinh, NguoiCapNhat)
VALUES
(N'ThoiGianTuDongDuyet', '20', N'Thời gian tự động duyệt lịch (phút)', N'Lịch hẹn', 1000),
(N'GioNhanLichCoDinh', '06:00', N'Giờ nhắc lịch cố định', N'Thông báo', 1000),
(N'SoLichToiDaTrongNgay', '10', N'Số lịch tối đa/ngày', N'Lịch hẹn', 1000);


SELECT 
    'TaiKhoanNguoiDung' AS TenBang, COUNT(*) AS SoBanGhi FROM TaiKhoanNguoiDung
UNION ALL
SELECT 'BenhNhan', COUNT(*) FROM BenhNhan
UNION ALL
SELECT 'NhanVien', COUNT(*) FROM NhanVien
UNION ALL
SELECT 'LoaiNhanVien', COUNT(*) FROM LoaiNhanVien
UNION ALL
SELECT 'PhanQuyen', COUNT(*) FROM PhanQuyen
UNION ALL
SELECT 'DichVu', COUNT(*) FROM DichVu
UNION ALL
SELECT 'GheNhaKhoa', COUNT(*) FROM GheNhaKhoa
UNION ALL
SELECT 'CauHinhHeThong', COUNT(*) FROM CauHinhHeThong;
SELECT *
FROM BenhNhan
SELECT *
FROM LoaiNhanVien
SELECT *
FROM NhanVien
SELECT *
FROM PhanQuyen
SELECT *
FROM DichVu
SELECT *
FROM GheNhaKhoa
SELECT *
FROM CauHinhHeThong