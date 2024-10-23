using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    public class ChatController : UTController
    {
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        chatTinNhanRepository _Mess_ser = new chatTinNhanRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        // GET: Messenges
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListFriend(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                filter = "";
            string MaNV = "";
            try
            {
                MaNV = Session["UserId"].ToString();
            }
            catch (Exception ex)
            {
            }

            var model = _kh_ser.List().Where(o => o.Id != MaNV && o.TenNhanVien.Contains(filter)).OrderByDescending(o => o.NgayDangNhap).ToList();

            return PartialView("ListFriend", model);

        }

        [HttpGet]
        public async Task<ActionResult> GetListTinNhanHtml(string MaNhan, int page, int Total)
        {
            int pagesize = 10;
            string MaNV = "";
            try
            {
                MaNV = Session["UserId"].ToString();
            }
            catch (Exception ex)
            {
            }
            int Count = 0;
            string kt = await _Mess_ser.GetListTinNhanHtml(MaNV, MaNhan, page, pagesize);
            Count = await _Mess_ser.CountGetListTinNhanHtml(MaNV, MaNhan, page, pagesize);
            if (Total == Count)
                kt = "";

            return Json(kt, JsonRequestBehavior.AllowGet);
        }
    }
}