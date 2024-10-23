using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using ECP_V2.Business;
using ECP_V2.Business.Repository;
using ECP_V2.Common;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Mvc;
using ECP_V2.Common.Helpers;
using System.Data.OleDb;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication;

namespace ECP_V2.WebApplication.Controllers
{
    public class EPhienLVController : UTController
    {
        private PhienLVRepository _phienlv_ser = new PhienLVRepository();
        private PhongBanRepository _department_ser = new PhongBanRepository();
        private DonViRepository _faculty_ser = new DonViRepository();
        int currentDonVi = 0;
        int currentPhongBan = 0;
        
        private void DisposeAll()
        {
            if (_phienlv_ser != null)
            {
                _phienlv_ser.Dispose();
                _phienlv_ser = null;
            }

            if (_department_ser != null)
            {
                _department_ser.Dispose();
                _department_ser = null;
            }

            if (_faculty_ser != null)
            {
                _faculty_ser.Dispose();
                _faculty_ser = null;
            }
        }

        public ActionResult Index()
        {
            //NLoger.Info("You have visited the PhienLV");
            currentDonVi = (int)Session["DonViID"];
            currentPhongBan = (int)Session["PhongBanId"];
            //Get CapDvi 
            //tblDonVi donVi = _faculty_ser.GetById(currentDonVi);
            //if(donVi.CapDvi == 0 && donVi.DviCha ==0)
            //{
            //    var donvicon = _faculty_ser.ListByParentId(donVi.Id).OrderBy(p => p.TenDonVi).ToList();
            //    //var listDvi = _faculty_ser.List().OrderBy(p => p.TenDonVi).ToList();
            //    ViewBag.ListDvi = new SelectList(donvicon, "Id", "TenDonVi");


            //    var listPban = _department_ser.List().OrderBy(c => c.TenPhongBan).ToList();
            //    ViewBag.ListPBan = new SelectList(listPban, "Id", "TenPhongBan");
            //}
            //else
            //{                               
            //    var listDvi = _faculty_ser.List().Where(p=>p.Id == donVi.Id).ToList();
            //    ViewBag.ListDvi = new SelectList(listDvi, "Id", "TenDonVi");
            //    var listPban = _department_ser.GetPhongBanByDonViID(donVi.Id).OrderBy(c => c.TenPhongBan).ToList();
            //    ViewBag.ListPBan = new SelectList(listPban, "Id", "TenPhongBan");

            //}

            DisposeAll();
            return View();
        }

        public ActionResult ListPhienLV()
        {
            int phongbanid = 0;
            if (Session["PhongBanId"] != null)
            {
                phongbanid = (int)Session["PhongBanId"];
            }
            List<PhienLVModel> model = new List<PhienLVModel>();
            var a = _phienlv_ser.List();
            IList<tblPhienLamViec> rtnList = _phienlv_ser.ListByPhongBanId(phongbanid);
            IBaseConverter<tblPhienLamViec, PhienLVModel> convtResult = new AutoMapConverter<tblPhienLamViec, PhienLVModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<PhienLVModel>();

            DisposeAll();

            return PartialView("_ListPhienLV", model);
        }

        IList<tblPhienLamViec> rtnList = null;
        public ActionResult List(int page, int? pageSize, string sortOrder, string searchString, string currentFilter,int? donviId,int? phongbanId)
        {
            int phongbanid = 0;
            if (Session["PhongBanId"] != null)
            {
                phongbanid = (int)Session["PhongBanId"];
            }
            if (pageSize != null)
            {
                PageSize = Convert.ToInt16(pageSize);
            }
            if (!string.IsNullOrEmpty(sortOrder))
            {
                ViewBag.CurrentSort = sortOrder;
            }
            else
            {
                ViewBag.CurrentSort = "name_asc";
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            List<PhienLVModel> model = new List<PhienLVModel>();
            if (rtnList == null)
                rtnList = _phienlv_ser.GetTableListById(donviId);

            if (!String.IsNullOrEmpty(searchString))
            {
                rtnList = rtnList.Where(s => s.NoiDung.Contains(searchString)
                                       || s.DiaDiem.Contains(searchString)).ToList();
            }
            IBaseConverter<tblPhienLamViec, PhienLVModel> convtResult = new AutoMapConverter<tblPhienLamViec, PhienLVModel>();
            var convtList = convtResult.ConvertObjectCollection(rtnList);
            model.AddRange(convtList);

            var ListNewsPageSize = new PagedData<PhienLVModel>();
            ListNewsPageSize.RecordsName = "Phiên làm việc";
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
                if (ListNewsPageSize.Data.Count() == 0)
                {
                    ListNewsPageSize.Data = model.Skip(PageSize * (page - 2)).Take(PageSize).ToList();
                }
                ListNewsPageSize.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)model.Count() / PageSize));
                ListNewsPageSize.RecordsPerPage = PageSize;
                ListNewsPageSize.CurrentPage = page;
                ListNewsPageSize.TotalRecords = model.Count();
            }
            else
            {
                ListNewsPageSize.Data = new List<PhienLVModel>();
                ListNewsPageSize.RecordsPerPage = 0;
                ListNewsPageSize.NumberOfPages = 0;
                ListNewsPageSize.CurrentPage = 0;
                ListNewsPageSize.TotalRecords = 0;
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderByDescending(s => s.NoiDung).ToList();
                    break;
                case "name_asc":
                    {
                        ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.NoiDung).ToList();
                        break;
                    }              
                default:
                    ListNewsPageSize.Data = ListNewsPageSize.Data.OrderBy(s => s.NoiDung).ToList();
                    break;
            }

            DisposeAll();

            return PartialView(ListNewsPageSize);
        }


        #region CmbPhongBan
        [HttpGet]
        public ActionResult CmbPhongBan(string DonViId)
        {
            ViewBag.PhongBan = PhongBanRepository.GetPhongBanByDonViIDHtml(DonViId, 0);

            DisposeAll();

            return View();
        }
        #endregion
    }
}