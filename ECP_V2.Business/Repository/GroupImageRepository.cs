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
    public class GroupImageRepository : RepositoryBase<tblGroupImage>
    {
        static string DBName { get; set; }
        public GroupImageRepository()
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

        public GroupImageRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(Object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblGroupImages.SingleOrDefault(o => o.Id == id);
                Context.tblGroupImages.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GroupImageGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GroupImageGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_GroupImageGetAll", Context.tblGroupImages.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override tblGroupImage GetById(Object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GroupImageGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblGroupImage>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.tblGroupImages.ToList();
                    MemoryCacheHelper.Add(DBName + "_GroupImageGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override List<tblGroupImage> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_GroupImageGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblGroupImage>)datacache;
                    return lst.OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    var lst = Context.tblGroupImages.ToList();
                    MemoryCacheHelper.Add(DBName + "_GroupImageGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public override List<tblGroupImage> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Create(tblGroupImage entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GroupImageGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GroupImageGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_GroupImageGetAll", Context.tblGroupImages.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override object Update(tblGroupImage entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_GroupImageGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_GroupImageGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_GroupImageGetAll", Context.tblGroupImages.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<tblGroupImage> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
