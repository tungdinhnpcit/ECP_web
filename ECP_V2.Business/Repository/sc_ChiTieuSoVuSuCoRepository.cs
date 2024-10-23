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
    public class sc_ChiTieuSoVuSuCoRepository : RepositoryBase<sc_ChiTieuSoVuSuCo>
    {
        public string Connectstr { get; set; }
        public sc_ChiTieuSoVuSuCoRepository()
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

        public sc_ChiTieuSoVuSuCoRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_ChiTieuSoVuSuCo.SingleOrDefault(o => o.Id == id);
                Context.sc_ChiTieuSoVuSuCo.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_ChiTieuSoVuSuCo entity, ref string strError)
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

        public override object Update(sc_ChiTieuSoVuSuCo entity, ref string strError)
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


        public override sc_ChiTieuSoVuSuCo GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_ChiTieuSoVuSuCo.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_ChiTieuSoVuSuCo> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<sc_ChiTieuSoVuSuCo> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
        public override List<sc_ChiTieuSoVuSuCo> List()
        {
            try
            {
                return Context.sc_ChiTieuSoVuSuCo.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public IEnumerable<sc_ChiTieuSoVuSuCo> GetListChiTieu(int Nam, string DonViId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from sc_ChiTieuSoVuSuCo where Nam=" + Nam + " and DonViId='" + DonViId + "' and LoaiSuCoId is null ";
                    return (IEnumerable<sc_ChiTieuSoVuSuCo>)db.Query<sc_ChiTieuSoVuSuCo>(query);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<sc_ChiTieuSoVuSuCo> GetListChiTieuTheoLoai(int Nam, string DonViId, int Loai)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from sc_ChiTieuSoVuSuCo where Nam=" + Nam + " and DonViId='" + DonViId + "' and LoaiSuCoId = " + Loai;
                    return (IEnumerable<sc_ChiTieuSoVuSuCo>)db.Query<sc_ChiTieuSoVuSuCo>(query);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public sc_ChiTieuSoVuSuCo ChiTieuNam_Search(string strcon, int Nam, int Thang, string MaDV)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(strcon))
                {
                    string query = "select * " +
                        " from sc_ChiTieuSoVuSuCo ct " +
                        "where ct.Nam=" + Nam + " " +
                        "and ct.Thang=" + Thang + " " +
                        "and ct.DonViId='" + MaDV + "' "
                        ;
                    var q = db.Query<sc_ChiTieuSoVuSuCo>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return null; }
        }


    }
}
