using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
    public class ReportV2Controller : UTController
    {

        //
        // GET: /Admin/PhienLV/
        private PhienLVRepository _plviec_ser = new PhienLVRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private BaoCaoRepository _baocao_ser = new BaoCaoRepository();
        //[AreaAuthorization]

        private void DisposeAll()
        {
            if (_plviec_ser != null)
            {
                _plviec_ser.Dispose();
                _plviec_ser = null;
            }

            if (_dvi_ser != null)
            {
                _dvi_ser.Dispose();
                _dvi_ser = null;
            }

            if (_baocao_ser != null)
            {
                _baocao_ser.Dispose();
                _baocao_ser = null;
            }
        }

        #region Index
        [HasCredential(MenuCode = "BCTHOP")]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region BaoCaoDauGio
        public ActionResult BaoCaoDauGio(string DateFrom, string DateTo)
        {
            IEnumerable<ECP_V2.DataAccess.TongHopBaoCaoDauGio_Result> model;
            // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
            if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
            {
                DateTime dts = new DateTime();
                DateTime dte = new DateTime();
                GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);

                model = _baocao_ser.Get_KeHoachTuan(dts, dte);

                ViewBag.DST = dts;
                ViewBag.DTE = dte;
            }
            else
            {
                DateTime dts = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dte = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                model = _baocao_ser.Get_KeHoachTuan(dts, dte);

                ViewBag.DST = dts;
                ViewBag.DTE = dte;
            }

            DisposeAll();

            return View(model);
        }
        public ActionResult BaoCaoDauGioV2(string DateFrom, string DateTo)
        {
            IEnumerable<ECP_V2.DataAccess.TongHopBaoCaoDauGioV2_Result> model;
            // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
            if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
            {
                DateTime dts = new DateTime();
                DateTime dte = new DateTime();
                GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);

                model = _baocao_ser.Get_KeHoachTuanV2(dts, dte);

                ViewBag.DST = dts;
                ViewBag.DTE = dte;
            }
            else
            {
                DateTime dts = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dte = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                model = _baocao_ser.Get_KeHoachTuanV2(dts, dte);

                ViewBag.DST = dts;
                ViewBag.DTE = dte;
            }

            DisposeAll();

            return View(model);
        }
        public void GetDateStartEnd_FromDateNow(ref DateTime DateStart, ref DateTime DateEnd, DateTime date)
        {
            // Sun: 0 - Mon: 1 - Tue: 2 - Web: 3 - Thu: 4 - Fri: 5 - Sat: 6
            // lay tho igian tu thu 3 tuan nay den thu 2 tuan sau
            int dayI = ((int)date.DayOfWeek);

            if (dayI == 1)
            {
                DateStart = date;
                DateEnd = date.AddDays(6);
            }
            else if (dayI == 2)
            {
                DateStart = date.AddDays(-1);
                DateEnd = date.AddDays(5);
            }
            else if (dayI == 3)
            {
                DateStart = date.AddDays(-2);
                DateEnd = date.AddDays(4);
            }
            else if (dayI == 4)
            {
                DateStart = date.AddDays(-3);
                DateEnd = date.AddDays(3);
            }
            else if (dayI == 5)
            {
                DateStart = date.AddDays(-4);
                DateEnd = date.AddDays(2);
            }
            else if (dayI == 6)
            {
                DateStart = date.AddDays(-5);
                DateEnd = date.AddDays(1);
            }
            else if (dayI == 0)
            {
                DateStart = date.AddDays(-6);
                DateEnd = date.AddDays(7);
            }

        }
        #endregion

        #region BaoCaoCuoiNgay
        public ActionResult BaoCaoCuoiNgay(string DateFrom, string DateTo)
        {
            IEnumerable<ECP_V2.DataAccess.TongHopBaoCaoCuoiNgay_Result> model;
            // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
            if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
            {
                DateTime dts = new DateTime();
                DateTime dte = new DateTime();
                GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);

                model = _baocao_ser.Get_KeHoachTuanCuoiNgay(dts, dte);

                ViewBag.DSTCN = dts;
                ViewBag.DTECN = dte;
            }
            else
            {
                DateTime dts = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dte = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                model = _baocao_ser.Get_KeHoachTuanCuoiNgay(dts, dte);

                ViewBag.DSTCN = dts;
                ViewBag.DTECN = dte;
            }

            DisposeAll();

            return View(model);
        }
        #endregion


    }
}
