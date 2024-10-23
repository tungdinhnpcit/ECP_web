namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPhongBan")]
    public partial class tblPhongBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPhongBan()
        {
            tblNhanViens = new HashSet<tblNhanVien>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string TenPhongBan { get; set; }

        [Required]
        [StringLength(50)]
        public string MoTa { get; set; }

        public int? MaDVi { get; set; }

        public virtual tblDonVi tblDonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblNhanVien> tblNhanViens { get; set; }
    }
}
