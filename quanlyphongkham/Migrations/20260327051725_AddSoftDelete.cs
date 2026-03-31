using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlyphongkham.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaiDichVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianThucHien = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "GheNhaKhoa",
                columns: table => new
                {
                    MaGhe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenGhe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayBatDauBaoTri = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucBaoTri = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GheNhaKhoa", x => x.MaGhe);
                });

            migrationBuilder.CreateTable(
                name: "LoaiNhanVien",
                columns: table => new
                {
                    MaLoaiNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiNV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiNhanVien", x => x.MaLoaiNV);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanNguoiDung",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LyDoKhoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianOTP = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNguoiDung", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanNhanVien",
                columns: table => new
                {
                    MaTaiKhoanNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNhanVien", x => x.MaTaiKhoanNV);
                });

            migrationBuilder.CreateTable(
                name: "VatTu",
                columns: table => new
                {
                    MaVatTu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVatTu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaiVatTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    SoLuongToiThieu = table.Column<int>(type: "int", nullable: false),
                    HanSuDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NhaCungCap = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTu", x => x.MaVatTu);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauDatLich",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoZalo = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauDatLich", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiaDichVu",
                columns: table => new
                {
                    MaGiaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaDichVu", x => x.MaGiaDichVu);
                    table.ForeignKey(
                        name: "FK_GiaDichVu_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    MaPhanQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiNV = table.Column<int>(type: "int", nullable: false),
                    XemLich = table.Column<bool>(type: "bit", nullable: false),
                    SuaLich = table.Column<bool>(type: "bit", nullable: false),
                    XemHoSo = table.Column<bool>(type: "bit", nullable: false),
                    SuaHoSo = table.Column<bool>(type: "bit", nullable: false),
                    XemDoanhThu = table.Column<bool>(type: "bit", nullable: false),
                    QuanLyKho = table.Column<bool>(type: "bit", nullable: false),
                    QuanLyNhanSu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyen", x => x.MaPhanQuyen);
                    table.ForeignKey(
                        name: "FK_PhanQuyen_LoaiNhanVien_MaLoaiNV",
                        column: x => x.MaLoaiNV,
                        principalTable: "LoaiNhanVien",
                        principalColumn: "MaLoaiNV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhan",
                columns: table => new
                {
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienSuBenh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiUng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChuBacSi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhan", x => x.MaBenhNhan);
                    table.ForeignKey(
                        name: "FK_BenhNhan_TaiKhoanNguoiDung_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoanNguoiDung",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoanNV = table.Column<int>(type: "int", nullable: false),
                    MaLoaiNV = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianBatDauLam = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BangCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoNamKinhNghiem = table.Column<int>(type: "int", nullable: false),
                    GioiThieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVien_LoaiNhanVien_MaLoaiNV",
                        column: x => x.MaLoaiNV,
                        principalTable: "LoaiNhanVien",
                        principalColumn: "MaLoaiNV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanVien_TaiKhoanNhanVien_MaTaiKhoanNV",
                        column: x => x.MaTaiKhoanNV,
                        principalTable: "TaiKhoanNhanVien",
                        principalColumn: "MaTaiKhoanNV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuanHeBenhNhan",
                columns: table => new
                {
                    MaQuanHe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false),
                    QuanHe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHeBenhNhan", x => x.MaQuanHe);
                    table.ForeignKey(
                        name: "FK_QuanHeBenhNhan_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuanHeBenhNhan_TaiKhoanNguoiDung_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoanNguoiDung",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BacSi",
                columns: table => new
                {
                    MaBacSi = table.Column<int>(type: "int", nullable: false),
                    SoChungChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChuyenKhoaChinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SoNamKinhNghiem = table.Column<int>(type: "int", nullable: false),
                    MoTaChuyenMon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSi", x => x.MaBacSi);
                    table.ForeignKey(
                        name: "FK_BacSi_NhanVien_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaoCao",
                columns: table => new
                {
                    MaBaoCao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiBaoCao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenBaoCao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiTao = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuongDanFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCao", x => x.MaBaoCao);
                    table.ForeignKey(
                        name: "FK_BaoCao_NhanVien_NguoiTao",
                        column: x => x.NguoiTao,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "CauHinhHeThong",
                columns: table => new
                {
                    MaCauHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCauHinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiCauHinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NguoiCapNhat = table.Column<int>(type: "int", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhHeThong", x => x.MaCauHinh);
                    table.ForeignKey(
                        name: "FK_CauHinhHeThong_NhanVien_NguoiCapNhat",
                        column: x => x.NguoiCapNhat,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "ChamCong",
                columns: table => new
                {
                    MaChamCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayLamViec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioVao = table.Column<TimeSpan>(type: "time", nullable: true),
                    GioRa = table.Column<TimeSpan>(type: "time", nullable: true),
                    SoGioLam = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCong", x => x.MaChamCong);
                    table.ForeignKey(
                        name: "FK_ChamCong_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichBaoTriGhe",
                columns: table => new
                {
                    MaBaoTri = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGhe = table.Column<int>(type: "int", nullable: false),
                    NgayBaoTri = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiBaoTri = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichBaoTriGhe", x => x.MaBaoTri);
                    table.ForeignKey(
                        name: "FK_LichBaoTriGhe_GheNhaKhoa_MaGhe",
                        column: x => x.MaGhe,
                        principalTable: "GheNhaKhoa",
                        principalColumn: "MaGhe",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichBaoTriGhe_NhanVien_NguoiBaoTri",
                        column: x => x.NguoiBaoTri,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "LichLamViec",
                columns: table => new
                {
                    MaLich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    Thu = table.Column<int>(type: "int", nullable: false),
                    CaLam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichLamViec", x => x.MaLich);
                    table.ForeignKey(
                        name: "FK_LichLamViec_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Luong",
                columns: table => new
                {
                    MaLuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    LuongCoBan = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    PhanTramHoaHong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SoTienHoaHong = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    Thuong = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    KhauTru = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    TongLuong = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    NgayTinhLuong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Luong", x => x.MaLuong);
                    table.ForeignKey(
                        name: "FK_Luong_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhapKho",
                columns: table => new
                {
                    MaNhapKho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVatTu = table.Column<int>(type: "int", nullable: false),
                    SoLuongNhap = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    ThanhTien = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiNhap = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapKho", x => x.MaNhapKho);
                    table.ForeignKey(
                        name: "FK_NhapKho_NhanVien_NguoiNhap",
                        column: x => x.NguoiNhap,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_NhapKho_VatTu_MaVatTu",
                        column: x => x.MaVatTu,
                        principalTable: "VatTu",
                        principalColumn: "MaVatTu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHen",
                columns: table => new
                {
                    MaLichHen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoanDatLich = table.Column<int>(type: "int", nullable: true),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false),
                    MaBacSi = table.Column<int>(type: "int", nullable: true),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaGhe = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioHen = table.Column<TimeSpan>(type: "time", nullable: false),
                    LyDoKham = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuanHe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KenhDatLich = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanVienXacNhan = table.Column<int>(type: "int", nullable: true),
                    ThoiGianXacNhan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHen", x => x.MaLichHen);
                    table.ForeignKey(
                        name: "FK_LichHen_BacSi_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSi",
                        principalColumn: "MaBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_GheNhaKhoa_MaGhe",
                        column: x => x.MaGhe,
                        principalTable: "GheNhaKhoa",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK_LichHen_NhanVien_MaNhanVienXacNhan",
                        column: x => x.MaNhanVienXacNhan,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_LichHen_TaiKhoanNguoiDung_MaTaiKhoanDatLich",
                        column: x => x.MaTaiKhoanDatLich,
                        principalTable: "TaiKhoanNguoiDung",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HoSoBenhAn",
                columns: table => new
                {
                    MaHoSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false),
                    MaBacSi = table.Column<int>(type: "int", nullable: false),
                    MaLichHen = table.Column<int>(type: "int", nullable: true),
                    MaGhe = table.Column<int>(type: "int", nullable: true),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrieuChung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhuongPhapDieuTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonThuoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoiDan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HenTaiKham = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DaThanhToan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoBenhAn", x => x.MaHoSo);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAn_BacSi_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSi",
                        principalColumn: "MaBacSi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAn_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAn_GheNhaKhoa_MaGhe",
                        column: x => x.MaGhe,
                        principalTable: "GheNhaKhoa",
                        principalColumn: "MaGhe");
                    table.ForeignKey(
                        name: "FK_HoSoBenhAn_LichHen_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHen",
                        principalColumn: "MaLichHen");
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: true),
                    MaLichHen = table.Column<int>(type: "int", nullable: true),
                    SoDienThoaiNhan = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianHenGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KetQua = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBao_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan");
                    table.ForeignKey(
                        name: "FK_ThongBao_LichHen_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHen",
                        principalColumn: "MaLichHen");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoSo",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    ThanhTien = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHoSo", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietHoSo_DichVu_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVu",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHoSo_HoSoBenhAn_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAn",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhXQuang",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiUpload = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhXQuang", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK_HinhAnhXQuang_HoSoBenhAn_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAn",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HinhAnhXQuang_NhanVien_NguoiUpload",
                        column: x => x.NguoiUpload,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    HinhThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CoTraGop = table.Column<bool>(type: "bit", nullable: false),
                    KeHoachTraGop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiThu = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    APIResponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToan_HoSoBenhAn_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAn",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThanhToan_NhanVien_NguoiThu",
                        column: x => x.NguoiThu,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "XuatKho",
                columns: table => new
                {
                    MaXuatKho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVatTu = table.Column<int>(type: "int", nullable: false),
                    SoLuongXuat = table.Column<int>(type: "int", nullable: false),
                    LyDoXuat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiXuat = table.Column<int>(type: "int", nullable: true),
                    MaHoSo = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XuatKho", x => x.MaXuatKho);
                    table.ForeignKey(
                        name: "FK_XuatKho_HoSoBenhAn_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoBenhAn",
                        principalColumn: "MaHoSo");
                    table.ForeignKey(
                        name: "FK_XuatKho_NhanVien_NguoiXuat",
                        column: x => x.NguoiXuat,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_XuatKho_VatTu_MaVatTu",
                        column: x => x.MaVatTu,
                        principalTable: "VatTu",
                        principalColumn: "MaVatTu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToanOnline",
                columns: table => new
                {
                    MaThanhToanOnline = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhToan = table.Column<int>(type: "int", nullable: false),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoTien = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToanOnline", x => x.MaThanhToanOnline);
                    table.ForeignKey(
                        name: "FK_ThanhToanOnline_ThanhToan_MaThanhToan",
                        column: x => x.MaThanhToan,
                        principalTable: "ThanhToan",
                        principalColumn: "MaThanhToan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoCao_NguoiTao",
                table: "BaoCao",
                column: "NguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhan_MaTaiKhoan",
                table: "BenhNhan",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhHeThong_NguoiCapNhat",
                table: "CauHinhHeThong",
                column: "NguoiCapNhat");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_MaNhanVien_NgayLamViec",
                table: "ChamCong",
                columns: new[] { "MaNhanVien", "NgayLamViec" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoSo_MaDichVu",
                table: "ChiTietHoSo",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoSo_MaHoSo",
                table: "ChiTietHoSo",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_GiaDichVu_MaDichVu",
                table: "GiaDichVu",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhXQuang_MaHoSo",
                table: "HinhAnhXQuang",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhXQuang_NguoiUpload",
                table: "HinhAnhXQuang",
                column: "NguoiUpload");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAn_MaBacSi",
                table: "HoSoBenhAn",
                column: "MaBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAn_MaBenhNhan",
                table: "HoSoBenhAn",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAn_MaGhe",
                table: "HoSoBenhAn",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAn_MaLichHen",
                table: "HoSoBenhAn",
                column: "MaLichHen");

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoTriGhe_MaGhe",
                table: "LichBaoTriGhe",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoTriGhe_NguoiBaoTri",
                table: "LichBaoTriGhe",
                column: "NguoiBaoTri");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBacSi",
                table: "LichHen",
                column: "MaBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBenhNhan",
                table: "LichHen",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaDichVu",
                table: "LichHen",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaGhe",
                table: "LichHen",
                column: "MaGhe");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaNhanVienXacNhan",
                table: "LichHen",
                column: "MaNhanVienXacNhan");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaTaiKhoanDatLich",
                table: "LichHen",
                column: "MaTaiKhoanDatLich");

            migrationBuilder.CreateIndex(
                name: "IX_LichLamViec_MaNhanVien_Thu_CaLam",
                table: "LichLamViec",
                columns: new[] { "MaNhanVien", "Thu", "CaLam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Luong_MaNhanVien",
                table: "Luong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaLoaiNV",
                table: "NhanVien",
                column: "MaLoaiNV");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaTaiKhoanNV",
                table: "NhanVien",
                column: "MaTaiKhoanNV");

            migrationBuilder.CreateIndex(
                name: "IX_NhapKho_MaVatTu",
                table: "NhapKho",
                column: "MaVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_NhapKho_NguoiNhap",
                table: "NhapKho",
                column: "NguoiNhap");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_MaLoaiNV",
                table: "PhanQuyen",
                column: "MaLoaiNV",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeBenhNhan_MaBenhNhan",
                table: "QuanHeBenhNhan",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeBenhNhan_MaTaiKhoan_MaBenhNhan",
                table: "QuanHeBenhNhan",
                columns: new[] { "MaTaiKhoan", "MaBenhNhan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_MaHoSo",
                table: "ThanhToan",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_NguoiThu",
                table: "ThanhToan",
                column: "NguoiThu");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanOnline_MaThanhToan",
                table: "ThanhToanOnline",
                column: "MaThanhToan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_MaBenhNhan",
                table: "ThongBao",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_MaLichHen",
                table: "ThongBao",
                column: "MaLichHen");

            migrationBuilder.CreateIndex(
                name: "IX_XuatKho_MaHoSo",
                table: "XuatKho",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_XuatKho_MaVatTu",
                table: "XuatKho",
                column: "MaVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_XuatKho_NguoiXuat",
                table: "XuatKho",
                column: "NguoiXuat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCao");

            migrationBuilder.DropTable(
                name: "CauHinhHeThong");

            migrationBuilder.DropTable(
                name: "ChamCong");

            migrationBuilder.DropTable(
                name: "ChiTietHoSo");

            migrationBuilder.DropTable(
                name: "GiaDichVu");

            migrationBuilder.DropTable(
                name: "HinhAnhXQuang");

            migrationBuilder.DropTable(
                name: "LichBaoTriGhe");

            migrationBuilder.DropTable(
                name: "LichLamViec");

            migrationBuilder.DropTable(
                name: "Luong");

            migrationBuilder.DropTable(
                name: "NhapKho");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "QuanHeBenhNhan");

            migrationBuilder.DropTable(
                name: "ThanhToanOnline");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "XuatKho");

            migrationBuilder.DropTable(
                name: "YeuCauDatLich");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "VatTu");

            migrationBuilder.DropTable(
                name: "HoSoBenhAn");

            migrationBuilder.DropTable(
                name: "LichHen");

            migrationBuilder.DropTable(
                name: "BacSi");

            migrationBuilder.DropTable(
                name: "BenhNhan");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "GheNhaKhoa");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "TaiKhoanNguoiDung");

            migrationBuilder.DropTable(
                name: "LoaiNhanVien");

            migrationBuilder.DropTable(
                name: "TaiKhoanNhanVien");
        }
    }
}
