﻿using ECP_V2.WebApplication.Helpers;
using System;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class PhaDatKhongDatController : Controller
    {
        // GET: Admin/PhaDatKhongDat
        [HasCredential(MenuCode = "PDKD")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string typeshow = "")
        {
            try
            {
                //ViewBag.TatCaNhanVien = _nhanvien_ser.List();
                ViewBag.MonthId = typeshow;

                //DisposeAll();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}