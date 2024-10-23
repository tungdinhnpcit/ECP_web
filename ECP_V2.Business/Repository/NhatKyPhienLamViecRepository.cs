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
    public class NhatKyPhienLamViecRepository : RepositoryBase<tblPhienLamViec_NhatKy>
    {
        public NhatKyPhienLamViecRepository()
            : base()
        {
        }

        public NhatKyPhienLamViecRepository(WorkUnit unit)
            : base(unit)
        {
        }
        

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblPhienLamViec_NhatKy.SingleOrDefault(o => o.Id == id);
                Context.tblPhienLamViec_NhatKy.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tblPhienLamViec_NhatKy entity, ref string strError)
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

        public override tblPhienLamViec_NhatKy GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tblPhienLamViec_NhatKy.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<tblPhienLamViec_NhatKy> List()
        {
            try
            {
                return Context.tblPhienLamViec_NhatKy.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tblPhienLamViec_NhatKy> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override List<tblPhienLamViec_NhatKy> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblPhienLamViec_NhatKy entity, ref string strError)
        {
            throw new NotImplementedException();
        }
    }
}
