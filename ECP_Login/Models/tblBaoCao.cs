namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblBaoCao")]
    public partial class tblBaoCao
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBaoCao()
        {
            tblBienBanKiemTras = new HashSet<tblBienBanKiemTra>();
        }

        public int Id { get; set; }

        public int DonViId { get; set; }

        [Required]
        [StringLength(500)]
        public string TieuDe { get; set; }

        public DateTime NgayTao { get; set; }

        [Required]
        [StringLength(50)]
        public string NguoiTao { get; set; }

        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        public string NguoiSua { get; set; }

        public DateTime? NgayDuyet { get; set; }

        [StringLength(50)]
        public string NguoiDuyet { get; set; }

        public short So_BPTC_ATLD { get; set; }

        public short So_PTT { get; set; }

        public short So_PCT { get; set; }

        public short Lenh_CT { get; set; }

        public short So_P_ATLD { get; set; }

        public short So_BB_ATLD { get; set; }

        public short So_BPTC_ATLD_TT { get; set; }

        public short So_PTT_TT { get; set; }

        public short So_PCT_TT { get; set; }

        public short Lenh_CT_TT { get; set; }

        public short So_CV_DB { get; set; }

        [StringLength(256)]
        public string ChiTiet_CV_DB { get; set; }

        public short So_CV_DK { get; set; }

        [StringLength(256)]
        public string ChiTiet_CV_DK { get; set; }

        public short So_CV_BS { get; set; }

        [StringLength(256)]
        public string ChiTiet_CV_BS { get; set; }

        public short So_CV_HB { get; set; }

        [StringLength(256)]
        public string NoiDung_CV_HB { get; set; }

        [StringLength(256)]
        public string LyDo_CV_HB { get; set; }

        public short SoNguoiViPham { get; set; }

        [StringLength(256)]
        public string ChiTietViPham { get; set; }

        [StringLength(256)]
        public string So_BPTC_ATLD_TT_CT { get; set; }

        [StringLength(256)]
        public string So_PTT_TT_CT { get; set; }

        [StringLength(256)]
        public string So_PCT_TT_CT { get; set; }

        [StringLength(256)]
        public string Lenh_CT_TT_CT { get; set; }

        public short? So_CV_DK_PCT { get; set; }

        public short? So_CV_DK_LC { get; set; }

        public short? So_CV_XH { get; set; }

        [StringLength(256)]
        public string LyDo_CV_XH { get; set; }

        public short? TongSoCV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBienBanKiemTra> tblBienBanKiemTras { get; set; }
    }
}
