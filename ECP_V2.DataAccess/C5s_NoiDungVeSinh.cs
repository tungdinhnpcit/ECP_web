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
    
    public partial class C5s_NoiDungVeSinh
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public Nullable<int> ThuTu { get; set; }
        public int MaLoai { get; set; }
        public double GiaTri { get; set; }
        public string DonViTinh { get; set; }
        public string GhiChu { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public string NguoiChuyen { get; set; }
        public Nullable<System.DateTime> NgayChuyen { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string MaDonVi { get; set; }
        public int TrangThai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
    
        public virtual C5s_DanhMucLoaiVeSinh C5s_DanhMucLoaiVeSinh { get; set; }
    }
}
