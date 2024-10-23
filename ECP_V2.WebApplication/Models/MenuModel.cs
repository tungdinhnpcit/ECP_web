using System;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.Models
{
    public class MenuModel
    {
        [Required]
        public int MenuId { get; set; }
        public int MenuParentId { get; set; }
        public int MenuLevel { get; set; }
        public string MenuCode { get; set; }
        [Required]
        public string MenuText { get; set; }
        public string MenuOrder { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public bool IsNewLetter { get; set; }
        public bool IsShowMenu { get; set; }
        public bool IsFrontPage { get; set; }
        public bool IsDisplay { get; set; }
        public string RoleId { get; set; }
        public string Class { get; set; }
        public Nullable<int> RoleView { get; set; }
    }
}