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
    public class AspNetUserRepository : RepositoryBase<AspNetUser>
    {
        static string DBName { get; set; }
        public AspNetUserRepository()
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

        public AspNetUserRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override AspNetUser GetById(object entityId)
        {
            try
            {
                string id = entityId.ToString();
                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    var lst = (List<AspNetUser>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.AspNetUsers.ToList();
                    MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public AspNetUser GetByUserName(string userName)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    var lst = (List<AspNetUser>)datacache;
                    return lst.SingleOrDefault(p => p.UserName.ToUpper() == userName.ToUpper());
                }
                else
                {
                    var lst = Context.AspNetUsers.ToList();
                    MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.UserName.ToUpper() == userName.ToUpper());
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
                string id = entityId.ToString();
                var entity = Context.AspNetUsers.SingleOrDefault(o => o.Id == id);
                Context.AspNetUsers.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_AspNetUserGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", Context.AspNetUsers.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(AspNetUser entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_AspNetUserGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", Context.AspNetUsers.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<AspNetUser> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    var lst = (List<AspNetUser>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.AspNetUsers.ToList();
                    MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch { return null; }
        }


        public override List<AspNetUser> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(AspNetUser entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_AspNetUserGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_AspNetUserGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_AspNetUserGetAll", Context.AspNetUsers.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<AspNetUser> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
