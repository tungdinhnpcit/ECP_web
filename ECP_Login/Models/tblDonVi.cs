namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblDonVi")]
    public partial class tblDonVi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDonVi()
        {
            tblPhongBans = new HashSet<tblPhongBan>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string TenDonVi { get; set; }

        [StringLength(50)]
        public string TenVietTat { get; set; }

        [StringLength(50)]
        public string MoTa { get; set; }

        public int? DviCha { get; set; }

        public int? CapDvi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPhongBan> tblPhongBans { get; set; }
    }
}
