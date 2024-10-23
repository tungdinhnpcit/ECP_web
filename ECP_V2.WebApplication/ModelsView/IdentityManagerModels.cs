using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.ModelsView
{
    public class ViewRole
    {
        [Key]
        [Display(Name = "Mã quyền")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Tên nhóm quyền không được để trống!")]
        [Display(Name = "Tên quyền")]
        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Loại quyền")]
        [Required(ErrorMessage = "Loại quyền nhập số nguyên, không được để trống!")]
        public int TypeOfRole { get; set; }
    }
    public class ViewUser
    {

        //Thông tin tài khoản

        [Display(Name = "Mã tài khoản")]
        public string Id { get; set; }
        [Display(Name = "Hòm thư")]
        public string Email { get; set; }
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Tài khoản đăng nhập")]
        public string UserName { get; set; }
        public string MA_DVIQLY { get; set; }
        public string MA_PBAN { get; set; }
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }
        [Display(Name = "Chức danh")]
        public string ChucDanh { get; set; }


        [Display(Name = "Nhóm quyền")]
        public List<string> Roles { get; set; }
    }

    public class ViewUserKD
    {

        //Thông tin tài khoản

        [Display(Name = "Mã tài khoản")]
        public string Id { get; set; }
        [Display(Name = "Hòm thư")]
        public string Email { get; set; }
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Tài khoản đăng nhập")]
        public string UserName { get; set; }
        public string MA_DVIQLY { get; set; }
        public string MA_PBAN { get; set; }
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }
        [Display(Name = "Chức danh")]
        public string ChucDanh { get; set; }


        [Display(Name = "Nhóm quyền")]
        public string Roles { get; set; }
    }

    public class InsertUser
    {
        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(100, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.", MinimumLength = 3)]
        [Display(Name = "Tài khoản đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [EmailAddress]
        [Display(Name = "Hòm thư điện tử")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(100, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu mới nhập không trùng với mật khẩu đã nhập.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Danh sách quyền")]
        public List<CheckBoxItem> RolesSys { get; set; }
        public List<CheckBoxItem> RolesFunc { get; set; }

        [StringLength(10, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Mã đơn vị quản lý")]
        public string MA_DVIQLY { get; set; }

        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Mã phòng ban")]
        public string MA_PBAN { get; set; }

        [StringLength(256)]
        [Display(Name = "Chức danh")]
        public string ChucDanh { get; set; }
    }

    public class EditUser
    {
        public EditUser()
        {
            RolesSys = new List<CheckBoxItem>();
            RolesFunc = new List<CheckBoxItem>();
        }

        //Thông tin các nhân
        [Key]
        [Display(Name = "Mã tài khoản")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [EmailAddress]
        [Display(Name = "Hòm thư điện tử")]
        public string Email { get; set; }

        public int? CustomerId { get; set; }

        public List<CheckBoxItem> RolesSys { get; set; }
        public List<CheckBoxItem> RolesFunc { get; set; }

        [StringLength(10, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Mã đơn vị quản lý")]
        public string MA_DVIQLY { get; set; }

        [StringLength(50, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.")]
        [Display(Name = "Mã phòng ban")]
        public string MA_PBAN { get; set; }
        [StringLength(256)]
        [Display(Name = "Chức danh")]
        public string ChucDanh { get; set; }
    }

    public class ChangePassword
    {
        [Display(Name = "Mã tài khoản")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Không được để trống!")]
        [StringLength(100, ErrorMessage = "Chuỗi {0} không được nhỏ hơn {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới nhập không trùng với mật khẩu đã nhập.")]
        public string ConfirmPassword { get; set; }
    }
}