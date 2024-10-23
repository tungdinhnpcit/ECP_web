using ECP_V2.Business.Repository;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Controllers
{
    public class BaoCaoTongHopController : Controller
    {
        BaoCaoRepository baoCaoRepository = new BaoCaoRepository();
        BaoCaoCuoiNgayRepository baoCaoCuoiNgayRepository = new BaoCaoCuoiNgayRepository();

        private void DisposeAll()
        {
            if (baoCaoRepository != null)
            {
                baoCaoRepository.Dispose();
                baoCaoRepository = null;
            }

            if (baoCaoCuoiNgayRepository != null)
            {
                baoCaoCuoiNgayRepository.Dispose();
                baoCaoCuoiNgayRepository = null;
            }
        }

        // GET: BaoCaoTongHop
        public ActionResult Index()
        {
            DisposeAll();

            return View();
        }

        public ActionResult List()
        {
            var baoCaoList = baoCaoCuoiNgayRepository.List();

            DisposeAll();

            return PartialView("List", baoCaoList);
        }

        public ActionResult BaoCaoTongHopDauGio()
        {
            DisposeAll();

            return View();
        }

        public ActionResult ListTongHopDauGio()
        {
            var baoCaoList = baoCaoRepository.List();

            DisposeAll();

            return PartialView("ListTongHopDauGio", baoCaoList);
        }
    }
}