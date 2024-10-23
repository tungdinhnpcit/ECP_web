namespace ECP_WEBAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblComment")]
    public partial class tblComment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string CommentContent { get; set; }

        public DateTime CreateTime { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        public short Priority { get; set; }

        public string Description { get; set; }

        public int? PhienLamViecId { get; set; }

        public virtual tblPhienLamViec tblPhienLamViec { get; set; }
    }
}
