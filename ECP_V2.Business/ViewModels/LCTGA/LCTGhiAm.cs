using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;

namespace ECP_V2.Business.ViewModels.LCTGA
{
    public partial class plv_PhieuCongTac_view2
    {
        public int ID { get; set; }
        public string SoPhieu { get; set; }
        public string NoiDung { get; set; }
        public Nullable<int> MaLP { get; set; }
        public Nullable<int> MaTT { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayCN { get; set; }
        public string NguoiCN { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string DieuKienAnToan { get; set; }
        public string DonViLienQuanQLVH { get; set; }
        public string NguoiChoPhep { get; set; }
        public string GiayBanGiaoQLVH { get; set; }
        public string ChiTietCatDien { get; set; }
        public string ChiTietNoiDat { get; set; }
        public string ChiTietRaoChan { get; set; }
        public string ChiTietBienBao { get; set; }
        public string PhamViLamViec { get; set; }
        public string CanhBaoNguyHiem { get; set; }
        public Nullable<bool> CHTT_B2 { get; set; }
        public Nullable<bool> GSAT_B2 { get; set; }
        public Nullable<System.DateTime> NgayGioKT_B2 { get; set; }
        public Nullable<bool> NGUOICP_B2 { get; set; }
        public string NoiDatTai { get; set; }
        public string AnToanKhac { get; set; }
        public Nullable<bool> CHTT_B3 { get; set; }
        public Nullable<bool> GSAT_B3 { get; set; }
        public Nullable<System.DateTime> NgayGioKT_B3 { get; set; }
        public Nullable<bool> CHTT_B6 { get; set; }
        public Nullable<bool> NGUOICP_B6 { get; set; }
        public Nullable<System.DateTime> NgayGioKT_B6 { get; set; }
        public Nullable<bool> NguoiTaoKT_B6 { get; set; }
        public Nullable<int> SoPhieuLenhInt { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayDuyetNPC { get; set; }
        public string NguoiDuyetNPC { get; set; }
        public Nullable<int> TrangThaiChuyen { get; set; }
        public string DonViId { get; set; }
        public Nullable<int> SoNguoiThamGia { get; set; }
        public string NguoiCapPhieu { get; set; }
        public string NguoiCapPhieu_Id { get; set; }
        public string TenDonVi_LCT { get; set; }
        public Nullable<int> PhongBanID_PCT { get; set; }
        public string PhongBan_PCT { get; set; }
        public Nullable<int> LoaiCongViecId { get; set; }
        public string MaYeuCauCRM { get; set; }
        public Nullable<int> TramId { get; set; }
        public Nullable<int> LoaiLCT { get; set; }
        public Nullable<int> TrangThaiDuyetGA { get; set; }
        public string LinkFile { get; set; }
        public string LanhDaoDuyetId { get; set; }
        public Nullable<int> IdGhiAm { get; set; }
    }


    public partial class DSLCTGhiAm
    {
        public Int64 Id { get; set; }
        public string DViCPhieu { get; set; }
        public string SoPhieu { get; set; }

        public string NguoiNhap { get; set; }
        public string NguoiNhanLenh { get; set; }
        public string LanhDao { get; set; }
        public string NgayNhap { get; set; }
        public string NgayNhanLenh { get; set; }
        public string NgayDuyet { get; set; }
        public int PhanLoai { get; set; }
        public string NgayCTac { get; set; }
        public int TrangThai { get; set; }
        public string LinkFile { get; set; }
        public string TGHoanThanh { get; set; }
        public string TenNguoiNhap { get; set; }
        public string TenNhanLenh { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string GCNhanLenh { get; set; }
        public string GCDuyet { get; set; }
        public int IdPhienLV { get; set; }
    }

    public partial class DSPCTGA
    {
        public int ID { get; set; }
        public string NoiDung { get; set; }
        public string SoPhieu { get; set; }

      
    }

}
