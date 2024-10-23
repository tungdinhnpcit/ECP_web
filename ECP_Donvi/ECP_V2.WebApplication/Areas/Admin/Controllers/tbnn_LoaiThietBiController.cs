using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Areas.Admin.Models;
using ECP_V2.WebApplication.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class tbnn_LoaiThietBiController : Controller
    {
        private tbnn_LoaiThietBiRepository _LoaiThietBi_ser = new tbnn_LoaiThietBiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        private void DisposeAll()
        {
            if (_LoaiThietBi_ser != null)
            {
                _LoaiThietBi_ser.Dispose();
                _LoaiThietBi_ser = null;
            }

        }

        // GET: Admin/CCDC
        public ActionResult Index()
        {
            DisposeAll();
            return View();
        }

        public ActionResult Create()
        {
            try
            { 
                tbnn_LoaiThietBiViewModels Model = new tbnn_LoaiThietBiViewModels();
                DisposeAll();
                return View(Model);
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
        }

        [HttpPost]
        public ActionResult Create(tbnn_LoaiThietBiViewModels model)
        {
            string kt = "|";
            try
            {

                tbnn_LoaiThietBi obj = new tbnn_LoaiThietBi();
                obj.TenLoai = model.TenLoai;

                obj.NgayTao = DateTime.Now;
                obj.NguoiTao = User.Identity.Name;

                string strError = "";
                object x = _LoaiThietBi_ser.Create(obj, ref strError);
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
                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo tên thiết bị nghiêm ngặt. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int page, int pageSize, string filter)
        {
            #region Check null
            if (string.IsNullOrEmpty(filter))
                filter = "";
            #endregion

            int Count = 0;




            List<tbnn_LoaiThietBiModel> model;
            model = _LoaiThietBi_ser.ListPaging(page, pageSize, filter).ToList();
            Count = _LoaiThietBi_ser.CountListPaging(filter);

            var ListNewsPageSize = new PageData<tbnn_LoaiThietBiModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Tên thiết bị nghiêm ngặt",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };
            }
            else
            {
                ListNewsPageSize.Data = new List<tbnn_LoaiThietBiModel>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Tên thiết bị nghiêm ngặt",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }

            DisposeAll();
            return PartialView("List", ListNewsPageSize);
        }

        public ActionResult Edit(int id)
        {
            tbnn_LoaiThietBiViewModels Model = null;
            try
            {
                var obj = _LoaiThietBi_ser.GetById(id);

                Model = new tbnn_LoaiThietBiViewModels();
                Model.ID = obj.ID;
                Model.TenLoai = obj.TenLoai;
                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;
                Model.NgaySua = obj.NgaySua;
                Model.NguoiSua = obj.NguoiSua;

                DisposeAll();
            }
            catch (Exception)
            {
                DisposeAll();
                throw;
            }
            return View(Model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbnn_LoaiThietBiViewModels model)
        {
            string kt = "|";
            try
            {

                if (ModelState.IsValid)
                {

                    var obj = _LoaiThietBi_ser.GetById(model.ID);

                    obj.ID = model.ID;
                    obj.TenLoai = model.TenLoai;

                    obj.NgaySua = DateTime.Now;
                    obj.NguoiSua = User.Identity.Name;

                    string strError = "";
                    object x = _LoaiThietBi_ser.Update(obj, ref strError);
                    if (x == null)
                    {
                    }
                    else
                    {
                        kt = x.ToString() + "|OK";


                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                kt = "|" + ex.Message;
            }
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _LoaiThietBi_ser.Delete(id, ref strError);
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

        public ActionResult DeleteAll(string id)
        {
            string kt = "";
            try
            {
                string strError = "";
                var x = _LoaiThietBi_ser.DeleteAll(id.Split(','), ref strError);
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