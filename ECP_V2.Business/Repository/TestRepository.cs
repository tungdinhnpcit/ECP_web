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
    public class TestRepository : RepositoryBase<Test>
    {
        public string Connectstr { get; set; }
        public TestRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public TestRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.Tests.SingleOrDefault(o => o.ID == id);
                Context.Tests.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(Test entity, ref string strError)
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

        public override Test GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.Tests.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<Test> List()
        {
            try
            {
                return Context.Tests.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<Test> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(Test entity, ref string strError)
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

        public override List<Test> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<Test> ListPaging(int page, int pageSize, string filter)
        {

            List<Test> lstData = new List<Test>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tb.[ID]) AS RowNum " +
                        ",* " +
                        "from Test tb " +
                        "where " +
                        "(tb.Name like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize " +
                        "order by ID "
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            filter = filter,
                        }))
                    {
                        var q = multipleresult.Read<Test>();
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
                        "from Test tb " +
                        "where " +
                        "(tb.Name like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = filter
                        }))
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
                var entity = Context.Tests.Where(o => entityId.ToList().Contains(o.ID.ToString()));
                Context.Tests.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        

    }

}
