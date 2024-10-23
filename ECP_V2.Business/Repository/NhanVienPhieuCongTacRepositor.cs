using Dapper;
using ECP_V2.Business.UnitOfWork;
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
    public class NhanVienPhieuCongTacRepositor : RepositoryBase<plv_PhieuCT_NhanVien>
    {
        public NhanVienPhieuCongTacRepositor()
            : base()
        {
        }

        public NhanVienPhieuCongTacRepositor(WorkUnit unit)
            : base(unit)
        {
        }

        public List<plv_PhieuCT_NhanVien> GetListByMaPhieu(int MaPhieu)
        {
            try
            {
                return Context.plv_PhieuCT_NhanVien.Where(x => x.MaPCT == MaPhieu).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public List<plv_PhieuCT_NhanVien> GetListByMaPhieuV2(int MaPhieu, string strcon)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(strcon))
                {
                    string query = "select * from plv_PhieuCT_NhanVien where MaPCT=" + MaPhieu + "";
                    var q = db.Query<plv_PhieuCT_NhanVien>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return null; }
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.plv_PhieuCT_NhanVien.SingleOrDefault(o => o.Id == id);
                Context.plv_PhieuCT_NhanVien.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }
        public string DeleteNhanVienPhieuCT(object PhieuId, object nhanVienId, ref string strError)
        {
            try
            {
                var id = int.Parse(PhieuId.ToString());
                string maNhanVien = nhanVienId.ToString();
                var entity = Context.plv_PhieuCT_NhanVien.FirstOrDefault(o => o.MaPCT == id && o.MaNV == maNhanVien);
                Context.plv_PhieuCT_NhanVien.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public string DeleteNhanVienPhieuCTIdOrName(object PhieuId, object nhanVienId, string tenNhanVien, ref string strError)
        {
            try
            {
                var id = int.Parse(PhieuId.ToString());
                string maNhanVien = nhanVienId.ToString();
                var entity = Context.plv_PhieuCT_NhanVien.FirstOrDefault(o => o.MaPCT == id && (o.MaNV == maNhanVien || tenNhanVien == o.TenNV));
                Context.plv_PhieuCT_NhanVien.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }


        public override object Create(plv_PhieuCT_NhanVien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.MaNV;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override plv_PhieuCT_NhanVien GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_PhieuCT_NhanVien.SingleOrDefault(p => p.MaPCT == id);
            }
            catch { return null; }
        }

        public bool CheckExistSoPhieu(object entityId, string SoPhieu)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.plv_PhieuCongTac.Any(p => p.ID != id && p.SoPhieu == SoPhieu);
            }
            catch { return false; }
        }

        public override List<plv_PhieuCT_NhanVien> List()
        {
            try
            {
                return Context.plv_PhieuCT_NhanVien.Take(100).ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override object Update(plv_PhieuCT_NhanVien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.MaNV;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }
        public override List<plv_PhieuCT_NhanVien> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override List<plv_PhieuCT_NhanVien> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
