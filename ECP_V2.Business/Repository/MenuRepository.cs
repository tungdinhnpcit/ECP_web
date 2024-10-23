using ECP_V2.Business.UnitOfWork;
using ECP_V2.Common.Helpers;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class MenuRepository : RepositoryBase<MenuMaster>
    {
        string DBName { get; set; }
        public MenuRepository()
          : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public MenuRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public List<MenuMaster> GetMenuByParentId(int id)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    var data = (List<MenuMaster>)datacache;
                    return data.Where(p => p.MenuParentId == id).ToList();
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(p => p.MenuParentId == id).ToList();
                }
            }
            catch (Exception ex) { return null; }
        }



        public override List<MenuMaster> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    return (List<MenuMaster>)datacache;
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch { return null; }
        }

        public List<MenuMaster> GetByRole(string roleId)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    var data = (List<MenuMaster>)datacache;
                    return data.Where(p => p.RoleId == roleId).ToList();
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(p => p.RoleId == roleId).ToList();
                }
            }
            catch { return null; }
        }


        public MenuMaster GetByName(string menuText)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    var data = (List<MenuMaster>)datacache;
                    return data.SingleOrDefault(p => p.MenuText == menuText);
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.MenuText == menuText);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetMenuById(int menuId)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    var data = (List<MenuMaster>)datacache;
                    return data.Where(p => p.MenuId == menuId).Single().MenuText;
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(p => p.MenuId == menuId).Single().MenuText;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.MenuMasters.SingleOrDefault(o => o.MenuId == id);
                Context.MenuMasters.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GetMenuAll");
                }
                MemoryCacheHelper.Add(DBName + "_GetMenuAll", Context.MenuMasters.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override MenuMaster GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    var data = (List<MenuMaster>)datacache;
                    return data.SingleOrDefault(p => p.MenuId == id);
                }
                else
                {
                    var lst = Context.MenuMasters.ToList();
                    MemoryCacheHelper.Add(DBName + "_GetMenuAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.MenuId == id);
                }
            }
            catch { return null; }
        }

        public override List<MenuMaster> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Create(MenuMaster entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GetMenuAll");
                }
                MemoryCacheHelper.Add(DBName + "_GetMenuAll", Context.MenuMasters.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.MenuId;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override object Update(MenuMaster entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GetMenuAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GetMenuAll");
                }
                MemoryCacheHelper.Add(DBName + "_GetMenuAll", Context.MenuMasters.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.MenuId;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<MenuMaster> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
