using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ECP_V2.DataAccess;
using ECP_V2.Business.Repository;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Models
{
    public class TramModel
    {
        [Required]
        public int Id { get; set; }        
        [Required(ErrorMessage = "Tên trạm không được bỏ trống.")]
        [DataType(DataType.Text)]
        public string Ten { get; set; }
        [DataType(DataType.Text)]
        public string MoTa { get; set; }
        [Required(ErrorMessage = "Phòng ban không được để trống.")]
        public int? PhongBanId { get; set; }
        public string TenPhongBan { get; set; }
        public string SDT { get; set; }
        public Nullable<int> Loai { get; set; }
    }
}