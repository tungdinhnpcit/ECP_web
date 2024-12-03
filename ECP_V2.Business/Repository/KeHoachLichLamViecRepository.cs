using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using Dapper;
using System.Configuration;
using System.Runtime.Remoting.Contexts;

namespace ECP_V2.Business.Repository
{
    public class KeHoachLichLamViecRepository : RepositoryBase<plv_KeHoachLichLamViec>
    {
        public string connection;

        public KeHoachLichLamViecRepository()
            : base()
        {
            try
            {
                var con = new SqlConnection(Context.Database.Connection.ConnectionString);
                connection = con.ConnectionString;
            }
            catch (Exception ex)
            { }
        }


        public async Task<dynamic> AddNew(plv_KeHoachLichLamViec input)
        {
            try
            {
                using (var connectionDB = new SqlConnection(connection))
                {
                   await connectionDB.OpenAsync();

                    var sql = @"INSERT INTO dbo.plv_KeHoachLichLamViec (PhienLamViecId, HinhThucKiemTra, NguoiDaiDienKT_Id, NguoiDaiDienKT)
                                values(@PhienLamViecId, @HinhThucKiemTra, @NguoiDaiDienKT_Id, @NguoiDaiDienKT)
                                SELECT SCOPE_IDENTITY();";
                    return await connectionDB.QueryFirstOrDefaultAsync<int>(sql, new { input.PhienLamViecId, input.HinhThucKiemTra, input.NguoiDaiDienKT_Id, input.NguoiDaiDienKT });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung, ví dụ: lỗi kết nối, lỗi không xác định
                Console.WriteLine("Lỗi: " + ex.Message);
                return Enumerable.Empty<Task>();
            }
        }



        public override object Create(plv_KeHoachLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override object Update(plv_KeHoachLichLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override plv_KeHoachLichLamViec GetById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> List()
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<plv_KeHoachLichLamViec> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override string Delete(object entityId, ref string strError)
        {
            throw new NotImplementedException();
        }
    }



}
