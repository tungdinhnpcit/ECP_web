using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class BaoCaoRepository : RepositoryBase<tblBaoCao>
    {
        public BaoCaoRepository()
          : base()
        {
        }

        public BaoCaoRepository(WorkUnit unit)
            : base(unit)
        {
        }
        public override object Create(tblBaoCao entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override string Delete(object entityId,ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblBaoCaos.SingleOrDefault(o => o.Id == id);
                Context.tblBaoCaos.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch(Exception ex) {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblBaoCao GetById(object entityId)
        {
            try
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var id = int.Parse(entityId.ToString());
                    var baocao = db.Context.tblBaoCaos.Where(p => p.Id == id).First();
                    return baocao;
                }
            }
            catch 
            {
                return null;
            }
        }

        public override List<tblBaoCao> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblBaoCao> List()
        {
            try
            {
                using (WorkUnit db = new WorkUnit())
                {
                    return db.Context.tblBaoCaos.ToList();
                }                
            }
            catch { return null; }
        }

        public override List<tblBaoCao> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblBaoCao entity, ref string strError)
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
                return 0;
            }
        }

        #region Tong hop bao cao dau gio
        public IEnumerable<TongHopBaoCaoDauGio_Result> Get_KeHoachTuan(DateTime? dts,DateTime? dte)
        {
            try
            {
                var kq = Context.TongHopBaoCaoDauGio(dts, dte).ToList();
                return kq;

            }
            catch (Exception ex)
            { return null; }
        }

        #endregion

        #region Tong hop bao cao dau gio V2
        public IEnumerable<TongHopBaoCaoDauGioV2_Result> Get_KeHoachTuanV2(DateTime? dts, DateTime? dte)
        {
            try
            {
                var kq = Context.TongHopBaoCaoDauGioV2(dts, dte).ToList();
                return kq;

            }
            catch (Exception ex)
            { return null; }
        }

        #endregion

        #region Tong hop bao cao dau gio V2
        public IEnumerable<TongHopBaoCaoCuoiNgay_Result> Get_KeHoachTuanCuoiNgay(DateTime? dts, DateTime? dte)
        {
            try
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var kq = db.Context.TongHopBaoCaoCuoiNgay(dts, dte).ToList();
                    return kq;
                }                    
            }
            catch (Exception ex)
            { return null; }
        }

        #endregion     
    }
}
