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
    public class TrangThaiPhienRepository : RepositoryBase<plv_TrangThaiPhien>
    {
        static string DBName { get; set; }
        public TrangThaiPhienRepository()
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

        public TrangThaiPhienRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_TrangThaiPhien.SingleOrDefault(o => o.Id == id);
                Context.plv_TrangThaiPhien.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TrangThaiPhienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TrangThaiPhienGetAll", Context.plv_TrangThaiPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(plv_TrangThaiPhien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TrangThaiPhienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TrangThaiPhienGetAll", Context.plv_TrangThaiPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override plv_TrangThaiPhien GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TrangThaiPhien>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.plv_TrangThaiPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_TrangThaiPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch { return null; }
        }

        public override List<plv_TrangThaiPhien> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TrangThaiPhien>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.plv_TrangThaiPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_TrangThaiPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_TrangThaiPhien> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(plv_TrangThaiPhien entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<plv_TrangThaiPhien> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
