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
    public class sc_KHGiaoTGXLSCTHARepository : RepositoryBase<sc_KHGiaoTGXLSCTHA>
    {
        public string Connectstr { get; set; }
        public sc_KHGiaoTGXLSCTHARepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                Connectstr = connection.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public sc_KHGiaoTGXLSCTHARepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_KHGiaoTGXLSCTHA.SingleOrDefault(o => o.Id == id);
                Context.sc_KHGiaoTGXLSCTHA.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_KHGiaoTGXLSCTHA entity, ref string strError)
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

        public override object Update(sc_KHGiaoTGXLSCTHA entity, ref string strError)
        {
            try
            {
                using (ECP_V2Entities db = new ECP_V2Entities())
                {
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return entity.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public override sc_KHGiaoTGXLSCTHA GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_KHGiaoTGXLSCTHA.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_KHGiaoTGXLSCTHA> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<sc_KHGiaoTGXLSCTHA> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
        public override List<sc_KHGiaoTGXLSCTHA> List()
        {
            try
            {
                return Context.sc_KHGiaoTGXLSCTHA.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public sc_KHGiaoTGXLSCTHA GetListChiTieu(int Nam, string DonViId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from sc_KHGiaoTGXLSCTHA where Nam=" + Nam + " and DonViId='" + DonViId + "'";
                    return (sc_KHGiaoTGXLSCTHA)db.Query<sc_KHGiaoTGXLSCTHA>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public sc_KHGiaoTGXLSCTHA ChiTieuNam_Search(string strcon, int Nam, int Thang, string MaDV)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(strcon))
                {
                    string query = "select * " +
                        " from sc_KHGiaoTGXLSCTHA ct " +
                        "where ct.Nam=" + Nam + " " +
                        "and ct.Thang=" + Thang + " " +
                        "and ct.DonViId='" + MaDV + "' "
                        ;
                    var q = db.Query<sc_KHGiaoTGXLSCTHA>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return null; }
        }


    }
}
