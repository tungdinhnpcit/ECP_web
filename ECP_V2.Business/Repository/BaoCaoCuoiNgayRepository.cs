using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class BaoCaoCuoiNgayRepository : RepositoryBase<tblBaoCaoCuoiNgay>
    {
        public BaoCaoCuoiNgayRepository()
          : base()
        {
        }

        public BaoCaoCuoiNgayRepository(WorkUnit unit)
            : base(unit)
        {
        }
        public override object Create(tblBaoCaoCuoiNgay entity, ref string strError)
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

        public override string Delete(object entityId,ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblBaoCaoCuoiNgays.SingleOrDefault(o => o.Id == id);
                Context.tblBaoCaoCuoiNgays.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch(Exception ex) {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblBaoCaoCuoiNgay GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var baocao = Context.tblBaoCaoCuoiNgays.Where(p => p.Id == id).Single();
                return baocao;
            }
            catch 
            {
                return null;
            }
        }

        public override List<tblBaoCaoCuoiNgay> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblBaoCaoCuoiNgay> List()
        {
            try
            {
                return Context.tblBaoCaoCuoiNgays.ToList();
            }
            catch { return null; }
        }

        public override List<tblBaoCaoCuoiNgay> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblBaoCaoCuoiNgay entity, ref string strError)
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
    }
}
