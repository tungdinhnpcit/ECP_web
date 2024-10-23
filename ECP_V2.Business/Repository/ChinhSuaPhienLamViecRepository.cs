using ECP_V2.Business.UnitOfWork;
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
    public class ChinhSuaPhienLamViecRepository : RepositoryBase<tblPhienLamViec_ChinhSua>
    {
        public ChinhSuaPhienLamViecRepository()
            : base()
        {
        }

        public ChinhSuaPhienLamViecRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblPhienLamViec_ChinhSua.SingleOrDefault(o => o.Id == id);
                Context.tblPhienLamViec_ChinhSua.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tblPhienLamViec_ChinhSua entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override tblPhienLamViec_ChinhSua GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tblPhienLamViec_ChinhSua.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<tblPhienLamViec_ChinhSua> List()
        {
            try
            {
                return Context.tblPhienLamViec_ChinhSua.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public List<tblPhienLamViec_ChinhSua> ListByPhienLamViecId(int phienLamViecId)
        {
            try
            {
                return Context.tblPhienLamViec_ChinhSua.Where(x => x.PhienLamViecId == phienLamViecId).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tblPhienLamViec_ChinhSua> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblPhienLamViec_ChinhSua entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<tblPhienLamViec_ChinhSua> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
