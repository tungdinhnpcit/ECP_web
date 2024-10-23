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
    public class DonViRepository : RepositoryBase<tblDonVi>
    {
        static string DBName { get; set; }
        public DonViRepository()
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

        public DonViRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override object Create(tblDonVi entity, ref string strError)
        {
            try
            {
                
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_DonViGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_DonViGetAll", Context.tblDonVis.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
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
                var id = entityId.ToString();
                var entity = Context.tblDonVis.SingleOrDefault(o => o.Id == id);
                Context.tblDonVis.Remove(entity);
                Context.SaveChanges();
                strError = "";

                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_DonViGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_DonViGetAll", Context.tblDonVis.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return "success";
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return "error";
            }
        }

        public override tblDonVi GetById(object entityId)
        {
            try
            {
                var id = entityId.ToString();
                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblDonVi>)datacache;
                    return lst.SingleOrDefault(o => o.Id == id);
                }
                else
                {
                    var lst = Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst.SingleOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<tblDonVi> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<tblDonVi> List()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    return (List<tblDonVi>)datacache;
                }
                else
                {
                    var lst = Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }

            }
            catch { return null; }
        }
        public List<tblDonVi> ListByParentId(string parentid)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    var lst = (List<tblDonVi>)datacache;
                    if (parentid == null)
                    {
                        return lst.Where(c => c.DviCha == null || c.DviCha == null).OrderBy(p => p.ViTri).ToList();
                    }
                    else
                    {
                        return lst.Where(x => x.DviCha == parentid).OrderBy(p => p.ViTri).ToList();
                    }
                }
                else
                {
                    var lst = Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    if (parentid == null)
                    {
                        //html = createDD(db.Context.tblDonVis.Where(c => c.DviCha == null || c.DviCha == 0).OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                        return lst.Where(c => c.DviCha == null || c.DviCha == null).OrderBy(p => p.ViTri).ToList();
                    }
                    else
                    {
                        //html = createDD(db.Context.tblDonVis.Where(c => c.Id == donviId).OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                        return lst.Where(x => x.DviCha == parentid).OrderBy(p => p.ViTri).ToList();
                    }
                }
            }
            catch { return null; }
        }

        public override List<tblDonVi> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblDonVi entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_DonViGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_DonViGetAll", Context.tblDonVis.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public static string GetAllDonViHtml(string DonViID)
        {
            string html = "";

            int donviId = 0;
            try
            {
                donviId = int.Parse(DonViID);
            }
            catch { }

            using (WorkUnit db = new WorkUnit())
            {
                if (donviId == 0)
                {
                    var data = db.Context.Database.SqlQuery<tblDonVi>("EXEC sp_DonVi_getall @DonViID",
                        new SqlParameter("@DonViID", "")).ToList();

                    foreach (var item in data)
                    {
                        html += "<option value=" + item.Id + ">" + item.TenDonVi.Trim() + "</option>";
                    }
                }
                else
                {
                    var data = db.Context.Database.SqlQuery<tblDonVi>("EXEC sp_DonVi_getall @DonViID",
                        new SqlParameter("@DonViID", donviId.ToString())).ToList();

                    foreach (var item in data)
                    {
                        html += "<option value=" + item.Id + ">" + item.TenDonVi.Trim() + "</option>";
                    }
                }
            }

            return html;
        }

        public static string GetAllDonViOption(string DonViID)
        {
            string html = "";
            string donviId = null;
            try
            {
                if (DonViID != null)
                    donviId = DonViID;
            }
            catch { }
            var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
            if (datacache != null)
            {
                var lst = (List<tblDonVi>)datacache;
                if (DonViID == null)
                {
                    html = createDD(lst.Where(c => (c.Id.Length == 4) || c.Id.ToUpper() == "PH" || c.Id.ToUpper() == "PN" || c.Id.ToUpper() == "PM").OrderBy(p => p.ViTri).ToList(), 1, "", DonViID);
                }
                else
                {
                    html = createDD(lst.Where(c => c.Id == donviId).OrderBy(p => p.ViTri).ToList(), 1, "", DonViID);
                }
            }
            else
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var lst = db.Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                    if (DonViID == null)
                    {
                        html = createDD(lst.Where(c => (c.Id.Length == 4) || c.Id.ToUpper() == "PH" || c.Id.ToUpper() == "PN" || c.Id.ToUpper() == "PM").OrderBy(p => p.ViTri).ToList(), 1, "", DonViID);
                    }
                    else
                    {
                        html = createDD(lst.Where(c => c.Id == donviId).OrderBy(p => p.ViTri).ToList(), 1, "", DonViID);
                    }
                }
            }
            return html;
        }

        public static string GetAllDonViHtml2(string DonViID)
        {
            string html = "";
            string donviId = null;
            try
            {
                if (DonViID != null)
                    donviId = DonViID;
            }
            catch { }
            var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
            if (datacache != null)
            {
                var lst = (List<tblDonVi>)datacache;
                if (DonViID == null)
                {
                    html = createDD(lst.Where(c => (c.Id.Length == 4) || c.Id.ToUpper() == "PH" || c.Id.ToUpper() == "PN" || c.Id.ToUpper() == "PM").OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                }
                else
                {
                    html = createDD(lst.Where(c => c.Id == donviId).OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                }
            }
            else
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var lst = db.Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                    if (DonViID == null)
                    {
                        html = createDD(lst.Where(c => (c.Id.Length == 4) || c.Id.ToUpper() == "PH" || c.Id.ToUpper() == "PN" || c.Id.ToUpper() == "PM").OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                    }
                    else
                    {
                        html = createDD(lst.Where(c => c.Id == donviId).OrderBy(p => p.ViTri).ToList(), 1, "", "0");
                    }
                }
            }
            return html;
        }
        static string createDD(List<tblDonVi> ds, int lv, string cap, string curr)
        {
            using (WorkUnit db = new WorkUnit())
            {
                string kq = "";
                if (ds.Count == 0)
                    return kq;
                else
                {
                    string space = "";
                    if (lv != 1)
                    {
                        for (int i = 0; i < lv; i++)
                        {
                            space += "&nbsp;&nbsp;";
                            for (int j = 1; j < i; j++)
                                space += "&nbsp;";
                        }
                    }
                    else
                    {
                        if (cap == "0")
                            kq += "<option value='0'>Tất cả</option>";
                    }

                    int cs = 1;
                    foreach (tblDonVi cate in ds)
                    {
                        var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                        if (datacache != null)
                        {
                            var lst = (List<tblDonVi>)datacache;
                            kq += "<option " + (cate.Id.ToString() == curr.ToString() ? "selected='selected'" : "") + " value=\'" + cate.Id + "\'>" + space + (cap == "" ? "" + cs : cap + "." + cs) + " " + cate.TenDonVi + "</option>";
                            List<tblDonVi> dsc = lst.Where(n => n.DviCha == cate.Id).OrderBy(p => p.ViTri).ToList();
                            kq += createDD(dsc, lv + 1, cap == "" ? "" + cs++ : cap + "." + cs++, curr);
                        }
                        else
                        {
                            var lst = db.Context.tblDonVis.ToList();
                            MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                            kq += "<option " + (cate.Id.ToString() == curr.ToString() ? "selected='selected'" : "") + " value=\'" + cate.Id + "\'>" + space + (cap == "" ? "" + cs : cap + "." + cs) + " " + cate.TenDonVi + "</option>";
                            List<tblDonVi> dsc = lst.Where(n => n.DviCha == cate.Id).OrderBy(p => p.ViTri).ToList();
                            kq += createDD(dsc, lv + 1, cap == "" ? "" + cs++ : cap + "." + cs++, curr);
                        }
                    }
                    return kq;
                }
            }
        }

        public object ImportList(List<tblDonVi> lstFal, ref string strError)
        {
            try
            {
                foreach (var item in lstFal)
                {
                    Context.Entry(item).State = EntityState.Added;
                }
                Context.SaveChanges();

                var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                if (datacache != null)
                {
                    MemoryCacheHelper.Delete(DBName + "_DonViGetAll");
                }
                MemoryCacheHelper.Add(DBName + "_DonViGetAll", Context.tblDonVis.ToList(), DateTimeOffset.UtcNow.AddMonths(1));

                strError = "";
                return "ok";
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public static List<string> GetAllDonViID(string DonViID)
        {
            List<string> kq = new List<string>();
            var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
            if (datacache != null)
            {
                var lst = (List<tblDonVi>)datacache;
                if (DonViID == null)
                {
                    GetDVID(lst.Where(c => c.DviCha == null || c.DviCha == null).OrderBy(p => p.ViTri).ToList(), kq);
                }
                else
                {
                    GetDVID(lst.Where(c => c.Id == DonViID).ToList(), kq);
                }
            }
            else
            {
                using (WorkUnit db = new WorkUnit())
                {
                    var lst = db.Context.tblDonVis.ToList();
                    MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                    if (DonViID == null)
                    {
                        GetDVID(lst.Where(c => c.DviCha == null || c.DviCha == null).OrderBy(p => p.ViTri).ToList(), kq);
                    }
                    else
                    {
                        GetDVID(lst.Where(c => c.Id == DonViID).ToList(), kq);
                    }
                }
            }
            return kq;
        }
        static void GetDVID(List<tblDonVi> ds, List<string> kq)
        {
            using (WorkUnit db = new WorkUnit())
            {
                if (ds.Count == 0)
                    return;
                else
                {
                    foreach (tblDonVi cate in ds)
                    {
                        var datacache = MemoryCacheHelper.GetValue(DBName + "_DonViGetAll");
                        if (datacache != null)
                        {
                            var lst = (List<tblDonVi>)datacache;
                            kq.Add(cate.Id);
                            List<tblDonVi> dsc = lst.Where(n => n.DviCha == cate.Id).ToList();
                            GetDVID(dsc, kq);
                        }
                        else
                        {
                            var lst = db.Context.tblDonVis.ToList();
                            MemoryCacheHelper.Add(DBName + "_DonViGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));

                            kq.Add(cate.Id);
                            List<tblDonVi> dsc = lst.Where(n => n.DviCha == cate.Id).ToList();
                            GetDVID(dsc, kq);
                        }
                    }
                }
            }
        }

        public string GetDvConnect()
        {
            try
            {
                var lst = Context.tblDonVis.ToList();
                var dvobj = lst.Where(d=>d.DviCha == "PA").FirstOrDefault();
                return dvobj.Id;
            }catch(Exception ex)
            {
                return null;
            }
        }

        public List<plv_LoaiPhieu> ListLoaiPhieu()
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue(DBName + "_LoaiPhieuGetAll");
                if (datacache != null)
                {
                    return (List<plv_LoaiPhieu>)datacache;
                }
                else
                {
                    var lst = Context.plv_LoaiPhieu.ToList();
                    MemoryCacheHelper.Add(DBName + "_LoaiPhieuGetAll", lst, DateTimeOffset.UtcNow.AddMonths(1));
                    return lst;
                }
            }
            catch { return null; }
        }
    }
}
