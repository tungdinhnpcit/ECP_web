using ECP_V2.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECP_V2.WebApplication.Areas.Admin.Models;
using ECP_V2.DataAccess;
using ECP_V2.Common.Mvc;
using System.Text;
using ECP_V2.Common.Helpers;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class ChiTieuSoVuSuCoController : UTController
    {
        private sc_ChiTieuSuCoTHASauMTRepository _chitieu_ser = new sc_ChiTieuSuCoTHASauMTRepository();
        private DonViRepository _DonVi_ser = new DonViRepository();

        // GET: Admin/ChiTieuSoVuSuCo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int Date)
        {
            var objDV = _DonVi_ser.List().FirstOrDefault(o => (o.Id.Length == 4) || o.Id.ToUpper() == "PH" || o.Id.ToUpper() == "PN" || o.Id.ToUpper() == "PM");
            string MaDV = "";
            if (objDV != null)
            {
                MaDV = objDV.Id;
            }
            //string MaDV = Session["DonViID"].ToString();

            DonViChiTieuSuCoViewModels obj = new DonViChiTieuSuCoViewModels();
            obj.MA_DVIQLY = MaDV;
            var lstData = _chitieu_ser.GetListChiTieu(Date, MaDV);
            if (lstData != null)
            {
                #region T1
                if (lstData.FirstOrDefault(o => o.Thang == 1) != null)
                {
                    obj.IdT1 = lstData.FirstOrDefault(o => o.Thang == 1).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuNam;
                    obj.ChiTieuT1 = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                    obj.SoVuSauMTT1 = lstData.FirstOrDefault(o => o.Thang == 1).SoVuSauMT;
                    obj.NPCIsOpenT1 = lstData.FirstOrDefault(o => o.Thang == 1).NPCIsOpen;
                    obj.ChiTieuT1SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T2
                if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                {
                    obj.IdT2 = lstData.FirstOrDefault(o => o.Thang == 2).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuNam;
                    obj.ChiTieuT2 = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                    obj.SoVuSauMTT2 = lstData.FirstOrDefault(o => o.Thang == 2).SoVuSauMT;
                    obj.NPCIsOpenT2 = lstData.FirstOrDefault(o => o.Thang == 2).NPCIsOpen;
                    obj.ChiTieuT2SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T3
                if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                {
                    obj.IdT3 = lstData.FirstOrDefault(o => o.Thang == 3).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuNam;
                    obj.ChiTieuT3 = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                    obj.SoVuSauMTT3 = lstData.FirstOrDefault(o => o.Thang == 3).SoVuSauMT;
                    obj.NPCIsOpenT3 = lstData.FirstOrDefault(o => o.Thang == 3).NPCIsOpen;
                    obj.ChiTieuT3SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T4
                if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                {
                    obj.IdT4 = lstData.FirstOrDefault(o => o.Thang == 4).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuNam;
                    obj.ChiTieuT4 = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                    obj.SoVuSauMTT4 = lstData.FirstOrDefault(o => o.Thang == 4).SoVuSauMT;
                    obj.NPCIsOpenT4 = lstData.FirstOrDefault(o => o.Thang == 4).NPCIsOpen;
                    obj.ChiTieuT4SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T5
                if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                {
                    obj.IdT5 = lstData.FirstOrDefault(o => o.Thang == 5).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuNam;
                    obj.ChiTieuT5 = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                    obj.SoVuSauMTT5 = lstData.FirstOrDefault(o => o.Thang == 5).SoVuSauMT;
                    obj.NPCIsOpenT5 = lstData.FirstOrDefault(o => o.Thang == 5).NPCIsOpen;
                    obj.ChiTieuT5SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T6
                if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                {
                    obj.IdT6 = lstData.FirstOrDefault(o => o.Thang == 6).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuNam;
                    obj.ChiTieuT6 = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                    obj.SoVuSauMTT6 = lstData.FirstOrDefault(o => o.Thang == 6).SoVuSauMT;
                    obj.NPCIsOpenT6 = lstData.FirstOrDefault(o => o.Thang == 6).NPCIsOpen;
                    obj.ChiTieuT6SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T7
                if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                {
                    obj.IdT7 = lstData.FirstOrDefault(o => o.Thang == 7).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuNam;
                    obj.ChiTieuT7 = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                    obj.SoVuSauMTT7 = lstData.FirstOrDefault(o => o.Thang == 7).SoVuSauMT;
                    obj.NPCIsOpenT7 = lstData.FirstOrDefault(o => o.Thang == 7).NPCIsOpen;
                    obj.ChiTieuT7SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T8
                if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                {
                    obj.IdT8 = lstData.FirstOrDefault(o => o.Thang == 8).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuNam;
                    obj.ChiTieuT8 = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                    obj.SoVuSauMTT8 = lstData.FirstOrDefault(o => o.Thang == 8).SoVuSauMT;
                    obj.NPCIsOpenT8 = lstData.FirstOrDefault(o => o.Thang == 8).NPCIsOpen;
                    obj.ChiTieuT8SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T9
                if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                {
                    obj.IdT9 = lstData.FirstOrDefault(o => o.Thang == 9).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuNam;
                    obj.ChiTieuT9 = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                    obj.SoVuSauMTT9 = lstData.FirstOrDefault(o => o.Thang == 9).SoVuSauMT;
                    obj.NPCIsOpenT9 = lstData.FirstOrDefault(o => o.Thang == 9).NPCIsOpen;
                    obj.ChiTieuT9SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T10
                if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                {
                    obj.IdT10 = lstData.FirstOrDefault(o => o.Thang == 10).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuNam;
                    obj.ChiTieuT10 = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                    obj.SoVuSauMTT10 = lstData.FirstOrDefault(o => o.Thang == 10).SoVuSauMT;
                    obj.NPCIsOpenT10 = lstData.FirstOrDefault(o => o.Thang == 10).NPCIsOpen;
                    obj.ChiTieuT10SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T11
                if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                {
                    obj.IdT11 = lstData.FirstOrDefault(o => o.Thang == 11).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuNam;
                    obj.ChiTieuT11 = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                    obj.SoVuSauMTT11 = lstData.FirstOrDefault(o => o.Thang == 11).SoVuSauMT;
                    obj.NPCIsOpenT11 = lstData.FirstOrDefault(o => o.Thang == 11).NPCIsOpen;
                    obj.ChiTieuT11SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;
                }
                #endregion

                #region T12
                if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                {
                    obj.IdT12 = lstData.FirstOrDefault(o => o.Thang == 12).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuNam;
                    obj.ChiTieuT12 = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                    obj.SoVuSauMTT12 = lstData.FirstOrDefault(o => o.Thang == 12).SoVuSauMT;
                    obj.NPCIsOpenT12 = lstData.FirstOrDefault(o => o.Thang == 12).NPCIsOpen;
                    obj.ChiTieuT12SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;
                }
                #endregion
            }

            return PartialView("List", obj);
        }

        public ActionResult Save(int Date, string MaDV, int? SuatNam
          , int? SuatT1, int? SuatT2, int? SuatT3, int? SuatT4
          , int? SuatT5, int? SuatT6, int? SuatT7, int? SuatT8
          , int? SuatT9, int? SuatT10, int? SuatT11, int? SuatT12
           , int? IdT1, int? IdT2, int? IdT3, int? IdT4, int? IdT5
           , int? IdT6, int? IdT7, int? IdT8, int? IdT9, int? IdT10
           , int? IdT11, int? IdT12)
        {

            int tongso = 0;
            int sothanhcong = 0;
            int soloi = 0;

            StringBuilder strErrorSum = new StringBuilder();
            StringBuilder strSuccessSum = new StringBuilder();

            string strError = "";
            try
            {

                #region T1
                if ((IdT1 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT1 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT1;
                        objct.ChiTieuThang = SuatT1;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 1<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 1 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T2
                if ((IdT2 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT2 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT2;
                        objct.ChiTieuThang = SuatT2;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 2<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 2 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T3
                if ((IdT3 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT3 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT3;
                        objct.ChiTieuThang = SuatT3;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 31<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 3 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T4
                if ((IdT4 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT4 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT4;
                        objct.ChiTieuThang = SuatT4;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 4<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 4 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T5
                if ((IdT5 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT5 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT5;
                        objct.ChiTieuThang = SuatT5;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 5<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 5 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T6
                if ((IdT6 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT6 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT6;
                        objct.ChiTieuThang = SuatT6;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 6<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 6 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T7
                if ((IdT7 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT7 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT7;
                        objct.ChiTieuThang = SuatT7;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 7<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 7 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T8
                if ((IdT8 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT8 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT8;
                        objct.ChiTieuThang = SuatT8;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 8<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 8 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T9
                if ((IdT9 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT9 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT9;
                        objct.ChiTieuThang = SuatT9;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 9<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 9 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T10
                if ((IdT10 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT10 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT10;
                        objct.ChiTieuThang = SuatT10;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 10<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 10 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T11
                if ((IdT11 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT11 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT11;
                        objct.ChiTieuThang = SuatT11;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 11<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 11 thành công</p>");
                        }
                    }
                }
                #endregion

                #region T12
                if ((IdT12 ?? 0) > 0)
                {
                    var objct = _chitieu_ser.GetById(IdT12 ?? 0);
                    if (objct.NPCIsOpen.GetValueOrDefault())
                    {

                        objct.ChiTieuThangSauDieuChuyen = SuatT12;
                        objct.ChiTieuThang = SuatT12;

                        if (objct.NguoiNhap == null)
                            objct.NguoiNhap = User.Identity.Name;
                        else
                            objct.NguoiSua = User.Identity.Name;
                        if (objct.NgayNhap == null)
                            objct.NgayNhap = DateTime.Now;
                        else
                            objct.NgaySua = DateTime.Now;
                        object x = _chitieu_ser.Update(objct, ref strError);
                        if (x == null)
                        {
                            soloi++;
                            strErrorSum.AppendLine("<hr style='margin:0'/> <p style='color:red;margin:0'>Lỗi trong quá trình cập nhật tháng 12<p>");
                        }
                        else
                        {
                            sothanhcong++;
                            strSuccessSum.AppendLine("<hr style='margin:0'/> <p style='color:green;margin:0'>Cập nhật tháng 12 thành công</p>");
                        }
                    }
                }
                #endregion

                string msg = "";
                if (soloi > 0)
                    msg = msg + strErrorSum.ToString() + "<br/>";
                if (sothanhcong > 0)
                    msg = msg + strSuccessSum.ToString() + "<br/>";

                return Json(new { type = "success", mess = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Lỗi trong quá trình lưu dữ liệu: " + ex.Message + "<br/>" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}