namespace ECP_WEBAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblBienBanKiemTra")]
    public partial class tblBienBanKiemTra
    {
        public int Id { get; set; }

        public int DonViId { get; set; }

        public int PhongBanId { get; set; }

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

        [Required]
        [StringLength(256)]
        public string DiaDiem { get; set; }

        [Required]
        [StringLength(50)]
        public string NguoiChuTri { get; set; }

        [StringLength(50)]
        public string ChucVuNguoiChuTri { get; set; }

        public short So_Nguoi_TD { get; set; }

        [StringLength(256)]
        public string TrangBiCaNhan { get; set; }

        [StringLength(256)]
        public string TrangBiThiCong { get; set; }

        [StringLength(256)]
        public string TrangBiAnToan { get; set; }

        [StringLength(256)]
        public string TrangBiKhac { get; set; }

        [StringLength(500)]
        public string YKien_NhacNho { get; set; }

        [StringLength(500)]
        public string TepDinhKem { get; set; }

        public int? BaoCaoId { get; set; }

        public virtual tblBaoCao tblBaoCao { get; set; }
    }
}
