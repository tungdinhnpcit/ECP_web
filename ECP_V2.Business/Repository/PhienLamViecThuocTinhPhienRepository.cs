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
    public class PhienLamViecThuocTinhPhienRepository : RepositoryBase<tblPhienLamViec_ThuocTinh>
    {
        public PhienLamViecThuocTinhPhienRepository()
            : base()
        {
        }

        public PhienLamViecThuocTinhPhienRepository(WorkUnit unit)
            : base(unit)
        {
        }
        

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblPhienLamViec_ThuocTinh.SingleOrDefault(o => o.PhienLamViecId == id);
                Context.tblPhienLamViec_ThuocTinh.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tblPhienLamViec_ThuocTinh entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.PhienLamViecId;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override tblPhienLamViec_ThuocTinh GetById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<tblPhienLamViec_ThuocTinh> GetByPhienLamViecId(object phienLamViecId)
        {
            try
            {
                int id = int.Parse(phienLamViecId.ToString());
                return Context.tblPhienLamViec_ThuocTinh.Where(x => x.PhienLamViecId == id).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public override List<tblPhienLamViec_ThuocTinh> List()
        {
            try
            {
                return Context.tblPhienLamViec_ThuocTinh.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tblPhienLamViec_ThuocTinh> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(tblPhienLamViec_ThuocTinh entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.PhienLamViecId;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override List<tblPhienLamViec_ThuocTinh> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
