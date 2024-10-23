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
    public class SoPhieuHienTaiRepository : RepositoryBase<plv_SoPhieuHienTai>
    {



        public override object Create(plv_SoPhieuHienTai entity, ref string strError)
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

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_SoPhieuHienTai.SingleOrDefault(o => o.ID == id);
                Context.plv_SoPhieuHienTai.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override plv_SoPhieuHienTai GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_SoPhieuHienTai.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }


        public override List<plv_SoPhieuHienTai> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


        public override List<plv_SoPhieuHienTai> List()
        {
            try
            {
                return Context.plv_SoPhieuHienTai.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_SoPhieuHienTai> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(plv_SoPhieuHienTai entity, ref string strError)
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

        //Namcv bổ sung việc cập nhật nhảy phiếu
        
        public plv_SoPhieuHienTai GetObjByDonVi(string constr, string MaDV, string MaLP, string TramId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    string query = "";
                        

                    if (!string.IsNullOrEmpty(TramId))
                    {
                        query = " select *, b.TenVietTat DonViTen from tblTram a join tblPhongban b on a.phongbanid = b.id " +
                            "where a.active = 1" +
                            " and a.malp = @MaLP" +
                            " and a.PhongBanID = @TramId ";
                    }
                    else
                    {
                        query = "select p.*, dbo.fnFirsties(d.TenDonVi) DonViTen from plv_SoPhieuHienTai p join tblDonvi d on p.donviid = d.id" +
                            " where p.DonViId=@MaDV " +
                            "and p.MaLP=@MaLP " +
                            "and Active = 1"
                            ;
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             MaDV = MaDV,
                             MaLP = MaLP,
                             TramId = TramId
                         }))
                    {
                        var q = multipleresult.Read<plv_SoPhieuHienTai>();
                        return q.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }

        public void updateSoPhieuHienTai(String constr, DateTime ngayTao, String nguoiTao, String soPhieu
            , bool isHand, String maPct, int Nam, int pbId, int malp, String madvi)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("pkg_sophieu_update_hientai", conn);

                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 3. add parameter to command, which will be passed to the stored procedure
                    cmd.Parameters.Add(new SqlParameter("@ngayTao", ngayTao));
                    cmd.Parameters.Add(new SqlParameter("@nguoiTao", nguoiTao));
                    cmd.Parameters.Add(new SqlParameter("@soPhieu", soPhieu));
                    cmd.Parameters.Add(new SqlParameter("@isHand", isHand));
                    cmd.Parameters.Add(new SqlParameter("@maPct", maPct));
                    cmd.Parameters.Add(new SqlParameter("@Nam", Nam));
                    cmd.Parameters.Add(new SqlParameter("@pbId", pbId));
                    cmd.Parameters.Add(new SqlParameter("@malp", malp));
                    cmd.Parameters.Add(new SqlParameter("@dvi", madvi));

                    // execute the command                    
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex) { }
        }

        public plv_SoPhieuHienTai_His GetObjByDonVi_His(string constr, string MaDV, string MaLP, string TramId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    string query = "select * from plv_SoPhieuHienTai_His p " +
                        "where p.DonViId=@MaDV " +
                        "and p.MaLP=@MaLP "
                        ;

                    if (!string.IsNullOrEmpty(TramId))
                    {
                        query = query + " and p.TramId = @TramId ";
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             MaDV = MaDV,
                             MaLP = MaLP,
                             TramId = TramId
                         }))
                    {
                        var q = multipleresult.Read<plv_SoPhieuHienTai_His>();
                        return q.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }

        public plv_SoPhieuHienTai GetObjByDate(string constr, string Thang, string Nam, string DonViId, string MaLP, string TramId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(constr))
                {
                    string query = "select * from plv_SoPhieuHienTai pct " +
                        "where pct.DonViId=@DonViId " +
                        //"and pct.Thang=@Thang " +
                        "and pct.Nam=@Nam " +
                        "and pct.MaLP=@MaLP " +
                        "and pct.Active = 1";

                    if (!string.IsNullOrEmpty(TramId))
                    {
                        query = query + " and pct.TramId = @TramId ";
                    }

                    query = query + " order by pct.ID desc ";
                    ;

                    using (var multipleresult = db.QueryMultiple(query,
                         new
                         {
                             Thang = Thang,
                             Nam = Nam,
                             DonViId = DonViId,
                             MaLP = MaLP,
                             TramId = TramId
                         }))
                    {
                        var q = multipleresult.Read<plv_SoPhieuHienTai>();
                        return q.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex) { return null; }
        }


    }
}
