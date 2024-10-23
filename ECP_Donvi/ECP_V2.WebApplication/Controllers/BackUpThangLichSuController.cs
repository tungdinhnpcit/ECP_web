using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Globalization;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BackUpThangLichSuController : UTController
    {
        private DonViRepository donViRepository = new DonViRepository();
        private ThangLamViecRepository thangLamViecRepository = new ThangLamViecRepository();

        private void DisposeAll()
        {
            if (donViRepository != null)
            {
                donViRepository.Dispose();
                donViRepository = null;
            }

            if (thangLamViecRepository != null)
            {
                thangLamViecRepository.Dispose();
                thangLamViecRepository = null;
            }
        }

        [HasCredential(MenuCode = "BACKUP_THANGLS")]
        public ActionResult Index()
        {
            DisposeAll();

            return View();
        }

        public ActionResult ListImageBackUp(string DonViId = "")
        {
            try
            {
                var thangLamViec = thangLamViecRepository.GetMaxByDonViId(DonViId);

                DisposeAll();

                return PartialView("ListImageBackUp", thangLamViec);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return PartialView("ListImageBackUp", null);
            }
        }

        [HttpPost]
        public JsonResult MoveFile(string DonViId = "", string phanHe = "")
        {
            try
            {
                bool move = false;
                DateTime ngayBackUp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                ngayBackUp = ngayBackUp.AddMonths(-3);

                if (phanHe.Equals(PhanHe.PhanHeImage))
                {
                    move = thangLamViecRepository.ExecuteMoveImageHistory(DonViId, ngayBackUp);
                }
                else if (phanHe.Equals(PhanHe.PhanHePhienLamViec))
                {
                    var existImage = thangLamViecRepository.CheckExistPhienLamViecIdImage(DonViId, ngayBackUp);

                    if (existImage)
                    {
                        DisposeAll();

                        return Json(new { success = false, responseText = "Vui lòng chuyển tháng dữ liệu phân hệ hình ảnh trước khi chuyển tháng dữ liệu lịch công tác" }, JsonRequestBehavior.AllowGet);
                    }

                    var existPCT = thangLamViecRepository.CheckExistPhienLamViecIdPhieuCongTac(DonViId, ngayBackUp);

                    if (existPCT)
                    {
                        DisposeAll();

                        return Json(new { success = false, responseText = "Vui lòng chuyển tháng dữ liệu phân hệ phiếu công tác trước khi chuyển tháng dữ liệu lịch công tác" }, JsonRequestBehavior.AllowGet);
                    }

                    move = thangLamViecRepository.ExecuteMovePhienLamViecHistory(DonViId, ngayBackUp);
                }
                else if (phanHe.Equals(PhanHe.PhanHePhieuCongTac))
                {
                    var existImage = thangLamViecRepository.CheckExistPhienLamViecIdImage(DonViId, ngayBackUp);

                    if (existImage)
                    {
                        DisposeAll();

                        return Json(new { success = false, responseText = "Vui lòng chuyển tháng dữ liệu phân hệ hình ảnh trước khi chuyển tháng dữ liệu lịch công tác" }, JsonRequestBehavior.AllowGet);
                    }

                    move = thangLamViecRepository.ExecuteMovePhieuCongTacHistory(DonViId, ngayBackUp);
                }
                else if (phanHe.Equals(PhanHe.PhanHeSuCo))
                {
                    move = thangLamViecRepository.ExecuteMoveSuCoHistory(DonViId, ngayBackUp);
                }

                if (move)
                {
                    var history = thangLamViecRepository.GetByDonViIdAndPhanHeAndThangAndNam(DonViId, phanHe, ngayBackUp.Month, ngayBackUp.Year);

                    if (history != null)
                    {
                        history.NguoiSua = User.Identity.Name;
                        history.NgaySua = DateTime.Now;

                        string strError = "";
                        object id = thangLamViecRepository.Update(history, ref strError);

                        if (id != null)
                        {
                            DisposeAll();

                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var entity = new S_THANG_LVIEC
                        {
                            DonViId = DonViId,
                            Nam = ngayBackUp.Year,
                            Thang = ngayBackUp.Month,
                            NgayTao = DateTime.Now,
                            NguoiTao = User.Identity.Name,
                            PhanHe = phanHe,
                            TrangThai = 1,
                        };

                        string strError = "";
                        object id = thangLamViecRepository.Create(entity, ref strError);

                        if (id != null)
                        {
                            DisposeAll();

                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                DisposeAll();

                return Json(new { success = false, responseText = "Chuyển tháng dữ liệu không thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false, responseText = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}