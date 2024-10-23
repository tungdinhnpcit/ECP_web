using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class MenuOfRoleRepository : RepositoryBase_V2
    {
        public MenuOfRoleRepository()
         : base()
        {
        }

        public MenuOfRoleRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public MenuOfRole Add(MenuOfRole news)
        {
            Context.MenuOfRoles.Add(news);
            Context.SaveChanges();


            return news;
        }

        public bool CheckExist(string roleId, int menuId)
        {
            return Context.MenuOfRoles.FirstOrDefault(x => x.RoleId.Equals(roleId) && x.MenuId == menuId) != null;
        }

        public bool CheckRole(string roleId, string menuCode, int roleView)
        {
            var menu = Context.Menus.Where(x => x.Code.Equals(menuCode) && (x.RoleView == null || x.RoleView == roleView)).FirstOrDefault();
            if (menu == null)
                return false;

            return Context.MenuOfRoles.FirstOrDefault(x => x.RoleId.Equals(roleId) && x.MenuId ==  menu.Id) != null;
        }

        public bool Delete(int entityId)
        {
            try
            {
                var entity = Context.MenuOfRoles.SingleOrDefault(o => o.Id == entityId);
                Context.MenuOfRoles.Remove(entity);

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void DeleteAllByRoleId(string roleId)
        {
            var list = Context.MenuOfRoles.Where(x => x.RoleId.Equals(roleId)).ToList();

            Context.MenuOfRoles.RemoveRange(list);
            Context.SaveChanges();
        }

        public List<NewMenuViewModel> GetAllMenuByRole(string roleId, int roleView)
        {
            var query = from a in Context.MenuOfRoles.Where(x => x.RoleId.Equals(roleId) && x.Status)
                        join b in Context.Menus on a.MenuId equals b.Id
                        where b.IsDelete != true && b.IsShowMenu != false && ( b.RoleView == null || b.RoleView == roleView)
                        select new NewMenuViewModel()
                        {
                            Class = b.Class,
                            Order = b.Order,
                            Description = b.Description,
                            Text = b.Text,
                            Code = b.Code,
                            Id = b.Id,
                            Level = b.Level,
                            ParentId = b.ParentId,
                            Url = b.Url,
                            IsDelete = b.IsDelete,
                            NgaySua = b.NgaySua,
                            NgayTao = b.NgayTao,
                            NgayXoa = b.NgayXoa,
                            NguoiSua = b.NguoiSua,
                            NguoiTao = b.NguoiTao,
                            NguoiXoa = b.NguoiXoa,
                            IsFrontPage = b.IsFrontPage,
                            IsNewLetter = b.IsNewLetter,
                            IsShowMenu = b.IsShowMenu,
                            RoleView = b.RoleView
                        };

            return query.OrderBy(x => x.ParentId).ThenBy(x => x.Order).Distinct().ToList();
        }

        public List<NewMenuViewModel> GetOptionSelectByRole(string roleId)
        {
            var query = from a in Context.Menus.Where(x => x.IsDelete != true)
                        join b in Context.MenuOfRoles.Where(x => x.RoleId.Equals(roleId) && x.Status) on a.Id equals b.MenuId into ps
                        from p in ps.DefaultIfEmpty()
                        select new NewMenuViewModel()
                        {
                            Class = a.Class,
                            Order = a.Order,
                            Description = a.Description,
                            Text = a.Text,
                            Code = a.Code,
                            Id = a.Id,
                            Level = a.Level,
                            ParentId = a.ParentId,
                            Url = a.Url,
                            IsDelete = a.IsDelete,
                            NgaySua = a.NgaySua,
                            NgayTao = a.NgayTao,
                            NgayXoa = a.NgayXoa,
                            NguoiSua = a.NguoiSua,
                            NguoiTao = a.NguoiTao,
                            NguoiXoa = a.NguoiXoa,
                            IsFrontPage = a.IsFrontPage,
                            IsNewLetter = a.IsNewLetter,
                            Check = p != null ? true: false
                        };

            return query.OrderBy(x => x.ParentId).ThenBy(x => x.Order).Distinct().ToList();
        }

        public List<RoleViewModel> GetAllRole()
        {
            var model = Context.AspNetRoles.AsNoTracking().Where(x => x.TypeOfRole == 1).Select(a => new RoleViewModel()
            {
                Description = a.Description,
                Discriminator = a.Discriminator,
                Id = a.Id,
                Name = a.Name,
                TypeOfRole = a.TypeOfRole
            });

            return model.ToList();
        }

        public MenuOfRole GetById(int id)
        {
            return Context.MenuOfRoles.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public MenuOfRoleViewModel GetInfoById(int id)
        {
            var a = Context.MenuOfRoles.AsNoTracking().SingleOrDefault(x => x.Id == id);

            var config = new MenuOfRoleViewModel()
            {
                Id = a.Id,
                MenuId = a.MenuId,
                RoleId = a.RoleId,
                Status = a.Status
            };

            return config;
        }

        public List<MenuOfRoleViewModel> GetAll()
        {
            return Context.MenuOfRoles.AsNoTracking().Select(a => new MenuOfRoleViewModel()
            {
                Id = a.Id,
                MenuId = a.MenuId,
                RoleId = a.RoleId,
                Status = a.Status
            }).ToList();
        }

    }

    public class MenuOfRoleViewModel
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int MenuId { get; set; }
        public bool Status { get; set; }
    }

    public class RoleViewModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public string Description { get; set; }
        public Nullable<int> TypeOfRole { get; set; }
    }
}
