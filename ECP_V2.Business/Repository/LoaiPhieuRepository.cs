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
    public class LoaiPhieuRepository : RepositoryBase<plv_LoaiPhieu>
    {
        static string DBName { get; set; }
        public LoaiPhieuRepository()
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

        public LoaiPhieuRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_LoaiPhieu.SingleOrDefault(o => o.MaLP == id);
                Context.plv_LoaiPhieu.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_LoaiPhieuGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_LoaiPhieuGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_LoaiPhieuGetAll", Context.plv_LoaiPhieu.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(plv_LoaiPhieu entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_LoaiPhieuGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_LoaiPhieuGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_LoaiPhieuGetAll", Context.plv_LoaiPhieu.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.MaLP;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override plv_LoaiPhieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_LoaiPhieuGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_LoaiPhieu>)datacache;
                    return lst.SingleOrDefault(o => o.MaLP == id);
                }
                else
                {
                    var lst = Context.plv_LoaiPhieu.ToList();
                    MemoryCacheHelper.Add(DBName + "_LoaiPhieuGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.MaLP == id);
                }
            }
            catch { return null; }
        }

        public override List<plv_LoaiPhieu> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_LoaiPhieuGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_LoaiPhieu>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.plv_LoaiPhieu.ToList();
                    MemoryCacheHelper.Add(DBName + "_LoaiPhieuGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_LoaiPhieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(plv_LoaiPhieu entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<plv_LoaiPhieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
