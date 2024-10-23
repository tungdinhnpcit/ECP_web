using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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
                foreach (var a in data)
                {
                    a.CCU_DCU = "";
                    var cc = db.BB_KSAT_VITRI_CONGC_SDUNG.Where(d => d.ID_BB == a.ID_BB && d.ID_VITRI == a.ID_VITRI_PMIS).ToList();
                    if (cc.Count > 0)
                        a.CCU_DCU = String.Join(";", cc.Select(g => g.TEN_CONGCU + "(" + g.SOLUONG + ")").ToArray());
                }
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("AddViTri")]
        [HttpGet]
        public object AddViTri(int id_bbks, string id_vitri, string ten_vitri, string loai_congviec, string ndung_congviec, string phuongan_thien, string bienphap_at)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();

                var bbks = db.BBAN_KSAT_VITRI.Where(x => x.ID_VITRI_PMIS == id_vitri && x.ID_BB == id_bbks).FirstOrDefault();

                if (bbks == null)
                {
                    BBAN_KSAT_VITRI d = new BBAN_KSAT_VITRI();
                    //d.ID = idMax;
                    d.ID_BB = id_bbks;
                    d.ID_VITRI_PMIS = id_vitri;
                    d.TEN_VITRI = ten_vitri;
                    d.LOAI_CVIEC = loai_congviec;
                    d.NDUNG_CVIEC = ndung_congviec;
                    d.PAN_THIEN = phuongan_thien;
                    d.BPHAP_ATOAN = bienphap_at;
                    db.BBAN_KSAT_VITRI.Add(d);
                    db.SaveChanges();
                    return ResponseData.ReturnSucess(d);
                }
                else
                    return ResponseData.ReturnFail("error: Đã thêm vị trí này rồi!");
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("AddCongCu")]
        [HttpGet]
        public object AddCongCu(int id_bbks, string id_vitri, string ten_vitri, int ID_CC, string TenLoai, int SOLUONG)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                BB_KSAT_VITRI_CONGC_SDUNG g = new BB_KSAT_VITRI_CONGC_SDUNG();
                g.ID_BB = id_bbks;
                g.ID_VITRI = id_vitri;
                g.TEN_VITRI = ten_vitri;
                g.ID_CONGCU = ID_CC;
                g.TEN_CONGCU = TenLoai;
                g.SOLUONG = SOLUONG;
                db.BB_KSAT_VITRI_CONGC_SDUNG.Add(g);
                db.SaveChanges();
                return ResponseData.ReturnSucess(g);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }


        //AddViTri perViTri
        [Route("GetAllBBKS_TT")]
        [HttpGet]
        public object GetAllBBKS_TT(string TRANG_THAI)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                string sText;
                if (TRANG_THAI == "CHUA_DUYET")
                    sText = "Chưa duyệt";
                else
                    sText = "Đã duyệt";
                var data = db.BBAN_KSAT_HTRUONG.Where(d => d.TINH_TRANG.ToUpper() == sText.Trim().ToUpper()).ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("DelViTri")]
        [HttpGet]
        public object DelViTri(int id_bbks, string id_vitri)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var d = db.BBAN_KSAT_VITRI.Where(e => e.ID_BB == id_bbks && e.ID_VITRI_PMIS == id_vitri).First();
                db.BBAN_KSAT_VITRI.Remove(d);
                db.SaveChanges();
                return ResponseData.ReturnSucess("ok");
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("DuyetBBKS")]
        [HttpGet]
        public object DuyetBBKS(int id_bbks, string ten_bb, string trang_thai)
        {
            try
            {
                string sText;
                if (trang_thai == "CHUA_DUYET")
                    sText = "Đã duyệt";
                else
                    sText = "Chưa duyệt";

                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();

                var bbks = db.BBAN_KSAT_HTRUONG.Where(x => x.ID == id_bbks).FirstOrDefault();

                if (bbks != null)
                {
                    bbks.TINH_TRANG = sText;
                    bbks.NGUOI_DUYET = ten_bb;
                    bbks.NGAY_DUYET = DateTime.Now;
                }

                db.SaveChanges();
                return ResponseData.ReturnSucess(bbks);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("GetAllCCDC")]
        [HttpGet]
        public object GetAllCCDC(int id_bbks)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var data = db.LoaiThietBis.Select(g => new
                {
                    g.ID,
                    g.TenLoai,
                    g.NamSX
                }).ToList();
                //var data = db.BBAN_KSAT_HTRUONG.Where(g => g.ID == id_bbks).ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }


    }

    public class ClsCCDC
    {
        public int ID { set; get; }
        public int NamSX { get; set; }
        public int SOLUONG { get; set; }
        public string TenLoai { get; set; }
    }
    public class AddViTri
    {
        public int id_bbks { get; set; }
        public string id_vitri { get; set; }
        public string ten_vitri { get; set; }
        public string loai_congviec { get; set; }
        public string ndung_congviec { get; set; }
        public string phuongan_thien { get; set; }
        public string bienphap_at { get; set; }
        public List<ClsCCDC> congcu_dungcu { get; set; }
    }
}