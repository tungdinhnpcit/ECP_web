using ECP_V2.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class PlanController : Controller
    {
        ApprovePlanReponsitory approvePlanReponsitory = new ApprovePlanReponsitory();
        List<DsPhancong> dsPhancongAll = new List<DsPhancong>();
        // GET: Admin/Plan
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string getData(string vdonvi)
        {
            //string donvi = Session["DonViID"].ToString();
            //string donvi = Request.QueryString.ToString().ToLower();
            string donvi = "";
            if (vdonvi == null)
            {
                donvi = Session["DonViID"].ToString();
            }
            else
            {
                donvi = vdonvi;
            }
            string url = System.Configuration.ConfigurationManager.AppSettings["API_PMIS"].ToString();
            string path = url + "/shared/service/S_ServiceClient.jsf?SOAP_NAME=at_share_dz_JSON&PDKEY=?&DONVI=" + donvi;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient c = new HttpClient();
            var res = c.GetAsync(path).Result;
            if (res.IsSuccessStatusCode)
            {
                var kq = res.Content.ReadAsStringAsync().Result;
                kq = kq.Replace("lst", "lstXT");

                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<pmisdata>>(kq);
                var t = CreateTreeXT(data);

                return Newtonsoft.Json.JsonConvert.SerializeObject(t);
            }

            return null;
        }


        //Get: DZ, TBA
        public JsonResult getLoaiKtra(string typeid)
        {
            var lstLoaiktra = new List<LoaiKtra>();
            lstLoaiktra = approvePlanReponsitory.GetLoaiKtras(typeid);

            return Json(lstLoaiktra, JsonRequestBehavior.AllowGet);
        }

        //Get ds Đội
        //Namcv edit: Get by Organization
        public JsonResult getDmDoi()
        {
            string donvi = Session["DonViID"].ToString();
            var lstDmPban = new List<PhongBan>();
            lstDmPban = approvePlanReponsitory.GetDmPhongBan(donvi, 1);

            return Json(lstDmPban, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string postDsPcong(string doiid, string dskt)
        {
            string madvql = Session["DonViID"].ToString();
            //string userid = User.Identity.Name;
            approvePlanReponsitory.PhancongKtra(madvql, doiid, dskt);

            return "SUCCESS";
        }

        [HttpPost]
        public string removeAssignment(string doiid, string dsAsset)
        {
            string madvql = Session["DonViID"].ToString();
            //string userid = User.Identity.Name;
            approvePlanReponsitory.RemoveAssignment(madvql, doiid, dsAsset);

            return "SUCCESS";
        }

        //Get: Ds đã phân công.
        public JsonResult getDsPhancong(string doiql)
        {
            var lstDsPhancong = new List<DsPhancong>();
            lstDsPhancong = approvePlanReponsitory.GetDsPhancongByDoiId(doiql);
            return Json(lstDsPhancong, JsonRequestBehavior.AllowGet);
        }

        //private List<DsPhancong> getDsPcongByMadvql()
        //{
        //    var lstDsPhancong = new List<DsPhancong>();
        //    lstDsPhancong = approvePlanReponsitory.GetDsPhancongByMadvql(Session["DonViID"].ToString());

        //    return lstDsPhancong;
        //}

        private string checkNodeInDsPcong(string assetid)
        {
            foreach (DsPhancong obj in dsPhancongAll)
            {
                if (assetid == obj.assetid)
                {
                    return obj.teamdesc;
                }
            }

            return "";
        }

        private object CreateTreeXT(List<pmisdata> data)
        {
            //dsPhancongAll = getDsPcongByMadvql();   

            // create root
            TreeItemLeaf root = new TreeItemLeaf("", "99", "root", "", true);
            // nhung thang khong cha gan vao cay luon
            var dzs = data[0].lstXT.Where(d => d.MA_CHA == "").ToList();
            string ten = "";
            string doiid = "";
            foreach (var i in dzs)
            {
                doiid = checkNodeInDsPcong(i.MA_PMIS);
                if (doiid != "")
                {
                    ten = i.TEN + " (" + doiid + ")";
                }
                else
                {
                    ten = i.TEN;
                }
                TreeItemLeaf u = new TreeItemLeaf(i.KIEU, i.MA_PMIS, ten, i.MA_PMIS + "-" + i.TEN, i.KIEU == "DUONGDAY" ? true : false);
                u.children = CreateCItem(u, data);
                root.children.Add(u);

            }
            return root;
        }
        // get tree item
        private List<TreeItemLeaf> CreateCItem(TreeItemLeaf a, List<pmisdata> data)
        {
            var lsx = data[0].lstXT;
            string ten = "";
            string doiid = "";
            //var ldz = data[1].lstDZ;
            List<TreeItemLeaf> c = new List<TreeItemLeaf>();
            if (lsx != null)
            {
                var cxt = lsx.Where(d => d.MA_CHA == a.key);

                foreach (var i in cxt)
                {
                    doiid = checkNodeInDsPcong(i.MA_PMIS);
                    if (doiid != "")
                    {
                        ten = i.TEN + " (" + doiid + ")";
                    }
                    else
                    {
                        ten = i.TEN;
                    }
                    TreeItemLeaf u = new TreeItemLeaf(i.KIEU, i.MA_PMIS, ten, i.MA_PMIS + "-" + i.TEN, i.KIEU == "DUONGDAY" ? true : false);
                    c.Add(u);
                    u.children = CreateCItem(u, data);
                }
            }
            //if (ldz != null)
            //{
            //    var cdz = ldz.Where(d => d.MA_CHA == a.key);
            //    foreach (var i in cdz)
            //    {
            //        TreeItemLeaf u = new TreeItemLeaf("", i.MA_PMIS, i.TEN, i.MA_PMIS + "-" + i.TEN);
            //        c.Add(u);
            //        u.children = CreateCItem(u, data);

            //    }
            //}

            return c;

        }

    }

    public class LstXT
    {
        public int STT { get; set; }
        public string MA_CHA { get; set; }
        public string MA_PMIS { get; set; }
        public string TEN_CHA { get; set; }
        public string TEN { get; set; }
        public string MA_DINHDANH { get; set; }
        public string KIEU { get; set; }
        public string TTHAI_VHANH { get; set; }
        public string SO_HUU { get; set; }
    }

    public class LstDZ
    {
        public int STT { get; set; }
        public string MA_CHA { get; set; }
        public string MA_PMIS { get; set; }
        public string TEN { get; set; }
        public string MA_DINHDANH { get; set; }
        public string TTHAI_VHANH { get; set; }
        public string SO_HUU { get; set; }
    }

    public class pmisdata
    {
        public List<LstXT> lstXT { get; set; }
        public List<LstDZ> lstDZ { get; set; }
    }

    public class TreeItemLeaf
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
        public IList<TreeItemLeaf> children;

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
        /// Initializes a new instance of the <see cref="TreeItemLeaf"/> class.
        /// </summary>
        public TreeItemLeaf()
        {
            children = new List<TreeItemLeaf>();
        }
        public string type;
        public int share;
        public bool expanded;
        public string icon;
        public int loai_vb;
        public object data;
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemLeaf"/> class.
        /// </summary>
        /// <param name="type">The type of node.</param>
        /// <param name="id">The Id of the node.</param>
        /// <param name="title">The Title of the node.</param>
        /// <param name="tooltip">The Tooltip of the node.</param>
        public TreeItemLeaf(string type, string id, String _title, String tooltip, bool f)
        {
            key = id;
            this.title = _title;
            folder = f;
            isLazy = false;
            this.tooltip = type;
            expanded = false;
            //icon = "/Content/Images/Edit.png";
            children = new List<TreeItemLeaf>();
        }

    }
}