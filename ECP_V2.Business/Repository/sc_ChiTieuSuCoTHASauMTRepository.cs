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
    public class sc_ChiTieuSuCoTHASauMTRepository : RepositoryBase<sc_ChiTieuSuCoTHASauMT>
    {
        public string Connectstr { get; set; }
        public sc_ChiTieuSuCoTHASauMTRepository()
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

        public sc_ChiTieuSuCoTHASauMTRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_ChiTieuSuCoTHASauMT.SingleOrDefault(o => o.Id == id);
                Context.sc_ChiTieuSuCoTHASauMT.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_ChiTieuSuCoTHASauMT entity, ref string strError)
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

        public override object Update(sc_ChiTieuSuCoTHASauMT entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public override sc_ChiTieuSuCoTHASauMT GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_ChiTieuSuCoTHASauMT.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_ChiTieuSuCoTHASauMT> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<sc_ChiTieuSuCoTHASauMT> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
        public override List<sc_ChiTieuSuCoTHASauMT> List()
        {
            try
            {
                return Context.sc_ChiTieuSuCoTHASauMT.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public IEnumerable<sc_ChiTieuSuCoTHASauMT> GetListChiTieu(int Nam, string DonViId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from sc_ChiTieuSuCoTHASauMT where Nam=" + Nam + " and DonViId='" + DonViId + "'";
                    return (IEnumerable<sc_ChiTieuSuCoTHASauMT>)db.Query<sc_ChiTieuSuCoTHASauMT>(query);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public sc_ChiTieuSuCoTHASauMT ChiTieuNam_Search(string strcon, int Nam, int Thang, string MaDV)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(strcon))
                {
                    string query = "select * " +
                        " from sc_ChiTieuSuCoTHASauMT ct " +
                        "where ct.Nam=" + Nam + " " +
                        "and ct.Thang=" + Thang + " " +
                        "and ct.DonViId='" + MaDV + "' "
                        ;
                    var q = db.Query<sc_ChiTieuSuCoTHASauMT>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return null; }
        }


    }
}
