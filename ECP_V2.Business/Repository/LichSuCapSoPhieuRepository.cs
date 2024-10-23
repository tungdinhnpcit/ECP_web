using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class LichSuCapSoPhieuRepository : RepositoryBase<plv_LichSuCapSoPhieu>
    {    


   
        public override object Create(plv_LichSuCapSoPhieu entity, ref string strError)
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

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_LichSuCapSoPhieu.SingleOrDefault(o => o.ID == id);
                Context.plv_LichSuCapSoPhieu.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override plv_LichSuCapSoPhieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_LichSuCapSoPhieu.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }


        public override List<plv_LichSuCapSoPhieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


        public override List<plv_LichSuCapSoPhieu> List()
        {
            try
            {
                return Context.plv_LichSuCapSoPhieu.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_LichSuCapSoPhieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(plv_LichSuCapSoPhieu entity, ref string strError)
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

        public List<plv_LichSuCapSoPhieu> GetListSearch(string constr, string MaDV, string MaLP, string TramId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    string query = "select * from plv_LichSuCapSoPhieu p " +
                        "where p.DonViId=@MaDV " +
                        "and p.MaLP=@MaLP "+
                        "and isnull(p.IsHand,0)=1 and (p.MaPCT is null or p.MaPCT=null) "
                        ;

                    if (!string.IsNullOrEmpty(TramId))
                    {
                        query = query + " and p.TramId=@TramId ";
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             MaDV = MaDV,
                             MaLP = MaLP,
                             TramId = TramId,
                         }))
                    {
                        var q = multipleresult.Read<plv_LichSuCapSoPhieu>();
                        return q.ToList();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }


    }
}
