using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class CommentRepository
    {

        #region GetPhienLVById
        public static IEnumerable<tblComment> GetCommentByPienLVId(int PienLVId)
        {
            try
            {
                using (WorkUnit unit = new WorkUnit())
                {
                    return unit.Context.Database.SqlQuery<tblComment>("EXEC sp_Comment_getallbyPhienLVID @PhienLVID",
                   new SqlParameter("@PhienLVID", PienLVId.ToString())).ToList();

                }
            }
            catch { return null; }
        }
        #endregion

        #region Comment_Add
        public string Comment_Add(tblComment cm)
        {
            string kt = "";
            try
            {
                using (WorkUnit unit = new WorkUnit())
                {
                    unit.Context.Database.ExecuteSqlCommand("EXEC sp_Comment_add @CommentContent,@CreateTime,@Username,@Priority,@Description,@PhienLamViecId",
                       new SqlParameter("@CommentContent", cm.@CommentContent),
                       new SqlParameter("@CreateTime", cm.CreateTime),
                       new SqlParameter("@Username", cm.Username),
                       new SqlParameter("@Priority", cm.Priority),
                       new SqlParameter("@Description", cm.Description),
                       new SqlParameter("@PhienLamViecId", cm.PhienLamViecId)
                       );

                }
            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }
        #endregion
    }

}
