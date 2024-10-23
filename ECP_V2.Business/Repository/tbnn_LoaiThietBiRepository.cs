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
    public class tbnn_LoaiThietBiRepository : RepositoryBase<tbnn_LoaiThietBi>
    {
        static string DBName { get; set; }
        public string Connectstr { get; set; }
        public tbnn_LoaiThietBiRepository(string connect)
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

        public tbnn_LoaiThietBiRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tbnn_LoaiThietBi.SingleOrDefault(o => o.ID == id);
                Context.tbnn_LoaiThietBi.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tbnn_LoaiThietBi entity, ref string strError)
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

        public override tbnn_LoaiThietBi GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tbnn_LoaiThietBi.SingleOrDefault(o => o.ID == id);
            }
            catch { return null; }
        }

        public override List<tbnn_LoaiThietBi> List()
        {
            try
            {
                return Context.tbnn_LoaiThietBi.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tbnn_LoaiThietBi> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tbnn_LoaiThietBi entity, ref string strError)
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

        public override List<tbnn_LoaiThietBi> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<tbnn_LoaiThietBi> GetAll()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_LoaiThietBi order by TenLoai";
                    var q = db.Query<tbnn_LoaiThietBi>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<tbnn_LoaiThietBi>(); }
        }

        public tbnn_LoaiThietBiModel GetObjByID(string ID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_LoaiThietBi where ID=" + ID;
                    var q = db.Query<tbnn_LoaiThietBiModel>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new tbnn_LoaiThietBiModel(); }
        }

        public List<tbnn_LoaiThietBiModel> ListPaging(int page, int pageSize, string filter)
        {
            List<tbnn_LoaiThietBiModel> lstData = new List<tbnn_LoaiThietBiModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tb.[ID]) AS RowNum " +
                        ",tb.ID,tb.TenLoai " +
                        ",tb.NgayTao,tb.NguoiTao,tb.NgaySua,tb.NguoiSua " +
                        "from tbnn_LoaiThietBi tb " +
                        "where " +
                        "(tb.TenLoai like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "
                        ;
                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize"
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize
                        }))
                    {
                        var q = multipleresult.Read<tbnn_LoaiThietBiModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string filter)
        {

            int count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(ID) " +
                        "from tbnn_LoaiThietBi tb " +
                        "where " +
                        "(tb.TenLoai like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "
                        ;
                    using (var multipleresult = db.QueryMultiple(query))
                    {
                        try
                        {
                            var q = multipleresult.Read<int>();
                            count = q.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            count = 0;
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return count;
        }

        public string DeleteAll(string[] entityId, ref string strError)
        {
            try
            {
                var entity = Context.tbnn_LoaiThietBi.Where(o => entityId.ToList().Contains(o.ID.ToString()));
                Context.tbnn_LoaiThietBi.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public List<tbnn_ThongKeoLoaiThietBiModel> GetListThongKe()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_LoaiThietBi";
                    var q = db.Query<tbnn_ThongKeoLoaiThietBiModel>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<tbnn_ThongKeoLoaiThietBiModel>(); }
        }

        public int GetIdByName(string Name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from tbnn_LoaiThietBi where UPPER(TenLoai) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

    }

    public class tbnn_LoaiThietBiModel
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
    }

    public class tbnn_ThongKeoLoaiThietBiModel
    {
        public int ID { get; set; }
        public string TenDonVi { get; set; }
        public string TenLoai { get; set; }
        public int SoLuong { get; set; }
    }
}
