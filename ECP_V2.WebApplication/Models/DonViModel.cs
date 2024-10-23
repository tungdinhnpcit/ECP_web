using System;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.Models
{
    public class DonViModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string TenDonVi { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public string SDT { get; set; }
        public string DviCha { get; set; }
        public Nullable<int> CapDvi { get; set; }
        public Nullable<int> ViTri { get; set; }
        public Nullable<int> MaLP { get; set; }
        //public Nullable<int> KieuDvi { get; set; }
        //public Nullable<int> LienKetDvi { get; set; }
    }


    public class ChamDiemAnToanModels
    {

        //tieu chi
        public string TenLoaiTC { get; set; }
        public int TieuChiId { get; set; }
        public decimal SoDiem { get; set; }
        public string DonViTinhDiem { get; set; }
        public string TenTieuChi { get; set; }
        public bool? IsQuanTrong { get; set; }
        public bool? IsSuatSuCo { get; set; }
        public int? ViTri { get; set; }
        public int? TieuChiCha { get; set; }

        //cham diem
        public int Id { get; set; }
        public decimal? SoDiemDaCham { get; set; }
        public decimal? SoSuatSuCo { get; set; }
        public decimal? SoSuatViPham { get; set; }
        public decimal? SoLuong { get; set; }
        public int? LoaiTieuChiId { get; set; }
    }

    public class TongHopChamDiemModels
    {
        public Nullable<int> CAP_DVI { get; set; }
        public string CHUC_VU { get; set; }
        public string CVU_UQUYEN { get; set; }
        public string DAI_DIEN { get; set; }
        public string DCHI_DVIUQ { get; set; }
        public string DIA_CHI { get; set; }
        public string DIEN_THOAI { get; set; }
        public string DTHOAI_KDOANH { get; set; }
        public string DTHOAI_NONG { get; set; }
        public string DTHOAI_TRUC { get; set; }
        public string EMAIL { get; set; }
        public string FAX { get; set; }
        public string MA_DVICTREN { get; set; }
        public string MA_DVIQLY { get; set; }
        public Nullable<int> MA_STHUE { get; set; }
        public Nullable<DateTime> NGAY_UQUYEN { get; set; }
        public Nullable<int> SO_UQUYEN { get; set; }
        public Nullable<int> SoTieuChiChuaDat { get; set; }
        public string TEN_DVIQLY { get; set; }
        public string TEN_DVIUQ { get; set; }
        public string TEN_TINH { get; set; }
        public string TNGUOI_UQUYEN { get; set; }
        public double TongDiemTru { get; set; }
        public Nullable<int> ViTriThangTruoc { get; set; }
        public string WEBSITE { get; set; }

    }

    public class KetQuaChamDiemModel2
    {
        public string DonViTinhDiem { get; set; }
        public int Id { get; set; }
        public string MA_DVIQLY { get; set; }
        public DateTime? NgayCham { get; set; }
        public DateTime? NgaySua { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiChamId { get; set; }
        public string NguoiSua { get; set; }
        public string NguoiTao { get; set; }
        public int? PhongBanId { get; set; }
        public decimal? SoDiem { get; set; }
        public int? SoLan { get; set; }
        public int? SoSuat { get; set; }
        public string TenPhongBan { get; set; }
        public string TenTieuChi { get; set; }
        public int? TieuChiId { get; set; }

    }

    public class KetQuaChamDiemModel
    {
        public int Id { get; set; }
        public int TieuChiId { get; set; }
        public int PhongBanId { get; set; }
        public string MA_DVIQLY { get; set; }
        public string NguoiChamId { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public decimal SoDiem { get; set; }
        public string DonViTinhDiem { get; set; }
        public decimal? SoLan { get; set; }
        public decimal? SoSuat { get; set; }
        public decimal? SoLuong { get; set; }
        public bool? IsLapLai { get; set; }
        public int? SoLanGap { get; set; }

        public string TenTieuChi { get; set; }
        public string TenPhongBan { get; set; }


    }

}