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
    public class TrangThaiCCDCRepository : RepositoryBase<CCDC_TrangThai>
    {
        public string Connectstr { get; set; }
        static string DBName { get; set; }
        public TrangThaiCCDCRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public TrangThaiCCDCRepository()
            : base()
        {
        }

        public TrangThaiCCDCRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.CCDC_TrangThai.SingleOrDefault(o => o.ID == id);
                Context.CCDC_TrangThai.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(CCDC_TrangThai entity, ref string strError)
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

        public override CCDC_TrangThai GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.CCDC_TrangThai.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<CCDC_TrangThai> List()
        {
            try
            {
                return Context.CCDC_TrangThai.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<CCDC_TrangThai> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(CCDC_TrangThai entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<CCDC_TrangThai> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public int GetIdByName(string Name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from CCDC_TrangThai where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

    }
}
