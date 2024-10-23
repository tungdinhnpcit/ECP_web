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
    public class sc_TaiNanSuCo_DonViRepository : RepositoryBase<sc_TaiNanSuCo_DonVi>
    {
        public string Connectstr { get; set; }
        static string DBName { get; set; }
        public sc_TaiNanSuCo_DonViRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public sc_TaiNanSuCo_DonViRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public sc_TaiNanSuCo_DonViRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_TaiNanSuCo_DonVi.SingleOrDefault(o => o.Id == id);
                Context.sc_TaiNanSuCo_DonVi.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_TaiNanSuCo_DonVi entity, ref string strError)
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

        public override sc_TaiNanSuCo_DonVi GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_TaiNanSuCo_DonVi.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_TaiNanSuCo_DonVi> List()
        {
            try
            {
                return Context.sc_TaiNanSuCo_DonVi.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_TaiNanSuCo_DonVi> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(sc_TaiNanSuCo_DonVi entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<sc_TaiNanSuCo_DonVi> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        //public int GetIdByName(string Name)
        //{
        //    try
        //    {
        //        using (IDbConnection db = new SqlConnection(Connectstr))
        //        {
        //            string query = "select ID from sc_TaiNanSuCo_DonVi where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
        //            var q = db.Query<int>(query);
        //            return q.FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex) { return new int(); }
        //}

    }
}
