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
    public class tbnn_SoTheoDoiTBNNRepository : RepositoryBase<tbnn_SoTheoDoiTBNN>
    {
        public string Connectstr { get; set; }
        public tbnn_SoTheoDoiTBNNRepository()
            : base()
        {
        }

        public tbnn_SoTheoDoiTBNNRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tbnn_SoTheoDoiTBNN.SingleOrDefault(o => o.ID == id);
                Context.tbnn_SoTheoDoiTBNN.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tbnn_SoTheoDoiTBNN entity, ref string strError)
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

        public override tbnn_SoTheoDoiTBNN GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tbnn_SoTheoDoiTBNN.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<tbnn_SoTheoDoiTBNN> List()
        {
            try
            {
                return Context.tbnn_SoTheoDoiTBNN.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tbnn_SoTheoDoiTBNN> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tbnn_SoTheoDoiTBNN entity, ref string strError)
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

        public override List<tbnn_SoTheoDoiTBNN> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


    }
}
