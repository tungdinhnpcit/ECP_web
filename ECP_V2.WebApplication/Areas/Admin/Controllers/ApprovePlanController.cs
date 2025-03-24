using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class ApprovePlanController : Controller
    {
        PhongBanRepository phongBanRepository = new PhongBanRepository();
        ApprovePlanReponsitory approvePlanReponsitory = new ApprovePlanReponsitory();
        string url = System.Configuration.ConfigurationManager.AppSettings["API_PMIS"].ToString();
        string pdkey = System.Configuration.ConfigurationManager.AppSettings["PDKEY"].ToString();
        string thang = string.Empty;
        string nam = string.Empty;
        string doiid = string.Empty;
        string nhomkt = string.Empty;
        List<PhongBan> lstPhongBan;
        List<DonVi> lstDonvis;
        // GET: Admin/ApprovePlan
        public ActionResult Index()
        {
            return View();
        }

        private void init()
        {
            List<NhomLoaiKtra> nhomKtras = approvePlanReponsitory.GetDmNhomKtra();
            nhomkt = nhomKtras[0].ma_loai_ktr;

            lstPhongBan = new List<PhongBan>();
            lstPhongBan = getDsPhongBan();
            doiid = lstPhongBan[0].Id;

            thang = DateTime.Now.Month.ToString();
            nam = DateTime.Now.Year.ToString();
        }

        public JsonResult getDmNhomKtr()
        {
            var nhomKtras = new List<NhomLoaiKtra>();
            nhomKtras = approvePlanReponsitory.GetDmNhomKtra();
            nhomkt = nhomKtras[0].ma_loai_ktr;

            return Json(nhomKtras, JsonRequestBehavior.AllowGet);
        }

        private List<PhongBan> getDsPhongBan()
        {
            var lstDmPban = new List<PhongBan>();
            lstDmPban = approvePlanReponsitory.GetDmPhongBan(Session["DonViID"].ToString(), 1);
            return lstDmPban;
        }

        public JsonResult getDmDoi()
        {
            string donvi = Session["DonViID"].ToString();
            if (lstPhongBan == null)
            {
                lstPhongBan = approvePlanReponsitory.GetDmPhongBan(donvi, 1);
                doiid = lstPhongBan[0].Id;
            }

            return Json(lstPhongBan, JsonRequestBehavior.AllowGet);
        }

        //Lấy danh sách đơn vị
        public JsonResult GetDmDonvi()
        {
            if (lstDonvis == null)
            {
                lstDonvis = approvePlanReponsitory.GetDmDonVi(Session["DonViID"].ToString());
            }

            return Json(lstDonvis, JsonRequestBehavior.AllowGet);
        }
        /*
         * 1.Get All tree Asset => Api Pmis
         * 2. Get Tree by TeamId => add stats to tree all => Ecp
         * 3. Get List Ecp => add properties. => Api PMIS
         */

        // GET: Ds Dz/Tba -> phân công cho đội
        public List<ApprovePlanViewModel> getAsset(string doiid, string thang, string nam)
        {
            string madvql = Session["DonViID"].ToString();
            List<ApprovePlanViewModel> assets = new List<ApprovePlanViewModel>();

            assets = approvePlanReponsitory.GetAssetByTeam(madvql, doiid, thang, nam).ToList();
            //assets = approvePlanReponsitory.GetAssetByTeam(madvql, doiid, thang, nam, nhomkt).ToList();

            return assets;
        }

        public JsonResult getLstAssetByTeamId(string doiid, string thang, string nam)
        {
            string madvql = Session["DonViID"].ToString();
            List<ApprovePlanViewModel> assets = new List<ApprovePlanViewModel>();

            //assets = approvePlanReponsitory.GetAssetByTeam(madvql, doiid, thang, nam, nhomkt).ToList();
            assets = approvePlanReponsitory.GetAssetByTeam(madvql, doiid, thang, nam).ToList();
            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        //Get: table json
        [HttpGet]
        public string getTableJson(string vthang, string vnam, string vdoiid, string vnhomkt)
        {
            doiid = vdoiid;
            nhomkt = vnhomkt;
            thang = vthang;
            nam = vnam;
            if (String.IsNullOrEmpty(doiid) || String.IsNullOrEmpty(nhomkt))
            {
                init();
            }
            //Get parameter
            return getTreeTableAsync().Result.ToString();
        }

        //Hàm lấy chuỗi json: có thể truyền biến vào
        private async Task<string> getTreeTableAsync()
        {
            string donvi = string.Empty;
            string path = url + "/shared/service/S_ServiceClient.jsf";
            //if (!String.IsNullOrEmpty(pdkey))
            //{
            //    path = url + "PMIS_Web/shared/service/S_ServiceClient.jsf?SOAP_NAME=at_share_dz_JSON&PDKEY="+pdkey+"&DONVI=" + donvi;
            //}
            //else
            //{/
            //    path = url + "PMIS_Web/shared/service/S_ServiceClient.jsf?SOAP_NAME=at_share_dz_JSON&PDKEY=?&DONVI=" + donvi;
            //}

            if (donvi == null)
            {
                donvi = Session["DonViID"].ToString();
            }
            else
            {
                donvi = Request.QueryString["vdonvi"].ToString();
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(path);
            var request = new RestRequest();

            // Thêm Header
            request.AddHeader("Accept", "application/x-www-form-urlencoded");

            // Thêm dữ liệu vào form-data (POST)
            request.AddParameter("SOAP_NAME", "at_share_dz_JSON");
            request.AddParameter("PDKEY", "?");
            request.AddParameter("DONVI", donvi);

            // Gửi request
            RestResponse response = client.PostAsync(request).Result;
            // Kiểm tra kết quả
            //return response.IsSuccessful ? response.Content : null;
            if (response.IsSuccessful)
            {
               var kq = response.Content.Replace("lst", "lstXT");

                var data = JsonConvert.DeserializeObject<List<pmisdata>>(kq);
                var dataAudit = convertListXTtoAssetAudit(data[0].lstXT);

                var t = CreateTreeXT(dataAudit);

                return JsonConvert.SerializeObject(t);
            }
            else
            {
                return null;
            }


        }
        

        private async Task<string> getAssetFromPmis(string dsAsset, string maNhomkt, string thang, string nam)
        {
            string path = url + "/shared/service/S_ServiceClient.jsf";

            var client = new RestClient(path);
            var request = new RestRequest();

            // Thêm Header
            request.AddHeader("Accept", "application/x-www-form-urlencoded");

            // Thêm dữ liệu vào form-data (POST)
            request.AddParameter("SOAP_NAME", "at_type_info_JSON");
            request.AddParameter("PDKEY", "?");
            request.AddParameter("ds_dt", dsAsset);
            request.AddParameter("ma_nhom_ktr", maNhomkt);
            request.AddParameter("thang", thang);
            request.AddParameter("nam", nam);

            // Gửi request
            RestResponse response =  client.PostAsync(request).Result; 
            // Kiểm tra kết quả
            //return response.IsSuccessful ? response.Content : null;
            if (response.IsSuccessful)
            {
                return JsonConvert.SerializeObject(response.Content);
            }
            else
            {
                return null;
            }
        }

        //Hàm lấy giá trị từ Ecp

        private List<LstAssetAudit> convertListXTtoAssetAudit(List<LstXT> lstXTs)
        {
            List<LstAssetAudit> lstAssetAudits = new List<LstAssetAudit>();
            List<ObjAssetAudit> objAssetAudits = new List<ObjAssetAudit>();
            ObjAssetAudit objAssetAudit;
            List<ApprovePlanViewModel> approvePlanViewModels = getAsset(doiid, thang, nam);
            //Lấy danh sách id đã phân công cho đội.
            List<AssetAudit> lstAssets = new List<AssetAudit>();

            foreach (ApprovePlanViewModel apv in approvePlanViewModels)
            {
                AssetAudit aud = new AssetAudit();
                aud.ASSETID = apv.AssetId;
                lstAssets.Add(aud);
            }

            string asset = JsonConvert.SerializeObject(lstAssets);
            string passet = "{\"lst\":" + asset + "}";

            var strAsset = getAssetFromPmis(passet, nhomkt, thang, nam).Result;
            //var strAsset = getAssetFromPmis(passet, thang, nam).Result;
            //string sJsonAsset = strAsset.Substring(2).Replace("]\"", "");
            var data = new List<XobjPmis>();
            if (strAsset != null)
            {
                var xxx = strAsset.Replace("lst", "lstXobjPmis").Replace("\\", "").Substring(1).TrimEnd('"');
                if (!xxx.Equals("Không xác định yêu cầu"))
                {
                    data = JsonConvert.DeserializeObject<List<XobjPmis>>(xxx);
                }
            }

            List<ObjAssetPmis> objAssetPmis = new List<ObjAssetPmis>();



            if (data.Count > 0)
            {
                objAssetPmis = data[0].lstXobjPmis;
            }
            if (lstXTs != null)
            {
                foreach (LstXT xt in lstXTs)
                {
                    objAssetAudit = new ObjAssetAudit();
                    objAssetAudit.KIEU = xt.KIEU;
                    objAssetAudit.MA_CHA = xt.MA_CHA;
                    objAssetAudit.MA_DINHDANH = xt.MA_DINHDANH;
                    objAssetAudit.MA_PMIS = xt.MA_PMIS;
                    objAssetAudit.STT = xt.STT;
                    //objAssetAudit.TRANG_THAI = 1;
                    objAssetAudit.TEN = xt.TEN;
                    objAssetAudit.SO_HUU = xt.SO_HUU;
                    objAssetAudit.TTHAI_VHANH = xt.TTHAI_VHANH;

                    foreach (ApprovePlanViewModel ap in approvePlanViewModels)
                    {
                        if (xt.MA_PMIS.Equals(ap.AssetId) && ap.MaLoaiKtra.Equals(nhomkt))
                        {
                            objAssetAudit.SO_CHO_THIEN = ap.SlDangcho;
                            objAssetAudit.SO_THIEN = ap.SlDath;
                            objAssetAudit.TRANG_THAI = 1;

                            break;
                        }
                    }

                    foreach (ObjAssetPmis o in objAssetPmis)
                    {
                        if (xt.MA_PMIS.Equals(o.ASSETID))
                        {
                            objAssetAudit.SO_VTRI = o.SL_VT;
                            objAssetAudit.SO_VTRI_KTRA = o.SL_KTR;
                            objAssetAudit.NGAY_THIEN = o.NGAY_MAX;
                            //objAssetAudit.TRANG_THAI = 2;
                            objAssetAudit.LOAI_KTR = o.LOAI_KTR;

                            break;
                        }
                    }

                    objAssetAudits.Add(objAssetAudit);
                }
            }
            LstAssetAudit a = new LstAssetAudit();
            a.lstAssetAudit = objAssetAudits;
            lstAssetAudits.Add(a);
            return lstAssetAudits;
        }

        [HttpPost]
        public string postTreeTable(string vthang, string vnam, string vloaihinh, string vdoiid)
        {
            this.thang = vthang;
            this.nam = vnam;
            this.nhomkt = vloaihinh;
            this.doiid = vdoiid;

            return getTreeTableAsync().Result.ToString();
        }

        //Post data -> insert to DB
        [HttpPost]
        public string postDataCrS(string doiid, string giobd, string giokt, string dskt)
        {
            string madvql = Session["DonViID"].ToString();
            string userid = User.Identity.Name;
            approvePlanReponsitory.CrSession(doiid, giobd, giokt, userid, madvql, dskt);

            return "SUCCESS";
        }
        /**
         * Xử lý cây
         * Lấy danh sách thiết bị đơn vị để build tree
         * 
         * **/
        private object CreateTreeXT(List<LstAssetAudit> data)
        {
            //dsPhancongAll = getDsPcongByMadvql();

            // create root
            TreeLeaf root = new TreeLeaf("", "99", "root", "", 1, false, true, 0, 0, 0, 0, 0, null, null, null, null);

            // nhung thang khong cha gan vao cay luon
            var dzs = data[0].lstAssetAudit.Where(d => d.MA_CHA == "").ToList();
            string ten = "";
            foreach (var i in dzs)
            {
                ten = i.TEN;

                TreeLeaf u = new TreeLeaf(i.KIEU, i.MA_PMIS, ten, i.MA_PMIS + "-" + i.TEN, 1, i.TRANG_THAI == 1 ? false : true, i.KIEU == "DUONGDAY" ? true : false, i.SO_CHO_THIEN, i.SO_THIEN, i.SOPHIEN, i.SO_VTRI, i.SO_VTRI_KTRA, i.NGAY_THIEN, i.LOAI_KTR, i.SO_HUU, i.TTHAI_VHANH);
                u.children = CreateTreeAll(u, data);
                root.children.Add(u);
            }
            return root;
        }
        // get tree item
        private List<TreeLeaf> CreateTreeAll(TreeLeaf a, List<LstAssetAudit> data)
        {
            try
            {
                var lsx = data[0].lstAssetAudit;
                List<TreeLeaf> c = new List<TreeLeaf>();
                if (lsx != null)
                {
                    var cxt = lsx.Where(d => d.MA_CHA == a.key);
                    string ten = string.Empty;
                    foreach (var i in cxt)
                    {
                        ten = i.TEN;
                        TreeLeaf u = new TreeLeaf(i.KIEU, i.MA_PMIS, ten, i.MA_PMIS + "-" + i.TEN, i.TRANG_THAI, i.TRANG_THAI == 1 ? false : true, i.KIEU == "DUONGDAY" ? true : false, i.SO_CHO_THIEN, i.SO_THIEN, i.SOPHIEN, i.SO_VTRI, i.SO_VTRI_KTRA, i.NGAY_THIEN, i.LOAI_KTR, i.SO_HUU, i.TTHAI_VHANH);
                        c.Add(u);
                        u.children = CreateTreeAll(u, data);
                    }
                }

                return c;
            }
            catch (Exception ex) { throw ex; }

        }
    }

    public class ObjAssetAudit
    {
        public int STT { get; set; }
        public string MA_CHA { get; set; }
        public string MA_PMIS { get; set; }
        public string TEN { get; set; }
        public string MA_DINHDANH { get; set; }
        public int SO_THIEN { get; set; }
        public int SO_CHO_THIEN { get; set; }
        public string NGAY_THIEN { get; set; }
        public int SO_VTRI { get; set; }
        public int SO_VTRI_KTRA { get; set; }
        public int TRANG_THAI { get; set; }
        public int SOPHIEN { get; set; }
        public string KIEU { get; set; }
        public string LOAI_KTR { get; set; }

        public string SO_HUU { get; set; }
        public string TTHAI_VHANH { get; set; }

    }

    //Object Pmis Asset
    public class ObjAsset
    {
        public int STT { get; set; }
        public string MA_CHA { get; set; }
        public string MA_PMIS { get; set; }
        public string TEN_CHA { get; set; }
        public string TEN { get; set; }
        public string MA_DINHDANH { get; set; }
        public string KIEU { get; set; }
        public string SO_HUU { get; set; }
        public string TTHAI_VHANH { get; set; }
    }

    public class XobjPmis
    {
        public List<ObjAssetPmis> lstXobjPmis { get; set; }
    }

    //Object Pmis Asset Audit
    public class ObjAssetPmis
    {
        public string ASSETID { get; set; }
        public int SL_VT { get; set; }
        public int SL_KTR { get; set; }
        public string LOAI_KTR { get; set; }
        public string NGAY_MAX { get; set; }
        public string NGAY_MIN { get; set; }
        public int THANG { get; set; }
        public int NAM { get; set; }
    }

    //List Object Pmis Asset

    public class LstObjAsset
    {
        public List<ObjAsset> lstObjAsset { get; set; }
    }

    public class LstAssetAudit
    {
        public List<ObjAssetAudit> lstAssetAudit { get; set; }
    }

    public class TreeLeaf
    {
        /// <summary>
        /// Gets the Title.
        /// </summary>
        public string title;

        /// <summary>
        /// Gets the Tooltip.
        /// </summary>
        public string tooltip;

        public decimal parent_id;
        /// <summary>
        /// Gets the key.
        /// </summary>
        public string key;

        /// <summary>
        /// Gets the Children.
        /// </summary>
        public IList<TreeLeaf> children;

        /// <summary>
        /// Gets the rel attr.
        /// </summary>
        public string rel;

        /// <summary>
        /// Gets the State.
        /// </summary>
        public bool folder;

        /// <summary>
        /// Gets the State.
        /// </summary>
        public bool isLazy;

        public int owner;
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeLeaf"/> class.
        /// </summary>
        public TreeLeaf()
        {
            children = new List<TreeLeaf>();
        }
        public string type;
        public int share;
        public bool expanded;
        public bool unselectable;
        public string icon;
        public int slcho;
        public int slthien;
        public int sovitri;
        public int sovtktra;
        public string ngaythien;
        public int sophien;
        public string loaiktra;
        public object data;
        public string sohuu;
        public string tthaivhanh;
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemLeaf"/> class.
        /// </summary>
        /// <param name="type">The type of node.</param>
        /// <param name="id">The Id of the node.</param>
        /// <param name="title">The Title of the node.</param>
        /// <param name="tooltip">The Tooltip of the node.</param>
        public TreeLeaf(string type, string id, String _title, String tooltip, int trangthai, bool unselectable, bool f, int slcho, int slthien, int sophien, int sovitri, int sovtktra, string ngaythien, string loaiktra, string sohuu, string tthaivhanh)
        {
            key = id;
            this.title = _title;
            folder = f;
            isLazy = false;
            this.tooltip = type;
            expanded = false;
            //icon = "/Content/Images/Edit.png";
            this.slcho = slcho;
            this.slthien = slthien;
            this.unselectable = unselectable;
            this.sophien = sophien;
            this.sovitri = sovitri;
            this.sovtktra = sovtktra;
            this.ngaythien = ngaythien;
            this.loaiktra = loaiktra;
            this.sohuu = sohuu;
            this.tthaivhanh = tthaivhanh;
            children = new List<TreeLeaf>();
        }

    }
}