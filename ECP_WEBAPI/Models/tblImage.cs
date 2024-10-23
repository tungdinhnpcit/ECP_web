namespace ECP_WEBAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblImage")]
    public partial class tblImage
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Url { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        [StringLength(200)]
        public string Comment { get; set; }

        [StringLength(10)]
        public string Tag { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public int? GroupId { get; set; }

        public int? PhienLamViecId { get; set; }

        [StringLength(128)]
        public string UserUp { get; set; }

        public int? isVideo { get; set; }

        [StringLength(200)]
        public string VideoPath { get; set; }

        public virtual tblGroupImage tblGroupImage { get; set; }

        public virtual tblPhienLamViec tblPhienLamViec { get; set; }
    }
}
