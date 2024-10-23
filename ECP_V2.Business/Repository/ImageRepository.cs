using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public enum ImgSort
    {
        THOIGIAN = 0,
        PHIENLV = 1,
        NHANVIEN = 2,
        DACOPHIEN = 3,
        CHUACOPHIEN = 4
    }

    public class ImagesRepository : RepositoryBase<tblImage>
    {
        public ImagesRepository()
            : base()
        {
        }

        public ImagesRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(Object entityId,ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblImages.SingleOrDefault(o => o.Id == id);
                Context.tblImages.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch(Exception ex) { strError = ex.Message; return "error"; }
        }

        public override tblImage GetById(Object entityId)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var id = int.Parse(entityId.ToString());
                    var obj = db.tblImages.Where(p => p.Id == id).FirstOrDefault();
                    return obj;
                }
              
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override List<tblImage> List()
        {
            try
            {
                return Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.Id).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<tblImage> List(out int sum, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            try
            {
                if (sort == (int)ImgSort.THOIGIAN)
                {
                    sum = Context.tblImages.Where(x => x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.PHIENLV)
                {
                    sum = Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.PhienLamViecId).Count();
                    return Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.PhienLamViecId).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.NHANVIEN)
                {
                    sum = Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.UserUp).Count();
                    return Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.UserUp).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.DACOPHIEN)
                {
                    sum = Context.tblImages.Where(x => x.IsDelete == false).Where(x => x.PhienLamViecId > 0).Count();
                    return Context.tblImages.Where(x => x.IsDelete == false).Where(x => x.PhienLamViecId > 0).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.CHUACOPHIEN)
                {
                    sum = Context.tblImages.Where(x => (x.PhienLamViecId == null || x.PhienLamViecId == 0) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => (x.PhienLamViecId == null || x.PhienLamViecId == 0) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else
                {
                    sum = Context.tblImages.Where(x => x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
            }
            catch { sum = 0; return null; }
        }
        public List<tblImage> ListByPhienLVId(int phienid)
        {
            try
            {
                return Context.tblImages.Where(x => x.PhienLamViecId == phienid && x.IsDelete == false).OrderByDescending(x => x.Id).ToList();
            }
            catch { return null; }
        }
        public List<tblImage> ListByPhongBanId(out int sum, int phongbanid, string ngayUpAnh, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            try
            {
                DateTime date = DateTime.Now;

                try
                {
                    date = DateTime.ParseExact(ngayUpAnh, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    date = DateTime.Now;
                }

                var nhanvien = Context.tblNhanViens.Where(x => x.PhongBanId == phongbanid).Select(x => x.Id).ToList();
                if (sort == (int)ImgSort.THOIGIAN)
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.PHIENLV)
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.PhienLamViecId).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.NHANVIEN)
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.UserUp).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.DACOPHIEN)
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.PhienLamViecId > 0 && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.PhienLamViecId > 0 && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.CHUACOPHIEN)
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (x.PhienLamViecId == null || x.PhienLamViecId == 0) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (x.PhienLamViecId == null || x.PhienLamViecId == 0) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else
                {
                    sum = Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).Count();
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (DbFunctions.TruncateTime(x.NgayCapNhat) == DbFunctions.TruncateTime(date) || string.IsNullOrEmpty(ngayUpAnh)) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
            }
            catch(Exception ex) { sum = 0; return null; }
        }
        public List<tblImage> ListByDonViId(string donviid, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            try
            {
                var nhanvien = (from dv in Context.tblDonVis
                                join pb in Context.tblPhongBans on dv.Id equals pb.MaDVi
                                join nv in Context.tblNhanViens on pb.Id equals nv.PhongBanId
                                where dv.Id == donviid
                                select nv.Id).ToList();
                if (sort == (int)ImgSort.THOIGIAN)
                {
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.PHIENLV)
                {
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.IsDelete == false).OrderByDescending(x => x.PhienLamViecId).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.NHANVIEN)
                {
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.IsDelete == false).OrderByDescending(x => x.UserUp).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.DACOPHIEN)
                {
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.PhienLamViecId > 0 && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.CHUACOPHIEN)
                {
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && (x.PhienLamViecId == null || x.PhienLamViecId == 0) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else
                    return Context.tblImages.Where(x => nhanvien.Contains(x.UserUp) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
            }
            catch { return null; }

        }
        public List<tblImage> ListByNhanVienId(out int sum, string nhanvienid, int sort = (int)ImgSort.THOIGIAN, int page = 0, int quantity = 8)
        {
            try
            {
                sum = Context.tblImages.Where(x => x.UserUp == nhanvienid).Count();
                if (sort == (int)ImgSort.THOIGIAN)
                {
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.PHIENLV)
                {
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && x.IsDelete == false).OrderByDescending(x => x.PhienLamViecId).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.NHANVIEN)
                {
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && x.IsDelete == false).OrderByDescending(x => x.UserUp).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.DACOPHIEN)
                {
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && x.PhienLamViecId > 0 && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else if (sort == (int)ImgSort.CHUACOPHIEN)
                {
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && (x.PhienLamViecId == null || x.PhienLamViecId == 0) && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
                }
                else
                    return Context.tblImages.Where(x => x.UserUp == nhanvienid && x.IsDelete == false).OrderByDescending(x => x.Id).Skip(page * quantity).Take(quantity).ToList();
            }
            catch { sum = 0; return null; }
        }
        public override List<tblImage> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Create(tblImage entity, ref string strError)
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

        public override object Update(tblImage entity, ref string strError)
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

        public override List<tblImage> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
