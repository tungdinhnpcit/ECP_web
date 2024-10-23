using ECP_V2.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/bbks")]
    public class BienBanKhaoSatAPIController : ApiController
    {
        // POST api/AccountAPI/ChangePassword
        [Route("hello")]
        [HttpGet]
        public object ChangePassword()
        {
            return "ok";
        }
        [Route("GetAllBBKS")]
        [HttpGet]
        public object GetAllBBKS()
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var data = db.BBAN_KSAT_HTRUONG.ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("AddBienBan")]
        [HttpGet]
        public object AddBienBan(String ten, string NguoiLap)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                //tìm max id
                var idMax = 1;
                var max = db.BBAN_KSAT_HTRUONG.OrderByDescending(x => x.ID).FirstOrDefault();
                if (max != null) { idMax = max.ID + 1; }

                BBAN_KSAT_HTRUONG d = new BBAN_KSAT_HTRUONG();
                //d.ID = idMax;
                d.TEN = ten;
                d.TINH_TRANG = "Chưa duyệt";
                d.NGUOI_LAP = NguoiLap;
                d.NGAY_LAP = DateTime.Now;
                db.BBAN_KSAT_HTRUONG.Add(d);
                db.SaveChanges();
                return ResponseData.ReturnSucess(d);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("EditBienBan")]
        [HttpGet]
        public object EditBienBan(String ten, string NguoiLap, string idbb)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                int ibb = Convert.ToInt32(idbb);
                var bbks = db.BBAN_KSAT_HTRUONG.Where(x => x.ID == ibb).FirstOrDefault();

                if (bbks != null)
                {
                    bbks.TEN = ten;
                    bbks.NGUOI_LAP = NguoiLap;
                }

                db.SaveChanges();
                return ResponseData.ReturnSucess(bbks);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("GetAllViTri")]
        [HttpGet]
        public object GetAllViTri(string id_bbks)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                int i = Convert.ToInt32(id_bbks);
                var data = db.BBAN_KSAT_VITRI.Where(x => x.ID_BB == i).ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("AddViTri")]
        [HttpGet]
        public object AddViTri(string id_bbks, string id_vitri, string ten_vitri)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();

                BBAN_KSAT_VITRI d = new BBAN_KSAT_VITRI();
                //d.ID = idMax;
                d.ID_BB = int.Parse(id_bbks);
                d.ID_VITRI_PMIS = id_vitri;
                d.TEN_VITRI = ten_vitri;
                
                db.BBAN_KSAT_VITRI.Add(d);
                db.SaveChanges();
                return ResponseData.ReturnSucess(d);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }


    }

}