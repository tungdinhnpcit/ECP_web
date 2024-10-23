using ECP_V2.Business.UnitOfWork;
using ECP_V2.Common.Helpers;
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
    public class ThuocTinhPhienRepository : RepositoryBase<plv_ThuocTinhPhien>
    {
        static string DBName { get; set; }
        public ThuocTinhPhienRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public ThuocTinhPhienRepository(WorkUnit unit)
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
                    var entity = db.plv_ThuocTinhPhien.SingleOrDefault(o => o.Id == id);
                    db.plv_ThuocTinhPhien.Remove(entity);
                    db.SaveChanges();
                    strError = "";

                    var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                    if (datacache != null)
                    {
                        MemoryCacheHelper.Delete(DBName + "_ThuocTinhPhienGetAll");
                    }
                    MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", Context.plv_ThuocTinhPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                    return "success";
                }
             
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(plv_ThuocTinhPhien entity, ref string strError)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Entry(entity).State = EntityState.Added;
                    db.SaveChanges();

                    var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                    if (datacache != null)
                    {
                        MemoryCacheHelper.Delete(DBName + "_ThuocTinhPhienGetAll");
                    }
                    MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", Context.plv_ThuocTinhPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                    return entity.Id;
                }
             
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override plv_ThuocTinhPhien GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_ThuocTinhPhien>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.plv_ThuocTinhPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }     
            }
            catch { return null; }
        }

        public List<plv_ThuocTinhPhien> GetByLoaiThuocTinh(int loaiThuocTinh)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_ThuocTinhPhien>)datacache;
                    return lst.Where(p => p.LoaiThuocTinh == loaiThuocTinh).ToList();
                }
                else
                {
                    var lst = Context.plv_ThuocTinhPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(p => p.LoaiThuocTinh == loaiThuocTinh).ToList();
                }
            }
            catch { return null; }
        }

        public override List<plv_ThuocTinhPhien> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                if (datacache != null)
                {
                    var lst = (List<plv_ThuocTinhPhien>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.plv_ThuocTinhPhien.ToList();
                    MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<plv_ThuocTinhPhien> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        //public override object Create(plv_PhieuCongTac entity, ref string strError)
        //{
        //    throw new NotImplementedException();
        //}

        public override object Update(plv_ThuocTinhPhien entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_ThuocTinhPhienGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_ThuocTinhPhienGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_ThuocTinhPhienGetAll", Context.plv_ThuocTinhPhien.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override List<plv_ThuocTinhPhien> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
