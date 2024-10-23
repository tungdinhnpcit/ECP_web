using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.PagingModel;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class NewMenuRepository : RepositoryBase_V2
    {
        public NewMenuRepository()
         : base()
        {
        }

        public NewMenuRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public Menu Add(Menu news)
        {
            Context.Menus.Add(news);
            Context.SaveChanges();


            return news;
        }

        public bool CheckExistCode(string code)
        {
            return Context.Menus.FirstOrDefault(x => x.Code.Equals(code)) != null;
        }

        public bool Delete(int entityId, string userId)
        {
            try
            {
                var list = Context.MenuOfRoles.Where(x => x.MenuId == entityId).ToList();
                Context.MenuOfRoles.RemoveRange(list);

                var entity = Context.Menus.SingleOrDefault(o => o.Id == entityId);
                Context.Menus.Remove(entity);

                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PagedResult<NewMenuViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = Context.Menus.AsNoTracking().Where(x => x.IsDelete != true);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Text.Contains(keyword) || x.Description.Contains(keyword) || x.Code.Contains(keyword));


            int totalRow = query.Count();

            query = query.OrderBy(x => x.Order)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(a => new NewMenuViewModel()
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
                IsShowMenu = a.IsShowMenu,
                RoleView = a.RoleView
            }).ToList();

            var paginationSet = new PagedResult<NewMenuViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return paginationSet;
        }

        public Menu GetById(int id)
        {
            return Context.Menus.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public NewMenuViewModel GetInfoById(int id)
        {
            var a = Context.Menus.AsNoTracking().SingleOrDefault(x => x.Id == id);

            var config = new NewMenuViewModel()
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
                IsShowMenu = a.IsShowMenu,
                RoleView = a.RoleView
            };

            return config;
        }

        public List<NewMenuViewModel> GetAll()
        {
            return Context.Menus.AsNoTracking().Where(x => x.IsDelete != true).OrderBy(x => x.Order).Select(a => new NewMenuViewModel()
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
                IsShowMenu = a.IsShowMenu,
                RoleView = a.RoleView
            }).ToList();
        }

        public List<NewMenuViewModel> GetAllParent()
        {
            try { 
            return Context.Menus.AsNoTracking().Where(x => x.ParentId == null && x.IsDelete != true).OrderBy(x => x.Order).Select(a => new NewMenuViewModel()
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
                IsShowMenu = a.IsShowMenu,
                RoleView = a.RoleView
            }).ToList();
            }catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NewMenuViewModel> GetAllChildrenByParentId(int id)
        {
            return Context.Menus.AsNoTracking().Where(x => x.ParentId == id && x.IsDelete != true).OrderBy(x => x.Order).Select(a => new NewMenuViewModel()
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
                IsShowMenu = a.IsShowMenu,
                RoleView = a.RoleView
            }).ToList();
        }

        public void Update(Menu moduleVm)
        {

            var module = Context.Menus.SingleOrDefault(x => x.Id == moduleVm.Id);
            if (module != null)
            {
                module.Class = moduleVm.Class;
                module.Code = moduleVm.Code;
                module.Description = moduleVm.Description;
                module.Level = moduleVm.Level;
                module.Order = moduleVm.Order;
                module.ParentId = moduleVm.ParentId;
                module.Text = moduleVm.Text;
                module.Url = moduleVm.Url;
                module.NguoiSua = moduleVm.NguoiSua;
                module.NgaySua = moduleVm.NgaySua;
                module.IsFrontPage = moduleVm.IsFrontPage;
                module.IsNewLetter = moduleVm.IsNewLetter;
                module.IsShowMenu = moduleVm.IsShowMenu;
                module.RoleView = moduleVm.RoleView;


                Context.SaveChanges();
            }

        }
    }

    public class NewMenuViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> Level { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string Order { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Class { get; set; }
        public System.DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<System.DateTime> NgayXoa { get; set; }
        public string NguoiXoa { get; set; }
        public Nullable<bool> IsNewLetter { get; set; }
        public Nullable<bool> IsFrontPage { get; set; }
        public Nullable<bool> IsShowMenu { get; set; }
        public Nullable<int> RoleView { get; set; }
        public bool? Check { get; set; }
    }
}
