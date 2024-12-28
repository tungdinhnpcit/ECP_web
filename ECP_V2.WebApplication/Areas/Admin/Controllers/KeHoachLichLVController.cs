using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common.Classes;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Logger;
using ECP_V2.WebApplication.Models;
using ECP_V2.WebApplication.Util;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.PeerToPeer;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
//using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static ECP_V2.WebApplication.Models.ImageModel;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Net.WebRequestMethods;
using Message = ECP_V2.DataAccess.Message;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class KeHoachLichLVController : UTController
    {

        //
        // GET: /Admin/PhienLV/
        private PhienLVRepository _plviec_ser = new PhienLVRepository();
        private ChinhSuaPhienLamViecRepository chinhSuaPhienLamViecRepository = new ChinhSuaPhienLamViecRepository();
        private PhieuCongTacRepository _pcongtac_ser = new PhieuCongTacRepository();
        private DonViRepository _dvi_ser = new DonViRepository();
        private PhongBanRepository _pban_ser = new PhongBanRepository();
        private NhanVienRepository _nhanvien_ser = new NhanVienRepository();
        private MessagesRepository messagesRepository = new MessagesRepository();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        private NhanVienRepository _kh_ser = new NhanVienRepository();
        private NhanVienPhieuCongTacRepositor _nvphieucontac = new NhanVienPhieuCongTacRepositor();
        private AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
        private ThuocTinhPhienRepository thuocTinhPhienRepository = new ThuocTinhPhienRepository();
        private TinhChatPhienRepository tinhChatPhienRepository = new TinhChatPhienRepository();
        private TrangThaiPhienRepository trangThaiPhienRepository = new TrangThaiPhienRepository();
        private PhienLamViecThuocTinhPhienRepository phienLamViecThuocTinhPhienRepository = new PhienLamViecThuocTinhPhienRepository();
        private NhatKyPhienLamViecRepository nhatKyPhienLamViecRepository = new NhatKyPhienLamViecRepository();
        private ImagesRepository _img_ser = new ImagesRepository();
        private GroupImageRepository _groupimg_ser = new GroupImageRepository();
        private LichSuCapSoPhieuRepository _lsCapSoPhieu_ser = new LichSuCapSoPhieuRepository();
        private SoPhieuHienTaiRepository _SoPhieuHienTai_ser = new SoPhieuHienTaiRepository();
        private ThangLamViecRepository _thanglamviec_ser = new ThangLamViecRepository();
        private d_LoaiCongViecRepository loaicv_ser = new d_LoaiCongViecRepository();
        private TramRepository tramRepository = new TramRepository();
        private KeHoachLichLamViecRepository _keHoachLichLamViecRepository = new KeHoachLichLamViecRepository();
        private string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;

        SafeTrainRepository safeTrainRepository = new SafeTrainRepository();
        ApprovePlanReponsitory appPlanRepo = new ApprovePlanReponsitory();

        //Call API để convert html tới pdf.        
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //string path = url + "ConvertHtmlToPdf";
        string path = System.Configuration.ConfigurationManager.AppSettings["API_CONVERT"].ToString() + "jxlsToFile";
        //Lấy file template từ database
        //[AreaAuthorization]


       

        public ActionResult DanhSachPLVChuaKiemTra(string NgayLamViec)
        {
            List<PhienLVModel> PhienLV = null;
            try
            {
                PhienLV =  _plviec_ser.Get_Plv_ChuaKiemTra_TheoNgay(NgayLamViec);
            }
            catch (Exception ex)
            {
                var emptyItem = new List<PhienLVModel> { new PhienLVModel { Id = 0, NoiDung = "Không có dữ liệu" } };
                return PartialView(emptyItem); // Trả về danh sách rỗng hoặc tùy chọn mặc định            }
            }
            if (PhienLV == null || !PhienLV.Any())
            {
                var emptyItem = new List<PhienLVModel> { new PhienLVModel { Id = 0, NoiDung = "Không có dữ liệu" } };
                return PartialView(emptyItem); // Trả về danh sách rỗng hoặc tùy chọn mặc định
            }

            return PartialView( PhienLV);
        }


    }
}