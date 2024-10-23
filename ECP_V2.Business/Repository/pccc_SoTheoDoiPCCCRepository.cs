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
    public class pccc_SoTheoDoiPCCCRepository : RepositoryBase<pccc_SoTheoDoiPCCC>
    {
        public string Connectstr { get; set; }
        public pccc_SoTheoDoiPCCCRepository()
            : base()
        {
        }

        public pccc_SoTheoDoiPCCCRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.pccc_SoTheoDoiPCCC.SingleOrDefault(o => o.ID == id);
                Context.pccc_SoTheoDoiPCCC.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(pccc_SoTheoDoiPCCC entity, ref string strError)
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

        public override pccc_SoTheoDoiPCCC GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.pccc_SoTheoDoiPCCC.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<pccc_SoTheoDoiPCCC> List()
        {
            try
            {
                return Context.pccc_SoTheoDoiPCCC.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<pccc_SoTheoDoiPCCC> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(pccc_SoTheoDoiPCCC entity, ref string strError)
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

        public override List<pccc_SoTheoDoiPCCC> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


    }
}
