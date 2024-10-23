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
    public class TaiLieuSoCCDCRepository : RepositoryBase<SoTheoDoiCCDCAT_TaiLieu>
    {
        public TaiLieuSoCCDCRepository()
            : base()
        {
        }

        public TaiLieuSoCCDCRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.SoTheoDoiCCDCAT_TaiLieu.SingleOrDefault(o => o.ID == id);
                Context.SoTheoDoiCCDCAT_TaiLieu.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(SoTheoDoiCCDCAT_TaiLieu entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override SoTheoDoiCCDCAT_TaiLieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.SoTheoDoiCCDCAT_TaiLieu.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<SoTheoDoiCCDCAT_TaiLieu> List()
        {
            try
            {
                return Context.SoTheoDoiCCDCAT_TaiLieu.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<SoTheoDoiCCDCAT_TaiLieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(SoTheoDoiCCDCAT_TaiLieu entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<SoTheoDoiCCDCAT_TaiLieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


    }
}
