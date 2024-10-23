using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        ImagesRepository _imagesRepository = new ImagesRepository();
        PhienLVRepository _phienLVRepository = new PhienLVRepository();
        DonViRepository _donViRepository = new DonViRepository();
        PhongBanRepository _phongBanRepository = new PhongBanRepository();
        NhanVienRepository _nhanvienRepository = new NhanVienRepository();
        AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
        private GroupImageRepository groupImageRepository = new GroupImageRepository();


        private void DisposeAll()
        {
            if (_imagesRepository != null)
            {
                _imagesRepository.Dispose();
                _imagesRepository = null;
            }

            if (_phienLVRepository != null)
            {
                _phienLVRepository.Dispose();
                _phienLVRepository = null;
            }

            if (_donViRepository != null)
            {
                _donViRepository.Dispose();
                _donViRepository = null;
            }

            if (_phongBanRepository != null)
            {
                _phongBanRepository.Dispose();
                _phongBanRepository = null;
            }

            if (_nhanvienRepository != null)
            {
                _nhanvienRepository.Dispose();
                _nhanvienRepository = null;
            }

            if (aspNetUserRepository != null)
            {
                aspNetUserRepository.Dispose();
                aspNetUserRepository = null;
            }

            if (groupImageRepository != null)
            {
                groupImageRepository.Dispose();
                groupImageRepository = null;
            }
        }

        //
        // GET: /Images/
        public ActionResult Index()
        {
            DisposeAll();

            return View();
        }
        public ActionResult List()
        {
            DisposeAll();

            return View();
        }
        public ActionResult Upload(int phienlvid = 0)
        {
            var phienLV = _phienLVRepository.GetById(phienlvid);
            Session["PhienLVId"] = phienlvid;
            ViewBag.PhienLVId = phienlvid;
            ViewBag.TenPhienLV = null;
            if (phienLV != null)
            {
                ViewBag.TenPhienLV = phienLV.NoiDung;
            }

            DisposeAll();

            return View();
        }
        public ActionResult Tree()
        {
            DisposeAll();

            return View();
        }
        public ActionResult Detail(int phienlvid = 0)
        {
            int sum = 0;
            var phienLV = _phienLVRepository.GetById(phienlvid);
            if (phienlvid >= 0 && phienLV != null)
            {
                ViewBag.PhienLV = phienLV;
                var images = _imagesRepository.ListByPhienLVId(phienlvid);
                //if (images == null || images.Count == 0)
                //{
                //    Response.Redirect("/Images/Upload?phienlvid=" + phienlvid);
                //}
                ViewBag.Images = images;

                DisposeAll();

                return View(phienLV);
            }
            else
            {
                phienLV = new tblPhienLamViec() { Id = 0, NoiDung = "Chưa xác định" };
                if (Session["UserId"] != null)
                {
                    string userid = Session["UserId"].ToString();
                    ViewBag.PhienLV = phienLV;
                    var images = _imagesRepository.ListByNhanVienId(out sum, userid);
                    if (images == null || images.Count == 0)
                    {
                        Response.Redirect("/Images/Upload?phienlvid=" + phienlvid);
                    }
                    ViewBag.Images = images;

                    DisposeAll();

                    return View(phienLV);
                }
                else
                {
                    ViewBag.Images = new List<tblImage>();

                    DisposeAll();

                    return View(phienLV);
                }
            }
        }
        public ActionResult Review()
        {
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            List<string> danhsachdonvi = new List<string>();
            List<int> danhsachphongban = new List<int>();
            if (nhanvien != null)
            {
                if (nhanvien.DonViId == null || nhanvien.DonViId == null)
                {
                    if (nhanvien.PhongBanId != null && nhanvien.PhongBanId != 0)
                        danhsachphongban.Add((int)nhanvien.PhongBanId);
                    //cap phong ban
                }
                else
                {
                    var donvi = _donViRepository.GetById(nhanvien.DonViId);
                    if (donvi != null)
                    {
                        danhsachdonvi.Add(donvi.Id);
                        var donvicon = _donViRepository.ListByParentId(donvi.Id);
                        if (donvicon != null)
                        {
                            //don vi cap cap
                            danhsachdonvi.AddRange(donvicon.Select(x => x.Id).ToList());
                        }
                        foreach (var item in danhsachdonvi)
                        {
                            var phongban = _phongBanRepository.GetPhongBanByDonViID(item);
                            if (phongban != null)
                            {
                                foreach (var itemPB in phongban)
                                {
                                    if (!danhsachphongban.Any(x => x == itemPB.Id))
                                        danhsachphongban.Add(itemPB.Id);
                                }
                            }
                        }
                    }
                }
            }

            if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
            {
                ViewBag.DonVi = _donViRepository.List();
                ViewBag.PhongBan = _phongBanRepository.List();
                //ViewBag.Images = _imagesRepository.List();
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    string donviid = null;
                    if (Session["DonViID"] != null)
                        donviid = Session["DonViID"].ToString();
                    ViewBag.DonVi = null;
                    if (danhsachdonvi.Count > 0)
                    {
                        ViewBag.DonVi = _donViRepository.List().Where(p => danhsachdonvi.Contains((p.Id != null) ? p.Id : null)).ToList();
                    }
                    List<tblPhongBan> listPhongBan = _phongBanRepository.List().Where(p => danhsachdonvi.Contains((p.MaDVi != null) ? p.MaDVi : null)).ToList();
                    ViewBag.PhongBan = listPhongBan;
                }
                else if (User.IsInRole("Worker"))
                {
                    ViewBag.DonVi = null;
                    ViewBag.PhongBan = null;
                }
            }

            DisposeAll();

            return View();
        }

        [HasCredential(MenuCode = "DMA;DSA;dma;Image")]
        public ActionResult Gallery()
        {
            TreeImageModel model = new TreeImageModel();
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            List<string> danhsachdonvi = new List<string>();
            List<int> danhsachphongban = new List<int>();
            if (nhanvien != null)
            {
                if (nhanvien.DonViId == null || nhanvien.DonViId == null)
                {
                    if (nhanvien.PhongBanId != null && nhanvien.PhongBanId != 0)
                        danhsachphongban.Add((int)nhanvien.PhongBanId);
                    //cap phong ban
                }
                else
                {
                    var donvi = _donViRepository.GetById(nhanvien.DonViId);
                    if (donvi != null)
                    {
                        danhsachdonvi.Add(donvi.Id);
                        var donvicon = _donViRepository.ListByParentId(donvi.Id);
                        if (donvicon != null)
                        {
                            //don vi cap cap
                            danhsachdonvi.AddRange(donvicon.Select(x => x.Id).ToList());
                        }
                        foreach (var item in danhsachdonvi)
                        {
                            var phongban = _phongBanRepository.GetPhongBanByDonViID(item);
                            if (phongban != null)
                            {
                                foreach (var itemPB in phongban)
                                {
                                    if (!danhsachphongban.Any(x => x == itemPB.Id))
                                        danhsachphongban.Add(itemPB.Id);
                                }
                            }
                        }
                    }
                }
            }

            if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
            {
                ViewBag.DonVi = _donViRepository.List();
                ViewBag.PhongBan = _phongBanRepository.List();
                model.LstDonVi = _donViRepository.List();
                model.LstPhongBan = _phongBanRepository.List();
            }
            else
            {
                if (User.IsInRole("Manager"))
                {
                    string donviid = null;
                    if (Session["DonViID"] != null)
                        donviid = Session["DonViID"].ToString();
                    ViewBag.DonVi = null;

                    if (danhsachdonvi.Count > 0)
                    {
                        var lstDonvi = _donViRepository.List().Where(p => danhsachdonvi.Contains((p.Id != null) ? p.Id : null)).ToList();
                        ViewBag.DonVi = lstDonvi;
                        model.LstDonVi = lstDonvi;
                    }
                    List<tblPhongBan> listPhongBan = _phongBanRepository.List().Where(p => danhsachdonvi.Contains((p.MaDVi != null) ? p.MaDVi : null)).ToList();
                    ViewBag.PhongBan = listPhongBan;

                    model.LstPhongBan = listPhongBan;

                }
                else if (User.IsInRole("Worker"))
                {
                    ViewBag.DonVi = null;
                    ViewBag.PhongBan = null;
                }
            }
            DisposeAll();

            return View(model);
        }
        public ActionResult Test()
        {
            DisposeAll();

            return View();
        }

        [HttpPost]
        public ActionResult Update(int id, string note, string comment, string tag, int groupid)
        {
            try
            {
                var img = _imagesRepository.GetById(id);
                if (img != null)
                {
                    img.Note = note;
                    img.Comment = comment;
                    img.Tag = tag;
                    img.NgayCapNhat = DateTime.Now;
                    img.GroupId = groupid;
                    string error = "";
                    _imagesRepository.Update(img, ref error);

                    DisposeAll();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateGroupId(int id, int groupid)
        {
            try
            {
                var img = _imagesRepository.GetById(id);
                if (img != null)
                {
                    img.GroupId = groupid;
                    string error = "";
                    _imagesRepository.Update(img, ref error);
                }

                DisposeAll();

                return Json(new { Status = 1, Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                DisposeAll();

                return Json(new { Status = 0, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string strError = "";
            try
            {
                var img = _imagesRepository.GetById(id);
                if (img != null)
                {
                    var user = aspNetUserRepository.GetByUserName(User.Identity.Name);

                    img.IsDelete = true;
                    img.NguoiXoa = user.Id;
                    img.NgayXoa = DateTime.Now;
                    _imagesRepository.Update(img, ref strError);
                }

                DisposeAll();

                return Json(new { Status = 1, Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                DisposeAll();

                return Json(new { Status = 0, Message = "Error: " + strError }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult _GetListItem(int phienlvid, int userid = 0, int type = 0)
        {
            int sum = 0;
            string viewName = "_GetListItem";
            if (type == 1)
                viewName = "_GetBoxItem";

            ViewBag.GroupImages = groupImageRepository.List().OrderBy(x => x.ThuTu).ToList();

            try
            {
                if (phienlvid > 0)
                {
                    var list = _imagesRepository.ListByPhienLVId(phienlvid);

                    DisposeAll();

                    return PartialView(viewName, list);
                }
                else
                {
                    if (Session["UserId"] != null)
                    {
                        string strUserid = Session["UserId"].ToString();
                        var images = _imagesRepository.ListByNhanVienId(out sum, strUserid);

                        DisposeAll();

                        return PartialView(viewName, images);
                    }
                }

            }
            catch
            {

            }

            DisposeAll();

            return PartialView(viewName, new List<tblImage>());
        }
        public ActionResult _GetListReview(string donviid, int? phongbanid, int? phienlvid, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            int sum = 0;
            ViewBag.Page = page;
            ViewBag.Quantity = quantity;
            List<tblImage> listImage = new List<tblImage>();
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            List<string> danhsachdonvi = new List<string>();
            List<int> danhsachphongban = new List<int>();
            if (nhanvien != null)
            {
                if (nhanvien.DonViId == null || nhanvien.DonViId == null)
                {
                    if (nhanvien.PhongBanId != null && nhanvien.PhongBanId != 0)
                        danhsachphongban.Add((int)nhanvien.PhongBanId);
                    //cap phong ban
                }
                else
                {
                    var donvi = _donViRepository.GetById(nhanvien.DonViId);
                    if (donvi != null)
                    {
                        danhsachdonvi.Add(donvi.Id);
                        var donvicon = _donViRepository.ListByParentId(donvi.Id);
                        if (donvicon != null)
                        {
                            //don vi cap cap
                            danhsachdonvi.AddRange(donvicon.Select(x => x.Id).ToList());
                        }
                        foreach (var item in danhsachdonvi)
                        {
                            var phongban = _phongBanRepository.GetPhongBanByDonViID(item);
                            if (phongban != null)
                            {
                                foreach (var itemPB in phongban)
                                {
                                    if (!danhsachphongban.Any(x => x == itemPB.Id))
                                        danhsachphongban.Add(itemPB.Id);
                                }
                            }
                        }
                    }
                }
            }
            if ((phongbanid != null && phongbanid > 0) || (donviid != null && !string.IsNullOrEmpty(donviid)))
            {
                if (danhsachphongban.Any(x => x == phongbanid))
                {
                    danhsachphongban = new List<int>() { (int)phongbanid };
                    int subSum = 0;
                    var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, (int)phongbanid, "", sort, page, quantity);
                    sum = sum + subSum;
                    if (listByPhongBanId != null)
                    {
                        listImage.AddRange(listByPhongBanId);
                    }
                }
                else if (danhsachdonvi.Any(x => x == donviid))
                {
                    danhsachdonvi = new List<string>() { donviid };
                    var listPhongBan = _phongBanRepository.List().Where(p => p.MaDVi == donviid).ToList();
                    if (listPhongBan != null)
                    {
                        foreach (var item in listPhongBan)
                        {
                            int subSum = 0;
                            var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, item.Id, "", sort, page, quantity);
                            sum = sum + subSum;
                            if (listByPhongBanId != null)
                            {
                                listImage.AddRange(listByPhongBanId);
                            }
                        }
                    }
                }
            }
            else
            {
                if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
                {
                    listImage = _imagesRepository.List(out sum, sort, page, quantity);
                }
                else
                {
                    if (User.IsInRole("Manager"))
                    {
                        List<tblPhongBan> listPhongBan = null;
                        if (danhsachdonvi != null)
                            listPhongBan = _phongBanRepository.List().Where(p => danhsachdonvi.Contains((p.MaDVi != null) ? p.MaDVi : null)).ToList();
                        if (listPhongBan != null)
                        {
                            foreach (var item in listPhongBan)
                            {
                                int subSum = 0;
                                var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, item.Id, "", sort, page, quantity);
                                sum = sum + subSum;
                                if (listByPhongBanId != null)
                                {
                                    listImage.AddRange(listByPhongBanId);
                                }
                            }
                        }
                    }
                    else if (User.IsInRole("Worker"))
                    {
                        listImage = _imagesRepository.ListByNhanVienId(out sum, nhanvien.Id);
                    }
                }
            }
            ViewBag.Sum = sum;

            DisposeAll();

            return PartialView("_GetBoxItem", listImage);
        }

        public ActionResult _GetListGallery(string donviid, int? phongbanid, int? phienlvid, string ngayupanh = "", int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            int sum = 0;
            ViewBag.Page = page;
            ViewBag.Quantity = quantity;
            List<tblImage> listImage = new List<tblImage>();
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            List<string> danhsachdonvi = new List<string>();
            List<int> danhsachphongban = new List<int>();
            if (nhanvien != null)
            {
                if (nhanvien.DonViId == null || nhanvien.DonViId == null)
                {
                    if (nhanvien.PhongBanId != null && nhanvien.PhongBanId != 0)
                        danhsachphongban.Add((int)nhanvien.PhongBanId);
                    //cap phong ban
                }
                else
                {
                    var donvi = _donViRepository.GetById(nhanvien.DonViId);
                    if (donvi != null)
                    {
                        danhsachdonvi.Add(donvi.Id);
                        var donvicon = _donViRepository.ListByParentId(donvi.Id);
                        if (donvicon != null)
                        {
                            //don vi cap cap
                            danhsachdonvi.AddRange(donvicon.Select(x => x.Id).ToList());
                        }
                        foreach (var item in danhsachdonvi)
                        {
                            var phongban = _phongBanRepository.GetPhongBanByDonViID(item);
                            if (phongban != null)
                            {
                                foreach (var itemPB in phongban)
                                {
                                    if (!danhsachphongban.Any(x => x == itemPB.Id))
                                        danhsachphongban.Add(itemPB.Id);
                                }
                            }
                        }
                    }
                }
            }
            if ((phongbanid != null && phongbanid > 0) || (donviid != null && !string.IsNullOrEmpty(donviid) && donviid != "0"))
            {
                if (danhsachphongban.Any(x => x == phongbanid))
                {
                    danhsachphongban = new List<int>() { (int)phongbanid };
                    int subSum = 0;
                    var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, (int)phongbanid, ngayupanh, sort, page, quantity);
                    sum = sum + subSum;
                    if (listByPhongBanId != null)
                    {
                        listImage.AddRange(listByPhongBanId);
                    }
                }
                else if (danhsachdonvi.Any(x => x == donviid))
                {
                    danhsachdonvi = new List<string>() { donviid };
                    var listPhongBan = _phongBanRepository.List().Where(p => p.MaDVi == donviid).ToList();
                    if (listPhongBan != null)
                    {
                        foreach (var item in listPhongBan)
                        {
                            int subSum = 0;
                            var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, item.Id, ngayupanh, sort, page, quantity);
                            sum = sum + subSum;
                            if (listByPhongBanId != null)
                            {
                                listImage.AddRange(listByPhongBanId);
                            }
                        }
                    }
                }
            }
            else
            {
                if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
                {
                    listImage = _imagesRepository.List(out sum, sort, page, quantity);
                }
                else
                {
                    if (User.IsInRole("Manager"))
                    {
                        List<tblPhongBan> listPhongBan = null;
                        if (danhsachdonvi != null)
                            listPhongBan = _phongBanRepository.List().Where(p => danhsachdonvi.Contains((p.MaDVi != null) ? p.MaDVi : null)).ToList();
                        if (listPhongBan != null)
                        {
                            foreach (var item in listPhongBan)
                            {
                                int subSum = 0;
                                var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, item.Id, ngayupanh, sort, page, quantity);
                                sum = sum + subSum;
                                if (listByPhongBanId != null)
                                {
                                    listImage.AddRange(listByPhongBanId);
                                }
                            }
                        }
                    }
                    else if (User.IsInRole("Worker"))
                    {
                        listImage = _imagesRepository.ListByNhanVienId(out sum, nhanvien.Id);
                    }
                }
            }
            ViewBag.Sum = sum;

            DisposeAll();

            return PartialView("_GetBoxItemGallery", listImage);
        }


        public ActionResult CKFinder()
        {
            DisposeAll();

            return View();
        }
    }
}