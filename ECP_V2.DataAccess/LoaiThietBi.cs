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
    
    public partial class LoaiThietBi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoaiThietBi()
        {
            this.ThietBiATLDs = new HashSet<ThietBiATLD>();
        }
    
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public Nullable<decimal> TaiTrong { get; set; }
        public string HoSoThietBi { get; set; }
        public string QuyTacDanhMa { get; set; }
    
        public virtual HangSanXuat HangSanXuat { get; set; }
        public virtual NhomThietBi NhomThietBi { get; set; }
        public virtual NuocSanXuat NuocSanXuat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThietBiATLD> ThietBiATLDs { get; set; }
    }
}
