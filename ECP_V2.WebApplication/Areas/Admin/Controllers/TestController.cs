using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class TestController : Controller
    {

        private TestRepository _test_ser = new TestRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        // GET: Admin/Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            try
            {
                TestViewModel obj = new TestViewModel();
                return View(obj);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult Create(TestViewModel model)
        {
            string kt = "|";
            string strError = "";
            try
            {
                Test obj = new Test();
                obj.Name = model.Name;

                object x = _test_ser.Create(obj, ref strError);
                if (int.Parse(x.ToString()) == 0)
                {
                }
                else
                {
                    kt = x.ToString() + "|OK";
                }

            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;

            }

            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int page, int pageSize, string filter, string DateFrom, string DateTo,
           string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT)
        {
            #region Check null
            if (string.IsNullOrEmpty(filter))
                filter = "";

            #endregion

            int Count = 0;

            List<Test> model;
            model = _test_ser.ListPaging(page, pageSize, filter).ToList();
            Count = _test_ser.CountListPaging(filter);


            var ListNewsPageSize = new PageData<Test>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Test",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };
            }
            else
            {
                ListNewsPageSize.Data = new List<Test>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Test",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }


            return PartialView("List", ListNewsPageSize);
        }

        public ActionResult Edit(int id)
        {
            TestViewModel Model = null;
            try
            {
                var obj = _test_ser.GetById(id);
                Model = new TestViewModel();
                Model.ID = obj.ID;
                Model.Name = obj.Name;
            }
            catch (Exception)
            {
                throw;
            }
            return View(Model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TestViewModel model)
        {
            string kt = "|";
            try
            {
                if (ModelState.IsValid)
                {
                    var obj = _test_ser.GetById(model.ID);
                    obj.Name = model.Name;

                    string strError = "";
                    object x = _test_ser.Update(obj, ref strError);
                    if (x == null)
                    {
                    }
                    else
                    {
                        kt = x.ToString() + "|OK";
                    }
                }
            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
            }
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAll(string id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _test_ser.DeleteAll(id.Split(','), ref strError);
                if (x == "success")
                {

                    return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không xóa được bản dữ liệu!");
                    kt = "Không xóa được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _test_ser.Delete(id, ref strError);
                if (x == "success")
                {

                    return Json(new { type = "success", mess = "Xóa dữ liệu thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Không xóa được bản dữ liệu!");
                    kt = "Không xóa được bản dữ liệu!";
                    return Json(new { type = "error", mess = kt }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không xóa được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}