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
    public class HangSanXuatController : Controller
    {
        private HangSanXuatRepository _HangSanXuatRepository = new HangSanXuatRepository();

        private void DisposeAll()
        {
            if (_HangSanXuatRepository != null)
            {
                _HangSanXuatRepository.Dispose();
                _HangSanXuatRepository = null;
            }
        }

        public HangSanXuatController()
        {

        }

        // GET: Admin/New
        [HasCredential(MenuCode = "HSX")]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetById(int id)
        {
            var model = _HangSanXuatRepository.GetInfoById(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll()
        {
            var model = _HangSanXuatRepository.List();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _HangSanXuatRepository.GetAllPaging(keyword, page, pageSize);

            DisposeAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveEntity(HangSanXuat modelVm)
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
                    if (modelVm.ID == 0)
                    {
                        if (!_HangSanXuatRepository.CheckExistName(modelVm.Name))
                        {
                            _HangSanXuatRepository.Add(modelVm);
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
                                message = "Tên đã tồn tại"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var strErr = "";
                        var model = _HangSanXuatRepository.GetById(modelVm.ID);
                        if (model.Name != modelVm.Name)
                        {
                            if (!_HangSanXuatRepository.CheckExistName(modelVm.Name))
                            {
                                _HangSanXuatRepository.Update(modelVm, ref strErr);
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
                                    message = "Tên đã tồn tại"
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            _HangSanXuatRepository.Update(modelVm, ref strErr);
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
                var strErr = "";
                _HangSanXuatRepository.Delete(id, ref strErr);

                DisposeAll();
                return Json(new
                {
                    status = true
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}