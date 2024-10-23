using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{

    [Authorize]
    public class PhieuCongTacController : Controller
    {
        PhieuCongTacRepository phieuCongTacRepository = new PhieuCongTacRepository();
        DonViRepository donViRepository = new DonViRepository();
        PhienLVRepository phienLVRepository = new PhienLVRepository();
        PhongBanRepository phongBanRepository = new PhongBanRepository();
        LoaiPhieuRepository loaiPhieuRepository = new LoaiPhieuRepository();
        TrangThaiPhieuRepository trangThaiPhieuRepository = new TrangThaiPhieuRepository();

        private void DisposeAll()
        {
            if (phieuCongTacRepository != null)
            {
                phieuCongTacRepository.Dispose();
                phieuCongTacRepository = null;
            }

            if (donViRepository != null)
            {
                donViRepository.Dispose();
                donViRepository = null;
            }

            if (phienLVRepository != null)
            {
                phienLVRepository.Dispose();
                phienLVRepository = null;
            }

            if (phongBanRepository != null)
            {
                phongBanRepository.Dispose();
                phongBanRepository = null;
            }

            if (loaiPhieuRepository != null)
            {
                loaiPhieuRepository.Dispose();
                loaiPhieuRepository = null;
            }

            if (trangThaiPhieuRepository != null)
            {
                trangThaiPhieuRepository.Dispose();
                trangThaiPhieuRepository = null;
            }
        }

        // GET: Admin/PhieuCongTac
        [HasCredential(MenuCode = "PHIEUCONGTAC")]
        public ActionResult Index()
        {
            ViewBag.LoaiPhieu = new SelectList(loaiPhieuRepository.List().OrderBy(x => x.MaLP), "MaLP", "TenLP");
            ViewBag.TrangThaiPhieu = new SelectList(trangThaiPhieuRepository.List().OrderBy(x => x.MaTT), "MaTT", "TenTT");
            ViewBag.DonViID = Session["DonViID"].ToString();

            DisposeAll();

            return View();
        }

        public ActionResult List(int page, int pageSize = 10, string sortOrder = "", string filter = "", int MaLoaiPhieu = 0, int MaTrangThai = 0, string DonViId = "")
        {
            List<plv_PhieuCongTac> model = new List<plv_PhieuCongTac>();
            List<plv_PhieuCongTac> phieuCongTacList = new List<plv_PhieuCongTac>();

            phieuCongTacList = phieuCongTacRepository.List();

            if (!String.IsNullOrEmpty(filter))
            {
                phieuCongTacList = phieuCongTacList.Where(s => s.NoiDung.ToLower().Contains(filter.ToLower()))
                                        .ToList();
            }

            if (!string.IsNullOrEmpty(DonViId))
            {
                var lstDviCon = donViRepository.List().Where(p => p.DviCha == DonViId).ToList().Select(x => x.Id);
                var phienLVList = phienLVRepository.List().Join(phongBanRepository.List().Where(s => s.MaDVi == DonViId || lstDviCon.Contains(s.MaDVi)), x => x.PhongBanID, y => y.Id, (x, y) => x).ToList();
                phieuCongTacList = phieuCongTacList.Join(phienLVList, x => x.ID, y => y.MaPCT, (x, y) => x).ToList();
            }

            if (MaLoaiPhieu > 0)
            {
                phieuCongTacList = phieuCongTacList.Where(x => x.MaLP == MaLoaiPhieu).ToList();
            }

            if (MaTrangThai > 0)
            {
                phieuCongTacList = phieuCongTacList.Where(x => x.MaTT == MaTrangThai).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    phieuCongTacList = phieuCongTacList.OrderByDescending(s => s.ID).ToList();
                    break;
                case "name_asc":
                    {
                        phieuCongTacList = phieuCongTacList.OrderBy(s => s.ID).ToList();
                        break;
                    }
                default:
                    phieuCongTacList = phieuCongTacList.OrderByDescending(s => s.ID).ToList();
                    break;
            }

            model = phieuCongTacList.Skip(pageSize * (page - 1)).Take(pageSize).ToList();

            //IBaseConverter<tblNhanVien, NhanVienModel> convtResult = new AutoMapConverter<tblNhanVien, NhanVienModel>();
            //var convtList = convtResult.ConvertObjectCollection(rtnList);

            //foreach (var item in convtList)
            //{
            //    item.RoleId = idenM.GetRoleOfUser(item.Id).FirstOrDefault();
            //}

            //if (!String.IsNullOrEmpty(roleId))
            //{
            //    var role = idenM.GetRoleById(roleId);
            //    convtList = convtList.Where(s => s.RoleId.ToLower() == role.Name.ToLower()).ToList();
            //}

            var ListNewsPageSize = new PagedData<plv_PhieuCongTac>();
            ListNewsPageSize.RecordsName = "Phiếu công tác";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model;
                //if (ListNewsPageSize.Data.Count() == 0)
                //{
                //    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                //}
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)phieuCongTacList.Count() / pageSize));
                ListNewsPageSize.RecordsPerPage = pageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = phieuCongTacList.Count();
            }
            else
            {
                ListNewsPageSize.Data = new List<plv_PhieuCongTac>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            ViewBag.DonViID = DonViId;

            DisposeAll();

            return PartialView("~/Areas/Admin/Views/PhieuCongTac/_List.cshtml", ListNewsPageSize);
        }

        [HttpPost]
        public JsonResult UpdateSoPhieu(int id, string soPhieu)
        {
            try
            {
                if (id > 0 && !string.IsNullOrEmpty(soPhieu))
                {
                    var phieu = phieuCongTacRepository.GetById(id);
                    phieu.SoPhieu = soPhieu;
                    phieu.MaTT = 2;

                    string error = "";
                    phieuCongTacRepository.Update(phieu, ref error);

                    DisposeAll();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateDongPhieu(int id)
        {
            try
            {
                if (id > 0)
                {
                    var phieu = phieuCongTacRepository.GetById(id);
                    phieu.MaTT = 3;

                    string error = "";
                    phieuCongTacRepository.Update(phieu, ref error);

                    DisposeAll();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}