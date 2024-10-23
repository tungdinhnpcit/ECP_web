using System;

namespace ECP_V2.WebApplication.Models
{
    public class api_ChamDiemAnToanModel
    {
        public string TenLoaiTC { get; set; }
        public int Id { get; set; }
        public int TieuChiId { get; set; }
        public int TieuChiCha { get; set; }
        public int PhongBanId { get; set; }
        public string MA_DVIQLY { get; set; }
        public decimal SoDiem { get; set; }
        public string DonViTinhDiem { get; set; }
        public int? SoLan { get; set; }
        public string NguoiChamId { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }

        public string TenTieuChi { get; set; }
        public decimal? SoDiemCham { get; set; }
        public decimal? SoDiemDaCham { get; set; }
        public decimal? TyLe { get; set; }
        public decimal? SoSuat { get; set; }
        public decimal? SoSuatViPham { get; set; }
        public bool? IsQuanTrong { get; set; }
        public bool? IsSuatSuCo { get; set; }
        public bool? IsChaSuatSuCo { get; set; }
        public bool? IsChaSuatDongBo { get; set; }
        public bool? IsNhapTrucTiepDiem { get; set; }
        public int? Tu { get; set; }
        public int? Den { get; set; }
        public int? ViPhamTu { get; set; }
        public int? ViPhamDen { get; set; }
        public bool IsDaChamDiem { get; set; }
        public int? MaTieuChiDongBo { get; set; }
        public string DonViDongBo { get; set; }
        public decimal? SoLuong { get; set; }
        public decimal? SoLuongDaCham { get; set; }
        public bool? IsChonDonVi { get; set; }
        public int? SoLanGap { get; set; }
        public decimal? SoSuatViPhamDaCham { get; set; }

        public string GhiChuViPham { get; set; }

        public string DropDviQlyHtml { get; set; }
    }

}