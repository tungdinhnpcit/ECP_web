using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class NewMenuController : Controller
    {
        private NewMenuRepository _menuRepository = new NewMenuRepository();

        private void DisposeAll()
        {
            if (_menuRepository != null)
            {
                _menuRepository.Dispose();
                _menuRepository = null;
            }
        }

        public NewMenuController()
        {

        }

        // GET: Admin/New
        [HasCredential(MenuCode = "MENU")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            var news = _menuRepository.GetInfoById(id);
            DisposeAll();
            return View(news);
        }

        [HttpGet]
        public ActionResult GetById(int id)
        {
            var model = _menuRepository.GetInfoById(id);

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll()
        {
            var model = _menuRepository.GetAll();

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllParent()
        {
            var model = _menuRepository.GetAllParent();

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _menuRepository.GetAllPaging(keyword, page, pageSize);

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveEntity(Menu moduleVm)
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
                        if (!_menuRepository.CheckExistCode(moduleVm.Code))
                        {
                            moduleVm.NgayTao = DateTime.Now;
                            moduleVm.NguoiTao = User.Identity.Name;
                            _menuRepository.Add(moduleVm);
                            DisposeAll();
                            return Json(new
                            {
                                status = true,
                                message = "Thêm thành công"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            DisposeAll();
                            return Json(new
                            {
                                status = false,
                                message = "Mã Code đã tồn tại"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var model = _menuRepository.GetById(moduleVm.Id);
                        if (model.Code != moduleVm.Code)
                        {
                            if (!_menuRepository.CheckExistCode(moduleVm.Code))
                            {
                                moduleVm.NguoiSua = User.Identity.Name;
                                moduleVm.NgaySua = DateTime.Now;
                                _menuRepository.Update(moduleVm);
                                DisposeAll();
                                return Json(new
                                {
                                    status = true,
                                    message = "Cập nhật thành công"
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                DisposeAll();
                                return Json(new
                                {
                                    status = false,
                                    message = "Mã Code đã tồn tại"
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            moduleVm.NguoiSua = User.Identity.Name;
                            moduleVm.NgaySua = DateTime.Now;
                            _menuRepository.Update(moduleVm);
                            DisposeAll();
                            return Json(new
                            {
                                status = true,
                                message = "Cập nhật thành công"
                            }, JsonRequestBehavior.AllowGet);
                        }
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
                _menuRepository.Delete(id, User.Identity.Name);

                DisposeAll();
                return Json(new
                {
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}