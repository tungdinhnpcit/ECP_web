using ECP_V2.WebApplication.Helpers;
using ECP_V2.WebApplication.Models;
using ECP_V2.WebApplication.ModelsView;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{

    [Authorize(Roles = "Admin,AdminDonVi")]
    public class RolesController : Controller
    {
        UltilHelper _helper = new UltilHelper();
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));

        private void DisposeAll()
        {
            //if (_roleManager != null)
            //{
            //    _roleManager.Dispose();
            //    _roleManager = null;
            //}
        }

        // GET: Roles
        [HasCredential(MenuCode = "ROLES")]
        public ActionResult Index()
        {
            var items = _roleManager.Roles.Select(x => new ViewRole
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                TypeOfRole = (int?)x.TypeOfRole ?? 0
            }).ToList();

            DisposeAll();

            return View(items);
        }

        // GET: Roles/Create
        [HasCredential(MenuCode = "ROLES")]
        public ActionResult Create()
        {
            ViewBag.LstRole = new SelectList(_helper.GetTypeOfRoles(), "Id", "Name", "1");

            DisposeAll();

            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Discriminator,Description,TypeOfRole")] ViewRole role)
        {
            if (ModelState.IsValid)
            {
                var x = _roleManager.Create(new ApplicationRole { Name = role.Name, Description = role.Description, TypeOfRole = role.TypeOfRole });
                if (x.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo quyền! \n"
                        + x.Errors.FirstOrDefault());
                }
            }

            ViewBag.LstRole = new SelectList(_helper.GetTypeOfRoles(), "Id", "Name", "1");

            DisposeAll();

            return View();
        }

        // GET: Roles/Edit/5
        [HasCredential(MenuCode = "ROLES")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = _roleManager.Roles.Where(x => x.Id == id).Select(x => new ViewRole { Id = x.Id, Name = x.Name, Description = x.Description, TypeOfRole = x.TypeOfRole.Value }).FirstOrDefault();
            if (item == null)
                return HttpNotFound();


            ViewBag.LstRole = new SelectList(_helper.GetTypeOfRoles(), "Id", "Name", item.TypeOfRole);

            DisposeAll();

            return View(item);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,TypeOfRole")] ViewRole role)
        {
            if (ModelState.IsValid)
            {
                var currentRole = _roleManager.Roles.FirstOrDefault(x => x.Id == role.Id);
                if (currentRole != null)
                {
                    currentRole.Name = role.Name;
                    currentRole.Description = role.Description;
                    currentRole.TypeOfRole = role.TypeOfRole;
                    var result = _roleManager.Update(currentRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể cập nhật quyền!" + result.Errors.FirstOrDefault());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Quyền không tồn tại!");
                }
            }

            ViewBag.LstRole = new SelectList(_helper.GetTypeOfRoles(), "Id", "Name", role.TypeOfRole);

            DisposeAll();

            return View();
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                DisposeAll();

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentRole = _roleManager.Roles.Select(x => new ViewRole { Id = x.Id, Name = x.Name })
                .FirstOrDefault(x => x.Id == id);
            if (currentRole == null)
            {
                DisposeAll();

                return HttpNotFound();
            }

            DisposeAll();

            return View(currentRole);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var currentRole = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
            if (currentRole != null)
            {
                var result = _roleManager.Delete(currentRole);
                if (result.Succeeded)
                {
                    DisposeAll();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Could not delete the role!" + result.Errors.FirstOrDefault());
                }
            }
            else
            {
                ModelState.AddModelError("", "Role does not exist!");
            }

            DisposeAll();

            return View();
        }


        public ActionResult GrantRole()
        {
            DisposeAll();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _roleManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}