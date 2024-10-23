using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class ThangLamViecRepository : RepositoryBase<S_THANG_LVIEC>
    {
        public ThangLamViecRepository()
            : base()
        {
        }

        public ThangLamViecRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var id = int.Parse(entityId.ToString());
                    var entity = db.S_THANG_LVIEC.SingleOrDefault(o => o.Id == id);
                    db.S_THANG_LVIEC.Remove(entity);
                    db.SaveChanges();
                    strError = "";
                    return "success";
                }

            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(S_THANG_LVIEC entity, ref string strError)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Entry(entity).State = EntityState.Added;
                    db.SaveChanges();
                    return entity.Id;
                }

            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override S_THANG_LVIEC GetById(object entityId)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var id = int.Parse(entityId.ToString());
                    return db.S_THANG_LVIEC.FirstOrDefault(p => p.Id == id);
                }
            }
            catch { return null; }
        }

        public S_THANG_LVIEC GetByDonViIdAndPhanHeAndThangAndNam(string donViId, string phanHe, int thang, int nam)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    return db.S_THANG_LVIEC.FirstOrDefault(x => x.DonViId.Equals(donViId) && x.PhanHe.Equals(phanHe) && x.Thang == thang && x.Nam == nam);
                }
            }
            catch { return null; }
        }

        #region GetImageByPhienLVId
        public S_THANG_LVIEC _GetMaxByDonViIdPhanHe(string donViId, string phanHe)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string sql = "select top(1) * from S_THANG_LVIEC " +
                        "where DonViId=@donViId " +
                        "and PhanHe=@phanHe " +
                        "order by Id desc "
                        ;
                    return db.Database.SqlQuery<S_THANG_LVIEC>(sql,
                   new SqlParameter("@donViId", donViId),
                   new SqlParameter("@phanHe", phanHe)
                   ).FirstOrDefault();
                }


            }
            catch (Exception ex) { return null; }
        }
        #endregion

        public List<S_THANG_LVIEC> GetMaxByDonViId(string donViId)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    return db.S_THANG_LVIEC.Where(x => x.DonViId.Equals(donViId))
                                        .GroupBy(x => x.PhanHe)
                                        .SelectMany(g => g.Where(x => x.Nam == g.Max(a => a.Nam)
                                                    && x.Thang == g.Max(a => a.Thang)))
                                        .ToList();
                }
            }
            catch { return null; }
        }

        public override List<S_THANG_LVIEC> List()
        {
            try
            {
                return Context.S_THANG_LVIEC.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<S_THANG_LVIEC> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(S_THANG_LVIEC entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override List<S_THANG_LVIEC> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }


        #region Execute Procedure

        public bool ExecuteMoveImageHistory(string donViId, DateTime ngayBackUp)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("MoveHAImageToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MoveImageToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExecuteMovePhieuCongTacHistory(string donViId, DateTime ngayBackUp)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("MoveTaiLieuToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePCTImageToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MoveLichSuCapSoPhieuToHistory @DonViId, @NgayBackUp",
                      new SqlParameter("DonViId", donViId),
                      new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhieuCongTac_NhanVienToHistory @DonViId, @NgayBackUp",
                     new SqlParameter("DonViId", donViId),
                     new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MoveSoPhieuHienTaiToHistory @DonViId, @NgayBackUp",
                   new SqlParameter("DonViId", donViId),
                   new SqlParameter("NgayBackUp", ngayBackUp)
                    );


                    // phien
                    db.Database.ExecuteSqlCommand("MoveCommentsToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViecThuocTinhToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );
                    // new
                    db.Database.ExecuteSqlCommand("MoveNhanVien_PhienLamViecToHistory @DonViId, @NgayBackUp",
                       new SqlParameter("DonViId", donViId),
                       new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViec_ChinhSuaToHistory @DonViId, @NgayBackUp",
                     new SqlParameter("DonViId", donViId),
                     new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViec_NhatKyToHistory @DonViId, @NgayBackUp",
                    new SqlParameter("DonViId", donViId),
                    new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViec_TheoDoiToHistory @DonViId, @NgayBackUp",
                  new SqlParameter("DonViId", donViId),
                  new SqlParameter("NgayBackUp", ngayBackUp)
                    );
                    
                    db.Database.ExecuteSqlCommand("MovePhienLamViecAndPhieuCongTacToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExecuteMoveSuCoHistory(string donViId, DateTime ngayBackUp)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("Move_sc_SuCoPTDToHis @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("Move_sc_TaiLieuToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("Move_sc_TaiNanSuCo_DonViToHis @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("Move_sc_KienNghiMienTru_TaiLieuToHis @DonViId, @NgayBackUp",
                       new SqlParameter("DonViId", donViId),
                       new SqlParameter("NgayBackUp", ngayBackUp)
                   );

                    db.Database.ExecuteSqlCommand("Move_sc_KienNghiMienTruToHis @DonViId, @NgayBackUp",
                       new SqlParameter("DonViId", donViId),
                       new SqlParameter("NgayBackUp", ngayBackUp)
                   );

                    db.Database.ExecuteSqlCommand("Move_sc_TaiNanSuCoToHis @DonViId, @NgayBackUp",
                       new SqlParameter("DonViId", donViId),
                       new SqlParameter("NgayBackUp", ngayBackUp)
                   );
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExecuteMovePhienLamViecHistory(string donViId, DateTime ngayBackUp)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("MoveCommentsToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViecThuocTinhToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );

                    db.Database.ExecuteSqlCommand("MovePhienLamViecToHistory @DonViId, @NgayBackUp",
                        new SqlParameter("DonViId", donViId),
                        new SqlParameter("NgayBackUp", ngayBackUp)
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckExistPhienLamViecIdImage(string donViId, DateTime ngayLamViec)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var list = db.tblPhienLamViecs.Where(x => x.DonViId.Equals(donViId) && (DbFunctions.TruncateTime(x.NgayLamViec) < DbFunctions.TruncateTime(ngayLamViec)))
                                .Join(db.tblImages, x => x.Id, y => y.PhienLamViecId, (x, y) => y);

                    if (list != null && list.Count() > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public bool CheckExistPhienLamViecIdPhieuCongTac(string donViId, DateTime ngayLamViec)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var list = db.tblPhienLamViecs.Where(x => x.DonViId.Equals(donViId) && (DbFunctions.TruncateTime(x.NgayLamViec) < DbFunctions.TruncateTime(ngayLamViec)))
                                .Join(db.plv_PhieuCongTac, x => x.MaPCT, y => y.ID, (x, y) => y);

                    if (list != null && list.Count() > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        #endregion
    }
}
