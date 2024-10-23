using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class SoTheoDoiCCDCRepository : RepositoryBase<SoTheoDoiCCDCAT>
    {
        public string Connectstr { get; set; }
        public SoTheoDoiCCDCRepository()
            : base()
        {
        }

        public SoTheoDoiCCDCRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.SoTheoDoiCCDCATs.SingleOrDefault(o => o.ID == id);
                Context.SoTheoDoiCCDCATs.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(SoTheoDoiCCDCAT entity, ref string strError)
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

        public override SoTheoDoiCCDCAT GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.SoTheoDoiCCDCATs.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<SoTheoDoiCCDCAT> List()
        {
            try
            {
                return Context.SoTheoDoiCCDCATs.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<SoTheoDoiCCDCAT> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(SoTheoDoiCCDCAT entity, ref string strError)
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

        public override List<SoTheoDoiCCDCAT> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


    }

}
