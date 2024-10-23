using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class SuCoViewModel
    {
        
        public int Id { get; set; }
        public string DonViId { get; set; }
        [Required(ErrorMessage = "Không được để trống cấp điện áp")]
        public string CapDienAp { get; set; }
        [Required(ErrorMessage = "Không được để trống tên thiết bị")]
        public string TenThietBi { get; set; }

        public string DienBienSuCo { get; set; }
        [Required(ErrorMessage = "Không được để trống đoạn tóm tắt")]
        public string TomTat { get; set; }
        public Nullable<bool> TinhTrangBienBan { get; set; }
        public Nullable<bool> HinhAnhSuCo { get; set; }

        [Required(ErrorMessage = "Không được để trống thời gian xuất hiện sự cố")]
        public Nullable<System.DateTime> ThoiGianXuatHien { get; set; }
        public Nullable<System.DateTime> ThoiGianBatDauKhacPhuc { get; set; }
        public Nullable<System.DateTime> ThoiGianKhacPhucXong { get; set; }
        public Nullable<System.DateTime> ThoiGianKhoiPhuc { get; set; }
        public string GioXuatHien { get; set; }        
        public string GioBDKhacPhuc { get; set; }
        public string GioKhacPhucXong { get; set; }
        public string GioKhoiPhuc { get; set; }

        public Nullable<double> T_XuatHienBatDauKhacPhuc { get; set; }
        public Nullable<double> T_BatDauDenKhacPhucXong { get; set; }
        public Nullable<double> T_KhacPhucXongDenKhoiPhuc { get; set; }
        public Nullable<double> T_TongThoiGianMatDien { get; set; }

        public Nullable<bool> IsGianDoan { get; set; }
        public Nullable<int> PhieuCongTacId { get; set; }
        [Required(ErrorMessage = "Không được để trống loại sự cố")]
        public Nullable<int> LoaiSuCoId { get; set; }
        [Required(ErrorMessage = "Không được để trống nguyên nhân")]
        public Nullable<int> NguyenNhanId { get; set; }
        [Required(ErrorMessage = "Không được để trống lý do")]
        public Nullable<int> LyDoId { get; set; }
        [Required(ErrorMessage = "Không được để trống tính chất")]
        public Nullable<int> TinhChatId { get; set; }

        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<decimal> KinhDo { get; set; }
        public Nullable<decimal> ViDo { get; set; }
        [Required(ErrorMessage = "Không được để trống loại tài sản")]
        public Nullable<bool> IsTaiSan { get; set; }
        public Nullable<bool> IsMienTru { get; set; }
        public string[] lstDonViSuCoId { get; set; }
        


        public string LoaiSuCo { get; set; }

        public string LyDo { get; set; }

        public string NguyenNhan { get; set; }

        public string TinhChat { get; set; }

        public string NT_HOTEN { get; set; }
        public string NT_SDT { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string ThoiTiet { get; set; }
        public string GhiChu { get; set; }
    }
}