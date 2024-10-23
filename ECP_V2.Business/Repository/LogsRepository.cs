using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class LogsRepository
    {

        #region AdvancedSearchLogs
        public List<Log> AdvancedSearchLogs(int page, int pagelength, string filter, string type,string DateFrom, string DateTo, string DonViID)
        {
            try
            {
                int donviId = 0;
                try
                {
                    donviId = int.Parse(DonViID);
                }
                catch { }

                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(filter))
                    filter = "";
                if (string.IsNullOrEmpty(type))
                    type = "";


                // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (WorkUnit unit = new WorkUnit())
                {
                    if (donviId == 0)
                    {
                        return unit.Context.Database.SqlQuery<Log>("EXEC sp_AdvancedSearchLogs @filter,@Type,@DonViID,@NgayBD,@NgayKT,@Skip,@Take",
                           new SqlParameter("@filter", filter),
                           new SqlParameter("@Type", type),
                           new SqlParameter("@DonViID", ""),
                           new SqlParameter("@NgayBD", start1),
                           new SqlParameter("@NgayKT", end1),
                           new SqlParameter("@Skip", page.ToString()),
                           new SqlParameter("@Take", pagelength.ToString())).ToList();
                    }
                    else
                    {
                        return unit.Context.Database.SqlQuery<Log>("EXEC sp_AdvancedSearchLogs @filter,@Type,@DonViID,@NgayBD,@NgayKT,@Skip,@Take",
                           new SqlParameter("@filter", filter),
                           new SqlParameter("@Type", type),
                           new SqlParameter("@DonViID", DonViID.ToString()),
                           new SqlParameter("@NgayBD", start1),
                           new SqlParameter("@NgayKT", end1),
                           new SqlParameter("@Skip", page.ToString()),
                           new SqlParameter("@Take", pagelength.ToString())).ToList();
                    }
                }
            }
            catch { return null; }
        }
        #endregion
        #region CountTotalLog
        public int CountTotalLog(string filter, string type, string DateFrom, string DateTo, string DonViID)
        {
            try
            {

                int donviId = 0;
                try
                {
                    donviId = int.Parse(DonViID);
                }
                catch { }

                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(filter))
                    filter = "";
                if (string.IsNullOrEmpty(type))
                    type = "";


                // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (WorkUnit unit = new WorkUnit())
                {
                    if (donviId == 0)
                    {
                        var data= unit.Context.Database.SqlQuery<int>("EXEC sp_CountAdvancedSearchLogs @filter,@Type,@DonViID,@NgayBD,@NgayKT",
                           new SqlParameter("@filter", filter),
                           new SqlParameter("@Type", type),
                           new SqlParameter("@DonViID", ""),
                           new SqlParameter("@NgayBD", start1),
                           new SqlParameter("@NgayKT", end1)).ToList();

                        if (data.Count > 0)
                            return int.Parse(data[0].ToString());
                        else return 0;
                    }
                    else
                    {
                        var data = unit.Context.Database.SqlQuery<int>("EXEC sp_CountAdvancedSearchLogs @filter,@Type,@DonViID,@NgayBD,@NgayKT",
                           new SqlParameter("@filter", filter),
                           new SqlParameter("@Type", type),
                           new SqlParameter("@DonViID", DonViID.ToString()),
                           new SqlParameter("@NgayBD", start1),
                           new SqlParameter("@NgayKT", end1)).ToList();

                        if (data.Count > 0)
                            return int.Parse(data[0].ToString());
                        else return 0;
                    }
                }

            }
            catch { return 0; }
        }
        #endregion

    }
}
