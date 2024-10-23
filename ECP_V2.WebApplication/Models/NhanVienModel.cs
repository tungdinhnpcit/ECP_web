using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ECP_V2.WebApplication.Models
{
    public class NhanVienModel
    {
        public string Id { get; set; }
        [DisplayName("Tên khách hàng")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string TenNhanVien { get; set; }
        public string ChucVu { get; set; }
        public string UrlImage { get; set; }
        public string BacHT { get; set; }
        public string BacThi { get; set; }
        public string BacAnToan { get; set; }
        public string NgaySinh { get; set; }
        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(11, ErrorMessage = "Số điện thoại phải từ 10 đến 11 ký tự.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được nhập số")]
        public string SoDT { get; set; }
        public string Email { get; set; }
        public bool IsCapPhieu { get; set; }
        public bool IsLanhDaoCv { get; set; }
        public bool IschiHuyTT { get; set; }
        public bool IsNguoiChoPhep { get; set; }
        public bool IsGiamSatAT { get; set; }
        public bool IsNguoiRaLenh { get; set; }
        public bool IsThiHanhLenh { get; set; }
        public string DonViId { get; set; }
        public int PhongBanId { get; set; }
        public string RoleId { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Không đúng với mật khẩu đã nhập.")]
        public string ConfirmPassword { get; set; }

        [UIHint("SignaturePad")]
        public byte[] ChuKySo { get; set; }

        public string ChuKySoBase64 { get; set; }
        public string Hsm_type { get; set; }
        public string Hsm_serial { get; set; }
        public string NhaMangSDT { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        //public IEnumerable<SelectListItem> ListDonVi { get; set; }

        //public IEnumerable<SelectListItem> ListPhongBan { get; set; }
        //public IEnumerable<SelectListItem> ListRole { get; set; }
        //public NhanVienModel(bool t)
        //{
        //    var svDonVi = new DonViRepository();
        //    var listDVi = svDonVi.List().OrderBy(c => c.TenDonVi);
        //    ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

        //    var svPhongBan = new PhongBanRepository();
        //    var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
        //    ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");

        //    IdentityManager idenM = new IdentityManager();
        //    var listRole = idenM.GetAllRole().OrderBy(p => p.Id);
        //    ListRole = new SelectList(listRole, "Id", "Name");
        //}
        public NhanVienModel()
        {

        }
        public NhanVienModel(string id)
        {
            //var svDonVi = new DonViRepository();
            //var listDVi = svDonVi.List().OrderBy(c => c.TenDonVi);
            //ListDonVi = new SelectList(listDVi, "Id", "TenDonvi");

            //var svPhongBan = new PhongBanRepository();
            //var listPban = svPhongBan.List().OrderBy(c => c.TenPhongBan);
            //ListPhongBan = new SelectList(listPban, "Id", "TenPhongBan");

            //IdentityManager idenM = new IdentityManager();
            //var listRole = idenM.GetAllRole().OrderBy(p => p.Id);
            //ListRole = new SelectList(listRole, "Id", "Name");
        }
    }
}