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
    public class KiemSoatDienThoaiRepository : RepositoryBase<KiemSoatDienThoai>
    {
        static string DBName { get; set; }
        public KiemSoatDienThoaiRepository()
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

        public KiemSoatDienThoaiRepository(WorkUnit unit)
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
                    var entity = db.KiemSoatDienThoais.SingleOrDefault(o => o.Id == id);
                    db.KiemSoatDienThoais.Remove(entity);
                    db.SaveChanges();
                    strError = "";

                    var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                    if (datacache != null)
                    {
                        MemoryCacheHelper.Delete(DBName + "_KiemSoatDienThoaiGetAll");
                    }
                    MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", Context.KiemSoatDienThoais.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                    return "success";
                }
             
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(KiemSoatDienThoai entity, ref string strError)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Entry(entity).State = EntityState.Added;
                    db.SaveChanges();

                    var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                    if (datacache != null)
                    {
                        MemoryCacheHelper.Delete(DBName + "_KiemSoatDienThoaiGetAll");
                    }
                    MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", Context.KiemSoatDienThoais.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                    return entity.Id;
                }
             
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override KiemSoatDienThoai GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                if (datacache != null)
                {
                    var lst = (List<KiemSoatDienThoai>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.KiemSoatDienThoais.ToList();
                    MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }            
            }
            catch { return null; }
        }

        public override List<KiemSoatDienThoai> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                if (datacache != null)
                {
                    var lst = (List<KiemSoatDienThoai>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.KiemSoatDienThoais.ToList();
                    MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<KiemSoatDienThoai> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }
        
        public List<KiemSoatDienThoai> ListByBaoCaoCuoiNgayId(int baoCaoCuoiNgayId)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                if (datacache != null)
                {
                    var lst = (List<KiemSoatDienThoai>)datacache;
                    return lst.Where(x => x.BaoCaoCuoiNgayId == baoCaoCuoiNgayId).ToList();
                }
                else
                {
                    var lst = Context.KiemSoatDienThoais.ToList();
                    MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(x => x.BaoCaoCuoiNgayId == baoCaoCuoiNgayId).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override object Update(KiemSoatDienThoai entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_KiemSoatDienThoaiGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_KiemSoatDienThoaiGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_KiemSoatDienThoaiGetAll", Context.KiemSoatDienThoais.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }
        }

        public override List<KiemSoatDienThoai> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
    }
}
