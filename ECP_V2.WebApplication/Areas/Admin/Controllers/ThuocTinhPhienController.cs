using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
    public class ThuocTinhPhienController : UTController
    {
        private ThuocTinhPhienRepository thuocTinhPhienRepository = new ThuocTinhPhienRepository();

        private void DisposeAll()
        {
            if (thuocTinhPhienRepository != null)
            {
                thuocTinhPhienRepository.Dispose();
                thuocTinhPhienRepository = null;
            }
        }

        // GET: Admin/ThuocTinhPhien
        public ActionResult Index()
        {
            DisposeAll();

            return View();
        }

        #region List
        [HttpGet]
        public ActionResult List(int page, int pageSize, string filter)
        {
            int Count = 0;

            List<plv_ThuocTinhPhien> model = new List<plv_ThuocTinhPhien>();

            model = thuocTinhPhienRepository.List().Where(x => x.LoaiThuocTinh == 5 && (x.TenThuocTinh.ToUpper().Contains(filter.ToUpper()) || string.IsNullOrEmpty(filter)))
                                    .OrderByDescending(x => x.NgayTao)
                                    .ToList();
            Count = model.Count;

            var ListNewsPageSize = new PagedData<plv_ThuocTinhPhien>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ListNewsPageSize.RecordsName = "Thuộc tính";
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize));
                ListNewsPageSize.RecordsPerPage = pageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = Count;

            }
            else
            {
                ListNewsPageSize.Data = new List<plv_ThuocTinhPhien>();
                ListNewsPageSize.RecordsName = "Thuộc tính";
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            DisposeAll();

            return PartialView("_List", ListNewsPageSize);
        }
        #endregion

        public ActionResult Add()
        {
            List<SelectListItem> trangThaiList = new List<SelectListItem>();

            SelectListItem item = new SelectListItem()
            {
                Value = "0",
                Text = "Tạm dừng"
            };

            trangThaiList.Add(item);

            item = new SelectListItem()
            {
                Value = "1",
                Text = "Kích hoạt"
            };

            trangThaiList.Add(item);

            ViewBag.TrangThai = trangThaiList;

            DisposeAll();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(plv_ThuocTinhPhien model, int TrangThai)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TenThuocTinh))
                {
                    return JsonError("Nhập tên thuộc tính.");
                }

                model.LoaiThuocTinh = 5;
                model.NgayTao = DateTime.Now;
                model.NguoiTao = User.Identity.Name;

                if (TrangThai == 0)
                {
                    model.TrangThai = false;
                }
                else
                {
                    model.TrangThai = true;
                }

                string strError = "";

                object x = thuocTinhPhienRepository.Create(model, ref strError);

                if (x == null)
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + strError);
                    return JsonError("Không thêm được bản ghi: " + strError);
                }
                else
                {
                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} thêm đơn vị {1} thành công", User.Identity.Name, eFalculty.TenDonVi));
                    return JsonSuccess(Url.Action("Index"), "Thêm bản ghi thành công!");
                }

            }
            catch (Exception ex)
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không thêm được bản ghi:" + ex.Message);
                return JsonError("Không thêm được bản ghi: " + ex.Message);
            }
        }

        public ActionResult Edit(int id)
        {
            var thuocTinhPhien = thuocTinhPhienRepository.GetById(id);

            List<SelectListItem> trangThaiList = new List<SelectListItem>();

            SelectListItem item = new SelectListItem()
            {
                Value = "0",
                Text = "Tạm dừng",
                Selected = (thuocTinhPhien.TrangThai == false) ? true : false
            };

            trangThaiList.Add(item);

            item = new SelectListItem()
            {
                Value = "1",
                Text = "Kích hoạt",
                Selected = (thuocTinhPhien.TrangThai == true) ? true : false
            };

            trangThaiList.Add(item);

            ViewBag.TrangThai = trangThaiList;

            DisposeAll();

            return View(thuocTinhPhien);
        }

        [HttpPost]
        public ActionResult Update(plv_ThuocTinhPhien model, int TrangThai)
        {
            if (string.IsNullOrEmpty(model.TenThuocTinh))
            {
                return JsonError("Nhập tên thuộc tính phiên.");
            }

            if (TrangThai == 0)
            {
                model.TrangThai = false;
            }
            else
            {
                model.TrangThai = true;
            }

            model.LoaiThuocTinh = 5;
            model.NgayTao = DateTime.Now;
            model.NguoiTao = User.Identity.Name;

            string strError = "";
            object x = thuocTinhPhienRepository.Update(model, ref strError);
            if (x == null || !String.IsNullOrEmpty(strError))
            {
                DisposeAll();

                NLoger.Error("loggerDatabase", "Không sửa được bản ghi:" + strError);
                return JsonError("Không sửa được bản ghi: " + strError);
            }
            else
            {
                DisposeAll();

                //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} sửa đơn vị {1} thành công", User.Identity.Name, eFalculty.TenDonVi));
                return JsonSuccess(Url.Action("Index"), "Thêm dữ liệu thành công!");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string strError = "";
            try
            {
                var x = thuocTinhPhienRepository.Delete(id, ref strError);
                if (x == "error")
                {
                    DisposeAll();

                    NLoger.Error("loggerDatabase", string.Format("Không xóa được thuộc tính phiên: {0}. Chi tiết: {1}", id, strError));
                    return Json(new { success = false, responseText = "Không xóa được thuộc tính phiên này do đã có dữ liệu! Liên hệ quản trị viên." }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    DisposeAll();

                    //NLoger.Info("loggerDatabase", string.Format("Tài khoản {0} xóa đơn vị {1} khỏi hệ thống", User.Identity.Name, id));                        
                    return Json(new { success = true, responseText = "Xóa thành công!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                DisposeAll();
                NLoger.Error("loggerDatabase", string.Format("Không xóa được thuộc tính phiên: {0}. Chi tiết: {1}", id, ex.Message));
                return Json(new { success = false, responseText = "Không xóa được thuộc tính phiên!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}