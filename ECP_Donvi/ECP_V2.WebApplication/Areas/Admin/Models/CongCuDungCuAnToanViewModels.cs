using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class CongCuDungCuAnToanViewModels
    {
        // thong tin thiet bi ATLD
        public int ID { get; set; }
        public string TenThietBi { get; set; }
        public string MaHieu { get; set; }
        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<int> PhongBanID { get; set; }
        public string DonViId { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string TenNSX { get; set; }
        public string TenPB { get; set; }
        public Nullable<int> MaLoai { get; set; }
        public string QuyTacDanhMa { get; set; }
        public int SoLuong { get; set; }
        public Nullable<int> MaTT { get; set; }
        public bool? switchTTKD { get; set; }

        //thong tin kiem tra
        public Nullable<int> LanKiemTra { get; set; }
        public Nullable<System.DateTime> NgayKiemTra { get; set; }
        public string NguoiKiemTra { get; set; }
        public string DonViKiemTra { get; set; }
        public string BienBanSo { get; set; }
        public Nullable<bool> KetQua { get; set; }
        public Nullable<System.DateTime> NgayKiemTraTiepTheo { get; set; }
        public string GhiChu { get; set; }

        public List<SoTheoDoiCCDCATViewModels> SoTheoDois { get; set; }
    }

    public class SoTheoDoiCCDCATViewModels
    {
        public int ID { get; set; }
        public Nullable<int> LanKiemTra { get; set; }
        public Nullable<System.DateTime> NgayKiemTra { get; set; }
        public string NguoiKiemTra { get; set; }
        public string DonViKiemTra { get; set; }
        public string BienBanSo { get; set; }
        public Nullable<bool> KetQua { get; set; }
        public Nullable<System.DateTime> NgayKiemTraTiepTheo { get; set; }
        public string GhiChu { get; set; }

    }
}