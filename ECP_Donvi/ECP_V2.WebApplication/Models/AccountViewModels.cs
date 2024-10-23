using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECP_V2.WebApplication.Models
{
    public class ExternalLoginConfirmationViewModel
    {        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Ghi nhớ?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        [EmailAddress]
        public string Email { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {        
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Email { get; set; }

        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Email { get; set; }

        
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Nhập giống mật khẩu bên trên.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Email { get; set; }
    }

    public class AdminViewModel
    {
        
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string UserName { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được trống")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ?")]
        public bool RememberMe { get; set; }
    }
}
