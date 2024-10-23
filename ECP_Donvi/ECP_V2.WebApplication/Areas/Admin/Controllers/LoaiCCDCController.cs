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
    public class LoaiCCDCController : Controller
    {
        private HangSanXuatRepository _HangSX_ser = new HangSanXuatRepository();
        private NuocSanXuatRepository _NuocSX_ser = new NuocSanXuatRepository();
        private LoaiThietBiRepository _LoaiThietBi_ser = new LoaiThietBiRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);

        private void DisposeAll()
        {
            if (_HangSX_ser != null)
            {
                _HangSX_ser.Dispose();
                _HangSX_ser = null;
            }

            if (_NuocSX_ser != null)
            {
                _NuocSX_ser.Dispose();
                _NuocSX_ser = null;
            }


        }

        // GET: Admin/CCDC
        public ActionResult Index(string MaNhom)
        {
            if (string.IsNullOrEmpty(MaNhom))
                ViewBag.MaNhom = "1";
            else
                ViewBag.MaNhom = MaNhom;

            DisposeAll();
            return View();
        }

        public ActionResult Create(int? MaNhom)
        {
            try
            {               
                CreateDropHangSX(null);
                CreateDropNuocSX(null);

                LoaiCCDCViewModels Model = new LoaiCCDCViewModels();
                Model.MaNhom = MaNhom ?? 1;

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
        public ActionResult Create(LoaiCCDCViewModels model)
        {
            string kt = "|";
            CreateDropHangSX(null);
            CreateDropNuocSX(null);

            try
            {

                LoaiThietBi obj = new LoaiThietBi();
                obj.TenLoai = model.TenLoai;
                obj.MaHSX = model.MaHSX;
                obj.MaNSX = model.MaNSX;
                obj.NamSX = model.NamSX;
                obj.NgayDuaVaoSuDung = model.NgayDuaVaoSuDung;
                obj.HanKiemDinh = model.HanKiemDinh;
                obj.MaNhom = model.MaNhom;
                obj.QuyTacDanhMa = model.QuyTacDanhMa;

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
                NLoger.Error("loggerDatabase", string.Format("Lỗi tạo công cụ dụng cụ. Chi tiết: {0}", ex.Message));
            }
            DisposeAll();
            return Json(kt, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List(int page, int pageSize, string filter, string DateFrom, string DateTo,
            string DonViId, string PhongBanId, string MaNhom)
        {
            #region Check null
            if (string.IsNullOrEmpty(filter))
                filter = "";
            if (string.IsNullOrEmpty(DonViId))
                DonViId = "";
            if (string.IsNullOrEmpty(PhongBanId))
                PhongBanId = "";
            #endregion

            int Count = 0;




            List<LoaiThietBiModel> model;
            model = _LoaiThietBi_ser.ListPaging(page, pageSize, filter, DateFrom, DateTo, DonViId, PhongBanId, MaNhom).ToList();
            Count = _LoaiThietBi_ser.CountListPaging(filter, DateFrom, DateTo, DonViId, PhongBanId, MaNhom);

            var ListNewsPageSize = new PageData<LoaiThietBiModel>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Loại công cụ dụng cụ an toàn",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };
            }
            else
            {
                ListNewsPageSize.Data = new List<LoaiThietBiModel>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Loại công cụ dụng cụ an toàn",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }

            DisposeAll();
            return PartialView("List", ListNewsPageSize);
        }

        private void CreateDropHangSX(object selectedItem)
        {

            var lst = _HangSX_ser.List();
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "Name", selectedItem);
                    ViewBag.HangSX = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "Name");
                    ViewBag.HangSX = Model;
                }

            }
            else
            {
                ViewBag.HangSX = null;
            }
        }

        private void CreateDropNuocSX(object selectedItem)
        {

            var lst = _NuocSX_ser.List();
            if (lst.Count > 0)
            {
                if (selectedItem != null)
                {
                    var Model = new SelectList(lst, "ID", "Name", selectedItem);
                    ViewBag.NuocSX = Model;
                }
                else
                {
                    var Model = new SelectList(lst, "ID", "Name");
                    ViewBag.NuocSX = Model;
                }

            }
            else
            {
                ViewBag.NuocSX = null;
            }
        }

        #region CmbPhongBan
        [HttpGet]
        public ActionResult CmbPhongBan(string DonViId)
        {
            try
            {
                ViewBag.PhongBan = PhongBanRepository.GetPhongBanByDonViIDHtml(DonViId, 0);
                DisposeAll();
                return View();
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không lấy được danh sách phòng ban", User.Identity.Name, ex.Message));
                return View();
            }

        }
        #endregion

        public ActionResult Edit(int id)
        {           
            LoaiCCDCViewModels Model = null;
            try
            {
                var obj = _LoaiThietBi_ser.GetById(id);

                CreateDropHangSX(obj.MaHSX);
                CreateDropNuocSX(obj.MaNSX);

                Model = new LoaiCCDCViewModels();
                Model.ID = obj.ID;
                Model.TenLoai = obj.TenLoai;
                Model.MaHSX = obj.MaHSX;
                Model.MaNSX = obj.MaNSX;
                Model.NamSX = obj.NamSX;
                Model.NgayDuaVaoSuDung = obj.NgayDuaVaoSuDung;
                Model.NgayTao = obj.NgayTao;
                Model.NguoiTao = obj.NguoiTao;
                Model.NgaySua = obj.NgaySua;
                Model.NguoiSua = obj.NguoiSua;
                Model.MaNhom = obj.MaNhom;
                Model.HanKiemDinh = obj.HanKiemDinh;
                Model.QuyTacDanhMa = obj.QuyTacDanhMa;

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
        public ActionResult Edit(LoaiCCDCViewModels model)
        {
            string kt = "|";
            try
            {

                if (ModelState.IsValid)
                {

                    var obj = _LoaiThietBi_ser.GetById(model.ID);

                    CreateDropHangSX(obj.MaHSX);
                    CreateDropNuocSX(obj.MaNSX);

                    obj.ID = model.ID;
                    obj.TenLoai = model.TenLoai;
                    obj.MaHSX = model.MaHSX;
                    obj.MaNSX = model.MaNSX;
                    obj.NamSX = model.NamSX;
                    obj.NgayDuaVaoSuDung = model.NgayDuaVaoSuDung;
                    obj.HanKiemDinh = model.HanKiemDinh;
                    obj.MaNhom = model.MaNhom;
                    obj.QuyTacDanhMa = model.QuyTacDanhMa;

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