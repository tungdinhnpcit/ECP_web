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
    public class SystemConfigRepository : RepositoryBase<SystemConfig>
    {
        public SystemConfigRepository()
            : base()
        {
        }

        public SystemConfigRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override SystemConfig GetById(object entityId)
        {
            try
            {
                int id = int.Parse(entityId.ToString());
                var entity = Context.SystemConfigs.Where(p => p.ID == id).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SystemConfig GetByName(string name)
        {
            try
            {
                var entity = Context.SystemConfigs.Where(p => p.Name == name).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                int id = int.Parse(entityId.ToString());
                var entity = Context.SystemConfigs.SingleOrDefault(o => o.ID == id);
                Context.SystemConfigs.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(SystemConfig entity, ref string strError)
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

        public override List<SystemConfig> List()
        {
            try
            {
                return Context.SystemConfigs.ToList();
            }
            catch { return null; }
        }


        public override List<SystemConfig> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(SystemConfig entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<SystemConfig> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
