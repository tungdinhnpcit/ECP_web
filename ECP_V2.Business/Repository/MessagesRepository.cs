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
using Dapper;

namespace ECP_V2.Business.Repository
{
    public class MessagesRepository : RepositoryBase<Message>
    {
        public MessagesRepository()
            : base()
        {
        }

        public MessagesRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.Messages.SingleOrDefault(o => o.Id == id);
                Context.Messages.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override Message GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var message = Context.Messages.Where(p => p.Id == id).FirstOrDefault();
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public tblDonVi GetDviById(int phongBanId)
        //{
        //    try
        //    {
        //        tblPhongBan phongBan = this.GetById(phongBanId);
        //        var donvi = Context.tblDonVis.Where(p => p.Id == phongBan.MaDVi).FirstOrDefault();
        //        return donvi;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public override List<Message> List()
        {
            try
            {
                return Context.Messages.ToList();
            }
            catch { return null; }
        }

        public List<Message> ListMessageByPosition(int position)
        {
            var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
            try
            {
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select * " +
                        "from Messages " +
                        "where LoaiThongBao= " + position + " "
                        ;
                    return db.Query<Message>(query).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<Message> ListMessageByPositionV2(int position, string MaTaiKhoan, string MA_DVIQLY)
        {
            var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
            try
            {
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select top(20) * " +
                        "from Messages " +
                        "where LoaiThongBao= " + position + " " +
                        "and MaTaiKhoan='" + MaTaiKhoan + "' " +
                        "and MA_DVIQLY='" + MA_DVIQLY + "'" +
                        "order by NgayTao desc "
                        ;
                    return db.Query<Message>(query).ToList();
                }
                //return Context.Messages.Where(x => x.LoaiThongBao == position).ToList();
            }
            catch
            {
                return null;
            }
        }

        public override List<Message> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Create(Message entity, ref string strError)
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

        public override object Update(Message entity, ref string strError)
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

        public override List<Message> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        //public static string GetPhongBanByDonViIDHtml(string DonViID, int PhongBanID)
        //{
        //    string html = "";
        //    using (WorkUnit db = new WorkUnit())
        //    {
        //        List<tblPhongBan> data = new List<tblPhongBan>();
        //        if (string.IsNullOrEmpty(DonViID))
        //        {
        //            data = db.Context.Database.SqlQuery<tblPhongBan>("EXEC sp_PhongBan_getallbyDonViID @DonViID",
        //           new SqlParameter("@DonViID", "")).ToList();
        //        }
        //        else
        //        {
        //            data = db.Context.Database.SqlQuery<tblPhongBan>("EXEC sp_PhongBan_getallbyDonViID @DonViID",
        //           new SqlParameter("@DonViID", DonViID)).ToList();
        //        }

        //        foreach (var item in data)
        //        {
        //            html += "<option value=" + item.Id + " " + (item.Id == PhongBanID ? "selected" : "") + " >" + item.TenPhongBan.Trim() + "</option>";
        //        }
        //    }
        //    return html;
        //}
        //public List<tblPhongBan> GetPhongBanByDonViID(string DonViID)
        //{
        //    try
        //    {
        //        return Context.tblPhongBans.Where(x => x.MaDVi == DonViID).ToList();
        //    }
        //    catch { return null; }
        //}
        //#region GetPhienLVById
        //public static int GetIdPhongBanByName(string Key, string donviId)
        //{
        //    try
        //    {
        //        using (WorkUnit db = new WorkUnit())
        //        {

        //            if (donviId == null)
        //            {
        //                var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
        //               new SqlParameter("@Key", Key),
        //               new SqlParameter("@DonViID", "")).ToList();
        //                if (data.Count > 0)
        //                    return data[0];
        //                else
        //                    return 0;
        //            }
        //            else
        //            {
        //                var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
        //               new SqlParameter("@Key", Key),
        //               new SqlParameter("@DonViID", donviId)).ToList();
        //                if (data.Count > 0)
        //                    return data[0];
        //                else
        //                    return 0;
        //            }
        //        }
        //    }
        //    catch { return 0; }
        //}

        //public static string GetPBanByName(string Key, string donviId)
        //{
        //    try
        //    {
        //        using (WorkUnit db = new WorkUnit())
        //        {

        //            if (donviId == null)
        //            {
        //                var data = db.Context.Database.SqlQuery<int>("EXEC sp_PhongBan_getIDbyName @Key,@DonViID",
        //               new SqlParameter("@Key", Key),
        //               new SqlParameter("@DonViID", "")).ToList();
        //                if (data.Count > 0)
        //                    return data[0].ToString();
        //                else
        //                    return null;
        //            }
        //            else
        //            {
        //                //tblPhongBan phongBan = this.GetById(phongBanId);
        //                var pBan = db.Context.tblPhongBans.Where(p => p.MaDVi == donviId && p.TenPhongBan.ToUpper().Contains(Key.ToUpper())).FirstOrDefault();
        //                if(pBan != null)
        //                {
        //                    return pBan.MaDVi;
        //                }
        //                return null;
        //            }
        //        }
        //    }
        //    catch { return null; }
        //}



        //public object ImportList(List<tblPhongBan> entity, ref string strError,out List<tblPhongBan> lstFail)
        //{
        //    lstFail = new List<tblPhongBan>();
        //    try
        //    {               
        //        Context.Configuration.AutoDetectChangesEnabled = false;
        //        Context.Configuration.ValidateOnSaveEnabled = false;
        //        entity = entity.Where(x => !Context.tblPhongBans.Any(y => y.Id == x.Id)).ToList();
        //        int addedCount = 0;
        //        foreach (var item in entity)
        //        {
        //            Context.Entry(item).State = EntityState.Added;
        //            try
        //            {

        //                Context.SaveChanges();
        //                addedCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                lstFail.Add(item);
        //                continue;
        //            }
        //        }

        //        strError = "";
        //        return "Số bản ghi thêm được: " + addedCount;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.Message;
        //        return 0;
        //    }
        //}
        //#endregion
    }
}
