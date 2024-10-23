namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPhienLamViec")]
    public partial class tblPhienLamViec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPhienLamViec()
        {
            tblComments = new HashSet<tblComment>();
            tblImages = new HashSet<tblImage>();
        }

        public int Id { get; set; }

        public int PhongBanID { get; set; }

        [Required]
        [StringLength(500)]
        public string NoiDung { get; set; }

        [Required]
        [StringLength(256)]
        public string DiaDiem { get; set; }

        public DateTime NgayLamViec { get; set; }

        public TimeSpan GioBd { get; set; }

        public TimeSpan GioKt { get; set; }

        [StringLength(256)]
        public string NguoiDuyet_SoPa { get; set; }

        [StringLength(256)]
        public string NguoiChiHuy { get; set; }

        [StringLength(256)]
        public string GiamSatVien { get; set; }

        [StringLength(256)]
        public string NguoiKiemSoat { get; set; }

        [StringLength(256)]
        public string NguoiKiemTraPhieu { get; set; }

        [StringLength(256)]
        public string LanhDaoTrucBan { get; set; }

        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        public string NguoiTao { get; set; }

        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        public string NguoiSua { get; set; }

        [StringLength(256)]
        public string TT_Phien { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }

        public int? NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblComment> tblComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblImage> tblImages { get; set; }
    }
}
