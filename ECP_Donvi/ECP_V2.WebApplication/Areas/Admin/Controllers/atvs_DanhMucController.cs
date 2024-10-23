using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class atvs_DanhMucController : Controller
    {
        // GET: Admin/atvs_DanhMuc
        private atvs_DanhMucRepository _danhmucRepository = new atvs_DanhMucRepository();

        private void DisposeAll()
        {
            if (_danhmucRepository != null)
            {
                _danhmucRepository.Dispose();
                _danhmucRepository = null;
            }
        }

        public atvs_DanhMucController()
        {

        }

        // GET: Admin/New
        [HasCredential(MenuCode = "ATVS_DANHMUC")]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetById(int id)
        {
            var model = _danhmucRepository.GetById(id);

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllParent()
        {
            var model = _danhmucRepository.GetAllParent();

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllPaging(int? type, string keyword, int page, int pageSize)
        {
            var model = _danhmucRepository.GetAllPaging(type, keyword, page, pageSize);

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveEntity(atvs_DanhMuc moduleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                DisposeAll();
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (moduleVm.Id == 0)
                    {
                        moduleVm.NgayTao = DateTime.Now;
                        moduleVm.NguoiTao = User.Identity.Name;
                        moduleVm.TrangThai = 1;
                        _danhmucRepository.Add(moduleVm);
                        DisposeAll();
                        return Json(new
                        {
                            status = true,
                            message = "Thêm thành công"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        moduleVm.NguoiSua = User.Identity.Name;
                        moduleVm.NgaySua = DateTime.Now;
                        _danhmucRepository.Update(moduleVm);
                        DisposeAll();
                        return Json(new
                        {
                            status = true,
                            message = "Cập nhật thành công"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    DisposeAll();
                    //_logger.LogError(ex.Message);
                    return Json(new
                    {
                        status = false,
                        message = "Có lỗi xảy ra"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                DisposeAll();
                return Json(new
                {
                    status = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _danhmucRepository.Delete(id, User.Identity.Name);

                DisposeAll();
                return Json(new
                {
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}