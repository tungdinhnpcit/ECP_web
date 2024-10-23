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
    public class TrangThaiPhieuRepository : RepositoryBase<plv_TrangThaiPhieu>
    {
        static string DBName { get; set; }
        public TrangThaiPhieuRepository()
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

        public TrangThaiPhieuRepository(WorkUnit unit)
            : base(unit)
        {
        }
        

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_TrangThaiPhieu.SingleOrDefault(o => o.MaTT == id);
                Context.plv_TrangThaiPhieu.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhieuGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TrangThaiPhieuGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TrangThaiPhieuGetAll", Context.plv_TrangThaiPhieu.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(plv_TrangThaiPhieu entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhieuGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_TrangThaiPhieuGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_TrangThaiPhieuGetAll", Context.plv_TrangThaiPhieu.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.MaTT;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override plv_TrangThaiPhieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhieuGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TrangThaiPhieu>)datacache;
                    return lst.SingleOrDefault(o => o.MaTT == id);
                }
                else
                {
                    var lst = Context.plv_TrangThaiPhieu.ToList();
                    MemoryCacheHelper.Add(DBName + "_TrangThaiPhieuGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.MaTT == id);
                }
            }
            catch { return null; }
        }

        public override List<plv_TrangThaiPhieu> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_TrangThaiPhieuGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_TrangThaiPhieu>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.plv_TrangThaiPhieu.ToList();
                    MemoryCacheHelper.Add(DBName + "_TrangThaiPhieuGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_TrangThaiPhieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(plv_TrangThaiPhieu entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<plv_TrangThaiPhieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
