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
    public class sc_KienNghiMienTru_TaiLieuRepository : RepositoryBase<sc_KienNghiMienTru_TaiLieu>
    {
        public string Connectstr { get; set; }
        public sc_KienNghiMienTru_TaiLieuRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                this.Connectstr = connection.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public sc_KienNghiMienTru_TaiLieuRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_KienNghiMienTru_TaiLieu.SingleOrDefault(o => o.Id == id);
                Context.sc_KienNghiMienTru_TaiLieu.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(sc_KienNghiMienTru_TaiLieu entity, ref string strError)
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

        public override sc_KienNghiMienTru_TaiLieu GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_KienNghiMienTru_TaiLieu.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }

        public override List<sc_KienNghiMienTru_TaiLieu> List()
        {
            try
            {
                return Context.sc_KienNghiMienTru_TaiLieu.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_KienNghiMienTru_TaiLieu> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(sc_KienNghiMienTru_TaiLieu entity, ref string strError)
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

        public IEnumerable<sc_KienNghiMienTru_TaiLieu> GetListTaiLieuByKienNghiId(string KienNghiId)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from sc_KienNghiMienTru_TaiLieu where KienNghiId=" + KienNghiId;
                return (IEnumerable<sc_KienNghiMienTru_TaiLieu>)db.Query<sc_KienNghiMienTru_TaiLieu>(query);
            }
        }

        public IEnumerable<sc_KienNghiMienTru_TaiLieu> GetListTaiLieuByKienNghiId_His(string KienNghiId)
        {
            using (IDbConnection db = new SqlConnection(Connectstr))
            {
                string query = "select * from sc_KienNghiMienTru_TaiLieu_His where KienNghiId=" + KienNghiId;
                return (IEnumerable<sc_KienNghiMienTru_TaiLieu>)db.Query<sc_KienNghiMienTru_TaiLieu>(query);
            }
        }

        public override List<sc_KienNghiMienTru_TaiLieu> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

       
    }
}
