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
    public class ChiTietBaoCaoCuoiNgayRepository : RepositoryBase<tblChiTietBaoCaoCuoiNgay>
    {
        public ChiTietBaoCaoCuoiNgayRepository()
          : base()
        {
        }

        public ChiTietBaoCaoCuoiNgayRepository(WorkUnit unit)
            : base(unit)
        {
        }
        public override object Create(tblChiTietBaoCaoCuoiNgay entity, ref string strError)
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
                var entity = Context.tblChiTietBaoCaoCuoiNgays.SingleOrDefault(o => o.Id == id);
                Context.tblChiTietBaoCaoCuoiNgays.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch(Exception ex) {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblChiTietBaoCaoCuoiNgay GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var baocao = Context.tblChiTietBaoCaoCuoiNgays.Where(p => p.Id == id).Single();
                return baocao;
            }
            catch 
            {
                return null;
            }
        }

        public override List<tblChiTietBaoCaoCuoiNgay> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblChiTietBaoCaoCuoiNgay> List()
        {
            try
            {
                return Context.tblChiTietBaoCaoCuoiNgays.ToList();
            }
            catch { return null; }
        }

        public override List<tblChiTietBaoCaoCuoiNgay> ListByQuery(string strQuery)
        {
            try
            {
                var id = int.Parse(strQuery.ToString());
                var baocao = Context.tblChiTietBaoCaoCuoiNgays.Where(p => p.IdBaoCao == id).ToList();
                return baocao;
            }
            catch
            {
                return null;
            }
        }

        public override object Update(tblChiTietBaoCaoCuoiNgay entity, ref string strError)
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
