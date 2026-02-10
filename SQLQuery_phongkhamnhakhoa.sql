CREATE DATABASE PhongKhamNhaKhoa;
GO

USE PhongKhamNhaKhoa;
GO

CREATE TABLE LoaiNhanVien (
    MaLoaiNV INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiNV NVARCHAR(50) NOT NULL,
    MoTa NVARCHAR(255),    
    CONSTRAINT CHK_LoaiNhanVien_TenLoaiNV_NotEmpty CHECK (LEN(RTRIM(LTRIM(TenLoaiNV))) > 0)
);

-- 2. NHÂN VIÊN
CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY(1000,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),
    ThoiGianBatDauLam DATE DEFAULT GETDATE(),
    MaLoaiNV INT NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'Đang làm việc',
    ChuyenKhoa NVARCHAR(255),
    BangCap NVARCHAR(MAX),
    GioiThieu NTEXT,
    HinhAnh VARCHAR(500),
    NgayCapNhatThongTin DATETIME DEFAULT GETDATE(),    
    CONSTRAINT FK_NhanVien_LoaiNhanVien FOREIGN KEY (MaLoaiNV) 
    REFERENCES LoaiNhanVien(MaLoaiNV),    
    CONSTRAINT CHK_NhanVien_Email_Format 
    CHECK (Email IS NULL OR Email LIKE '%_@__%.__%'),    
    CONSTRAINT CHK_NhanVien_SoDienThoai_Format
    CHECK (SoDienThoai IS NULL OR SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    CONSTRAINT CHK_NhanVien_GioiTinh_Valid
    CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),    
    CONSTRAINT CHK_NhanVien_TrangThai_Valid
    CHECK (TrangThai IN (N'Đang làm việc', N'Đã nghỉ việc', N'Tạm nghỉ')),    
);

-- 3. TÀI KHOẢN NHÂN VIÊN
CREATE TABLE TaiKhoanNhanVien (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT UNIQUE NOT NULL,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',    
    CONSTRAINT FK_TaiKhoanNhanVien_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,    
    CONSTRAINT CHK_TaiKhoanNhanVien_TenDangNhap_Length
    CHECK (LEN(TenDangNhap) >= 6 AND LEN(TenDangNhap) <= 50),    
    CONSTRAINT CHK_TaiKhoanNhanVien_TrangThai_Valid
    CHECK (TrangThai IN (N'Hoạt động', N'Bị khóa', N'Chờ kích hoạt')),    
    CONSTRAINT CHK_TaiKhoanNhanVien_MatKhau_Complexity
    CHECK (LEN(MatKhau) >= 8)
);

-- 4. PHÂN QUYỀN CHI TIẾT
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
    SuaThongTinCaNhan BIT DEFAULT 0,    
    CONSTRAINT FK_PhanQuyen_LoaiNhanVien FOREIGN KEY (MaLoaiNV) 
    REFERENCES LoaiNhanVien(MaLoaiNV),    
    CONSTRAINT UQ_PhanQuyen_MaLoaiNV UNIQUE (MaLoaiNV),    
    CONSTRAINT CHK_PhanQuyen_Bits_Valid
    CHECK (
        XemLich IN (0, 1) AND
        SuaLich IN (0, 1) AND
        XemHoSo IN (0, 1) AND
        SuaHoSo IN (0, 1) AND
        XemDoanhThu IN (0, 1) AND
        QuanLyKho IN (0, 1) AND
        QuanLyNhanSu IN (0, 1) AND
        SuaThongTinCaNhan IN (0, 1)
    )
);

-- 5. TÀI KHOẢN BỆNH NHÂN
CREATE TABLE TaiKhoanBenhNhan (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255),
    HoTen NVARCHAR(100),
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),    
    OTP VARCHAR(6),
    ThoiGianOTP DATETIME,    
    NgayTao DATETIME DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    LyDoKhoa NVARCHAR(500),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_Email_Format
    CHECK (Email IS NULL OR Email LIKE '%_@__%.__%'),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_SoDienThoai_Format
    CHECK (SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_GioiTinh_Valid
    CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_OTP_Format
    CHECK (OTP IS NULL OR (LEN(OTP) = 6 AND OTP NOT LIKE '%[^0-9]%')),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_TrangThai_Valid
    CHECK (TrangThai IN (N'Hoạt động', N'Bị khóa', N'Chờ xác thực')),    
    CONSTRAINT CHK_TaiKhoanBenhNhan_TenDangNhap_Length
    CHECK (LEN(TenDangNhap) >= 6 AND LEN(TenDangNhap) <= 50)
);

-- 6. BỆNH NHÂN
CREATE TABLE BenhNhan (
    MaBenhNhan INT IDENTITY(10000,1) PRIMARY KEY,
    MaTaiKhoan INT,    
    -- Thông tin cơ bản
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NOT NULL,
    Email VARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),    
    -- Thông tin y tế
    TienSuBenh NTEXT,
    DiUng NTEXT,
    GhiChuBacSi NTEXT,    
    -- Phân loại
    LoaiBenhNhan NVARCHAR(20) DEFAULT N'Khách mới',    
    -- Quản lý
    NgayDangKy DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',    
    -- Quan hệ với chủ tài khoản
    MaTaiKhoanDaiDien INT,
    QuanHeVoiChuTK NVARCHAR(50),
    
    CONSTRAINT FK_BenhNhan_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoan) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan) ON DELETE SET NULL,    
    CONSTRAINT FK_BenhNhan_TaiKhoanBenhNhan_DaiDien FOREIGN KEY (MaTaiKhoanDaiDien) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT UQ_BenhNhan_MaTaiKhoan UNIQUE (MaTaiKhoan),
    
    CONSTRAINT CHK_BenhNhan_SoDienThoai_Format
    CHECK (SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    
    CONSTRAINT CHK_BenhNhan_Email_Format
    CHECK (Email IS NULL OR Email LIKE '%_@__%.__%'),
    
    CONSTRAINT CHK_BenhNhan_GioiTinh_Valid
    CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    
    CONSTRAINT CHK_BenhNhan_LoaiBenhNhan_Valid
    CHECK (LoaiBenhNhan IN (N'Khách mới', N'Khách cũ')),
    
    CONSTRAINT CHK_BenhNhan_TrangThai_Valid
    CHECK (TrangThai IN (N'Hoạt động', N'Tạm ngưng', N'Đã xóa')),
    
    CONSTRAINT CHK_BenhNhan_QuanHeVoiChuTK_Valid
    CHECK (QuanHeVoiChuTK IS NULL OR QuanHeVoiChuTK IN (
        N'Bản thân', N'Vợ/Chồng', N'Con cái', N'Cha mẹ', 
        N'Anh/Chị/Em', N'Họ hàng', N'Bạn bè', N'Khác'
    ))
);

-- =============================================
-- NHÓM 2: DỊCH VỤ & LỊCH HẸN (5 bảng)
-- =============================================

-- 7. DỊCH VỤ
CREATE TABLE DichVu (
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(200) NOT NULL,
    LoaiDichVu NVARCHAR(50),
    MoTa NTEXT,
    ThoiGianThucHien INT, -- phút
    TrangThai NVARCHAR(20) DEFAULT N'Khả dụng',
    
    CONSTRAINT CHK_DichVu_TenDichVu_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenDichVu))) > 0),
    
    CONSTRAINT CHK_DichVu_LoaiDichVu_Valid
    CHECK (LoaiDichVu IN (N'Tổng quát', N'Thẩm mỹ', N'Chỉnh nha', N'Implant', N'Trẻ em')),
    
    CONSTRAINT CHK_DichVu_ThoiGianThucHien_Valid
    CHECK (ThoiGianThucHien IS NULL OR ThoiGianThucHien > 0),
    
    CONSTRAINT CHK_DichVu_TrangThai_Valid
    CHECK (TrangThai IN (N'Khả dụng', N'Tạm ngưng', N'Ngừng cung cấp'))
);

-- 8. GIÁ DỊCH VỤ
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
    CHECK (DonGia > 0),
    
    CONSTRAINT CHK_GiaDichVu_NgayApDung_Valid
    CHECK (NgayApDung <= GETDATE()),
    
    CONSTRAINT CHK_GiaDichVu_NgayKetThuc_Valid
    CHECK (NgayKetThuc IS NULL OR NgayKetThuc > NgayApDung)
);

-- 9. GHẾ NHA KHOA
CREATE TABLE GheNhaKhoa (
    MaGhe INT IDENTITY(1,1) PRIMARY KEY,
    TenGhe NVARCHAR(100) NOT NULL,
    ViTri NVARCHAR(255),
    TrangThai NVARCHAR(20) DEFAULT N'Trống',
    ThoiGianBaoTri DATE,
    MoTa NVARCHAR(500),
    
    CONSTRAINT CHK_GheNhaKhoa_TrangThai_Valid
    CHECK (TrangThai IN (N'Trống', N'Bận', N'Bảo trì', N'Hỏng')),
    
    CONSTRAINT CHK_GheNhaKhoa_TenGhe_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenGhe))) > 0)
);

-- 10. LỊCH HẸN
CREATE TABLE LichHen (
    MaLichHen INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Quan hệ tài khoản
    MaTaiKhoanDatLich INT NOT NULL,
    MaBenhNhan INT NOT NULL,
    
    -- Thông tin lịch hẹn
    MaBacSi INT,
    MaDichVu INT NOT NULL,
    MaGhe INT,
    NgayDat DATETIME DEFAULT GETDATE(),
    NgayHen DATE NOT NULL,
    GioHen TIME NOT NULL,
    LyDoKham NTEXT,
    
    -- Kênh đặt lịch
    KenhDatLich NVARCHAR(50),
    
    -- Trạng thái
    TrangThai NVARCHAR(20) DEFAULT N'Chờ xác nhận',
    
    -- Xác nhận
    MaNhanVienXacNhan INT,
    ThoiGianXacNhan DATETIME,
    LoaiXacNhan NVARCHAR(20) DEFAULT N'Thủ công',
    
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_LichHen_TaiKhoanBenhNhan_DatLich FOREIGN KEY (MaTaiKhoanDatLich) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan) ON DELETE CASCADE,
    
    CONSTRAINT FK_LichHen_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan) ON DELETE CASCADE,
    
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
    CHECK (TrangThai IN (
        N'Chờ xác nhận', N'Đã xác nhận', N'Đã hủy', 
        N'Hoàn thành', N'Không đến', N'Đang khám'
    )),
    
    CONSTRAINT CHK_LichHen_LoaiXacNhan_Valid
    CHECK (LoaiXacNhan IN (N'Tự động', N'Thủ công')),
    
    CONSTRAINT CHK_LichHen_NgayHen_Valid
    CHECK (NgayHen >= CAST(GETDATE() AS DATE)),
    
    CONSTRAINT CHK_LichHen_GioHen_Valid
    CHECK (
        (GioHen >= '07:30:00' AND GioHen <= '12:00:00') OR
        (GioHen >= '14:00:00' AND GioHen <= '19:30:00')
    ),
    
    CONSTRAINT CHK_LichHen_NgayDat_Before_NgayHen
    CHECK (NgayDat <= CAST(NgayHen AS DATETIME) + CAST(GioHen AS DATETIME))
);

-- 11. LỊCH LÀM VIỆC
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

-- =============================================
-- NHÓM 3: HỒ SƠ & ĐIỀU TRỊ (4 bảng)
-- =============================================

-- 12. HỒ SƠ BỆNH ÁN
CREATE TABLE HoSoBenhAn (
    MaHoSo INT IDENTITY(1,1) PRIMARY KEY,
    MaBenhNhan INT NOT NULL,
    MaBacSi INT NOT NULL,
    MaLichHen INT,
    MaGhe INT,
    
    -- Tài khoản đại diện
    MaTaiKhoanDaiDien INT,
    
    -- Thông tin khám
    NgayKham DATETIME DEFAULT GETDATE(),
    TrieuChung NTEXT,
    ChanDoan NTEXT,
    PhuongPhapDieuTri NTEXT,
    DonThuoc NTEXT,
    LoiDan NTEXT,
    HenTaiKham DATE,
    
    -- Thông tin tài chính
    TongTien DECIMAL(15,2),
    DaThanhToan BIT DEFAULT 0,
    
    CONSTRAINT FK_HoSoBenhAn_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan) ON DELETE CASCADE,
    
    CONSTRAINT FK_HoSoBenhAn_NhanVien_BacSi FOREIGN KEY (MaBacSi) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_HoSoBenhAn_LichHen FOREIGN KEY (MaLichHen) 
    REFERENCES LichHen(MaLichHen),
    
    CONSTRAINT FK_HoSoBenhAn_GheNhaKhoa FOREIGN KEY (MaGhe) 
    REFERENCES GheNhaKhoa(MaGhe),
    
    CONSTRAINT FK_HoSoBenhAn_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoanDaiDien) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT CHK_HoSoBenhAn_NgayKham_Valid
    CHECK (NgayKham <= GETDATE()),
    
    CONSTRAINT CHK_HoSoBenhAn_TongTien_Valid
    CHECK (TongTien >= 0),
    
    CONSTRAINT CHK_HoSoBenhAn_DaThanhToan_Valid
    CHECK (DaThanhToan IN (0, 1)),
    
    CONSTRAINT CHK_HoSoBenhAn_HenTaiKham_Valid
    CHECK (HenTaiKham IS NULL OR HenTaiKham > NgayKham)
);

-- 13. CHI TIẾT HỒ SƠ
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
    CHECK (SoLuong > 0),
    
    CONSTRAINT CHK_ChiTietHoSo_DonGia_Valid
    CHECK (DonGia >= 0),
    
    CONSTRAINT CHK_ChiTietHoSo_ThanhTien_Valid
    CHECK (ThanhTien = SoLuong * DonGia)
);

-- 14. HÌNH ẢNH X-QUANG
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
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_HinhAnhXQuang_LoaiAnh_Valid
    CHECK (LoaiAnh IN (N'X-Quang', N'Lâm sàng', N'Trước/Sau', N'Khác')),
    
    CONSTRAINT CHK_HinhAnhXQuang_TenFile_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenFile))) > 0)
);

-- 15. KẾ HOẠCH ĐIỀU TRỊ
CREATE TABLE KeHoachDieuTri (
    MaKeHoach INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo INT NOT NULL,
    TenKeHoach NVARCHAR(200) NOT NULL,
    MoTa NTEXT,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE,
    TrangThai NVARCHAR(20) DEFAULT N'Đang thực hiện',
    TienDo NTEXT,
    
    CONSTRAINT FK_KeHoachDieuTri_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo) ON DELETE CASCADE,
    
    CONSTRAINT CHK_KeHoachDieuTri_NgayBatDau_Valid
    CHECK (NgayBatDau >= CAST(GETDATE() AS DATE)),
    
    CONSTRAINT CHK_KeHoachDieuTri_NgayKetThuc_Valid
    CHECK (NgayKetThuc IS NULL OR NgayKetThuc > NgayBatDau),
    
    CONSTRAINT CHK_KeHoachDieuTri_TrangThai_Valid
    CHECK (TrangThai IN (N'Đang thực hiện', N'Hoàn thành', N'Hủy bỏ', N'Tạm hoãn'))
);

-- =============================================
-- NHÓM 4: KHO VẬT TƯ (5 bảng)
-- =============================================

-- 16. NHÀ CUNG CẤP
CREATE TABLE NhaCungCap (
    MaNhaCungCap INT IDENTITY(1,1) PRIMARY KEY,
    TenNhaCungCap NVARCHAR(200) NOT NULL,
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    DiaChi NVARCHAR(255),
    NguoiLienHe NVARCHAR(100),
    GhiChu NVARCHAR(500),
    
    CONSTRAINT CHK_NhaCungCap_Email_Format
    CHECK (Email IS NULL OR Email LIKE '%_@__%.__%'),
    
    CONSTRAINT CHK_NhaCungCap_SoDienThoai_Format
	CHECK (SoDienThoai IS NULL OR SoDienThoai LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%')
);

-- 17. VẬT TƯ
CREATE TABLE VatTu (
    MaVatTu INT IDENTITY(1,1) PRIMARY KEY,
    TenVatTu NVARCHAR(200) NOT NULL,
    LoaiVatTu NVARCHAR(50),
    DonViTinh NVARCHAR(50),
    SoLuongTon INT DEFAULT 0,
    SoLuongToiThieu INT DEFAULT 10,
    HanSuDung DATE,
    NhaCungCap INT,
    ViTriLuuKho NVARCHAR(100),
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_VatTu_NhaCungCap FOREIGN KEY (NhaCungCap) 
    REFERENCES NhaCungCap(MaNhaCungCap),
    
    CONSTRAINT CHK_VatTu_TenVatTu_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenVatTu))) > 0),
    
    CONSTRAINT CHK_VatTu_LoaiVatTu_Valid
    CHECK (LoaiVatTu IN (N'Thuốc men', N'Dụng cụ', N'Vật tư tiêu hao', N'Khác')),
    
    CONSTRAINT CHK_VatTu_SoLuongTon_Valid
    CHECK (SoLuongTon >= 0),
    
    CONSTRAINT CHK_VatTu_SoLuongToiThieu_Valid
    CHECK (SoLuongToiThieu >= 0),
    
    CONSTRAINT CHK_VatTu_DonViTinh_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(DonViTinh))) > 0)
);

-- 18. NHẬP KHO
CREATE TABLE NhapKho (
    MaNhapKho INT IDENTITY(1,1) PRIMARY KEY,
    MaVatTu INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    DonGiaNhap DECIMAL(15,2),
    ThanhTien DECIMAL(15,2),
    NgayNhap DATETIME DEFAULT GETDATE(),
    MaNhaCungCap INT,
    NguoiNhap INT,
    SoHoaDon VARCHAR(50),
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_NhapKho_VatTu FOREIGN KEY (MaVatTu) 
    REFERENCES VatTu(MaVatTu) ON DELETE CASCADE,
    
    CONSTRAINT FK_NhapKho_NhaCungCap FOREIGN KEY (MaNhaCungCap) 
    REFERENCES NhaCungCap(MaNhaCungCap),
    
    CONSTRAINT FK_NhapKho_NhanVien FOREIGN KEY (NguoiNhap) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_NhapKho_SoLuongNhap_Valid
    CHECK (SoLuongNhap > 0),
    
    CONSTRAINT CHK_NhapKho_DonGiaNhap_Valid
    CHECK (DonGiaNhap > 0),
    
    CONSTRAINT CHK_NhapKho_ThanhTien_Valid
    CHECK (ThanhTien = SoLuongNhap * DonGiaNhap)
);

-- 19. XUẤT KHO
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
    CHECK (SoLuongXuat > 0),
    
    CONSTRAINT CHK_XuatKho_LyDoXuat_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(LyDoXuat))) > 0)
);

-- 20. KIỂM KÊ KHO
CREATE TABLE KiemKeKho (
    MaKiemKe INT IDENTITY(1,1) PRIMARY KEY,
    MaVatTu INT NOT NULL,
    SoLuongThucTe INT,
    SoLuongHeThong INT,
    ChenhLech INT,
    LyDo NVARCHAR(255),
    NgayKiemKe DATE,
    NguoiKiemKe INT,
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_KiemKeKho_VatTu FOREIGN KEY (MaVatTu) 
    REFERENCES VatTu(MaVatTu) ON DELETE CASCADE,
    
    CONSTRAINT FK_KiemKeKho_NhanVien FOREIGN KEY (NguoiKiemKe) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_KiemKeKho_SoLuongThucTe_Valid
    CHECK (SoLuongThucTe >= 0),
    
    CONSTRAINT CHK_KiemKeKho_SoLuongHeThong_Valid
    CHECK (SoLuongHeThong >= 0),
    
    CONSTRAINT CHK_KiemKeKho_ChenhLech_Valid
    CHECK (ChenhLech = SoLuongThucTe - SoLuongHeThong)
);

-- =============================================
-- NHÓM 5: TÀI CHÍNH & LƯƠNG (5 bảng)
-- =============================================

-- 21. THANH TOÁN
CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo INT NOT NULL,
    SoTien DECIMAL(15,2) NOT NULL,
    HinhThucThanhToan NVARCHAR(50),
    CoTraGop BIT DEFAULT 0,
    KeHoachTraGop NVARCHAR(500),
    NgayThanhToan DATETIME DEFAULT GETDATE(),
    NguoiThu INT,
    
    -- Tài khoản thanh toán
    MaTaiKhoanThanhToan INT,
    
    GhiChu NVARCHAR(500),
    
    CONSTRAINT FK_ThanhToan_HoSoBenhAn FOREIGN KEY (MaHoSo) 
    REFERENCES HoSoBenhAn(MaHoSo) ON DELETE CASCADE,
    
    CONSTRAINT FK_ThanhToan_NhanVien FOREIGN KEY (NguoiThu) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_ThanhToan_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoanThanhToan) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT CHK_ThanhToan_SoTien_Valid
    CHECK (SoTien > 0),
    
    CONSTRAINT CHK_ThanhToan_HinhThucThanhToan_Valid
    CHECK (HinhThucThanhToan IN (N'Tiền mặt', N'Chuyển khoản')),
    
    CONSTRAINT CHK_ThanhToan_CoTraGop_Valid
    CHECK (CoTraGop IN (0, 1))
);

-- 22. LỊCH SỬ THANH TOÁN (TRẢ GÓP)
CREATE TABLE LichSuThanhToan (
    MaLichSuTT INT IDENTITY(1,1) PRIMARY KEY,
    MaThanhToan INT NOT NULL,
    SoDot INT,
    DotHienTai INT DEFAULT 1,
    SoTienDot DECIMAL(15,2),
    NgayThanhToanDuKien DATE,
    NgayThanhToanThucTe DATE,
    TrangThai NVARCHAR(20) DEFAULT N'Chờ thanh toán',
    
    CONSTRAINT FK_LichSuThanhToan_ThanhToan FOREIGN KEY (MaThanhToan) 
    REFERENCES ThanhToan(MaThanhToan) ON DELETE CASCADE,
    
    CONSTRAINT CHK_LichSuThanhToan_SoDot_Valid
    CHECK (SoDot > 0),
    
    CONSTRAINT CHK_LichSuThanhToan_DotHienTai_Valid
    CHECK (DotHienTai BETWEEN 1 AND SoDot),
    
    CONSTRAINT CHK_LichSuThanhToan_SoTienDot_Valid
    CHECK (SoTienDot > 0),
    
    CONSTRAINT CHK_LichSuThanhToan_TrangThai_Valid
    CHECK (TrangThai IN (N'Chờ thanh toán', N'Đã thanh toán', N'Quá hạn'))
);

-- 23. LƯƠNG CƠ BẢN
CREATE TABLE LuongCoBan (
    MaLuongCoBan INT IDENTITY(1,1) PRIMARY KEY,
    MaLoaiNV INT NOT NULL,
    LuongCoBan DECIMAL(15,2) NOT NULL,
    PhanTramHoaHong DECIMAL(5,2) DEFAULT 0,
    NgayApDung DATE DEFAULT GETDATE(),
    NgayKetThuc DATE,
    
    CONSTRAINT FK_LuongCoBan_LoaiNhanVien FOREIGN KEY (MaLoaiNV) 
    REFERENCES LoaiNhanVien(MaLoaiNV),
    
    CONSTRAINT UQ_LuongCoBan_MaLoaiNV_NgayApDung UNIQUE (MaLoaiNV, NgayApDung),
    
    CONSTRAINT CHK_LuongCoBan_LuongCoBan_Valid
    CHECK (LuongCoBan >= 0),
    
    CONSTRAINT CHK_LuongCoBan_PhanTramHoaHong_Valid
    CHECK (PhanTramHoaHong BETWEEN 0 AND 100)
);

-- 24. LƯƠNG
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
    NguoiTinh INT,
    TrangThai NVARCHAR(20) DEFAULT N'Chờ thanh toán',
    
    CONSTRAINT FK_Luong_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,
    
    CONSTRAINT FK_Luong_NhanVien_Tinh FOREIGN KEY (NguoiTinh) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT UQ_Luong_NhanVien_Thang_Nam UNIQUE (MaNhanVien, Thang, Nam),
    
    CONSTRAINT CHK_Luong_Thang_Valid
    CHECK (Thang BETWEEN 1 AND 12),
    
    CONSTRAINT CHK_Luong_Nam_Valid
    CHECK (Nam BETWEEN 2000 AND 2100),
    
    CONSTRAINT CHK_Luong_LuongCoBan_Valid
    CHECK (LuongCoBan >= 0),
    
    CONSTRAINT CHK_Luong_PhanTramHoaHong_Valid
    CHECK (PhanTramHoaHong BETWEEN 0 AND 100),
    
    CONSTRAINT CHK_Luong_SoTienHoaHong_Valid
    CHECK (SoTienHoaHong >= 0),
    
    CONSTRAINT CHK_Luong_Thuong_Valid
    CHECK (Thuong >= 0),
    
    CONSTRAINT CHK_Luong_KhauTru_Valid
    CHECK (KhauTru >= 0),
    
    CONSTRAINT CHK_Luong_TongLuong_Valid
    CHECK (TongLuong = LuongCoBan + SoTienHoaHong + Thuong - KhauTru),
    
    CONSTRAINT CHK_Luong_TrangThai_Valid
    CHECK (TrangThai IN (N'Chờ thanh toán', N'Đã thanh toán', N'Đang tính toán'))
);

-- =============================================
-- NHÓM 6: THÔNG BÁO & HỆ THỐNG (4 bảng)
-- =============================================

-- 25. THÔNG BÁO
CREATE TABLE ThongBao (
    MaThongBao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiThongBao NVARCHAR(50),
    MaBenhNhan INT,
    MaLichHen INT,
    
    -- Gửi đến tài khoản nào
    MaTaiKhoanNhanThongBao INT NOT NULL,
    
    NoiDung NTEXT NOT NULL,
    HinhThuc NVARCHAR(50),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa gửi',
    ThoiGianGui DATETIME,
    ThoiGianHenGui DATETIME,
    KetQua NVARCHAR(500),
    
    CONSTRAINT FK_ThongBao_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_ThongBao_LichHen FOREIGN KEY (MaLichHen) 
    REFERENCES LichHen(MaLichHen),
    
    CONSTRAINT FK_ThongBao_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoanNhanThongBao) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT CHK_ThongBao_LoaiThongBao_Valid
    CHECK (LoaiThongBao IN (
        N'Xác nhận lịch', N'Nhắc lịch', N'Nhắc tái khám', 
        N'Chúc mừng sinh nhật', N'Cảnh báo', N'Khác'
    )),
    
    CONSTRAINT CHK_ThongBao_HinhThuc_Valid
    CHECK (HinhThuc IN (N'SMS', N'Zalo', N'Email', N'Trong hệ thống')),
    
    CONSTRAINT CHK_ThongBao_TrangThai_Valid
    CHECK (TrangThai IN (N'Chưa gửi', N'Đã gửi', N'Gửi lỗi', N'Đang gửi'))
);

-- 26. CHẤM CÔNG
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
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,
    
    CONSTRAINT UQ_ChamCong_NhanVien_Ngay UNIQUE (MaNhanVien, NgayLamViec),
    
    CONSTRAINT CHK_ChamCong_SoGioLam_Valid
    CHECK (SoGioLam >= 0 AND SoGioLam <= 24),
    
    CONSTRAINT CHK_ChamCong_GioVao_GioRa_Valid
    CHECK (
        (GioVao IS NULL AND GioRa IS NULL) OR
        (GioVao IS NOT NULL AND GioRa IS NOT NULL AND GioRa > GioVao)
    ),
    
    CONSTRAINT CHK_ChamCong_TrangThai_Valid
    CHECK (TrangThai IN (N'Đúng giờ', N'Đi muộn', N'Về sớm', N'Nghỉ phép', N'Nghỉ không phép'))
);

-- 27. CẤU HÌNH HỆ THỐNG
CREATE TABLE CauHinhHeThong (
    MaCauHinh INT IDENTITY(1,1) PRIMARY KEY,
    TenCauHinh NVARCHAR(100) NOT NULL,
    GiaTri NVARCHAR(MAX),
    MoTa NVARCHAR(255),
    LoaiCauHinh NVARCHAR(50),
    NguoiCapNhat INT,
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_CauHinhHeThong_NhanVien FOREIGN KEY (NguoiCapNhat) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_CauHinhHeThong_TenCauHinh_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenCauHinh))) > 0),
    
    CONSTRAINT CHK_CauHinhHeThong_GiaTri_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(GiaTri))) > 0)
);

-- 28. LOG HỆ THỐNG
CREATE TABLE LogHeThong (
    MaLog INT IDENTITY(1,1) PRIMARY KEY,
    LoaiLog NVARCHAR(50),
    MaNhanVien INT,
    MaBenhNhan INT,
    MaTaiKhoanBenhNhan INT,
    ChiTiet NVARCHAR(MAX),
    ThoiGian DATETIME DEFAULT GETDATE(),
    DiaChiIP VARCHAR(50),
    
    CONSTRAINT FK_LogHeThong_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT FK_LogHeThong_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_LogHeThong_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoanBenhNhan) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT CHK_LogHeThong_LoaiLog_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(LoaiLog))) > 0),
    
    CONSTRAINT CHK_LogHeThong_ChiTiet_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(ChiTiet))) > 0)
);

-- =============================================
-- NHÓM 7: ĐÁNH GIÁ & BÁO CÁO (3 bảng)
-- =============================================

-- 29. ĐÁNH GIÁ NHÂN VIÊN
CREATE TABLE DanhGiaNhanVien (
    MaDanhGia INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT NOT NULL,
    MaBenhNhan INT NOT NULL,
    
    -- Tài khoản đánh giá
    MaTaiKhoanDanhGia INT NOT NULL,
    
    Diem INT,
    NoiDungDanhGia NTEXT,
    NgayDanhGia DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Hiển thị',
    
    CONSTRAINT FK_DanhGiaNhanVien_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,
    
    CONSTRAINT FK_DanhGiaNhanVien_BenhNhan FOREIGN KEY (MaBenhNhan) 
    REFERENCES BenhNhan(MaBenhNhan),
    
    CONSTRAINT FK_DanhGiaNhanVien_TaiKhoanBenhNhan FOREIGN KEY (MaTaiKhoanDanhGia) 
    REFERENCES TaiKhoanBenhNhan(MaTaiKhoan),
    
    CONSTRAINT CHK_DanhGiaNhanVien_Diem_Valid
    CHECK (Diem BETWEEN 1 AND 5),
    
    CONSTRAINT CHK_DanhGiaNhanVien_TrangThai_Valid
    CHECK (TrangThai IN (N'Hiển thị', N'Ẩn', N'Chờ duyệt'))
);

-- 30. THỐNG KÊ ĐÁNH GIÁ
CREATE TABLE ThongKeDanhGia (
    MaThongKe INT IDENTITY(1,1) PRIMARY KEY,
    MaNhanVien INT NOT NULL UNIQUE,
    DiemTrungBinh FLOAT DEFAULT 0,
    TongSoDanhGia INT DEFAULT 0,
    SoDanhGia5Sao INT DEFAULT 0,
    SoDanhGia4Sao INT DEFAULT 0,
    SoDanhGia3Sao INT DEFAULT 0,
    SoDanhGia2Sao INT DEFAULT 0,
    SoDanhGia1Sao INT DEFAULT 0,
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_ThongKeDanhGia_NhanVien FOREIGN KEY (MaNhanVien) 
    REFERENCES NhanVien(MaNhanVien) ON DELETE CASCADE,
    
    CONSTRAINT CHK_ThongKeDanhGia_DiemTrungBinh_Valid
    CHECK (DiemTrungBinh >= 0 AND DiemTrungBinh <= 5),
    
    CONSTRAINT CHK_ThongKeDanhGia_TongSoDanhGia_Valid
    CHECK (TongSoDanhGia >= 0)
);

-- 31. BÁO CÁO
CREATE TABLE BaoCao (
    MaBaoCao INT IDENTITY(1,1) PRIMARY KEY,
    LoaiBaoCao NVARCHAR(50),
    TenBaoCao NVARCHAR(100),
    ThoiGianBatDau DATE,
    ThoiGianKetThuc DATE,
    NguoiTao INT,
    NgayTao DATETIME DEFAULT GETDATE(),
    DuongDanFile VARCHAR(500),
    TrangThai NVARCHAR(20) DEFAULT N'Đã tạo',
    
    CONSTRAINT FK_BaoCao_NhanVien FOREIGN KEY (NguoiTao) 
    REFERENCES NhanVien(MaNhanVien),
    
    CONSTRAINT CHK_BaoCao_LoaiBaoCao_Valid
    CHECK (LoaiBaoCao IN (N'Doanh thu', N'Bệnh nhân', N'Lịch hẹn', N'Vật tư', N'Nhân sự', N'Khác')),
    
    CONSTRAINT CHK_BaoCao_TenBaoCao_NotEmpty
    CHECK (LEN(RTRIM(LTRIM(TenBaoCao))) > 0),
    
    CONSTRAINT CHK_BaoCao_ThoiGianBatDau_ThoiGianKetThuc_Valid
    CHECK (
        (ThoiGianBatDau IS NULL AND ThoiGianKetThuc IS NULL) OR
        (ThoiGianBatDau IS NOT NULL AND ThoiGianKetThuc IS NOT NULL AND ThoiGianKetThuc >= ThoiGianBatDau)
    ),
    
    CONSTRAINT CHK_BaoCao_TrangThai_Valid
    CHECK (TrangThai IN (N'Đã tạo', N'Đang xử lý', N'Hoàn thành', N'Lỗi'))
);

