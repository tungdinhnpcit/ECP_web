namespace ECP_Login.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MenuMaster")]
    public partial class MenuMaster
    {
        [Key]
        public int MenuId { get; set; }

        public int? MenuParentId { get; set; }

        public int? MenuLevel { get; set; }

        [StringLength(50)]
        public string MenuCode { get; set; }

        [Required]
        [StringLength(255)]
        public string MenuText { get; set; }

        [Required]
        [StringLength(50)]
        public string MenuOrder { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        public bool? IsNewLetter { get; set; }

        public bool? IsShowMenu { get; set; }

        public bool? IsFrontPage { get; set; }

        public bool? IsDisplay { get; set; }

        [StringLength(128)]
        public string RoleId { get; set; }

        [StringLength(128)]
        public string Class { get; set; }
    }
}
