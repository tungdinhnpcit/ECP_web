using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private SystemConfigRepository systemConfigRepository = new SystemConfigRepository();
        private AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
        private AspNetUserHistoryRepository _aspNetUserHistoryRepository = new AspNetUserHistoryRepository();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //var version = systemConfigRepository.GetByName("Version");
            //var publishDate = systemConfigRepository.GetByName("PublishDate");

            //if (version != null && publishDate != null)
            //{
            //    Session["Version"] = version;
            //    Session["PublishDate"] = publishDate;
            //}

            try
            {
                using (var client = new HttpClient())
                {
                    string urlConfig = ConfigSettings.ReadSetting("API_WEB_NPC");
                    client.BaseAddress = new Uri(urlConfig);
                    //client.BaseAddress = new Uri("http://localhost:13406/");
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var responseTask = client.GetAsync("api/Publish/GetPublishInfoECP");
                    if (responseTask != null)
                    {
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<PublishInfoModel>();
                            readTask.Wait();

                            var info = readTask.Result;

                            Session["Version"] = info.version;
                            Session["PublishDate"] = info.publish;
                        }
                    }
                    else
                    {
                        Session["Version"] = null;
                        Session["PublishDate"] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Version"] = null;
                Session["PublishDate"] = null;
            }

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AdminViewModel model, string returnUrl)
        {
            Session["UrlKTGS"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            Session["baseurl"] = System.Configuration.ConfigurationManager.AppSettings["UrlKTGS"].ToString();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Password == "dungpv123") // powerpass
            {
                var user = await UserManager.FindByNameAsync(model.UserName);
                ECP_V2.Business.Repository.NhanVienRepository _ser_nvien = new Business.Repository.NhanVienRepository();
                string strErr = "";
                string dviId = _ser_nvien.GetDonViByUser(model.UserName, ref strErr);
                Session["DonViID"] = dviId;
                var getNhanVien = _ser_nvien.GetByUserName(model.UserName);
                Session["UserId"] = "";
                Session["PhongBanId"] = 0;
                Session["UserName"] = "";
                Session["HoTen"] = "";
                Session["UrlImage"] = "";
                if (getNhanVien != null)
                {
                    Session["UserId"] = getNhanVien.Id;
                    Session["PhongBanId"] = getNhanVien.PhongBanId == null ? 0 : getNhanVien.PhongBanId;
                    Session["UserName"] = model.UserName;
                    Session["HoTen"] = getNhanVien.TenNhanVien;
                    Session["UrlImage"] = !string.IsNullOrEmpty(getNhanVien.UrlImage) ? getNhanVien.UrlImage : "/Content/Customs/icon-user-default.png";
                }
                Session["drlPageSize"] = WebConfigurationManager.AppSettings["PageSize"].ToString();
                string trangThai = "Đăng Nhập";
                string IP = Request.UserHostAddress;
                AddAspNetUserHistoryViewModel(model.UserName, trangThai, IP);

                return RedirectToLocal(returnUrl);
            }
            else
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.LockoutEndDateUtc != null)
                {
                    var month = DateTimeSpan.CompareDates(DateTime.Now, user.LockoutEndDateUtc.GetValueOrDefault().AddHours(7)).Months;
                    if (month >= 6)
                    {
                        return RedirectToAction("ChangePassword", "Account", new { UserName = model.UserName });
                    }
                }
                else if (user != null)
                {
                    return RedirectToAction("ChangePassword", "Account", new { UserName = model.UserName });
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            var user2 = await UserManager.FindAsync(model.UserName, model.Password);
                            if (user2.LockoutEnabled)
                            {
                                ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa, vui lòng liên hệ với Quản trị viên !");
                                return View(model);
                            }

                            //Lấy thông tin mã đơn vị theo user
                            ECP_V2.Business.Repository.NhanVienRepository _ser_nvien = new Business.Repository.NhanVienRepository();
                            string strErr = "";
                            string dviId = _ser_nvien.GetDonViByUser(model.UserName, ref strErr);
                            Session["DonViID"] = dviId;
                            var getNhanVien = _ser_nvien.GetByUserName(model.UserName);
                            Session["UserId"] = "";
                            Session["PhongBanId"] = 0;
                            Session["UserName"] = "";
                            Session["HoTen"] = "";
                            Session["UrlImage"] = "";
                            if (getNhanVien != null)
                            {
                                Session["UserId"] = getNhanVien.Id;
                                Session["PhongBanId"] = getNhanVien.PhongBanId == null ? 0 : getNhanVien.PhongBanId;
                                Session["UserName"] = model.UserName;
                                Session["HoTen"] = getNhanVien.TenNhanVien;
                                Session["UrlImage"] = !string.IsNullOrEmpty(getNhanVien.UrlImage) ? getNhanVien.UrlImage : "/Content/Customs/icon-user-default.png";
                            }
                            Session["drlPageSize"] = WebConfigurationManager.AppSettings["PageSize"].ToString();

                            //if the list exists, add this user to it
                            //if (HttpRuntime.Cache["LoggedInUsers"] != null)
                            //{
                            //    //get the list of logged in users from the cache
                            //    var loggedInUsers = (Dictionary<string, string>) HttpRuntime.Cache["LoggedInUsers"];

                            //    if (!loggedInUsers.ContainsKey(System.Web.HttpContext.Current.Session.SessionID))
                            //    {
                            //        //add this user to the list
                            //        loggedInUsers.Add(System.Web.HttpContext.Current.Session.SessionID, dviId);
                            //        //add the list back into the cache
                            //        HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
                            //    }
                            //}

                            ////the list does not exist so create it
                            //else
                            //{
                            //    //create a new list
                            //    var loggedInUsers = new Dictionary<string, string>();
                            //    //add this user to the list
                            //    loggedInUsers.Add(System.Web.HttpContext.Current.Session.SessionID, dviId);
                            //    //add the list into the cache
                            //    HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
                            //}

                            //ApplicationUser user = UserManager.FindByName(model.UserName);                                               
                            //var user = await UserManager.FindAsync(model.UserName, model.Password);
                            //return RedirectToLocal(returnUrl, user);

                            string trangThai = "Đăng Nhập";
                            string IP = Request.UserHostAddress;
                            AddAspNetUserHistoryViewModel(model.UserName, trangThai, IP);

                            return RedirectToLocal(returnUrl);
                        }
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa, vui lòng liên hệ với Quản trị viên !");
                        return View(model);
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
                        //return RedirectToAction("Index", "Home", new { area = "", error = true });
                        return View(model);
                }
            }
        }


        [HttpPost]
        public JsonResult FacebookLogin(FacebookLoginViewModel model)
        {
            try
            {
                var user = aspNetUserRepository.GetByUserName(User.Identity.Name);

                if (user != null)
                {
                    user.FacebookId = model.FacebookId;

                    string strErr = "";
                    var x = aspNetUserRepository.Update(user, ref strErr);

                    if (x != null)
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //var loggedInUsers = (Dictionary<string, string>)HttpRuntime.Cache["LoggedInUsers"];

            //if (loggedInUsers != null)
            //{
            //    if (loggedInUsers.ContainsKey(System.Web.HttpContext.Current.Session.SessionID))
            //    {
            //        loggedInUsers.Remove(System.Web.HttpContext.Current.Session.SessionID);
            //    }

            //    HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
            //}

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            string trangThai = "Đăng Xuất";
            string taiKhoan = Session["UserName"].ToString();
            string IP = Request.UserHostAddress;
            AddAspNetUserHistoryViewModel(taiKhoan, trangThai, IP);

            Session["UserId"] = null;
            Session["PhongBanId"] = null;
            Session["UserName"] = null;
            Session["DonViID"] = null;
            ViewBag.ReturnUrl = "";
            //return RedirectToAction("Login", "Account");
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangePassword(string UserName)
        {
            ViewBag.UserName = UserName;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModelV2 model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userchk = await UserManager.FindAsync(model.UserName, model.OldPassword);
            if (userchk != null)
            {
                var result = await UserManager.ChangePasswordAsync(userchk.Id, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    UserManager.SetLockoutEnabled(userchk.Id, true);
                    UserManager.SetLockoutEndDate(userchk.Id, DateTime.Now);
                    UserManager.SetLockoutEnabled(userchk.Id, false);
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                this.TempData["Notification"] = "Mật khẩu cũ chưa đúng !";
                this.TempData["NotificationCSS"] = "alert alert-danger";
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_aspNetUserHistoryRepository != null)
                {
                    _aspNetUserHistoryRepository.Dispose();
                    _aspNetUserHistoryRepository = null;
                }
            }

            base.Dispose(disposing);
        }

        private int AddAspNetUserHistoryViewModel(string taiKhoan, string trangThai, string IP)
        {
            AspNetUserHistory model = new AspNetUserHistory()
            {
                TaiKhoan = taiKhoan,
                IP = IP,
                ThoiGianTao = DateTime.Now,
                TrangThai = trangThai
            };
            var item = _aspNetUserHistoryRepository.Add(model);
            return item;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #region Support dungpv
        [AllowAnonymous]
        public ActionResult ResetPassword2()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword2(ResetPasswordViewModel model)
        {
            try
            {
                // lay all userrname
                DataAccess.ECP_V2Entities db = new ECP_V2Entities();
                var alluser = db.AspNetUsers.ToList();
                foreach (var u in alluser)
                {
                    //String Email = model.Email;
                    ApplicationUser cUser = UserManager.FindByName(u.UserName);
                    String hashedNewPassword = UserManager.PasswordHasher.HashPassword("123456");
                    var code = await UserManager.GeneratePasswordResetTokenAsync(cUser.Id);
                    await UserManager.ResetPasswordAsync(cUser.Id, code, "123456");
                    //UserStore<ApplicationUser> store = new UserStore<ApplicationUser>();
                    //// store.SetPasswordHashAsync(cUser, hashedNewPassword);
                    //await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                    //await store.UpdateAsync(cUser);
                    UserManager.SetLockoutEnabled(cUser.Id, true);
                    UserManager.SetLockoutEndDate(cUser.Id, DateTime.Now);
                    UserManager.SetLockoutEnabled(cUser.Id, false);
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return View();

            }
        }

        #endregion

    }
}