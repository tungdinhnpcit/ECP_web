using ECP_V2.Business.Repository;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class MenuOfRoleController : Controller
    {
        private MenuOfRoleRepository _menuRepository = new MenuOfRoleRepository();

        // GET: Admin/MenuOfRole
        [HasCredential(MenuCode = "MENUOFROLE")]
        public ActionResult Index()
        {
            var listRole = _menuRepository.GetAllRole();

            return View(listRole);
        }

        public ActionResult GetOptionSelectByRole(string roleId)
        {
            var model = _menuRepository.GetOptionSelectByRole(roleId);
            var listParent = model.Where(x => x.ParentId == null).ToList();
            return Json(new
            {
                data = model,
                listParent = listParent,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(string roleId, List<int> listMenu)
        {
            try
            {
                _menuRepository.DeleteAllByRoleId(roleId);

                foreach (var item in listMenu)
                {
                    if (!_menuRepository.CheckExist(roleId, item))
                    {
                        _menuRepository.Add(new MenuOfRole()
                        {
                            MenuId = item,
                            RoleId = roleId,
                            Status = true
                        });
                    }
                }

                return Json(new
                {
                    status = true,
                    message = "Cập nhật thành công"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Có lỗi xảy ra"
                });
            }
        }
    }
}