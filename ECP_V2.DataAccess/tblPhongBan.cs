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
    
    public partial class tblPhongBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPhongBan()
        {
            this.pccc_ThietBiPCCC = new HashSet<pccc_ThietBiPCCC>();
            this.tbnn_ThietBiNghiemNgat = new HashSet<tbnn_ThietBiNghiemNgat>();
            this.ThietBiATLDs = new HashSet<ThietBiATLD>();
            this.tblNhanViens = new HashSet<tblNhanVien>();
        }
    
        public int Id { get; set; }
        public string TenPhongBan { get; set; }
        public string MoTa { get; set; }
        public string MaDVi { get; set; }
        public string TenVietTat { get; set; }
        public string SDT { get; set; }
        public Nullable<int> LoaiPB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pccc_ThietBiPCCC> pccc_ThietBiPCCC { get; set; }
        public virtual tblDonVi tblDonVi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbnn_ThietBiNghiemNgat> tbnn_ThietBiNghiemNgat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThietBiATLD> ThietBiATLDs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblNhanVien> tblNhanViens { get; set; }
    }
}
