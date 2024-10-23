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
    public class TinhChatPhienRepository : RepositoryBase<plv_TinhChatPhien>
    {
        static string DBName { get; set; }
        public TinhChatPhienRepository()
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

        public TinhChatPhienRepository(WorkUnit unit)
            : base(unit)
        {
        }
        

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_TinhChatPhien.SingleOrDefault(o => o.Id == id);
                Context.plv_TinhChatPhien.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TinhChatPhienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TinhChatPhienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TinhChatPhienGetAll", Context.plv_TinhChatPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(plv_TinhChatPhien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TinhChatPhienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TinhChatPhienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TinhChatPhienGetAll", Context.plv_TinhChatPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override plv_TinhChatPhien GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TinhChatPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TinhChatPhien>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.plv_TinhChatPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_TinhChatPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }         
            }
            catch { return null; }
        }

        public override List<plv_TinhChatPhien> List()
        {         
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TinhChatPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TinhChatPhien>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.plv_TinhChatPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_TinhChatPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_TinhChatPhien> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(plv_TinhChatPhien entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<plv_TinhChatPhien> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
