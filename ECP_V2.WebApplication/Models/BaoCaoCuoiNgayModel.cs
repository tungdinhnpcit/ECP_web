using System;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.Models
{
    public class BaoCaoCuoiNgayModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string DonViId { get; set; }
        public string TruongDonVi { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayBaoCao { get; set; }
        public int? So_CV_KH { get; set; }
        public int? So_CV_BS { get; set; }
        public int? So_CV_DX { get; set; }
        public int? So_CV_Ko_KH { get; set; }
        public int? So_Dvi_GuiAnh { get; set; }
        public int? So_Dvi_Ktra { get; set; }
        public string ChiTietDviKtra { get; set; }
        public int? Tong_PTT { get; set; }
        public int? Tong_PCT { get; set; }
        public int? Tong_LCT { get; set; }
        public int? Kq_SoBB { get; set; }
        public int? Kq_KiemTra { get; set; }
        public int? Kq_Cv_KoDat { get; set; }
        public int? Kq_Cv_KetThuc { get; set; }
        public string ViPham { get; set; }
        public string DeNghi { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public int So_Dvi_ChuaGuiAnh { get; set; }
        public int Tong_So_PhienLV { get; set; }
        public int So_Dvi_GuiThemAnh { get; set; }
        public int Tong_So_PhienLV_ChuaGuiAnh_NgayHomTruoc { get; set; }

        public BaoCaoCuoiNgayModel()
        {

        }
    }
}