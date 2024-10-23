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
    public class PhongBanRepository : RepositoryBase<tblPhongBan>
    {
        static string DBName { get; set; }
        public PhongBanRepository()
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

        public PhongBanRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblPhongBans.SingleOrDefault(o => o.Id == id);
                Context.tblPhongBans.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_PhongBanGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", Context.tblPhongBans.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override tblPhongBan GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.tblPhongBans.ToList();
                    MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public tblPhongBan GetByName(string name)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    return lst.Where(p => p.TenPhongBan.ToLower().Equals(name.Trim().ToLower())).Single();
                }
                else
                {
                    var lst = Context.tblPhongBans.ToList();
                    MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(p => p.TenPhongBan.ToLower().Equals(name.Trim().ToLower())).Single();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public tblDonVi GetDviById(int phongBanId)
        {
            try
            {
                tblPhongBan phongBan = this.GetById(phongBanId);
                var donvi = Context.tblDonVis.Where(p => p.Id == phongBan.MaDVi).FirstOrDefault();
                return donvi;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<tblPhongBan> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    return lst;
                }
                else
                {
                    var lst = Context.tblPhongBans.ToList();
                    MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch (Exception ex) { return null; }
        }

        public override List<tblPhongBan> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Create(tblPhongBan entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_PhongBanGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", Context.tblPhongBans.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override object Update(tblPhongBan entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_PhongBanGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", Context.tblPhongBans.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override List<tblPhongBan> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public static List<tblPhongBan> GetPhongBanByDonViIDHtmlCustom(string DonViID)
        {
            List<tblPhongBan> data = new List<tblPhongBan>();
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    if (string.IsNullOrEmpty(DonViID))
                    {
                        data = lst.Where(x => x.LoaiPB != 2).ToList();
                    }
                    else
                    {
                        data = lst.Where(x => x.MaDVi == DonViID && x.LoaiPB != 2).ToList();
                    }
                }
                else
                {
                    using (WorkUnit db = new WorkUnit())
                    {
                        var lst = db.Context.tblPhongBans.ToList();
                        MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                        if (string.IsNullOrEmpty(DonViID))
                        {
                            data = lst.Where(x => x.LoaiPB != 2).ToList();
                        }
                        else
                        {
                            data = lst.Where(x => x.MaDVi == DonViID && x.LoaiPB != 2).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return data;
        }

        public static List<tblPhongBan> GetPhongBanByDonViIDHtmlBenNgoai(string DonViID)
        {
            List<tblPhongBan> data = new List<tblPhongBan>();
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    if (string.IsNullOrEmpty(DonViID))
                    {
                        data = lst.Where(x => x.LoaiPB == 2).ToList();
                    }
                    else
                    {
                        data = lst.Where(x => x.MaDVi == DonViID && x.LoaiPB == 2).ToList();
                    }
                }
                else
                {
                    using (WorkUnit db = new WorkUnit())
                    {
                        var lst = db.Context.tblPhongBans.ToList();
                        MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                        if (string.IsNullOrEmpty(DonViID))
                        {
                            data = lst.Where(x => x.LoaiPB == 2).ToList();
                        }
                        else
                        {
                            data = lst.Where(x => x.MaDVi == DonViID && x.LoaiPB == 2).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return data;
        }

        public static string GetPhongBanByTenDonViByID(int phongBanID)
        {
            try
            {
                if (phongBanID > 0)
                {
                    var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                    if (datacache != null)
                    {
                        var lst = (List<tblPhongBan>)datacache;
                        var phongBanObj = lst.SingleOrDefault(x => x.Id == phongBanID);
                        if (phongBanObj != null)
                        {
                            return phongBanObj.TenPhongBan;
                        }
                        else
                        {
                            return "Không tồn tại " + phongBanID;
                        }
                    }
                    else
                    {
                        using (WorkUnit db = new WorkUnit())
                        {
                            var lst = db.Context.tblPhongBans.ToList();
                            MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                            var phongBanObj = lst.SingleOrDefault(x => x.Id == phongBanID);
                            if (phongBanObj != null)
                            {
                                return phongBanObj.TenPhongBan;
                            }
                            else
                            {
                                return "Không tồn tại " + phongBanID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return "Không tồn tại";
        }

        public static string GetPhongBanByDonViIDHtml(string DonViID, int PhongBanID)
        {
            string html = "";
            List<tblPhongBan> data = new List<tblPhongBan>();

            var datacache = MemoryCacheHelper.GetValue(DBName + "_GetPhongBanByDonViIDHtml_" + DonViID + "_" + PhongBanID);
            if (datacache != null)
            {
                data = (List<tblPhongBan>)datacache;
            }
            else
            {
                using (WorkUnit db = new WorkUnit())
                {
                    if (string.IsNullOrEmpty(DonViID))
                    {
                        data = db.Context.Database.SqlQuery<tblPhongBan>("EXEC sp_PhongBan_getallbyDonViID @DonViID",
                       new SqlParameter("@DonViID", "")).ToList();
                    }
                    else
                    {
                        data = db.Context.Database.SqlQuery<tblPhongBan>("EXEC sp_PhongBan_getallbyDonViID @DonViID",
                       new SqlParameter("@DonViID", DonViID)).ToList();
                    }

                    MemoryCacheHelper.Add(DBName + "_GetPhongBanByDonViIDHtml_" + DonViID + "_" + PhongBanID, data, DateTimeOffset.UtcNow.AddMonths(1));
                }
            }

            foreach (var item in data)
            {
                html += "<option value=" + item.Id + " " + (item.Id == PhongBanID ? "selected" : "") + " >" + item.TenPhongBan.Trim() + "</option>";
            }
            return html;
        }

        public static string GetPhongBanByDonViIDHtmlTaoCongViec(int PhongBanID)
        {
            string html = "";
            var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
            if (datacache != null)
            {
                var lst = (List<tblPhongBan>)datacache;
                var data = lst.Where(x => x.Id == PhongBanID).FirstOrDefault();
                html += "<option value=" + data.Id + " selected>" + data.TenPhongBan.Trim() + "</option>";
            }
            else
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var lst = db.Context.tblPhongBans.ToList();
                    var data = lst.Where(x => x.Id == PhongBanID).FirstOrDefault();
                    html += "<option value=" + data.Id + " selected>" + data.TenPhongBan.Trim() + "</option>";

                    MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                }
            }
            return html;
        }

        public List<tblPhongBan> GetPhongBanByDonViID(string DonViID)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblPhongBan>)datacache;
                    return lst.Where(x => x.MaDVi == DonViID).ToList();
                }
                else
                {
                    var lst = Context.tblPhongBans.ToList();
                    MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.Where(x => x.MaDVi == DonViID).ToList();
                }
            }
            catch { return null; }
        }

        #region GetPhienLVById
        public static int GetIdPhongBanByName(string Key, string donviId)
        {
            try
            {
                Key = Key.ToUpper();
                using (WorkUnit db = new WorkUnit())
                {
                    if (donviId == null)
                    {
                        var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
                       new SqlParameter("@Key", Key),
                       new SqlParameter("@DonViID", "")).ToList();
                        if (data.Count > 0)
                            return data[0];
                        else
                            return 0;
                    }
                    else
                    {
                        var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
                       new SqlParameter("@Key", Key),
                       new SqlParameter("@DonViID", donviId)).ToList();
                        if (data.Count > 0)
                            return data[0];
                        else
                            return 0;
                    }
                }
            }
            catch { return 0; }
        }

        public static int GetIdPhongBanByNameDonViIdNotNull(string Key, string donviId)
        {
            try
            {
                Key = Key.ToUpper();
                using (WorkUnit db = new WorkUnit())
                {
                    if (!string.IsNullOrEmpty(donviId))
                    {
                        var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
                       new SqlParameter("@Key", Key),
                       new SqlParameter("@DonViID", donviId)).ToList();
                        if (data.Count > 0)
                            return data[0];
                        else
                            return 0;
                    }
                }
            }
            catch { return 0; }
            return 0;
        }

        public static string GetPBanByName(string Key, string donviId)
        {
            try
            {
                using (WorkUnit db = new WorkUnit())
                {

                    if (donviId == null)
                    {
                        var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
                       new SqlParameter("@Key", Key),
                       new SqlParameter("@DonViID", "")).ToList();
                        if (data.Count > 0)
                            return data[0].ToString();
                        else
                            return null;
                    }
                    else
                    {
                        //tblPhongBan phongBan = this.GetById(phongBanId);
                        var pBan = db.Context.tblPhongBans.Where(p => p.MaDVi == donviId && p.TenPhongBan.ToUpper().Contains(Key.ToUpper())).FirstOrDefault();
                        if (pBan != null)
                        {
                            return pBan.MaDVi;
                        }
                        return null;
                    }
                }
            }
            catch { return null; }
        }

        public object ImportList(List<tblPhongBan> entity, ref string strError, out List<tblPhongBan> lstFail)
        {
            lstFail = new List<tblPhongBan>();
            try
            {
                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;
                entity = entity.Where(x => !Context.tblPhongBans.Any(y => y.Id == x.Id)).ToList();
                int addedCount = 0;
                foreach (var item in entity)
                {
                    Context.Entry(item).State = EntityState.Added;
                    try
                    {

                        Context.SaveChanges();
                        addedCount++;
                    }
                    catch (Exception ex)
                    {
                        lstFail.Add(item);
                        continue;
                    }
                }

                var datacache = MemoryCacheHelper.GetValue(DBName + "_PhongBanGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_PhongBanGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_PhongBanGetAll", Context.tblPhongBans.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                strError = "";
                return "Số bản ghi thêm được: " + addedCount;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }
        #endregion
    }
}
