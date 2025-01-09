using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.LCTGA;
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
    public class LCTGARepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public LCTGARepository(string strConn)
        : base()
        {
            this.Connectstr = strConn;
        }

        public LCTGARepository(WorkUnit unit)
            : base(unit)
        {
        }



        public List<DSLCTGhiAm> GetDSLenhGhiAm(int TrangThai, string DViCPhieu, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "exec dbo.ws_get_ds_lenh_ghiam_donvi @DViCPhieu, @TrangThai, @FromDate,@ToDate;";
                    var data = db.Query<DSLCTGhiAm>(query, new { DViCPhieu, TrangThai, FromDate, ToDate }).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetChiTietLCT(Int64 LCTGhiAmId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "exec dbo.ws_get_chi_tiet_lct @LCTGhiAmId;";
                    var data = db.Query<dynamic>(query, new { LCTGhiAmId });
                    var dataRT = data.First();
                    if (dataRT.JSON != null)
                    {
                       
                        return dataRT.JSON.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
