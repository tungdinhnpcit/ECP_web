using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ECP_V2.Business;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Models
{
    public class BaoCaoModel
    {       
        [Required(ErrorMessage = "Không được bỏ trống")]
        public int Id { get; set; }
        public string DonViId { get; set; }   
        public string TieuDe { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NguoiDuyet { get; set; }
        public short So_BPTC_ATLD { get; set; }
        public short So_PTT { get; set; }
        public short So_PCT { get; set; }
        public short Lenh_CT { get; set; }
        public short So_P_ATLD { get; set; }
        public short So_BB_ATLD { get; set; }
        public short So_BPTC_ATLD_TT { get; set; }
        public short So_PTT_TT { get; set; }
        public short So_PCT_TT { get; set; }
        public short Lenh_CT_TT { get; set; }
        public short So_CV_DB { get; set; }
        public string ChiTiet_CV_DB { get; set; }
        public short So_CV_DK { get; set; }
        public string ChiTiet_CV_DK { get; set; }
        public short So_CV_BS { get; set; }
        public short So_CV_DX { get; set; }
        public string ChiTiet_CV_BS { get; set; }
        public string ChiTiet_CV_DX { get; set; }
        public short So_CV_HB { get; set; }
        public string NoiDung_CV_HB { get; set; }
        public string LyDo_CV_HB { get; set; }
        public short SoNguoiViPham { get; set; }
        public string ChiTietViPham { get; set; }
        public string So_BPTC_ATLD_TT_CT { get; set; }
        public string So_PTT_TT_CT { get; set; }
        public string So_PCT_TT_CT { get; set; }
        public string Lenh_CT_TT_CT { get; set; }
        public short? So_CV_DK_PCT { get; set; }
        public short? So_CV_DK_LC { get; set; }
        public short? So_CV_XH { get; set; }
        public string LyDo_CV_XH { get; set; }
        public short? TongSoCV { get; set; }

        public BaoCaoModel()
        {           
          
        }
    }
}