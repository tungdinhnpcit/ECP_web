using ECP_V2.Business.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Models
{
    public class PhongBanModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên phòng ban không được bỏ trống.")]
        [DataType(DataType.Text)]
        public string TenPhongBan { get; set; }
        [DataType(DataType.Text)]
        public string MoTa { get; set; }
        [Required(ErrorMessage = "Đơn vị không được để trống.")]
        public string MaDVi { get; set; }
        public string TenVietTat { get; set; }
        public string TenDonVi { get; set; }
        public string SDT { get; set; }
        public Nullable<int> LoaiPB { get; set; }
        public IEnumerable<SelectListItem> ListDonVi { get; set; }

        public PhongBanModel()
        {
            var svDonVi = new DonViRepository();
            var lstDonVi = svDonVi.List().OrderBy(c => c.TenDonVi);
            ListDonVi = new SelectList(lstDonVi, "Id", "TenDonVi");
        }
    }
}