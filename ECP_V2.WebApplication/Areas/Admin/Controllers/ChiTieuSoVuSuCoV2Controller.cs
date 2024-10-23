using ECP_V2.Business.Repository;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Areas.Admin.Models;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class ChiTieuSoVuSuCoV2Controller : UTController
    {
        private sc_ChiTieuSoVuSuCoRepository _chitieu_ser = new sc_ChiTieuSoVuSuCoRepository();
        private DonViRepository _DonVi_ser = new DonViRepository();
        public sc_TaiNanSuCoRepository _SuCo_ser = new sc_TaiNanSuCoRepository(System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString);
        string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;
        private sc_KHGiaoTGXLSCTHARepository _chitieuthoigian_ser = new sc_KHGiaoTGXLSCTHARepository();

        // GET: Admin/ChiTieuSoVuSuCo
        [HasCredential(MenuCode = "CTNSVSC;KBSC")]
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

            List<DonViChiTieuSuCoViewModels> model = new List<DonViChiTieuSuCoViewModels>();
            var lstdv = _DonVi_ser.List().Where(o => o.Id != MaDV && o.MaLP == 1);
            var lstChiTieuCongTy = _chitieu_ser.GetListChiTieu(Date, MaDV);
            foreach (var item in lstdv)
            {
                DonViChiTieuSuCoViewModels obj = new DonViChiTieuSuCoViewModels();
                obj.MA_DVIQLY = item.Id;
                obj.TEN_DVIQLY = item.TenDonVi;
                obj.ChiTieuNamCongTy = lstChiTieuCongTy != null ? lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 1).ChiTieuNam : 0;

                var lstData = _chitieu_ser.GetListChiTieu(Date, item.Id);
                if (lstData != null)
                {
                    #region T1
                    if (lstData.FirstOrDefault(o => o.Thang == 1) != null)
                    {
                        obj.IdT1 = lstData.FirstOrDefault(o => o.Thang == 1).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuNam;
                        obj.ChiTieuT1 = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                        obj.NPCIsOpenT1 = lstData.FirstOrDefault(o => o.Thang == 1).IsOpen;
                        obj.ChiTieuT1SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 1)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 2))
                        {
                            try
                            {
                                obj.SoVuSauMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT1 = lstData.FirstOrDefault(o => o.Thang == 1).SoVuSauMT;
                            //obj.SoVuTruocMTT1 = lstData.FirstOrDefault(o => o.Thang == 1).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T2
                    if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                    {
                        obj.IdT2 = lstData.FirstOrDefault(o => o.Thang == 2).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuNam;
                        obj.ChiTieuT2 = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                        obj.NPCIsOpenT2 = lstData.FirstOrDefault(o => o.Thang == 2).IsOpen;
                        obj.ChiTieuT2SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 2)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 3))
                        {
                            try
                            {
                                obj.SoVuSauMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT2 = lstData.FirstOrDefault(o => o.Thang == 2).SoVuSauMT;
                            //obj.SoVuTruocMTT2 = lstData.FirstOrDefault(o => o.Thang == 2).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    #endregion

                    #region T3
                    if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                    {
                        obj.IdT3 = lstData.FirstOrDefault(o => o.Thang == 3).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuNam;
                        obj.ChiTieuT3 = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                        obj.NPCIsOpenT3 = lstData.FirstOrDefault(o => o.Thang == 3).IsOpen;
                        obj.ChiTieuT3SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 3 && DateTime.Now.Year > 2020)
                           || (DateTime.Now.Year == Date && DateTime.Now.Month == 4 && DateTime.Now.Year > 2020))
                        {
                            try
                            {
                                obj.SoVuSauMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT3 = lstData.FirstOrDefault(o => o.Thang == 3).SoVuSauMT;
                            //obj.SoVuTruocMTT3 = lstData.FirstOrDefault(o => o.Thang == 3).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T4
                    if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                    {
                        obj.IdT4 = lstData.FirstOrDefault(o => o.Thang == 4).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuNam;
                        obj.ChiTieuT4 = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                        obj.NPCIsOpenT4 = lstData.FirstOrDefault(o => o.Thang == 4).IsOpen;
                        obj.ChiTieuT4SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 4)
                           || (DateTime.Now.Year == Date && DateTime.Now.Month == 5))
                        {
                            try
                            {
                                obj.SoVuSauMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT4 = lstData.FirstOrDefault(o => o.Thang == 4).SoVuSauMT;
                            //obj.SoVuTruocMTT4 = lstData.FirstOrDefault(o => o.Thang == 4).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T5
                    if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                    {
                        obj.IdT5 = lstData.FirstOrDefault(o => o.Thang == 5).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuNam;
                        obj.ChiTieuT5 = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                        obj.NPCIsOpenT5 = lstData.FirstOrDefault(o => o.Thang == 5).IsOpen;
                        obj.ChiTieuT5SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 5)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 6))
                        {
                            try
                            {
                                obj.SoVuSauMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT5 = lstData.FirstOrDefault(o => o.Thang == 5).SoVuSauMT;
                            //obj.SoVuTruocMTT5 = lstData.FirstOrDefault(o => o.Thang == 5).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T6
                    if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                    {
                        obj.IdT6 = lstData.FirstOrDefault(o => o.Thang == 6).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuNam;
                        obj.ChiTieuT6 = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                        obj.NPCIsOpenT6 = lstData.FirstOrDefault(o => o.Thang == 6).IsOpen;
                        obj.ChiTieuT6SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 6)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 7))
                        {
                            try
                            {
                                obj.SoVuSauMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT6 = lstData.FirstOrDefault(o => o.Thang == 6).SoVuSauMT;
                            //obj.SoVuTruocMTT6 = lstData.FirstOrDefault(o => o.Thang == 6).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T7
                    if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                    {
                        obj.IdT7 = lstData.FirstOrDefault(o => o.Thang == 7).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuNam;
                        obj.ChiTieuT7 = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                        obj.NPCIsOpenT7 = lstData.FirstOrDefault(o => o.Thang == 7).IsOpen;
                        obj.ChiTieuT7SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 7)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 8))
                        {
                            try
                            {
                                obj.SoVuSauMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT7 = lstData.FirstOrDefault(o => o.Thang == 7).SoVuSauMT;
                            //obj.SoVuTruocMTT7 = lstData.FirstOrDefault(o => o.Thang == 7).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T8
                    if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                    {
                        obj.IdT8 = lstData.FirstOrDefault(o => o.Thang == 8).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuNam;
                        obj.ChiTieuT8 = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                        obj.NPCIsOpenT8 = lstData.FirstOrDefault(o => o.Thang == 8).IsOpen;
                        obj.ChiTieuT8SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 8)
                           || (DateTime.Now.Year == Date && DateTime.Now.Month == 9))
                        {
                            try
                            {
                                obj.SoVuSauMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT8 = lstData.FirstOrDefault(o => o.Thang == 8).SoVuSauMT;
                            //obj.SoVuTruocMTT8 = lstData.FirstOrDefault(o => o.Thang == 8).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T9
                    if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                    {
                        obj.IdT9 = lstData.FirstOrDefault(o => o.Thang == 9).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuNam;
                        obj.ChiTieuT9 = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                        obj.NPCIsOpenT9 = lstData.FirstOrDefault(o => o.Thang == 9).IsOpen;
                        obj.ChiTieuT9SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 9)
                           || (DateTime.Now.Year == Date && DateTime.Now.Month == 10))
                        {
                            try
                            {
                                obj.SoVuSauMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT9 = lstData.FirstOrDefault(o => o.Thang == 9).SoVuSauMT;
                            //obj.SoVuTruocMTT9 = lstData.FirstOrDefault(o => o.Thang == 9).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T10
                    if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                    {
                        obj.IdT10 = lstData.FirstOrDefault(o => o.Thang == 10).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuNam;
                        obj.ChiTieuT10 = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                        obj.NPCIsOpenT10 = lstData.FirstOrDefault(o => o.Thang == 10).IsOpen;
                        obj.ChiTieuT10SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 10)
                             || (DateTime.Now.Year == Date && DateTime.Now.Month == 11))
                        {
                            try
                            {
                                obj.SoVuSauMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT10 = lstData.FirstOrDefault(o => o.Thang == 10).SoVuSauMT;
                            //obj.SoVuTruocMTT10 = lstData.FirstOrDefault(o => o.Thang == 10).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T11
                    if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                    {
                        obj.IdT11 = lstData.FirstOrDefault(o => o.Thang == 11).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuNam;
                        obj.ChiTieuT11 = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                        obj.NPCIsOpenT11 = lstData.FirstOrDefault(o => o.Thang == 11).IsOpen;
                        obj.ChiTieuT11SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;

                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 11)
                            || (DateTime.Now.Year == Date && DateTime.Now.Month == 12))
                        {
                            try
                            {
                                obj.SoVuSauMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT11 = lstData.FirstOrDefault(o => o.Thang == 11).SoVuSauMT;
                            //obj.SoVuTruocMTT11 = lstData.FirstOrDefault(o => o.Thang == 11).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    #region T12
                    if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                    {
                        obj.IdT12 = lstData.FirstOrDefault(o => o.Thang == 12).Id;
                        obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuNam;
                        obj.ChiTieuT12 = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                        obj.NPCIsOpenT12 = lstData.FirstOrDefault(o => o.Thang == 12).IsOpen;
                        obj.ChiTieuT12SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;

                        //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 12)
                        //  || (DateTime.Now.AddYears(1).Year == Date && DateTime.Now.Month == 1))
                        if ((DateTime.Now.Year == Date && DateTime.Now.Month == 12)
                            || (DateTime.Now.Year == Date + 1 && DateTime.Now.Month == 1))
                        {
                            try
                            {
                                obj.SoVuSauMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                                    - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                                    - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                                    );
                            }
                            catch (Exception ex)
                            { }
                        }
                        else
                        {
                            obj.SoVuSauMTT12 = lstData.FirstOrDefault(o => o.Thang == 12).SoVuSauMT;
                            //obj.SoVuTruocMTT12 = lstData.FirstOrDefault(o => o.Thang == 12).SoVuTruocMT;
                        }

                        try
                        {
                            obj.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }

                    }
                    #endregion

                    model.Add(obj);
                }

                #region Theo loai
                List<ChiTieuSuCoTheoLoaiModels> lstTheoLoai = new List<ChiTieuSuCoTheoLoaiModels>();
                ChiTieuSuCoTheoLoaiModels objtl = new ChiTieuSuCoTheoLoaiModels();
                objtl.Ten = "số vụ do khách hàng gây ra";
                var lstDataCongTyTheoLoai = _chitieu_ser.GetListChiTieuTheoLoai(Date, MaDV, -1);
                objtl.ChiTieuNamCongTy = lstDataCongTyTheoLoai != null ? lstDataCongTyTheoLoai.FirstOrDefault(o => o.Thang == 1).ChiTieuNam : 0;

                try
                {
                    objtl.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
                try
                {
                    objtl.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }

                var lstDataTheoLoai = _chitieu_ser.GetListChiTieuTheoLoai(Date, item.Id, -1);
                if (lstDataTheoLoai != null)
                {
                    #region T1
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1) != null)
                    {
                        objtl.IdT1 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).ChiTieuNam;
                        objtl.ChiTieuT1 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                        objtl.ChiTieuT1SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T2
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2) != null)
                    {
                        objtl.IdT2 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).ChiTieuNam;
                        objtl.ChiTieuT2 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                        objtl.ChiTieuT2SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T3
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3) != null)
                    {
                        objtl.IdT3 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).ChiTieuNam;
                        objtl.ChiTieuT3 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                        objtl.ChiTieuT3SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T4
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4) != null)
                    {
                        objtl.IdT4 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).ChiTieuNam;
                        objtl.ChiTieuT4 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                        objtl.ChiTieuT4SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T5
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5) != null)
                    {
                        objtl.IdT5 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5).ChiTieuNam;
                        objtl.ChiTieuT5 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                        objtl.ChiTieuT5SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T6
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6) != null)
                    {
                        objtl.IdT6 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).ChiTieuNam;
                        objtl.ChiTieuT6 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                        objtl.ChiTieuT6SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T7
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7) != null)
                    {
                        objtl.IdT7 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).ChiTieuNam;
                        objtl.ChiTieuT7 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                        objtl.ChiTieuT7SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T8
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8) != null)
                    {
                        objtl.IdT8 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).ChiTieuNam;
                        objtl.ChiTieuT8 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                        objtl.ChiTieuT8SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T9
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9) != null)
                    {
                        objtl.IdT9 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).ChiTieuNam;
                        objtl.ChiTieuT9 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                        objtl.ChiTieuT9SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T10
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10) != null)
                    {
                        objtl.IdT10 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).ChiTieuNam;
                        objtl.ChiTieuT10 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                        objtl.ChiTieuT10SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T11
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11) != null)
                    {
                        objtl.IdT11 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).ChiTieuNam;
                        objtl.ChiTieuT11 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                        objtl.ChiTieuT11SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion

                    #region T12
                    if (lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12) != null)
                    {
                        objtl.IdT12 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).Id;
                        objtl.ChiTieuNam = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).ChiTieuNam;
                        objtl.ChiTieuT12 = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                        objtl.ChiTieuT12SauDieuChuyen = lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;
                    }
                    #endregion
                }

                lstTheoLoai.Add(objtl);

                obj.chitieutheoloais = lstTheoLoai;
                #endregion
            }

            #region Check khoa
            try
            {
                ViewBag.NPCIsOpen = lstChiTieuCongTy.Any(o => o.IsOpen.GetValueOrDefault());
            }
            catch (Exception ex)
            { }
            #endregion

            return PartialView("List", model);
        }

        public ActionResult Save(int Date, string[] MaDV
           , int?[] IdT1, int?[] IdT2, int?[] IdT3, int?[] IdT4, int?[] IdT5
           , int?[] IdT6, int?[] IdT7, int?[] IdT8, int?[] IdT9, int?[] IdT10
           , int?[] IdT11, int?[] IdT12
           , int?[] IdKHT1, int?[] IdKHT2, int?[] IdKHT3, int?[] IdKHT4, int?[] IdKHT5
           , int?[] IdKHT6, int?[] IdKHT7, int?[] IdKHT8, int?[] IdKHT9, int?[] IdKHT10
           , int?[] IdKHT11, int?[] IdKHT12
           , int?[] ChiTieuT1, int?[] ChiTieuT2, int?[] ChiTieuT3, int?[] ChiTieuT4
           , int?[] ChiTieuT5, int?[] ChiTieuT6, int?[] ChiTieuT7, int?[] ChiTieuT8
           , int?[] ChiTieuT9, int?[] ChiTieuT10, int?[] ChiTieuT11, int?[] ChiTieuT12, int?[] ChiTieuNam
           , int?[] ChiTieuKHT1, int?[] ChiTieuKHT2, int?[] ChiTieuKHT3, int?[] ChiTieuKHT4
           , int?[] ChiTieuKHT5, int?[] ChiTieuKHT6, int?[] ChiTieuKHT7, int?[] ChiTieuKHT8
           , int?[] ChiTieuKHT9, int?[] ChiTieuKHT10, int?[] ChiTieuKHT11, int?[] ChiTieuKHT12, int?[] ChiTieuKHNam
            )
        {
            string strError = "";
            try
            {
                if (MaDV == null)
                {
                    return Json(new { type = "error", mess = "Không tìm thấy đơn vị !" }, JsonRequestBehavior.AllowGet);
                }

                for (int i = 0; i < MaDV.Count(); i++)
                {

                    #region T1
                    if ((IdT1[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 1;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT1[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT1[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT1[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT1[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 1;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT1[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT1[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT1[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T2
                    if ((IdT2[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 2;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT2[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT2[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT2[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT2[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 2;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT2[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT2[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT2[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T3
                    if ((IdT3[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 3;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        // objct.ChiTieuThang = ChiTieuT3[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT3[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT3[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT3[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 3;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT3[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT3[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT3[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T4
                    if ((IdT4[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 4;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT4[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT4[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT4[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT4[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 4;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT4[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT4[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT4[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T5
                    if ((IdT5[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 5;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT5[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT5[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT5[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT5[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 5;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT5[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT5[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT5[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T6
                    if ((IdT6[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 6;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT6[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT6[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT6[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT6[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 6;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT6[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT6[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT6[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T7
                    if ((IdT7[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 7;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT7[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT7[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT7[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT7[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 7;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT7[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT7[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT7[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T8
                    if ((IdT8[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 8;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT8[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT8[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT8[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT8[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 8;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT8[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT8[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT8[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T9
                    if ((IdT9[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 9;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT9[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT9[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT9[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT9[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 9;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT9[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT9[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT9[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T10
                    if ((IdT10[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 10;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT10[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT10[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT10[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT10[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 10;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT10[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT10[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT10[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T11
                    if ((IdT11[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 11;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT11[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT11[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT11[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT11[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 11;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT11[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT11[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT11[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                    #region T12
                    if ((IdT12[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 12;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT12[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdT12[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuNam[i];
                        //objct.ChiTieuThang = ChiTieuT12[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }

                    if ((IdKHT12[i] ?? 0) == 0)
                    {
                        sc_ChiTieuSoVuSuCo objct = new sc_ChiTieuSoVuSuCo();
                        objct.Nam = Date;
                        objct.Thang = 12;
                        objct.DonViId = MaDV[i];
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT12[i];
                        objct.CongTNguoiNhap = User.Identity.Name;
                        objct.CongTyNgayNhap = DateTime.Now;
                        objct.LoaiSuCoId = -1;
                        objct.IsOpen = true;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieu_ser.GetById(IdKHT12[i] ?? 0);
                        objct.ChiTieuNam = ChiTieuKHNam[i];
                        //objct.ChiTieuThang = ChiTieuKHT12[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    #endregion

                }

                return Json(new { type = "success", mess = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Lỗi trong quá trình lưu dữ liệu: " + ex.Message + "<br/>" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Khoa(bool IsOpen, int Date, string[] MaDV
           , int?[] IdT1, int?[] IdT2, int?[] IdT3, int?[] IdT4, int?[] IdT5
           , int?[] IdT6, int?[] IdT7, int?[] IdT8, int?[] IdT9, int?[] IdT10
           , int?[] IdT11, int?[] IdT12
           , int?[] SoVuSauMTT1, int?[] SoVuSauMTT2, int?[] SoVuSauMTT3, int?[] SoVuSauMTT4, int?[] SoVuSauMTT5
            , int?[] SoVuSauMTT6, int?[] SoVuSauMTT7, int?[] SoVuSauMTT8, int?[] SoVuSauMTT9, int?[] SoVuSauMTT10
            , int?[] SoVuSauMTT11, int?[] SoVuSauMTT12
            , int?[] SoVuTruocMTT1, int?[] SoVuTruocMTT2, int?[] SoVuTruocMTT3, int?[] SoVuTruocMTT4, int?[] SoVuTruocMTT5
            , int?[] SoVuTruocMTT6, int?[] SoVuTruocMTT7, int?[] SoVuTruocMTT8, int?[] SoVuTruocMTT9, int?[] SoVuTruocMTT10
            , int?[] SoVuTruocMTT11, int?[] SoVuTruocMTT12
            )
        {
            string kt = "";
            string strError = "";
            string error = "";
            try
            {
                for (int i = 0; i < MaDV.Count(); i++)
                {
                    var lstDataTheoLoai = _chitieu_ser.GetListChiTieuTheoLoai(Date, MaDV[i], -1);
                    #region T1
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT1[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT1[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT1[i];
                        objct.SoVuTruocMT = SoVuTruocMTT1[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT1[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 1)
                    {
                        var objct = _chitieu_ser.GetById(IdT1[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT1[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT1[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 1).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T2
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT2[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT2[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT2[i];
                        objct.SoVuTruocMT = SoVuTruocMTT2[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT2[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 2)
                    {
                        var objct = _chitieu_ser.GetById(IdT2[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT2[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT2[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 2).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T3
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT3[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT3[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT3[i];
                        objct.SoVuTruocMT = SoVuTruocMTT3[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT3[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 3)
                    {
                        var objct = _chitieu_ser.GetById(IdT3[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT3[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT3[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 3).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T4
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT4[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT4[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT4[i];
                        objct.SoVuTruocMT = SoVuTruocMTT4[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT4[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 4)
                    {
                        var objct = _chitieu_ser.GetById(IdT4[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT4[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT4[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 4).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T5
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT5[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT5[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT5[i];
                        objct.SoVuTruocMT = SoVuTruocMTT5[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT5[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 5)
                    {
                        var objct = _chitieu_ser.GetById(IdT5[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT5[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT5[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 5) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 51).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T6
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT6[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT6[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT6[i];
                        objct.SoVuTruocMT = SoVuTruocMTT6[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT6[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 6)
                    {
                        var objct = _chitieu_ser.GetById(IdT6[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT6[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT6[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 6).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T7
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT7[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT7[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT7[i];
                        objct.SoVuTruocMT = SoVuTruocMTT7[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT7[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 7)
                    {
                        var objct = _chitieu_ser.GetById(IdT7[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT7[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT7[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 7).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T8
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT8[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT8[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT8[i];
                        objct.SoVuTruocMT = SoVuTruocMTT8[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT8[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 8)
                    {
                        var objct = _chitieu_ser.GetById(IdT8[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT8[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT8[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 8).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T9
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT9[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT9[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT9[i];
                        objct.SoVuTruocMT = SoVuTruocMTT9[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT9[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 9)
                    {
                        var objct = _chitieu_ser.GetById(IdT9[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT9[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT9[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 9).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T10
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT10[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT10[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT10[i];
                        objct.SoVuTruocMT = SoVuTruocMTT10[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT10[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 10)
                    {
                        var objct = _chitieu_ser.GetById(IdT10[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT10[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT10[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 10).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T11
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT11[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT11[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT11[i];
                        objct.SoVuTruocMT = SoVuTruocMTT11[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT11[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 11)
                    {
                        var objct = _chitieu_ser.GetById(IdT11[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT11[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT11[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 11).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region T12
                    //update lai so lieu so vu sau mien tru truoc khi mo khoa
                    if ((IdT12[i] ?? 0) > 0 && IsOpen)
                    {
                        var objct = _chitieu_ser.GetById(IdT12[i] ?? 0);
                        objct.SoVuSauMT = SoVuSauMTT12[i];
                        objct.SoVuTruocMT = SoVuTruocMTT12[i];
                        objct.CongTNguoiSua = User.Identity.Name;
                        objct.CongTNgaySua = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);
                    }
                    if ((IdT12[i] ?? 0) > 0 && DateTime.Now.Year == Date && DateTime.Now.Month <= 12)
                    {
                        var objct = _chitieu_ser.GetById(IdT12[i] ?? 0);
                        objct.IsOpen = IsOpen;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).Id);
                            objctloai.IsOpen = IsOpen;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    else if ((IdT12[i] ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT12[i] ?? 0);
                        objct.IsOpen = false;
                        objct.NguoiMo = User.Identity.Name;
                        objct.NgayMo = DateTime.Now;
                        var id = _chitieu_ser.Update(objct, ref strError);

                        if (lstDataTheoLoai != null && lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12) != null)
                        {
                            var objctloai = _chitieu_ser.GetById(lstDataTheoLoai.FirstOrDefault(o => o.Thang == 12).Id);
                            objctloai.IsOpen = false;
                            objctloai.NguoiMo = User.Identity.Name;
                            objctloai.NgayMo = DateTime.Now;
                            var idloai = _chitieu_ser.Update(objctloai, ref strError);
                        }
                    }
                    #endregion

                    #region Can lai so lieu
                    //if (DateTime.Now.Month != 1)
                    //{
                    //    var objThangHT = _chitieu_ser.ChiTieuNam_Search(strcon, DateTime.Now.Year, DateTime.Now.Month, MaDV[i]);
                    //    var objThangTruoc = _chitieu_ser.ChiTieuNam_Search(strcon, DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, MaDV[i]);
                    //    if (objThangTruoc.ChiTieuThangSauDieuChuyen <= 0)
                    //    {
                    //        error = error + "Chưa nhập chỉ tiêu tháng sau điều chuyển cho tháng " + DateTime.Now.AddMonths(-1).Month + "\n";
                    //    }
                    //    else
                    //    {
                    //        var lstData = _chitieu_ser.GetListChiTieu(Date, MaDV[i]);
                    //        if (objThangHT != null && objThangTruoc != null && IsOpen)
                    //        {
                    //            var chitieunam = objThangHT.ChiTieuNam ?? 0;
                    //            var tongsovu = lstData.Sum(o => o.SoVuSauMT ?? 0);

                    //            if (tongsovu <= chitieunam)
                    //            {
                    //                var sovuconlai = (objThangTruoc.ChiTieuThangSauDieuChuyen ?? (objThangTruoc.ChiTieuThang ?? 0)) - (objThangTruoc.SoVuSauMT ?? 0);
                    //                objThangHT.ChiTieuThangSauDieuChuyen = (objThangHT.ChiTieuThangSauDieuChuyen ?? (objThangTruoc.ChiTieuThang ?? 0)) + sovuconlai;
                    //                var idht = _chitieu_ser.Update(objThangHT, ref strError);

                    //                objThangTruoc.ChiTieuThangSauDieuChuyen = objThangTruoc.SoVuSauMT ?? 0;
                    //                var idtt = _chitieu_ser.Update(objThangTruoc, ref strError);
                    //            }
                    //            else
                    //            {
                    //                foreach (var item in lstData.Where(o => o.Nam == DateTime.Now.Year && o.Thang >= DateTime.Now.Month))
                    //                {
                    //                    item.ChiTieuThangSauDieuChuyen = 0;
                    //                    var id = _chitieu_ser.Update(item, ref strError);
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                    #endregion

                }
                return Json(new { type = "success", mess = "Lưu dữ liệu thành công ! \n" + error }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                kt = ex.Message;
                return Json(new { type = "error", mess = "Không lưu được dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HasCredential(MenuCode = "CTNSCDV;DK")]
        public ActionResult IndexDonVi()
        {
            return View();
        }

        public ActionResult ListDonVi(int Date)
        {
            string MaDV = Session["DonViID"].ToString();
            var objDV = _DonVi_ser.GetById(MaDV);
            DonViChiTieuSuCoViewModels obj = new DonViChiTieuSuCoViewModels();
            obj.TEN_DVIQLY = objDV.TenDonVi;
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
                    obj.NPCIsOpenT1 = lstData.FirstOrDefault(o => o.Thang == 1).IsOpen;
                    obj.ChiTieuT1SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 1)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 2))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT1 = lstData.FirstOrDefault(o => o.Thang == 1).SoVuSauMT;
                    //    //obj.SoVuTruocMTT1 = lstData.FirstOrDefault(o => o.Thang == 1).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T2
                if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                {
                    obj.IdT2 = lstData.FirstOrDefault(o => o.Thang == 2).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuNam;
                    obj.ChiTieuT2 = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                    obj.NPCIsOpenT2 = lstData.FirstOrDefault(o => o.Thang == 2).IsOpen;
                    obj.ChiTieuT2SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 2)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 3))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT2 = lstData.FirstOrDefault(o => o.Thang == 2).SoVuSauMT;
                    //    //obj.SoVuTruocMTT2 = lstData.FirstOrDefault(o => o.Thang == 2).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T3
                if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                {
                    obj.IdT3 = lstData.FirstOrDefault(o => o.Thang == 3).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuNam;
                    obj.ChiTieuT3 = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                    obj.NPCIsOpenT3 = lstData.FirstOrDefault(o => o.Thang == 3).IsOpen;
                    obj.ChiTieuT3SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 3 && DateTime.Now.Year > 2020)
                    //      || (DateTime.Now.Year == Date && DateTime.Now.Month == 4 && DateTime.Now.Year > 2020))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT3 = lstData.FirstOrDefault(o => o.Thang == 3).SoVuSauMT;
                    //    //obj.SoVuTruocMTT3 = lstData.FirstOrDefault(o => o.Thang == 3).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T4
                if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                {
                    obj.IdT4 = lstData.FirstOrDefault(o => o.Thang == 4).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuNam;
                    obj.ChiTieuT4 = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                    obj.NPCIsOpenT4 = lstData.FirstOrDefault(o => o.Thang == 4).IsOpen;
                    obj.ChiTieuT4SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 4)
                    //       || (DateTime.Now.Year == Date && DateTime.Now.Month == 5))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT4 = lstData.FirstOrDefault(o => o.Thang == 4).SoVuSauMT;
                    //    //obj.SoVuTruocMTT4 = lstData.FirstOrDefault(o => o.Thang == 4).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }

                }
                #endregion

                #region T5
                if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                {
                    obj.IdT5 = lstData.FirstOrDefault(o => o.Thang == 5).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuNam;
                    obj.ChiTieuT5 = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                    obj.NPCIsOpenT5 = lstData.FirstOrDefault(o => o.Thang == 5).IsOpen;
                    obj.ChiTieuT5SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 5)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 6))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT5 = lstData.FirstOrDefault(o => o.Thang == 5).SoVuSauMT;
                    //    //obj.SoVuTruocMTT5 = lstData.FirstOrDefault(o => o.Thang == 5).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T6
                if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                {
                    obj.IdT6 = lstData.FirstOrDefault(o => o.Thang == 6).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuNam;
                    obj.ChiTieuT6 = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                    obj.NPCIsOpenT6 = lstData.FirstOrDefault(o => o.Thang == 6).IsOpen;
                    obj.ChiTieuT6SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 6)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 7))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT6 = lstData.FirstOrDefault(o => o.Thang == 6).SoVuSauMT;
                    //    //obj.SoVuTruocMTT6 = lstData.FirstOrDefault(o => o.Thang == 6).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T7
                if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                {
                    obj.IdT7 = lstData.FirstOrDefault(o => o.Thang == 7).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuNam;
                    obj.ChiTieuT7 = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                    obj.NPCIsOpenT7 = lstData.FirstOrDefault(o => o.Thang == 7).IsOpen;
                    obj.ChiTieuT7SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 7)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 8))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT7 = lstData.FirstOrDefault(o => o.Thang == 7).SoVuSauMT;
                    //    //obj.SoVuTruocMTT7 = lstData.FirstOrDefault(o => o.Thang == 7).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T8
                if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                {
                    obj.IdT8 = lstData.FirstOrDefault(o => o.Thang == 8).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuNam;
                    obj.ChiTieuT8 = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                    obj.NPCIsOpenT8 = lstData.FirstOrDefault(o => o.Thang == 8).IsOpen;
                    obj.ChiTieuT8SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 8)
                    //       || (DateTime.Now.Year == Date && DateTime.Now.Month == 9))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT8 = lstData.FirstOrDefault(o => o.Thang == 8).SoVuSauMT;
                    //    //obj.SoVuTruocMTT8 = lstData.FirstOrDefault(o => o.Thang == 8).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T9
                if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                {
                    obj.IdT9 = lstData.FirstOrDefault(o => o.Thang == 9).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuNam;
                    obj.ChiTieuT9 = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                    obj.NPCIsOpenT9 = lstData.FirstOrDefault(o => o.Thang == 9).IsOpen;
                    obj.ChiTieuT9SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 9)
                    //       || (DateTime.Now.Year == Date && DateTime.Now.Month == 10))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT9 = lstData.FirstOrDefault(o => o.Thang == 9).SoVuSauMT;
                    //    //obj.SoVuTruocMTT9 = lstData.FirstOrDefault(o => o.Thang == 9).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }

                #endregion

                #region T10
                if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                {
                    obj.IdT10 = lstData.FirstOrDefault(o => o.Thang == 10).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuNam;
                    obj.ChiTieuT10 = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                    obj.NPCIsOpenT10 = lstData.FirstOrDefault(o => o.Thang == 10).IsOpen;
                    obj.ChiTieuT10SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 10)
                    //         || (DateTime.Now.Year == Date && DateTime.Now.Month == 11))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT10 = lstData.FirstOrDefault(o => o.Thang == 10).SoVuSauMT;
                    //    //obj.SoVuTruocMTT10 = lstData.FirstOrDefault(o => o.Thang == 10).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }

                }
                #endregion

                #region T11
                if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                {
                    obj.IdT11 = lstData.FirstOrDefault(o => o.Thang == 11).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuNam;
                    obj.ChiTieuT11 = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                    obj.NPCIsOpenT11 = lstData.FirstOrDefault(o => o.Thang == 11).IsOpen;
                    obj.ChiTieuT11SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 11)
                    //        || (DateTime.Now.Year == Date && DateTime.Now.Month == 12))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT11 = lstData.FirstOrDefault(o => o.Thang == 11).SoVuSauMT;
                    //    //obj.SoVuTruocMTT11 = lstData.FirstOrDefault(o => o.Thang == 11).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion

                #region T12
                if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                {
                    obj.IdT12 = lstData.FirstOrDefault(o => o.Thang == 12).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuNam;
                    obj.ChiTieuT12 = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                    obj.NPCIsOpenT12 = lstData.FirstOrDefault(o => o.Thang == 12).IsOpen;
                    obj.ChiTieuT12SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;

                    //if ((DateTime.Now.Year == Date && DateTime.Now.Month == 12)
                    //        || (DateTime.Now.AddYears(1).Year == Date && DateTime.Now.Month == 1))
                    //{
                    //    try
                    //    {
                    //        obj.SoVuSauMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", ""))
                    //            - (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "", "", true, "dachuyen", "")
                    //            - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "true", "110", "", true, "dachuyen", "")
                    //            );
                    //    }
                    //    catch (Exception ex)
                    //    { }
                    //}
                    //else
                    //{
                    //    obj.SoVuSauMTT12 = lstData.FirstOrDefault(o => o.Thang == 12).SoVuSauMT;
                    //    //obj.SoVuTruocMTT12 = lstData.FirstOrDefault(o => o.Thang == 12).SoVuTruocMT;
                    //}

                    if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
                    {
                        try
                        {
                            obj.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        try
                        {
                            obj.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                                - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "")
                                );
                        }
                        catch (Exception ex)
                        { }
                    }
                }
                #endregion
            }

            return PartialView("ListDonVi", obj);
        }

        public ActionResult SaveDonVi(int Date, string MaDV, int? SuatNam
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
                    if (objct.IsOpen.GetValueOrDefault())
                    {
                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT1;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT1;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT2;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT2;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT3;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT3;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT4;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT4;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {
                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT5;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT5;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT6;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT6;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT7;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT7;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT8;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT8;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT9;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT9;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT10;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT10;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT11;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT11;

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
                    if (objct.IsOpen.GetValueOrDefault())
                    {

                        if (objct.ChiTieuThang == null)
                            objct.ChiTieuThang = SuatT12;
                        else
                            objct.ChiTieuThangSauDieuChuyen = SuatT12;

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

        public ActionResult IndexThoiGian()
        {
            return View();
        }

        public ActionResult ListThoiGian(int Date)
        {
            var objDV = _DonVi_ser.List().FirstOrDefault(o => (o.Id.Length == 4) || o.Id.ToUpper() == "PH" || o.Id.ToUpper() == "PN" || o.Id.ToUpper() == "PM");
            string MaDV = "";
            if (objDV != null)
            {
                MaDV = objDV.Id;
            }
            //string MaDV = Session["DonViID"].ToString();
            var ChiTieuCongTy = _chitieuthoigian_ser.GetListChiTieu(Date, MaDV);
            var lstdv = _DonVi_ser.List().Where(o => o.Id != MaDV && o.MaLP == 1);
            List<DonViChiTieuThoiGianSuCoViewModels> model = new List<DonViChiTieuThoiGianSuCoViewModels>();

            foreach (var item in lstdv)
            {
                DonViChiTieuThoiGianSuCoViewModels obj = new DonViChiTieuThoiGianSuCoViewModels();
                obj.MA_DVIQLY = item.Id;
                obj.TEN_DVIQLY = item.TenDonVi;

                var data = _chitieuthoigian_ser.GetListChiTieu(Date, item.Id);
                if (data != null)
                {
                    obj.Id = data.Id;
                    obj.Nam = data.Nam;
                    obj.TrungBinhNam = Math.Round(data.TrungBinhNam ?? 0, 2);
                    obj.Thang1 = Math.Round(data.Thang1 ?? 0, 2);
                    obj.Thang2 = Math.Round(data.Thang2 ?? 0, 2);
                    obj.Thang3 = Math.Round(data.Thang3 ?? 0, 2);
                    obj.Thang4 = Math.Round(data.Thang4 ?? 0, 2);
                    obj.Thang5 = Math.Round(data.Thang5 ?? 0, 2);
                    obj.Thang6 = Math.Round(data.Thang6 ?? 0, 2);
                    obj.Thang7 = Math.Round(data.Thang7 ?? 0, 2);
                    obj.Thang8 = Math.Round(data.Thang8 ?? 0, 2);
                    obj.Thang9 = Math.Round(data.Thang9 ?? 0, 2);
                    obj.Thang10 = Math.Round(data.Thang10 ?? 0, 2);
                    obj.Thang11 = Math.Round(data.Thang11 ?? 0, 2);
                    obj.Thang12 = Math.Round(data.Thang12 ?? 0, 2);

                    obj.TrungBinhNamDC = Math.Round(data.TrungBinhNamDC ?? 0, 2);
                    obj.Thang1DC = Math.Round(data.Thang1DC ?? 0, 2);
                    obj.Thang2DC = Math.Round(data.Thang2DC ?? 0, 2);
                    obj.Thang3DC = Math.Round(data.Thang3DC ?? 0, 2);
                    obj.Thang4DC = Math.Round(data.Thang4DC ?? 0, 2);
                    obj.Thang5DC = Math.Round(data.Thang5DC ?? 0, 2);
                    obj.Thang6DC = Math.Round(data.Thang6DC ?? 0, 2);
                    obj.Thang7DC = Math.Round(data.Thang7DC ?? 0, 2);
                    obj.Thang8DC = Math.Round(data.Thang8DC ?? 0, 2);
                    obj.Thang9DC = Math.Round(data.Thang9DC ?? 0, 2);
                    obj.Thang10DC = Math.Round(data.Thang10DC ?? 0, 2);
                    obj.Thang11DC = Math.Round(data.Thang11DC ?? 0, 2);
                    obj.Thang12DC = Math.Round(data.Thang12DC ?? 0, 2);

                    obj.IsOpenT1 = data.IsOpenT1;
                    obj.IsOpenT2 = data.IsOpenT2;
                    obj.IsOpenT3 = data.IsOpenT3;
                    obj.IsOpenT4 = data.IsOpenT4;
                    obj.IsOpenT5 = data.IsOpenT5;
                    obj.IsOpenT6 = data.IsOpenT6;
                    obj.IsOpenT7 = data.IsOpenT7;
                    obj.IsOpenT8 = data.IsOpenT8;
                    obj.IsOpenT9 = data.IsOpenT9;
                    obj.IsOpenT10 = data.IsOpenT10;
                    obj.IsOpenT11 = data.IsOpenT11;
                    obj.IsOpenT12 = data.IsOpenT12;
                    obj.IsOpenNam = data.IsOpenNam;

                    #region Thoi gian theo thang
                    double? TTGT1 = 0, TTGT2 = 0, TTGT3 = 0, TTGT4 = 0, TTGT5 = 0, TTGT6 = 0
                        , TTGT7 = 0, TTGT8 = 0, TTGT9 = 0, TTGT10 = 0, TTGT11 = 0, TTGT12 = 0;
                    double? TTGT1DC = 0, TTGT2DC = 0, TTGT3DC = 0, TTGT4DC = 0, TTGT5DC = 0, TTGT6DC = 0
                       , TTGT7DC = 0, TTGT8DC = 0, TTGT9DC = 0, TTGT10DC = 0, TTGT11DC = 0, TTGT12DC = 0;
                    var lstData = _chitieu_ser.GetListChiTieu(Date, item.Id);
                    if (lstData != null)
                    {
                        if (lstData.FirstOrDefault(o => o.Thang == 1) != null)
                        {
                            try
                            {
                                TTGT1 = obj.Thang1 * lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                                obj.TGT1 = string.Format("{0:#,0.##}", obj.Thang1) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT1);

                                TTGT1DC = obj.Thang1DC * lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;
                                obj.TGT1DC = string.Format("{0:#,0.##}", obj.Thang1DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT1DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                        {
                            try
                            {
                                TTGT2 = obj.Thang2 * lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                                obj.TGT2 = string.Format("{0:#,0.##}", obj.Thang2) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT2);

                                TTGT2DC = obj.Thang2DC * lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;
                                obj.TGT2DC = string.Format("{0:#,0.##}", obj.Thang2DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT2DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                        {
                            try
                            {
                                TTGT3 = obj.Thang3 * lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                                obj.TGT3 = string.Format("{0:#,0.##}", obj.Thang3) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT3);

                                TTGT3DC = obj.Thang3DC * lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;
                                obj.TGT3DC = string.Format("{0:#,0.##}", obj.Thang3DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT3DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                        {
                            try
                            {
                                TTGT4 = obj.Thang4 * lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                                obj.TGT4 = string.Format("{0:#,0.##}", obj.Thang4) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT4);

                                TTGT4DC = obj.Thang4DC * lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;
                                obj.TGT4DC = string.Format("{0:#,0.##}", obj.Thang4DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT4DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                        {
                            try
                            {
                                TTGT5 = obj.Thang5 * lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                                obj.TGT5 = string.Format("{0:#,0.##}", obj.Thang5) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT5);

                                TTGT5DC = obj.Thang5DC * lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;
                                obj.TGT5DC = string.Format("{0:#,0.##}", obj.Thang5DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT5DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                        {
                            try
                            {
                                TTGT6 = obj.Thang6 * lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                                obj.TGT6 = string.Format("{0:#,0.##}", obj.Thang6) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT6);

                                TTGT6DC = obj.Thang6DC * lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;
                                obj.TGT6DC = string.Format("{0:#,0.##}", obj.Thang6DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT6DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                        {
                            try
                            {
                                TTGT7 = obj.Thang7 * lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                                obj.TGT7 = string.Format("{0:#,0.##}", obj.Thang7) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT7);

                                TTGT7DC = obj.Thang7DC * lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;
                                obj.TGT7DC = string.Format("{0:#,0.##}", obj.Thang7DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT7DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                        {
                            try
                            {
                                TTGT8 = obj.Thang8 * lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                                obj.TGT8 = string.Format("{0:#,0.##}", obj.Thang8) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT8);

                                TTGT8DC = obj.Thang8DC * lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;
                                obj.TGT8DC = string.Format("{0:#,0.##}", obj.Thang8DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT8DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                        {
                            try
                            {
                                TTGT9 = obj.Thang9 * lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                                obj.TGT9 = string.Format("{0:#,0.##}", obj.Thang9) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT9);

                                TTGT9DC = obj.Thang9DC * lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;
                                obj.TGT9DC = string.Format("{0:#,0.##}", obj.Thang9DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT9DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                        {
                            try
                            {
                                TTGT10 = obj.Thang10 * lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                                obj.TGT10 = string.Format("{0:#,0.##}", obj.Thang10) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT10);

                                TTGT10DC = obj.Thang10DC * lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;
                                obj.TGT10DC = string.Format("{0:#,0.##}", obj.Thang10DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT10DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                        {
                            try
                            {
                                TTGT11 = obj.Thang11 * lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                                obj.TGT11 = string.Format("{0:#,0.##}", obj.Thang11) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT11);

                                TTGT11DC = obj.Thang11DC * lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;
                                obj.TGT11DC = string.Format("{0:#,0.##}", obj.Thang11DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT11DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                        {
                            try
                            {
                                TTGT12 = obj.Thang12 * lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                                obj.TGT12 = string.Format("{0:#,0.##}", obj.Thang12) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang)
                                    + "=" + string.Format("{0:#,0.##}", TTGT12);

                                TTGT12DC = obj.Thang12DC * lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;
                                obj.TGT12DC = string.Format("{0:#,0.##}", obj.Thang12DC) + "*"
                                    + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen)
                                    + "=" + string.Format("{0:#,0.##}", TTGT12DC);
                            }
                            catch (Exception ex)
                            { }
                        }
                    }

                    obj.TGTBN = (TTGT1 ?? 0) + (TTGT2 ?? 0) + (TTGT3 ?? 0) + (TTGT4 ?? 0) + (TTGT5 ?? 0)
                        + (TTGT6 ?? 0) + (TTGT7 ?? 0) + (TTGT8 ?? 0) + (TTGT9 ?? 0) + (TTGT10 ?? 0)
                        + (TTGT11 ?? 0) + (TTGT12 ?? 0);

                    obj.TGTBNDC = (TTGT1DC ?? 0) + (TTGT2DC ?? 0) + (TTGT3DC ?? 0) + (TTGT4DC ?? 0) + (TTGT5DC ?? 0)
                       + (TTGT6DC ?? 0) + (TTGT7DC ?? 0) + (TTGT8DC ?? 0) + (TTGT9DC ?? 0) + (TTGT10DC ?? 0)
                       + (TTGT11DC ?? 0) + (TTGT12DC ?? 0);

                    #endregion

                }

                #region thuc hien
                try
                {
                    var TGT1 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT1 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT1 = string.Format("{0:#,0.##}", TGT1) + "/" + string.Format("{0:#,0.##}", SoVuT1) + "=" + string.Format("{0:#,0.##}", (TGT1 / SoVuT1));
                    obj.THTGT1 = TGT1;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT2 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT2 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT2 = string.Format("{0:#,0.##}", TGT2) + "/" + string.Format("{0:#,0.##}", SoVuT2) + "=" + string.Format("{0:#,0.##}", (TGT2 / SoVuT2));
                    obj.THTGT2 = TGT2;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT3 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT3 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT3 = string.Format("{0:#,0.##}", TGT3) + "/" + string.Format("{0:#,0.##}", SoVuT3) + "=" + string.Format("{0:#,0.##}", (TGT3 / SoVuT3));
                    obj.THTGT3 = TGT3;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT4 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT4 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT4 = string.Format("{0:#,0.##}", TGT4) + "/" + string.Format("{0:#,0.##}", SoVuT4) + "=" + string.Format("{0:#,0.##}", (TGT4 / SoVuT4));
                    obj.THTGT4 = TGT4;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT5 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT5 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT5 = string.Format("{0:#,0.##}", TGT5) + "/" + string.Format("{0:#,0.##}", SoVuT5) + "=" + string.Format("{0:#,0.##}", (TGT5 / SoVuT5));
                    obj.THTGT5 = TGT5;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT6 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT6 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT6 = string.Format("{0:#,0.##}", TGT6) + "/" + string.Format("{0:#,0.##}", SoVuT6) + "=" + string.Format("{0:#,0.##}", (TGT6 / SoVuT6));
                    obj.THTGT6 = TGT6;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT7 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT7 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT7 = string.Format("{0:#,0.##}", TGT7) + "/" + string.Format("{0:#,0.##}", SoVuT7) + "=" + string.Format("{0:#,0.##}", (TGT7 / SoVuT7));
                    obj.THTGT7 = TGT7;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT8 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT8 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT8 = string.Format("{0:#,0.##}", TGT8) + "/" + string.Format("{0:#,0.##}", SoVuT8) + "=" + string.Format("{0:#,0.##}", (TGT8 / SoVuT8));
                    obj.THTGT8 = TGT8;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT9 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT9 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT9 = string.Format("{0:#,0.##}", TGT9) + "/" + string.Format("{0:#,0.##}", SoVuT9) + "=" + string.Format("{0:#,0.##}", (TGT9 / SoVuT9));
                    obj.THTGT9 = TGT9;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT10 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT10 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT10 = string.Format("{0:#,0.##}", TGT10) + "/" + string.Format("{0:#,0.##}", SoVuT10) + "=" + string.Format("{0:#,0.##}", (TGT10 / SoVuT10));
                    obj.THTGT10 = TGT10;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT11 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT11 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT11 = string.Format("{0:#,0.##}", TGT11) + "/" + string.Format("{0:#,0.##}", SoVuT11) + "=" + string.Format("{0:#,0.##}", (TGT11 / SoVuT11));
                    obj.THTGT11 = TGT11;
                }
                catch (Exception ex)
                { }

                try
                {
                    var TGT12 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT12 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), item.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT12 = string.Format("{0:#,0.##}", TGT12) + "/" + string.Format("{0:#,0.##}", SoVuT12) + "=" + string.Format("{0:#,0.##}", (TGT12 / SoVuT12));
                    obj.THTGT12 = TGT12;
                }
                catch (Exception ex)
                { }
                #endregion

                model.Add(obj);
            }

            #region Check khoa
            try
            {
                if (ChiTieuCongTy != null && (ChiTieuCongTy.IsOpenT1.GetValueOrDefault() || ChiTieuCongTy.IsOpenT2.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenT3.GetValueOrDefault() || ChiTieuCongTy.IsOpenT4.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenT5.GetValueOrDefault() || ChiTieuCongTy.IsOpenT6.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenT7.GetValueOrDefault() || ChiTieuCongTy.IsOpenT8.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenT9.GetValueOrDefault() || ChiTieuCongTy.IsOpenT10.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenT11.GetValueOrDefault() || ChiTieuCongTy.IsOpenT12.GetValueOrDefault()
                    || ChiTieuCongTy.IsOpenNam.GetValueOrDefault()))
                    ViewBag.NPCIsOpen = true;
            }
            catch (Exception ex)
            { }
            #endregion

            return PartialView("ListThoiGian", model);
        }

        public ActionResult SaveThoiGian(int Date, string[] MaDV, int?[] Id, float?[] TBN
           , float?[] T1, float?[] T2, float?[] T3, float?[] T4, float?[] T5
           , float?[] T6, float?[] T7, float?[] T8, float?[] T9, float?[] T10
           , float?[] T11, float?[] T12)
        {
            string strError = "";
            try
            {
                if (MaDV == null)
                {
                    return Json(new { type = "error", mess = "Không tìm thấy đơn vị !" }, JsonRequestBehavior.AllowGet);
                }

                for (int i = 0; i < MaDV.Count(); i++)
                {
                    #region T1
                    if ((Id[i] ?? 0) == 0)
                    {
                        sc_KHGiaoTGXLSCTHA objct = new sc_KHGiaoTGXLSCTHA();
                        objct.Nam = Date;
                        objct.DonViId = MaDV[i];
                        objct.NguoiNhap = User.Identity.Name;
                        objct.NgayNhap = DateTime.Now;

                        objct.TrungBinhNam = TBN[i];
                        //objct.Thang1 = T1[i];
                        //objct.Thang2 = T2[i];
                        //objct.Thang3 = T3[i];
                        //objct.Thang4 = T4[i];
                        //objct.Thang5 = T5[i];
                        //objct.Thang6 = T6[i];
                        //objct.Thang7 = T7[i];
                        //objct.Thang8 = T8[i];
                        //objct.Thang9 = T9[i];
                        //objct.Thang10 = T10[i];
                        //objct.Thang11 = T11[i];
                        //objct.Thang12 = T12[i];

                        objct.IsOpenT1 = true;
                        objct.IsOpenT2 = true;
                        objct.IsOpenT3 = true;
                        objct.IsOpenT4 = true;
                        objct.IsOpenT5 = true;
                        objct.IsOpenT6 = true;
                        objct.IsOpenT7 = true;
                        objct.IsOpenT8 = true;
                        objct.IsOpenT9 = true;
                        objct.IsOpenT10 = true;
                        objct.IsOpenT11 = true;
                        objct.IsOpenT12 = true;
                        objct.IsOpenNam = true;

                        var id = _chitieuthoigian_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieuthoigian_ser.GetById(Id[i] ?? 0);
                        objct.NguoiSua = User.Identity.Name;
                        objct.NgaySua = DateTime.Now;

                        objct.TrungBinhNam = TBN[i];
                        //objct.Thang1 = T1[i];
                        //objct.Thang2 = T2[i];
                        //objct.Thang3 = T3[i];
                        //objct.Thang4 = T4[i];
                        //objct.Thang5 = T5[i];
                        //objct.Thang6 = T6[i];
                        //objct.Thang7 = T7[i];
                        //objct.Thang8 = T8[i];
                        //objct.Thang9 = T9[i];
                        //objct.Thang10 = T10[i];
                        //objct.Thang11 = T11[i];
                        //objct.Thang12 = T12[i];

                        var id = _chitieuthoigian_ser.Update(objct, ref strError);
                    }
                    #endregion

                }

                return Json(new { type = "success", mess = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Lỗi trong quá trình lưu dữ liệu: " + ex.Message + "<br/>" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult KhoaThoiGian(bool IsOpen, int Date, int?[] Id, string[] MaDV)
        {
            string strError = "";
            try
            {
                for (int i = 0; i < MaDV.Count(); i++)
                {
                    if ((Id[i] ?? 0) == 0)
                    {
                        sc_KHGiaoTGXLSCTHA objct = new sc_KHGiaoTGXLSCTHA();
                        objct.Nam = Date;
                        objct.DonViId = MaDV[i];
                        objct.NguoiNhap = User.Identity.Name;
                        objct.NgayNhap = DateTime.Now;

                        objct.IsOpenT1 = IsOpen;
                        objct.IsOpenT2 = IsOpen;
                        objct.IsOpenT3 = IsOpen;
                        objct.IsOpenT4 = IsOpen;
                        objct.IsOpenT5 = IsOpen;
                        objct.IsOpenT6 = IsOpen;
                        objct.IsOpenT7 = IsOpen;
                        objct.IsOpenT8 = IsOpen;
                        objct.IsOpenT9 = IsOpen;
                        objct.IsOpenT10 = IsOpen;
                        objct.IsOpenT11 = IsOpen;
                        objct.IsOpenT12 = IsOpen;
                        objct.IsOpenNam = IsOpen;

                        var id = _chitieuthoigian_ser.Create(objct, ref strError);
                    }
                    else
                    {
                        var objct = _chitieuthoigian_ser.GetById(Id[i] ?? 0);
                        objct.NguoiSua = User.Identity.Name;
                        objct.NgaySua = DateTime.Now;

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 1)
                        {
                            objct.IsOpenT1 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT1 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 2)
                        {
                            objct.IsOpenT2 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT2 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 3)
                        {
                            objct.IsOpenT3 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT3 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 4)
                        {
                            objct.IsOpenT4 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT4 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 5)
                        {
                            objct.IsOpenT5 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT5 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 6)
                        {
                            objct.IsOpenT6 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT6 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 7)
                        {
                            objct.IsOpenT7 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT7 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 8)
                        {
                            objct.IsOpenT8 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT8 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 9)
                        {
                            objct.IsOpenT9 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT9 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 10)
                        {
                            objct.IsOpenT10 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT10 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 11)
                        {
                            objct.IsOpenT11 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT11 = false;
                        }

                        if (DateTime.Now.Year == Date && DateTime.Now.Month <= 12)
                        {
                            objct.IsOpenT12 = IsOpen;
                        }
                        else
                        {
                            objct.IsOpenT12 = false;
                        }

                        objct.IsOpenNam = IsOpen;

                        var id = _chitieuthoigian_ser.Update(objct, ref strError);
                    }
                }

                return Json(new { type = "success", mess = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Lỗi trong quá trình lưu dữ liệu: " + ex.Message + "<br/>" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IndexDonViTheoLoai()
        {
            return View();
        }

        public ActionResult ListDonViTheoLoai(int Date, int Loai)
        {
            string MaDV = Session["DonViID"].ToString();
            var objDV = _DonVi_ser.GetById(MaDV);
            DonViChiTieuSuCoViewModels obj = new DonViChiTieuSuCoViewModels();
            obj.TEN_DVIQLY = objDV.TenDonVi;
            obj.MA_DVIQLY = MaDV;

            #region thuc hien
            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT1 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT2 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT3 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT4 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT5 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT6 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT7 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT8 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT9 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT10 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT11 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    obj.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    obj.SoVuTruocMTT12 = (_SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "", "khachhang", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), MaDV, "", "", "", "", "", "", "", "", "110", "khachhang", true, "dachuyen", "")
                        );
                }
                catch (Exception ex)
                { }
            }
            #endregion

            var lstData = _chitieu_ser.GetListChiTieuTheoLoai(Date, MaDV, Loai);
            if (lstData != null)
            {
                #region T1
                if (lstData.FirstOrDefault(o => o.Thang == 1) != null)
                {
                    obj.IdT1 = lstData.FirstOrDefault(o => o.Thang == 1).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuNam;
                    obj.ChiTieuT1 = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                    obj.NPCIsOpenT1 = lstData.FirstOrDefault(o => o.Thang == 1).IsOpen;
                    obj.ChiTieuT1SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT1 = true;
                }
                #endregion

                #region T2
                if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                {
                    obj.IdT2 = lstData.FirstOrDefault(o => o.Thang == 2).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuNam;
                    obj.ChiTieuT2 = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                    obj.NPCIsOpenT2 = lstData.FirstOrDefault(o => o.Thang == 2).IsOpen;
                    obj.ChiTieuT2SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT2 = true;
                }
                #endregion

                #region T3
                if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                {
                    obj.IdT3 = lstData.FirstOrDefault(o => o.Thang == 3).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuNam;
                    obj.ChiTieuT3 = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                    obj.NPCIsOpenT3 = lstData.FirstOrDefault(o => o.Thang == 3).IsOpen;
                    obj.ChiTieuT3SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT3 = true;
                }
                #endregion

                #region T4
                if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                {
                    obj.IdT4 = lstData.FirstOrDefault(o => o.Thang == 4).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuNam;
                    obj.ChiTieuT4 = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                    obj.NPCIsOpenT4 = lstData.FirstOrDefault(o => o.Thang == 4).IsOpen;
                    obj.ChiTieuT4SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT4 = true;
                }
                #endregion

                #region T5
                if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                {
                    obj.IdT5 = lstData.FirstOrDefault(o => o.Thang == 5).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuNam;
                    obj.ChiTieuT5 = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                    obj.NPCIsOpenT5 = lstData.FirstOrDefault(o => o.Thang == 5).IsOpen;
                    obj.ChiTieuT5SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT5 = true;
                }
                #endregion

                #region T6
                if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                {
                    obj.IdT6 = lstData.FirstOrDefault(o => o.Thang == 6).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuNam;
                    obj.ChiTieuT6 = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                    obj.NPCIsOpenT6 = lstData.FirstOrDefault(o => o.Thang == 6).IsOpen;
                    obj.ChiTieuT6SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT6 = true;
                }
                #endregion

                #region T7
                if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                {
                    obj.IdT7 = lstData.FirstOrDefault(o => o.Thang == 7).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuNam;
                    obj.ChiTieuT7 = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                    obj.NPCIsOpenT7 = lstData.FirstOrDefault(o => o.Thang == 7).IsOpen;
                    obj.ChiTieuT7SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT7 = true;
                }
                #endregion

                #region T8
                if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                {
                    obj.IdT8 = lstData.FirstOrDefault(o => o.Thang == 8).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuNam;
                    obj.ChiTieuT8 = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                    obj.NPCIsOpenT8 = lstData.FirstOrDefault(o => o.Thang == 8).IsOpen;
                    obj.ChiTieuT8SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT8 = true;
                }
                #endregion

                #region T9
                if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                {
                    obj.IdT9 = lstData.FirstOrDefault(o => o.Thang == 9).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuNam;
                    obj.ChiTieuT9 = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                    obj.NPCIsOpenT9 = lstData.FirstOrDefault(o => o.Thang == 9).IsOpen;
                    obj.ChiTieuT9SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT9 = true;
                }
                #endregion

                #region T10
                if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                {
                    obj.IdT10 = lstData.FirstOrDefault(o => o.Thang == 10).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuNam;
                    obj.ChiTieuT10 = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                    obj.NPCIsOpenT10 = lstData.FirstOrDefault(o => o.Thang == 10).IsOpen;
                    obj.ChiTieuT10SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT10 = true;
                }
                #endregion

                #region T11
                if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                {
                    obj.IdT11 = lstData.FirstOrDefault(o => o.Thang == 11).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuNam;
                    obj.ChiTieuT11 = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                    obj.NPCIsOpenT11 = lstData.FirstOrDefault(o => o.Thang == 11).IsOpen;
                    obj.ChiTieuT11SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT11 = true;
                }
                #endregion

                #region T12
                if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                {
                    obj.IdT12 = lstData.FirstOrDefault(o => o.Thang == 12).Id;
                    obj.ChiTieuNam = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuNam;
                    obj.ChiTieuT12 = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                    obj.NPCIsOpenT12 = lstData.FirstOrDefault(o => o.Thang == 12).IsOpen;
                    obj.ChiTieuT12SauDieuChuyen = lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;
                }
                else
                {
                    obj.NPCIsOpenT12 = true;
                }
                #endregion
            }

            return PartialView("ListDonViTheoLoai", obj);
        }

        public ActionResult SaveDonViTheoLoai(int Date, string MaDV, int Loai
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
            var lstChiTieuCongTy = _chitieu_ser.GetListChiTieu(Date, MaDV);

            string strError = "";
            try
            {
                #region T1
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 1) != null)
                {
                    if ((IdT1 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT1 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT1;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT1;

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
                }
                #endregion

                #region T2
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 2) != null)
                {
                    if ((IdT2 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT2 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT2;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT2;

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
                }
                #endregion

                #region T3
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 3) != null)
                {
                    if ((IdT3 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT3 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT3;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT3;

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
                }
                #endregion

                #region T4
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 4) != null)
                {
                    if ((IdT4 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT4 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT4;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT4;

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
                }
                #endregion

                #region T5
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 5) != null)
                {
                    if ((IdT5 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT5 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT5;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT5;

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
                }
                #endregion

                #region T6
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 6) != null)
                {
                    if ((IdT6 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT6 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT6;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT6;

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
                }
                #endregion

                #region T7
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 7) != null)
                {
                    if ((IdT7 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT7 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT7;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT7;

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
                }
                #endregion

                #region T8
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 8) != null)
                {
                    if ((IdT8 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT8 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT8;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT8;

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
                }
                #endregion

                #region T9
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 9) != null)
                {
                    if ((IdT9 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT9 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT9;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT9;

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
                }
                #endregion

                #region T10
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 10) != null)
                {
                    if ((IdT10 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT10 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT10;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT10;

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
                }
                #endregion

                #region T11
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 11) != null)
                {
                    if ((IdT11 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT11 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT11;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT11;

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
                }
                #endregion

                #region T12
                if (lstChiTieuCongTy != null && lstChiTieuCongTy.FirstOrDefault(o => o.Thang == 12) != null)
                {
                    if ((IdT12 ?? 0) > 0)
                    {
                        var objct = _chitieu_ser.GetById(IdT12 ?? 0);
                        if (objct.IsOpen.GetValueOrDefault())
                        {
                            if (objct.ChiTieuThang == null)
                                objct.ChiTieuThang = SuatT12;
                            else
                                objct.ChiTieuThangSauDieuChuyen = SuatT12;

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

        public ActionResult IndexDonViThoiGian()
        {
            return View();
        }

        public ActionResult ListDonViThoiGian(int Date)
        {
            string MaDV = Session["DonViID"].ToString();
            var objDV = _DonVi_ser.GetById(MaDV);

            DonViChiTieuThoiGianSuCoViewModels obj = new DonViChiTieuThoiGianSuCoViewModels();
            obj.MA_DVIQLY = objDV.Id;
            obj.TEN_DVIQLY = objDV.TenDonVi;

            var data = _chitieuthoigian_ser.GetListChiTieu(Date, objDV.Id);
            if (data != null)
            {
                obj.Id = data.Id;
                obj.Nam = data.Nam;

                if (data.TrungBinhNam != null)
                    obj.TrungBinhNam = Math.Round(data.TrungBinhNam ?? 0, 2);
                if (data.Thang1 != null)
                    obj.Thang1 = Math.Round(data.Thang1 ?? 0, 2);
                if (data.Thang2 != null)
                    obj.Thang2 = Math.Round(data.Thang2 ?? 0, 2);
                if (data.Thang3 != null)
                    obj.Thang3 = Math.Round(data.Thang3 ?? 0, 2);
                if (data.Thang4 != null)
                    obj.Thang4 = Math.Round(data.Thang4 ?? 0, 2);
                if (data.Thang5 != null)
                    obj.Thang5 = Math.Round(data.Thang5 ?? 0, 2);
                if (data.Thang6 != null)
                    obj.Thang6 = Math.Round(data.Thang6 ?? 0, 2);
                if (data.Thang7 != null)
                    obj.Thang7 = Math.Round(data.Thang7 ?? 0, 2);
                if (data.Thang8 != null)
                    obj.Thang8 = Math.Round(data.Thang8 ?? 0, 2);
                if (data.Thang9 != null)
                    obj.Thang9 = Math.Round(data.Thang9 ?? 0, 2);
                if (data.Thang10 != null)
                    obj.Thang10 = Math.Round(data.Thang10 ?? 0, 2);
                if (data.Thang11 != null)
                    obj.Thang11 = Math.Round(data.Thang11 ?? 0, 2);
                if (data.Thang12 != null)
                    obj.Thang12 = Math.Round(data.Thang12 ?? 0, 2);

                if (data.TrungBinhNamDC != null)
                    obj.TrungBinhNamDC = Math.Round(data.TrungBinhNamDC ?? 0, 2);
                if (data.Thang1DC != null)
                    obj.Thang1DC = Math.Round(data.Thang1DC ?? 0, 2);
                if (data.Thang2DC != null)
                    obj.Thang2DC = Math.Round(data.Thang2DC ?? 0, 2);
                if (data.Thang3DC != null)
                    obj.Thang3DC = Math.Round(data.Thang3DC ?? 0, 2);
                if (data.Thang4DC != null)
                    obj.Thang4DC = Math.Round(data.Thang4DC ?? 0, 2);
                if (data.Thang5DC != null)
                    obj.Thang5DC = Math.Round(data.Thang5DC ?? 0, 2);
                if (data.Thang6DC != null)
                    obj.Thang6DC = Math.Round(data.Thang6DC ?? 0, 2);
                if (data.Thang7DC != null)
                    obj.Thang7DC = Math.Round(data.Thang7DC ?? 0, 2);
                if (data.Thang8DC != null)
                    obj.Thang8DC = Math.Round(data.Thang8DC ?? 0, 2);
                if (data.Thang9DC != null)
                    obj.Thang9DC = Math.Round(data.Thang9DC ?? 0, 2);
                if (data.Thang10DC != null)
                    obj.Thang10DC = Math.Round(data.Thang10DC ?? 0, 2);
                if (data.Thang11DC != null)
                    obj.Thang11DC = Math.Round(data.Thang11DC ?? 0, 2);
                if (data.Thang12DC != null)
                    obj.Thang12DC = Math.Round(data.Thang12DC ?? 0, 2);

                obj.IsOpenT1 = data.IsOpenT1;
                obj.IsOpenT2 = data.IsOpenT2;
                obj.IsOpenT3 = data.IsOpenT3;
                obj.IsOpenT4 = data.IsOpenT4;
                obj.IsOpenT5 = data.IsOpenT5;
                obj.IsOpenT6 = data.IsOpenT6;
                obj.IsOpenT7 = data.IsOpenT7;
                obj.IsOpenT8 = data.IsOpenT8;
                obj.IsOpenT9 = data.IsOpenT9;
                obj.IsOpenT10 = data.IsOpenT10;
                obj.IsOpenT11 = data.IsOpenT11;
                obj.IsOpenT12 = data.IsOpenT12;
                obj.IsOpenNam = data.IsOpenNam;

                #region Thoi gian theo thang
                double? TTGT1 = 0, TTGT2 = 0, TTGT3 = 0, TTGT4 = 0, TTGT5 = 0, TTGT6 = 0
                    , TTGT7 = 0, TTGT8 = 0, TTGT9 = 0, TTGT10 = 0, TTGT11 = 0, TTGT12 = 0;
                double? TTGT1DC = 0, TTGT2DC = 0, TTGT3DC = 0, TTGT4DC = 0, TTGT5DC = 0, TTGT6DC = 0
                   , TTGT7DC = 0, TTGT8DC = 0, TTGT9DC = 0, TTGT10DC = 0, TTGT11DC = 0, TTGT12DC = 0;
                var lstData = _chitieu_ser.GetListChiTieu(Date, objDV.Id);
                if (lstData != null)
                {
                    if (lstData.FirstOrDefault(o => o.Thang == 1) != null)
                    {
                        try
                        {
                            TTGT1 = obj.Thang1 * lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang;
                            obj.TGT1 = string.Format("{0:#,0.##}", obj.Thang1) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT1);

                            TTGT1DC = obj.Thang1DC * lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen;
                            obj.TGT1DC = string.Format("{0:#,0.##}", obj.Thang1DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 1).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT1DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 2) != null)
                    {
                        try
                        {
                            TTGT2 = obj.Thang2 * lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang;
                            obj.TGT2 = string.Format("{0:#,0.##}", obj.Thang2) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT2);

                            TTGT2DC = obj.Thang2DC * lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen;
                            obj.TGT2DC = string.Format("{0:#,0.##}", obj.Thang2DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 2).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT2DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 3) != null)
                    {
                        try
                        {
                            TTGT3 = obj.Thang3 * lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang;
                            obj.TGT3 = string.Format("{0:#,0.##}", obj.Thang3) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT3);

                            TTGT3DC = obj.Thang3DC * lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen;
                            obj.TGT3DC = string.Format("{0:#,0.##}", obj.Thang3DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 3).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT3DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 4) != null)
                    {
                        try
                        {
                            TTGT4 = obj.Thang4 * lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang;
                            obj.TGT4 = string.Format("{0:#,0.##}", obj.Thang4) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT4);

                            TTGT4DC = obj.Thang4DC * lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen;
                            obj.TGT4DC = string.Format("{0:#,0.##}", obj.Thang4DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 4).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT4DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 5) != null)
                    {
                        try
                        {
                            TTGT5 = obj.Thang5 * lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang;
                            obj.TGT5 = string.Format("{0:#,0.##}", obj.Thang5) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT5);

                            TTGT5DC = obj.Thang5DC * lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen;
                            obj.TGT5DC = string.Format("{0:#,0.##}", obj.Thang5DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 5).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT5DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 6) != null)
                    {
                        try
                        {
                            TTGT6 = obj.Thang6 * lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang;
                            obj.TGT6 = string.Format("{0:#,0.##}", obj.Thang6) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT6);

                            TTGT6DC = obj.Thang6DC * lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen;
                            obj.TGT6DC = string.Format("{0:#,0.##}", obj.Thang6DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 6).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT6DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 7) != null)
                    {
                        try
                        {
                            TTGT7 = obj.Thang7 * lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang;
                            obj.TGT7 = string.Format("{0:#,0.##}", obj.Thang7) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT7);

                            TTGT7DC = obj.Thang7DC * lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen;
                            obj.TGT7DC = string.Format("{0:#,0.##}", obj.Thang7DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 7).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT7DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 8) != null)
                    {
                        try
                        {
                            TTGT8 = obj.Thang8 * lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang;
                            obj.TGT8 = string.Format("{0:#,0.##}", obj.Thang8) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT8);

                            TTGT8DC = obj.Thang8DC * lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen;
                            obj.TGT8DC = string.Format("{0:#,0.##}", obj.Thang8DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 8).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT8DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 9) != null)
                    {
                        try
                        {
                            TTGT9 = obj.Thang9 * lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang;
                            obj.TGT9 = string.Format("{0:#,0.##}", obj.Thang9) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT9);

                            TTGT9DC = obj.Thang9DC * lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen;
                            obj.TGT9DC = string.Format("{0:#,0.##}", obj.Thang9DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 9).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT9DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 10) != null)
                    {
                        try
                        {
                            TTGT10 = obj.Thang10 * lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang;
                            obj.TGT10 = string.Format("{0:#,0.##}", obj.Thang10) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT10);

                            TTGT10DC = obj.Thang10DC * lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen;
                            obj.TGT10DC = string.Format("{0:#,0.##}", obj.Thang10DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 10).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT10DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 11) != null)
                    {
                        try
                        {
                            TTGT11 = obj.Thang11 * lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang;
                            obj.TGT11 = string.Format("{0:#,0.##}", obj.Thang11) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT11);

                            TTGT11DC = obj.Thang11DC * lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen;
                            obj.TGT11DC = string.Format("{0:#,0.##}", obj.Thang11DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 11).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT11DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (lstData.FirstOrDefault(o => o.Thang == 12) != null)
                    {
                        try
                        {
                            TTGT12 = obj.Thang12 * lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang;
                            obj.TGT12 = string.Format("{0:#,0.##}", obj.Thang12) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThang)
                                + "=" + string.Format("{0:#,0.##}", TTGT12);

                            TTGT12DC = obj.Thang12DC * lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen;
                            obj.TGT12DC = string.Format("{0:#,0.##}", obj.Thang12DC) + "*"
                                + string.Format("{0:#,0.##}", lstData.FirstOrDefault(o => o.Thang == 12).ChiTieuThangSauDieuChuyen)
                                + "=" + string.Format("{0:#,0.##}", TTGT12DC);
                        }
                        catch (Exception ex)
                        { }
                    }
                }

                obj.TGTBN = (TTGT1 ?? 0) + (TTGT2 ?? 0) + (TTGT3 ?? 0) + (TTGT4 ?? 0) + (TTGT5 ?? 0)
                    + (TTGT6 ?? 0) + (TTGT7 ?? 0) + (TTGT8 ?? 0) + (TTGT9 ?? 0) + (TTGT10 ?? 0)
                    + (TTGT11 ?? 0) + (TTGT12 ?? 0);

                obj.TGTBNDC = (TTGT1DC ?? 0) + (TTGT2DC ?? 0) + (TTGT3DC ?? 0) + (TTGT4DC ?? 0) + (TTGT5DC ?? 0)
                   + (TTGT6DC ?? 0) + (TTGT7DC ?? 0) + (TTGT8DC ?? 0) + (TTGT9DC ?? 0) + (TTGT10DC ?? 0)
                   + (TTGT11DC ?? 0) + (TTGT12DC ?? 0);

                #endregion

            }

            #region thuc hien
            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT1 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT1 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT1 = string.Format("{0:#,0.##}", TGT1) + "/" + string.Format("{0:#,0.##}", SoVuT1) + "=" + string.Format("{0:#,0.##}", (TGT1 / SoVuT1));
                    obj.THTGT1 = TGT1;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT1 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT1 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 1, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 1, DateTime.DaysInMonth(Date, 1))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT1 = string.Format("{0:#,0.##}", TGT1) + "/" + string.Format("{0:#,0.##}", SoVuT1) + "=" + string.Format("{0:#,0.##}", (TGT1 / SoVuT1));
                    obj.THTGT1 = TGT1;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT2 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT2 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT2 = string.Format("{0:#,0.##}", TGT2) + "/" + string.Format("{0:#,0.##}", SoVuT2) + "=" + string.Format("{0:#,0.##}", (TGT2 / SoVuT2));
                    obj.THTGT2 = TGT2;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT2 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT2 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 2, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 2, DateTime.DaysInMonth(Date, 2))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT2 = string.Format("{0:#,0.##}", TGT2) + "/" + string.Format("{0:#,0.##}", SoVuT2) + "=" + string.Format("{0:#,0.##}", (TGT2 / SoVuT2));
                    obj.THTGT2 = TGT2;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT3 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT3 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT3 = string.Format("{0:#,0.##}", TGT3) + "/" + string.Format("{0:#,0.##}", SoVuT3) + "=" + string.Format("{0:#,0.##}", (TGT3 / SoVuT3));
                    obj.THTGT3 = TGT3;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT3 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT3 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 3, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 3, DateTime.DaysInMonth(Date, 3))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT3 = string.Format("{0:#,0.##}", TGT3) + "/" + string.Format("{0:#,0.##}", SoVuT3) + "=" + string.Format("{0:#,0.##}", (TGT3 / SoVuT3));
                    obj.THTGT3 = TGT3;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT4 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT4 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT4 = string.Format("{0:#,0.##}", TGT4) + "/" + string.Format("{0:#,0.##}", SoVuT4) + "=" + string.Format("{0:#,0.##}", (TGT4 / SoVuT4));
                    obj.THTGT4 = TGT4;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT4 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT4 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 4, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 4, DateTime.DaysInMonth(Date, 4))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT4 = string.Format("{0:#,0.##}", TGT4) + "/" + string.Format("{0:#,0.##}", SoVuT4) + "=" + string.Format("{0:#,0.##}", (TGT4 / SoVuT4));
                    obj.THTGT4 = TGT4;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT5 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT5 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT5 = string.Format("{0:#,0.##}", TGT5) + "/" + string.Format("{0:#,0.##}", SoVuT5) + "=" + string.Format("{0:#,0.##}", (TGT5 / SoVuT5));
                    obj.THTGT5 = TGT5;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT5 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT5 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 5, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 5, DateTime.DaysInMonth(Date, 5))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT5 = string.Format("{0:#,0.##}", TGT5) + "/" + string.Format("{0:#,0.##}", SoVuT5) + "=" + string.Format("{0:#,0.##}", (TGT5 / SoVuT5));
                    obj.THTGT5 = TGT5;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT6 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT6 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT6 = string.Format("{0:#,0.##}", TGT6) + "/" + string.Format("{0:#,0.##}", SoVuT6) + "=" + string.Format("{0:#,0.##}", (TGT6 / SoVuT6));
                    obj.THTGT6 = TGT6;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT6 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT6 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 6, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 6, DateTime.DaysInMonth(Date, 6))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT6 = string.Format("{0:#,0.##}", TGT6) + "/" + string.Format("{0:#,0.##}", SoVuT6) + "=" + string.Format("{0:#,0.##}", (TGT6 / SoVuT6));
                    obj.THTGT6 = TGT6;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT7 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT7 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT7 = string.Format("{0:#,0.##}", TGT7) + "/" + string.Format("{0:#,0.##}", SoVuT7) + "=" + string.Format("{0:#,0.##}", (TGT7 / SoVuT7));
                    obj.THTGT7 = TGT7;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT7 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT7 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 7, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 7, DateTime.DaysInMonth(Date, 7))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT7 = string.Format("{0:#,0.##}", TGT7) + "/" + string.Format("{0:#,0.##}", SoVuT7) + "=" + string.Format("{0:#,0.##}", (TGT7 / SoVuT7));
                    obj.THTGT7 = TGT7;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT8 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT8 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT8 = string.Format("{0:#,0.##}", TGT8) + "/" + string.Format("{0:#,0.##}", SoVuT8) + "=" + string.Format("{0:#,0.##}", (TGT8 / SoVuT8));
                    obj.THTGT8 = TGT8;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT8 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT8 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 8, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 8, DateTime.DaysInMonth(Date, 8))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT8 = string.Format("{0:#,0.##}", TGT8) + "/" + string.Format("{0:#,0.##}", SoVuT8) + "=" + string.Format("{0:#,0.##}", (TGT8 / SoVuT8));
                    obj.THTGT8 = TGT8;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT9 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT9 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT9 = string.Format("{0:#,0.##}", TGT9) + "/" + string.Format("{0:#,0.##}", SoVuT9) + "=" + string.Format("{0:#,0.##}", (TGT9 / SoVuT9));
                    obj.THTGT9 = TGT9;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT9 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT9 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 9, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 9, DateTime.DaysInMonth(Date, 9))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT9 = string.Format("{0:#,0.##}", TGT9) + "/" + string.Format("{0:#,0.##}", SoVuT9) + "=" + string.Format("{0:#,0.##}", (TGT9 / SoVuT9));
                    obj.THTGT9 = TGT9;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT10 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT10 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT10 = string.Format("{0:#,0.##}", TGT10) + "/" + string.Format("{0:#,0.##}", SoVuT10) + "=" + string.Format("{0:#,0.##}", (TGT10 / SoVuT10));
                    obj.THTGT10 = TGT10;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT10 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT10 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 10, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 10, DateTime.DaysInMonth(Date, 10))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT10 = string.Format("{0:#,0.##}", TGT10) + "/" + string.Format("{0:#,0.##}", SoVuT10) + "=" + string.Format("{0:#,0.##}", (TGT10 / SoVuT10));
                    obj.THTGT10 = TGT10;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT11 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT11 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT11 = string.Format("{0:#,0.##}", TGT11) + "/" + string.Format("{0:#,0.##}", SoVuT11) + "=" + string.Format("{0:#,0.##}", (TGT11 / SoVuT11));
                    obj.THTGT11 = TGT11;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT11 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT11 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 11, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 11, DateTime.DaysInMonth(Date, 11))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT11 = string.Format("{0:#,0.##}", TGT11) + "/" + string.Format("{0:#,0.##}", SoVuT11) + "=" + string.Format("{0:#,0.##}", (TGT11 / SoVuT11));
                    obj.THTGT11 = TGT11;
                }
                catch (Exception ex)
                { }
            }

            if (MaDV.Length == 4 || MaDV.ToUpper() == "PH" || MaDV.ToUpper() == "PN" || MaDV.ToUpper() == "PM")
            {
                try
                {
                    var TGT12 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT12 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), "", "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT12 = string.Format("{0:#,0.##}", TGT12) + "/" + string.Format("{0:#,0.##}", SoVuT12) + "=" + string.Format("{0:#,0.##}", (TGT12 / SoVuT12));
                    obj.THTGT12 = TGT12;
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    var TGT12 = _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", "dachuyen", "")
                        - _SuCo_ser.SumTongThoiGianMatDien("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", "dachuyen", "");
                    var SoVuT12 = _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "", "", true, "dachuyen", "")
                        - _SuCo_ser.CountListPaging("", (new DateTime(Date, 12, 1)).ToString("dd/MM/yyyy"), (new DateTime(Date, 12, DateTime.DaysInMonth(Date, 12))).ToString("dd/MM/yyyy"), objDV.Id.ToString(), "", "", "", "", "", "", "", "", "110", "", true, "dachuyen", "");
                    obj.THT12 = string.Format("{0:#,0.##}", TGT12) + "/" + string.Format("{0:#,0.##}", SoVuT12) + "=" + string.Format("{0:#,0.##}", (TGT12 / SoVuT12));
                    obj.THTGT12 = TGT12;
                }
                catch (Exception ex)
                { }
            }
            #endregion


            return PartialView("ListDonViThoiGian", obj);
        }

        public ActionResult SaveDonViThoiGian(int Date, string[] MaDV, int?[] Id, float?[] TBN
           , float?[] T1, float?[] T2, float?[] T3, float?[] T4, float?[] T5
           , float?[] T6, float?[] T7, float?[] T8, float?[] T9, float?[] T10
           , float?[] T11, float?[] T12)
        {
            string strError = "";
            try
            {
                if (MaDV == null)
                {
                    return Json(new { type = "error", mess = "Không tìm thấy đơn vị !" }, JsonRequestBehavior.AllowGet);
                }

                for (int i = 0; i < MaDV.Count(); i++)
                {
                    #region Thang
                    if ((Id[i] ?? 0) > 0)
                    {
                        var objct = _chitieuthoigian_ser.GetById(Id[i] ?? 0);
                        objct.NguoiSua = User.Identity.Name;
                        objct.NgaySua = DateTime.Now;

                        if (objct.IsOpenNam.GetValueOrDefault())
                        {
                            if (objct.TrungBinhNam == null)
                                objct.TrungBinhNam = TBN[i];
                            else
                                objct.TrungBinhNamDC = TBN[i];
                        }
                        if (objct.IsOpenT1.GetValueOrDefault())
                        {
                            if (objct.Thang1 == null)
                                objct.Thang1 = T1[i];
                            else
                                objct.Thang1DC = T1[i];
                        }
                        if (objct.IsOpenT2.GetValueOrDefault())
                        {
                            if (objct.Thang2 == null)
                                objct.Thang2 = T2[i];
                            else
                                objct.Thang2DC = T2[i];
                        }
                        if (objct.IsOpenT3.GetValueOrDefault())
                        {
                            if (objct.Thang3 == null)
                                objct.Thang3 = T3[i];
                            else
                                objct.Thang3DC = T3[i];
                        }
                        if (objct.IsOpenT4.GetValueOrDefault())
                        {
                            if (objct.Thang4 == null)
                                objct.Thang4 = T4[i];
                            else
                                objct.Thang4DC = T4[i];
                        }
                        if (objct.IsOpenT5.GetValueOrDefault())
                        {
                            if (objct.Thang5 == null)
                                objct.Thang5 = T5[i];
                            else
                                objct.Thang5DC = T5[i];
                        }
                        if (objct.IsOpenT6.GetValueOrDefault())
                        {
                            if (objct.Thang6 == null)
                                objct.Thang6 = T6[i];
                            else
                                objct.Thang6DC = T6[i];
                        }
                        if (objct.IsOpenT7.GetValueOrDefault())
                        {
                            if (objct.Thang7 == null)
                                objct.Thang7 = T7[i];
                            else
                                objct.Thang7DC = T7[i];
                        }
                        if (objct.IsOpenT8.GetValueOrDefault())
                        {
                            if (objct.Thang8 == null)
                                objct.Thang8 = T8[i];
                            else
                                objct.Thang8DC = T8[i];
                        }
                        if (objct.IsOpenT9.GetValueOrDefault())
                        {
                            if (objct.Thang9 == null)
                                objct.Thang9 = T9[i];
                            else
                                objct.Thang9DC = T9[i];
                        }
                        if (objct.IsOpenT10.GetValueOrDefault())
                        {
                            if (objct.Thang10 == null)
                                objct.Thang10 = T10[i];
                            else
                                objct.Thang10DC = T10[i];
                        }
                        if (objct.IsOpenT11.GetValueOrDefault())
                        {
                            if (objct.Thang11 == null)
                                objct.Thang11 = T11[i];
                            else
                                objct.Thang11DC = T11[i];
                        }
                        if (objct.IsOpenT12.GetValueOrDefault())
                        {
                            if (objct.Thang12 == null)
                                objct.Thang12 = T12[i];
                            else
                                objct.Thang12DC = T12[i];
                        }

                        var id = _chitieuthoigian_ser.Update(objct, ref strError);
                    }
                    #endregion

                }

                return Json(new { type = "success", mess = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", mess = "Lỗi trong quá trình lưu dữ liệu: " + ex.Message + "<br/>" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}