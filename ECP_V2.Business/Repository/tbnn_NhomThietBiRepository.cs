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
    public class tbnn_NhomThietBiRepository : RepositoryBase<tbnn_NhomThietBi>
    {
        static string DBName { get; set; }
        public string Connectstr { get; set; }
        public tbnn_NhomThietBiRepository(string connect)
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

        public tbnn_NhomThietBiRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tbnn_NhomThietBi.SingleOrDefault(o => o.ID == id);
                Context.tbnn_NhomThietBi.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tbnn_NhomThietBi entity, ref string strError)
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

        public override tbnn_NhomThietBi GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.tbnn_NhomThietBi.SingleOrDefault(o => o.ID == id);
            }
            catch { return null; }
        }

        public override List<tbnn_NhomThietBi> List()
        {
            try
            {
                return Context.tbnn_NhomThietBi.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tbnn_NhomThietBi> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tbnn_NhomThietBi entity, ref string strError)
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

        public override List<tbnn_NhomThietBi> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<tbnn_NhomThietBi> GetAll()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_NhomThietBi";
                    var q = db.Query<tbnn_NhomThietBi>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<tbnn_NhomThietBi>(); }
        }

        public tbnn_NhomThietBi GetObjByID(string ID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_NhomThietBi where ID=" + ID;
                    var q = db.Query<tbnn_NhomThietBi>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new tbnn_NhomThietBi(); }
        }

        public List<tbnn_NhomThietBi> ListPaging(int page, int pageSize, string filter)
        {
            List<tbnn_NhomThietBi> lstData = new List<tbnn_NhomThietBi>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tb.[ID]) AS RowNum " +
                        ",tb.ID,tb.Name " +
                        ",tb.NgayTao,tb.NguoiTao,tb.NgaySua,tb.NguoiSua " +
                        "from tbnn_NhomThietBi tb " +
                        "where " +
                        "(tb.Name like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "
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
                        var q = multipleresult.Read<tbnn_NhomThietBi>();
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
                        "from tbnn_NhomThietBi tb " +
                        "where " +
                        "(tb.Name like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "
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
                var entity = Context.tbnn_NhomThietBi.Where(o => entityId.ToList().Contains(o.ID.ToString()));
                Context.tbnn_NhomThietBi.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public List<tbnn_ThongKeoLoaiThietBiModel> GetListThongKe(string MaNhom)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from tbnn_NhomThietBi where MaNhom=" + MaNhom;
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
                    string query = "select ID from tbnn_NhomThietBi where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

    }



}
