//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECP_V2.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class vsld_QuanTrac
    {
        public int Id { get; set; }
        public string DonVi { get; set; }
        public string DonViId { get; set; }
        public int Nam { get; set; }
        public Nullable<int> PLSK_Tong_Nam { get; set; }
        public Nullable<int> PLSK_Tong_Nu { get; set; }
        public Nullable<int> PLSK_Loai1_Nam { get; set; }
        public Nullable<int> PLSK_Loai1_Nu { get; set; }
        public Nullable<int> PLSK_Loai2_Nam { get; set; }
        public Nullable<int> PLSK_Loai2_Nu { get; set; }
        public Nullable<int> PLSK_Loai3_Nam { get; set; }
        public Nullable<int> PLSK_Loai3_Nu { get; set; }
        public Nullable<int> PLSK_Loai4_Nam { get; set; }
        public Nullable<int> PLSK_Loai4_Nu { get; set; }
        public Nullable<int> PLSK_Loai5_Nam { get; set; }
        public Nullable<int> PLSK_Loai5_Nu { get; set; }
        public Nullable<int> KQDK_TongMau { get; set; }
        public Nullable<int> KQDK_VuotMuc { get; set; }
        public Nullable<int> KQDK_BD1 { get; set; }
        public Nullable<int> KQDK_BD2 { get; set; }
        public Nullable<int> KQDK_BD3 { get; set; }
        public Nullable<int> KQDK_BD4 { get; set; }
        public Nullable<decimal> KQDK_ChiPhiDK { get; set; }
        public string KQDK_DonViThucHien { get; set; }
        public string GhiChu { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string NguoiChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayChuyenNPC { get; set; }
        public System.DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
        public string NguoiXoa { get; set; }
        public int TrangThai { get; set; }
    }
}
