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
    public class tbnn_TaiLieu_SoTheoDoiTBNNRepository : RepositoryBase<tbnn_SoTheoDoiTBNN_TaiLieu>
    {
        public tbnn_TaiLieu_SoTheoDoiTBNNRepository()
            : base()
        {
        }

        public tbnn_TaiLieu_SoTheoDoiTBNNRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tbnn_SoTheoDoiTBNN_TaiLieu.SingleOrDefault(o => o.ID == id);
                Context.tbnn_SoTheoDoiTBNN_TaiLieu.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tbnn_SoTheoDoiTBNN_TaiLieu entity, ref string strError)
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

        public override tbnn_SoTheoDoiTBNN_TaiLieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tbnn_SoTheoDoiTBNN_TaiLieu.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<tbnn_SoTheoDoiTBNN_TaiLieu> List()
        {
            try
            {
                return Context.tbnn_SoTheoDoiTBNN_TaiLieu.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tbnn_SoTheoDoiTBNN_TaiLieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tbnn_SoTheoDoiTBNN_TaiLieu entity, ref string strError)
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

        public override List<tbnn_SoTheoDoiTBNN_TaiLieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


    }
}
