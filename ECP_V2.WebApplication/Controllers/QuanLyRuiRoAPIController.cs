using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Linq;
using System.Web.Http;

namespace ECP_V2.WebApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/qlrr")]
    public class QuanLyRuiRoAPIController : ApiController
    {
        // POST api/AccountAPI/ChangePassword


        [Route("hello")]
        [HttpGet]
        public object hello()
        {
            return ResponseData.ReturnSucess("ok");
        }
        [Route("AdddanhMuc")]
        [HttpGet]
        public object AdddanhMuc(String ten)
        {
            try
            {

                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                DMUC_RRO d = new DMUC_RRO();
                d.TEN = ten;
                db.DMUC_RRO.Add(d);
                db.SaveChanges();
                return ResponseData.ReturnSucess(d);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("DelDanhMuc")]
        [HttpGet]
        public object DelDanhMuc(int ID)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var d = db.DMUC_RRO.Where(e => e.ID == ID).First();
                db.DMUC_RRO.Remove(d);
                db.SaveChanges();
                return ResponseData.ReturnSucess("ok");
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }
        [Route("GetAllDanhMuc")]
        [HttpGet]
        public object GetAllDanhMuc()
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var data = db.DMUC_RRO.ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("GetAllData")]
        [HttpGet]
        public object GetAllData(string madvi, string TRANG_THAI)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();

                var d = (from i in db.QLY_RRO
                         join j in db.DMUC_RRO
                         on i.ID_LOAI equals j.ID
                         select new { i, j }).ToList();

                var data = db.QLY_RRO.Where(g => g.MA_DV == madvi && g.TRANG_THAI == TRANG_THAI).ToList();
                return ResponseData.ReturnSucess(data);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }


        [Route("AddRuiRo")]
        [HttpGet]
        public object AddRuiRo(string TEN_VITRI, string NOI_DUNG, string BIEN_PHAP
            , string MADVI, int ID_LOAI, string IDPMIS)
        {
            try
            {
                QLY_RRO ql = new QLY_RRO();
                ql.TEN_VITRI = TEN_VITRI;
                ql.MA_DV = MADVI;
                ql.NOI_DUNG = NOI_DUNG;
                ql.BIEN_PHAP = BIEN_PHAP;
                ql.TRANG_THAI = "CHUA_KP";
                ql.ID_LOAI = ID_LOAI;
                ql.ID_VITRI_PMIS = IDPMIS;
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                db.QLY_RRO.Add(ql);
                db.SaveChanges();
                return ResponseData.ReturnSucess(ql);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

        [Route("RemoveRuiRo")]
        [HttpGet]
        public object RemoveRuiRo(int id)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var r = db.QLY_RRO.Where(d => d.ID == id).First();
                db.QLY_RRO.Remove(r);
                db.SaveChanges();
                return ResponseData.ReturnSucess("ok");
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }


        [Route("UpdateKhacPhuc")]
        [HttpGet]
        public object UpdateKhacPhuc(int ID, string ND_KHAC_PHUC, string TRANG_THAI)
        {
            try
            {
                DataAccess.ECP_V2Entities db = new DataAccess.ECP_V2Entities();
                var r = db.QLY_RRO.Where(d => d.ID == ID).First();
                r.ND_KHAC_PHUC = ND_KHAC_PHUC;
                r.TRANG_THAI = TRANG_THAI;
                db.SaveChanges();
                return ResponseData.ReturnSucess(r);
            }
            catch (Exception ex)
            {
                return ResponseData.ReturnFail(ex.Message);
            }
        }

    }

}