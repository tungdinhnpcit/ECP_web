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
    public class ChiTietBaoCaoRepository : RepositoryBase<tblChiTietBaoCao>
    {
        public ChiTietBaoCaoRepository()
          : base()
        {
        }

        public ChiTietBaoCaoRepository(WorkUnit unit)
            : base(unit)
        {
        }
        public override object Create(tblChiTietBaoCao entity, ref string strError)
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
                var entity = Context.tblChiTietBaoCaos.SingleOrDefault(o => o.Id == id);
                Context.tblChiTietBaoCaos.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch(Exception ex) {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblChiTietBaoCao GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var baocao = Context.tblChiTietBaoCaos.Where(p => p.Id == id).Single();
                return baocao;
            }
            catch 
            {
                return null;
            }
        }

        public override List<tblChiTietBaoCao> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblChiTietBaoCao> List()
        {
            try
            {
                return Context.tblChiTietBaoCaos.ToList();
            }
            catch { return null; }
        }

        public override List<tblChiTietBaoCao> ListByQuery(string strQuery)
        {
            try
            {
                var id = int.Parse(strQuery.ToString());
                var baocao = Context.tblChiTietBaoCaos.Where(p => p.IdBaoCao == id).ToList();
                return baocao;
            }
            catch
            {
                return null;
            }
        }

        public override object Update(tblChiTietBaoCao entity, ref string strError)
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
