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
    
    public partial class bcbs_ChiTieu
    {
        public int Id { get; set; }
        public string TenChiTieu { get; set; }
        public string DonViTinh { get; set; }
        public string KieuDuLieu { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string NguoiXoa { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
    }
}
