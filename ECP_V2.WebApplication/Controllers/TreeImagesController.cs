using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    public class TreeImagesController : Controller
    {
        ImagesRepository _imagesRepository = new ImagesRepository();
        PhienLVRepository _phienLVRepository = new PhienLVRepository();
        DonViRepository _donViRepository = new DonViRepository();
        PhongBanRepository _phongBanRepository = new PhongBanRepository();
        NhanVienRepository _nhanvienRepository = new NhanVienRepository();

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
        }

        // GET: TreeImages
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            TreeImageModel model = new TreeImageModel();
            List<string> danhsachdonvi = new List<string>();
            List<int> danhsachphongban = new List<int>();

            //if (nhanvien != null)
            //{
            //    if (nhanvien.DonViId == 0 || nhanvien.DonViId == null)
            //    {
            //        if (nhanvien.PhongBanId != null && nhanvien.PhongBanId != 0)
            //            danhsachphongban.Add((int)nhanvien.PhongBanId);
            //        //cap phong ban
            //    }
            //    else
            //    {
            //        var donvi = _donViRepository.GetById(nhanvien.DonViId);
            //        if (donvi != null)
            //        {
            //            danhsachdonvi.Add(donvi.Id);
            //            var donvicon = _donViRepository.ListByParentId(donvi.Id);
            //            if (donvicon != null)
            //            {
            //                //don vi cap con
            //                danhsachdonvi.AddRange(donvicon.Select(x => x.Id).ToList());
            //            }
            //            foreach (var item in danhsachdonvi)
            //            {
            //                var phongban = _phongBanRepository.GetPhongBanByDonViID(item);
            //                if (phongban != null)
            //                {
            //                    foreach (var itemPB in phongban)
            //                    {
            //                        if (!danhsachphongban.Any(x => x == itemPB.Id))
            //                            danhsachphongban.Add(itemPB.Id);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
            {
                ViewBag.DonVi = _donViRepository.List();
                ViewBag.PhongBan = _phongBanRepository.List();
                //

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
            return View(model);
        }

        public ActionResult GetImageByDvi(int? donviid, int? phongbanid, int? phienlvid, int? sort = (int)ImgSort.THOIGIAN, int? page = 0, int? quantity = 8)
        {
            int sum = 0;
            ViewBag.Page = page;
            ViewBag.Quantity = quantity;
            List<tblImage> listImage = new List<tblImage>();
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
            {
                listImage = _imagesRepository.List(out sum, 1, 0, 8);
            }
            //else
            //{
            //    if (User.IsInRole("Manager"))
            //    {
            //        List<tblPhongBan> listPhongBan = null;
            //        if (danhsachdonvi != null)
            //            listPhongBan = _phongBanRepository.List().Where(p => danhsachdonvi.Contains((p.MaDVi != null) ? (int)p.MaDVi : -1)).ToList();
            //        if (listPhongBan != null)
            //        {
            //            foreach (var item in listPhongBan)
            //            {
            //                int subSum = 0;
            //                var listByPhongBanId = _imagesRepository.ListByPhongBanId(out subSum, item.Id, sort, page, quantity);
            //                sum = sum + subSum;
            //                if (listByPhongBanId != null)
            //                {
            //                    listImage.AddRange(listByPhongBanId);
            //                }
            //            }
            //        }
            //    }
            //    else if (User.IsInRole("Worker"))
            //    {
            //        listImage = _imagesRepository.ListByNhanVienId(out sum, nhanvien.Id);
            //    }
            //}
            ViewBag.Sum = sum;

            DisposeAll();

            return Json(listImage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStateList()
        {

            int sum = 0;
            List<tblImage> listImage = new List<tblImage>();
            var username = User.Identity.Name;
            var nhanvien = _nhanvienRepository.GetByUserName(username);
            if (User.IsInRole("Admin") || User.IsInRole("Master") || User.IsInRole("Visitor"))
            {
                listImage = _imagesRepository.List(out sum, 1, 0, 8);
            }
            //var listImageView = from p in listImage
            //                    select new ListImage
            //                    {
            //                       Id = p.Id,
            //                       Url = p.Url,
            //                       Note = p.Note,
            //                       Comment = p.Comment,
            //                       PhienLamViecId = p.PhienLamViecId,
            //                       Tag = p.Tag,
            //                       NgayCapNhat = p.NgayCapNhat,
            //                       GroupId = p.GroupId,
            //                       isVideo = p.isVideo,
            //                       UserUp = p.UserUp,
            //                       VideoPath = p.VideoPath
            //                    };

            //if(listImageView.Count() > 0)
            //{
            //    return this.Json(listImageView.ToList(), JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return this.Json(null, JsonRequestBehavior.AllowGet);
            //}

            DisposeAll();

            return PartialView("_ListImage", listImage.ToList());


        }
        public ActionResult _GetListGallery(string donviid, int? phongbanid, int? phienlvid, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
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

            return PartialView("_GetBoxItemGallery", listImage);
        }
    }
}