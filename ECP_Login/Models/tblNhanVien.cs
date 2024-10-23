namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblNhanVien")]
    public partial class tblNhanVien
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenNhanVien { get; set; }

        [StringLength(200)]
        public string ChucVu { get; set; }

        [StringLength(500)]
        public string UrlImage { get; set; }

        [StringLength(10)]
        public string BacHT { get; set; }

        [StringLength(10)]
        public string BacThi { get; set; }

        [StringLength(10)]
        public string BacAnToan { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public DateTime NgaySinh { get; set; }

        [StringLength(256)]
        public string DiaChi { get; set; }

        [StringLength(15)]
        public string SoDT { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        public bool? IsCapPhieu { get; set; }

        public bool? IsLanhDaoCv { get; set; }

        public bool? IschiHuyTT { get; set; }

        public bool? IsNguoiChoPhep { get; set; }

        public bool? IsGiamSatAT { get; set; }

        public bool? IsNguoiRaLenh { get; set; }

        public bool? IsThiHanhLenh { get; set; }

        public int? PhongBanId { get; set; }

        public int? DonViId { get; set; }

        public virtual tblPhongBan tblPhongBan { get; set; }
    }
}
