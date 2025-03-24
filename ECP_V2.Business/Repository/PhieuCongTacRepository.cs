using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class PhieuCongTacRepository : RepositoryBase<plv_PhieuCongTac>
    {

        public PhieuCongTacRepository()
            : base()
        {
        }

        public PhieuCongTacRepository(WorkUnit unit)
            : base(unit)
        {
        }

        #region GetPhieuCongTacByID
        public static plv_PhieuCongTac GetPhieuCongTacByID(int ID)
        {
            try
            {
                using (WorkUnit unit = new WorkUnit())
                {
                    return unit.Context.Database.SqlQuery<plv_PhieuCongTac>("EXEC sp_plv_PhieuCongTac_getByID @ID",
                   new SqlParameter("@ID", ID.ToString())).FirstOrDefault();
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion

        #region GetTaiLieuByMaPhieu
        public static IEnumerable<plv_TaiLieu> GetTaiLieuByMaPhieu(int MaPCT)
        {
            try
            {
                using (WorkUnit unit = new WorkUnit())
                {
                    return unit.Context.Database.SqlQuery<plv_TaiLieu>("EXEC sp_plv_TaiLieu_getallbyPhieuID @MaPCT",
                   new SqlParameter("@MaPCT", MaPCT.ToString())).ToList();

                }
            }
            catch { return null; }
        }
        #endregion

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_PhieuCongTac.SingleOrDefault(o => o.ID == id);
                Context.plv_PhieuCongTac.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

      

        public override object Create(plv_PhieuCongTac entity, ref string strError)
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

        public override plv_PhieuCongTac GetById(object entityId)
        {
            try
            {
                int? id = entityId != null ? int.Parse(entityId.ToString()) : (int?)0;
                return Context.plv_PhieuCongTac.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public plv_PhieuCongTac_His GetById_His(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_PhieuCongTac_His.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public bool CheckExistSoPhieu(object entityId, string SoPhieu)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_PhieuCongTac.Any(p => p.ID == id && p.SoPhieu == SoPhieu);
            }
            catch (Exception ex) { return false; }
        }

        public override List<plv_PhieuCongTac> List()
        {
            try
            {
                return Context.plv_PhieuCongTac.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_PhieuCongTac> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public List<plv_PhieuCongTac_Sign> LstPhieucongtacSignByPlvId(int idphien)
        {
            return Context.plv_PhieuCongTac_Sign.Where(e => e.PhienLamViecID == idphien).ToList();
        }
        public int XoaPhieucongtacSignByPlvId(int idphien)
        {
            var phieuCongTacs = Context.plv_PhieuCongTac_Sign.Where(e => e.PhienLamViecID == idphien).ToList();
            if (phieuCongTacs.Any())
            {
                Context.plv_PhieuCongTac_Sign.RemoveRange(phieuCongTacs);
                Context.SaveChanges();
                return phieuCongTacs.Count;
            }

            return 0;
        }
        public bool Revert_KySoLoi(string constr, int PhienLamViecId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    // Câu lệnh xóa các bản ghi liên quan đến PhienLamViecID
                    string query = @"
                delete from plv_PhieuCongTac_Sign where PhienLamViecID= @PhienLamViecID;
                delete from plv_PhieuCongTac_hientruong where PhienLamViecID= @PhienLamViecID;
                delete from plv_PhieuCT_NhanVien_hientruong where PhienLamViecID= @PhienLamViecID; ";

                    var rowsAffected = db.Execute(query, new { PhienLamViecId });

                    if (rowsAffected > 0)
                    {
                        return true; 
                    }

                    return false; 
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override object Update(plv_PhieuCongTac entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<plv_PhieuCongTac> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public plv_PhieuCongTac GetObjByDate(string constr, string TuNgay, string DenNgay, string MaDV, string MaLP)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    string query = "select * from plv_PhieuCongTac pct " +
                        "where NgayDuyet between @TuNgay and @DenNgay " +
                        "and @MaDV in (select plv.DonViId from tblPhienLamViec plv where plv.MaPCT=pct.ID) " +
                        "and pct.SoPhieu!='' " +
                        "and pct.MaLP=@MaLP " +
                        "order by NgayDuyet desc "
                        ;
                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             TuNgay = TuNgay,
                             DenNgay = DenNgay,
                             MaDV = MaDV,
                             MaLP = MaLP,
                         }))
                    {
                        var q = multipleresult.Read<plv_PhieuCongTac>();
                        return q.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }

        public List<plv_PhieuCongTac> GetObjPhieu(string constr, string MaDV, string MaLP, string TramId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    //Namcv chỉnh sửa để lấy số theo định dạng mới 4/2022
                    string query = "select * from plv_PhieuCongTac pct " +
                        "where @MaDV in (select plv.DonViId from tblPhienLamViec plv where plv.MaPCT=pct.ID) " +
                        "and pct.SoPhieu!='' " +
                        "and pct.MaLP=@MaLP " +
                        "and pct.NGAYTAO > '2022/04/01' "
                        ;

                    if (!string.IsNullOrEmpty(TramId))
                    {
                        query = query + " and pct.TramId=@TramId ";
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             MaDV = MaDV,
                             MaLP = MaLP,
                             TramId = TramId
                         }))
                    {
                        var q = multipleresult.Read<plv_PhieuCongTac>();
                        return q.ToList();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }

        public int GetPhieuCTac(string iddv, int nam, int malp)
        {
            try
            {
                int number = 1;
                var d = Context.plv_SoPhieuHienTai.Where(e => e.DonViId == iddv && e.Nam == nam && e.MaLP == malp).FirstOrDefault();
                if (d != null)
                {
                    try
                    {
                        int n = 1;
                        int.TryParse(d.SoPhieu, out n);
                        number = n + 1;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    d.SoPhieu = number.ToString();
                }
                else
                {
                    plv_SoPhieuHienTai p = new plv_SoPhieuHienTai()
                    {
                        Nam = nam,
                        DonViId = iddv,
                        IsHand = false,
                        MaLP = malp,
                        SoPhieu = number.ToString(),
                    };
                    Context.plv_SoPhieuHienTai.Add(p);
                }
                // cạp nhật số mới

                Context.SaveChanges();

                return number;

            }
            catch (Exception ex) { throw ex; }
        }

        //Namcv edit GetPhieuCtacUseStore
        // 05/03/2022

        public string GetSoPhieuCtac(string iddv, int nam, int? malp, string userid, int nhayso)
        {
            string sophieu = "";
            try
            {

                using (SqlConnection conn = new SqlConnection(Context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    SqlCommand sql_cmnd = new SqlCommand("pkg_get_sophieu", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@iddv", SqlDbType.NVarChar).Value = iddv;
                    sql_cmnd.Parameters.AddWithValue("@nam", SqlDbType.Int).Value = nam;
                    sql_cmnd.Parameters.AddWithValue("@malp", SqlDbType.Int).Value = malp;
                    sql_cmnd.Parameters.AddWithValue("@userid", SqlDbType.NVarChar).Value = userid;
                    sql_cmnd.Parameters.AddWithValue("@nhayso", SqlDbType.Int).Value = nhayso;
                    object o = sql_cmnd.ExecuteScalar();
                    sophieu = (o == null) ? String.Empty : o.ToString();

                    return sophieu;
                    conn.Close();
                }


                return sophieu;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }

    public class SoPhieuModel
    {
        public int ID { get; set; }
        public string SoPhieu { get; set; }
        public DateTime Ngay { get; set; }
        public int SoLuong { get; set; }
    }

}
