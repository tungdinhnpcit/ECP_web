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
    
    public partial class NuocSanXuat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NuocSanXuat()
        {
            this.LoaiThietBis = new HashSet<LoaiThietBi>();
            this.pccc_LoaiThietBi = new HashSet<pccc_LoaiThietBi>();
            this.pccc_ThietBiPCCC = new HashSet<pccc_ThietBiPCCC>();
            this.ThietBiATLDs = new HashSet<ThietBiATLD>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoaiThietBi> LoaiThietBis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pccc_LoaiThietBi> pccc_LoaiThietBi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pccc_ThietBiPCCC> pccc_ThietBiPCCC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThietBiATLD> ThietBiATLDs { get; set; }
    }
}
