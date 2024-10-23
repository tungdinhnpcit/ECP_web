using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class sc_ThietBiSuCoController : Controller
    {
        private readonly sc_ThietBiSuCoRepository _thietbiRepository = new sc_ThietBiSuCoRepository();
        private readonly sc_DonViTinhRepository _donvitinhRepository = new sc_DonViTinhRepository();
        private readonly sc_MaHieuVatTuRepository _mahieuRepository = new sc_MaHieuVatTuRepository();
        private readonly sc_NhaSanXuatRepository _nhasxRepository = new sc_NhaSanXuatRepository();


        // GET: Admin/vsld_QuanTrac
        //[HasCredential(MenuCode = "SC_TBSC")]
        public ActionResult Index(int scId)
        {
            if (scId < 0)
                return Redirect("/Admin/SuCo");
            ViewBag.SuCoId = scId;
            return View();
        }


        public ActionResult GetById(int id)
        {
            var model = _thietbiRepository.GetById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDonViTinh()
        {
            var model = _donvitinhRepository.GetAll();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult GetAllMaHieu()
        {
            var model = _mahieuRepository.GetAll();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult GetAllNhaSX()
        {
            var model = _nhasxRepository.GetAll();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPaging(int scId, string keyword, int page, int pageSize)
        {
            var model = _thietbiRepository.GetAllPaging(scId, keyword, page, pageSize);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEntity(sc_ThietBiSuCo model, string lydo)
        {
            try
            {
                if (model.Id == 0)
                {
                    model.NgayTao = DateTime.Now;
                    model.NguoiTao = User.Identity.Name;
                    model.TrangThai = 1;

                    var newM = _thietbiRepository.Add(model);

                    return Json(new
                    {
                        status = true,
                        message = "Lưu thành công"
                    });
                }
                else
                {
                    model.NgaySua = DateTime.Now;
                    model.NguoiSua = User.Identity.Name;

                    decimal d = 0;
                    _thietbiRepository.Update(model);

                    return Json(new
                    {
                        status = true,
                        message = "Lưu thành công"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }

        [HttpPost]
        public ActionResult DeleteEntity(int id)
        {
            try
            {

                var model = _thietbiRepository.Delete(id, User.Identity.Name);

                if (model != null)
                {
                    return Json(new
                    {
                        status = true,
                        message = "Xóa thành công"
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        message = "Có lỗi xảy ra"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }


    }
}