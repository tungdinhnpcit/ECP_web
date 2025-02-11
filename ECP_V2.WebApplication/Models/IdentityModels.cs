using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECP_V2.WebApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("IdentityDbContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class IdentityManager
    {
        public bool RoleExists(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.RoleExists(name);
        }

        public IdentityRole GetRole(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.FindByName(name);
        }

        public IdentityRole GetRoleById(string roleId)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.FindById(roleId);
        }

        public IList<IdentityRole> GetAllRole()
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.Roles.ToList();
        }

        //public IList<IdentityRole> GetRoleByType(string type)
        //{
        //    var rm = new RoleManager<IdentityRole>(
        //        new RoleStore<IdentityRole>(new ApplicationDbContext()));
        //    return rm.Roles.Where(p=>p.).ToList();
        //}


        public bool CreateRole(string name)
        {

            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        public bool UpdateRole(IdentityRole role)
        {

            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            //var idResult = rm.Update(rm.FindByName(name));
            var idResult = rm.Update(role);
            return idResult.Succeeded;
        }

        public bool DeleteRole(string id)
        {
            try
            {
                var rm = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
                IdentityRole role = rm.FindById(id);
                var idResult = rm.Delete(role);
                return idResult.Succeeded;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }


        public IdentityResult CreateUser(ApplicationUser user, string password)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.Create(user, password);
            return idResult;
        }

        public bool UpdateUser(ApplicationUser user)
        {
            try
            {
                //IdentityDbContext context = new IdentityDbContext();

                var um = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(new ApplicationDbContext()));

                //context.Entry(user).State = EntityState.Modified;

                var idResult = um.Update<ApplicationUser, string>(user);

                return idResult.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteUser(string id)
        {
            try
            {
                var um = new UserManager<ApplicationUser>(
                   new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = um.FindById(id);
                var idResult = um.Delete(user);
                return idResult.Succeeded;

            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

        public ApplicationUser GetUser(string userName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.FindByName(userName);
            if (idResult != null)
            {
                return idResult;
            }
            else
                return null;
        }

        public IList<string> GetRoleOfUser(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.GetRoles(userId);
            if (idResult != null)
            {
                return idResult;
            }
            else
                return null;
        }

        public IList<string> GetRoleSystemOfUser(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.GetRoles(userId);
            if (idResult != null)
            {
                return idResult;
            }
            else
                return null;
        }

        public string GetRoleOfUserByType(string userId, int typeOfRole)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.GetRoles(userId);

            RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
            var lstRoleType = _roleManager.Roles.Where(p => p.TypeOfRole == typeOfRole && idResult.Contains(p.Name)).Select(p => p.Id);
            if (lstRoleType != null)
            {
                return lstRoleType.FirstOrDefault();
            }
            else
                return null;
        }


        public bool AddUserToRole(string userName, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindByName(userName);
            // Bổ sung kiểm tra nếu tồn tại role thì trả về true
            if (um.IsInRole(user.Id, roleName))
            {
                return true;
            }

            var idResult = um.AddToRole(user.Id, roleName);
            return idResult.Succeeded;
        }

        public bool IsInRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.IsInRole(userId, roleName);
            return idResult;
        }

        public bool RemoveFromRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindById(userId);
            var idResult = um.RemoveFromRole(userId, roleName);
            return idResult.Succeeded;
        }

        public bool RemoveFromRoleByName(string userName, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindByName(userName);
            var idResult = um.RemoveFromRole(user.Id, roleName);
            return idResult.Succeeded;
        }

        public void ClearUserRoles(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                um.RemoveFromRole(userId, role.RoleId);
            }
        }
    }
}