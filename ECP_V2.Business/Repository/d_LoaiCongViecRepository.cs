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
    public class d_LoaiCongViecRepository : RepositoryBase<d_LoaiCongViec>
    {
        public override object Create(d_LoaiCongViec entity, ref string strError)
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

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.d_LoaiCongViec.SingleOrDefault(o => o.Id == id);
                Context.d_LoaiCongViec.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override d_LoaiCongViec GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.d_LoaiCongViec.SingleOrDefault(p => p.Id == id);
            }
            catch { return null; }
        }



        public override List<d_LoaiCongViec> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<d_LoaiCongViec> GetByType(object entityId)
        {
            try
            {
                int id = Convert.ToInt32(entityId);
                return Context.d_LoaiCongViec.Where(p => p.TrangThai.Value == id).ToList();
            }
            catch (Exception ex) { return null; }
        }


        public override List<d_LoaiCongViec> List()
        {
            try
            {
                return Context.d_LoaiCongViec.OrderBy(x => x.MaLoai).ThenBy(x => x.STT).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public List<d_LoaiCongViecModel> GetList()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select * " +
                        ",TenLoai=(select Ten from d_NhomLoaiCongViec where Id=MaLoai) " +
                        ",STTLoai=(select STT from d_NhomLoaiCongViec where Id=MaLoai) " +
                        "from d_LoaiCongViec";

                    var q = db.Query<d_LoaiCongViecModel>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<d_LoaiCongViec> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(d_LoaiCongViec entity, ref string strError)
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
    }
    public class d_LoaiCongViecModel
    {
        public int Id { get; set; }
        public string TenLoaiCongViec { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaLoai { get; set; }
        public Nullable<int> STT { get; set; }
        public string TenLoai { get; set; }
        public Nullable<int> STTLoai { get; set; }
    }
}
