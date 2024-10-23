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
    public class chatTinNhanRepository : RepositoryBase<chatTinNhan>
    {
        public string Connectstr { get; set; }

        public chatTinNhanRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public chatTinNhanRepository()
            : base()
        {
        }

        public chatTinNhanRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = Int64.Parse(entityId.ToString());
                var entity = Context.chatTinNhans.SingleOrDefault(o => o.MaTN == id);
                Context.chatTinNhans.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(chatTinNhan entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.MaTN;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override chatTinNhan GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.chatTinNhans.SingleOrDefault(p => p.MaTN == id);
            }
            catch { return null; }
        }

        public override List<chatTinNhan> List()
        {
            try
            {
                return Context.chatTinNhans.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<chatTinNhan> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(chatTinNhan entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<chatTinNhan> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateConnectionId(string MaNV, string ConnectionId)
        {
            bool kq = false;
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "update tblNhanVien set " +
                            "ConnectionId = '" + ConnectionId + "' , " +
                            "NgayDangNhap = getdate() " +
                            "where Id = @MaNV";
                    int i = await db.ExecuteAsync(query, new { MaNV = MaNV });
                    kq = true;
                }
            }
            catch (Exception ex)
            { }
            return kq;
        }

        public async Task<string> GetConnectionIdByMaNV(string MaNV)
        {
            string kq = "";
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ConnectionId from tblNhanVien nv " +
                        "where nv.Id=@MaNV "
                       ;
                    using (var multipleresult = await db.QueryMultipleAsync(query,
                        new
                        {
                            MaNV = MaNV
                        }))
                    {
                        var q = await multipleresult.ReadAsync<MapMe>();
                        kq = q.FirstOrDefault().ConnectionId.ToString();

                    }
                }
            }
            catch (Exception ex)
            { }
            return kq;
        }

        public async Task<string> GetListTinNhanHtml(string MaGui, string MaNhan, int page, int pageSize)
        {
            string html = "";
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    List<DanhSachTinNhan> lstData = new List<DanhSachTinNhan>();
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tn.NgayGui desc) AS RowNum, " +
                        "tn.MaTN,tn.NgayGui,tn.NoiDung,tn.MaGui,tn.MaNhan,tn.IsDelete,tn.MaTT, " +
                        "AnhDaiDienGui=(select nv.UrlImage from tblNhanVien nv where nv.Id=MaGui), " +
                        "AnhDaiDienNhan=(select nv.UrlImage from tblNhanVien nv where nv.Id=MaNhan) " +
                        "from chattinnhan tn " +
                        "where ((tn.MaGui=@MaGui and tn.MaNhan=@MaNhan) " +
                        "or (tn.MaGui=@MaNhan and tn.MaNhan=@MaGui)) " +
                        "and isnull(tn.IsDelete,0)=0 " +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize";

                    using (var multipleresult = await db.QueryMultipleAsync(query,
                        new
                        {
                            MaGui = MaGui,
                            MaNhan = MaNhan,
                            page = page,
                            pageSize = pageSize
                        }))
                    {

                        var q = await multipleresult.ReadAsync<DanhSachTinNhan>();
                        lstData = q.ToList();

                        foreach (var item in lstData.OrderBy(o => o.NgayGui))
                        {
                            if (item.MaGui == MaGui)
                            {
                                html = html + "<li class='pl-2 pr-2 text-center timesend mb-1'>" + string.Format("{0:HH:mm, dd}", item.NgayGui) + " Tháng " + string.Format("{0:MM, yyyy}", item.NgayGui) + "</li>";
                                html = html + "<li class='p-1 rounded mb-1'><div class='send-msg'><div class='send-msg-desc text-center mt-1 ml-1 pl-2 pr-2'><p class='pl-2 pr-2 rounded'>" + item.NoiDung + "</p></div></div></li>";
                            }
                            else if (item.MaNhan == MaGui)
                            {
                                html = html + "<li class='pl-2 pr-2 text-center timereceive mb-1'>" + string.Format("{0:HH:mm, dd}", item.NgayGui) + " Tháng " + string.Format("{0:MM, yyyy}", item.NgayGui) + "</li>";
                                html = html + "<li class='p-1 rounded mb-1'><div class='receive-msg'><img src='" + (!string.IsNullOrEmpty(item.AnhDaiDienGui) ? item.AnhDaiDienGui : "/Content/Customs/icon-user-default.png") + "'><div class='receive-msg-desc  text-center mt-1 ml-1 pl-2 pr-2'><p class='pl-2 pr-2 rounded'>" + item.NoiDung + "</p></div></div></li>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return html;
        }

        public async Task<int> CountGetListTinNhanHtml(string MaGui, string MaNhan, int page, int pageSize)
        {
            int count = 0;
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {

                    string query = "select count(MaTN) from chattinnhan tn " +
                        "where ((tn.MaGui=@MaGui and tn.MaNhan=@MaNhan) " +
                        "or (tn.MaGui=@MaNhan and tn.MaNhan=@MaGui)) " +
                        "and isnull(tn.IsDelete,0)=0; "
                        ;
                    using (var multipleresult = await db.QueryMultipleAsync(query,
                        new
                        {
                            MaGui = MaGui,
                            MaNhan = MaNhan,
                            page = page,
                            pageSize = pageSize
                        }))
                    {
                        try
                        {
                            var q = await multipleresult.ReadAsync<int>();
                            count = q.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            count = 0;
                        }


                    }
                }
            }
            catch (Exception ex)
            { }
            return count;
        }

        public async Task<List<DanhSachTinNhan>> GetListTinNhan(string MaNV, int page, int pageSize, string filter)
        {
            List<DanhSachTinNhan> lstData = new List<DanhSachTinNhan>();
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string lstMaNV = "";
                    //tim kiem nhan vien
                    using (var multipleresult = await db.QueryMultipleAsync(
                        "select Id from tblNhanVien nv where"+
                        " (nv.TenNhanVien like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') "))
                    {
                        var lstData1 = await multipleresult.ReadAsync<int>();
                        lstMaNV = ",";
                        foreach (var item in lstData1)
                        {
                            lstMaNV = lstMaNV + item + ",";
                        }
                        lstMaNV = lstMaNV.TrimEnd(',');
                        lstMaNV = lstMaNV + ",";


                    };

                    string query =
                    "select * from ( " +
                    "select ROW_NUMBER() OVER (ORDER BY NgayGui desc) AS RowNum, " +
                    "HoTen,MaGui,MaNhan,AnhDaiDienGui,NgayGui " +
                    ",ClassActive=case when (MaGuiCuoi=@MaNV or MaTT!=1) then '' else 'msgactive' end " +
                    ",NoiDung=case when MaGuiCuoi=@MaNV then N'Bạn: '+NoiDung else NoiDung end " +
                    "from (select  " +
                    "HoTen=case when (tn.MaGui!=@MaNV and tn.MaNhan!=@MaNV) then nvg.HoTen+', '+nvn.HoTen else (case when tn.MaGui!=@MaNV then nvg.HoTen else nvn.HoTen end) end, " +
                    "tn.MaGui, " +
                    "tn.MaNhan, " +
                    "MaGuiCuoi=(select top(1) MaGui from chattinnhan where ((MaGui=tn.MaGui and MaNhan=tn.MaNhan) or (MaGui=tn.MaNhan and MaNhan=tn.MaGui)) and isnull(tn.IsDelete,0)=0 order by NgayGui desc), " +
                    "AnhDaiDienGui=case when (tn.MaGui!=@MaNV and tn.MaNhan!=@MaNV) then 'avatar_group.png' else (case when tn.MaGui!=@MaNV then nvg.UrlImage else nvn.UrlImage end) end, " +
                    "NoiDung=(select top(1) NoiDung from chattinnhan where ((MaGui=tn.MaGui and MaNhan=tn.MaNhan) or (MaGui=tn.MaNhan and MaNhan=tn.MaGui)) and isnull(tn.IsDelete,0)=0 order by NgayGui desc), " +
                    "NgayGui=(select top(1) NgayGui from chattinnhan where ((MaGui=tn.MaGui and MaNhan=tn.MaNhan) or (MaGui=tn.MaNhan and MaNhan=tn.MaGui)) and isnull(tn.IsDelete,0)=0 order by NgayGui desc), " +
                    "MaTT=(select top(1) MaTT from chattinnhan where ((MaGui=tn.MaGui and MaNhan=tn.MaNhan) or (MaGui=tn.MaNhan and MaNhan=tn.MaGui)) and isnull(tn.IsDelete,0)=0 order by NgayGui desc) " +
                    "from ChatTinNhan tn " +
                    "join tblNhanVien nvg on nvg.MaNV=tn.MaGui " +
                    "join tblNhanVien nvn on nvn.MaNV=tn.MaNhan " +
                    "where tn.MaCha is null " +
                    "and (('" + lstMaNV + "' like N'%,' + Cast(tn.MaGui as nvarchar(50)) + N',%') " +
                    "or ('" + lstMaNV + "' like N'%,' + Cast(tn.MaNhan as nvarchar(50)) + N',%')) " +
                    ") as kq " +
                    ") as kq " +
                    "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize";

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            MaNV = MaNV,
                            page = page,
                            pageSize = pageSize,
                            
                        }))
                    {
                        var q = await multipleresult.ReadAsync<DanhSachTinNhan>();
                        lstData = q.ToList();

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lstData;
        }
    }
    public class MapMe
    {
        public Guid? ConnectionId { get; set; }
    }
    public class DanhSachTinNhan
    {
        public Int64 MaTN { get; set; }
        public DateTime? NgayGui { get; set; }
        public string NoiDung { get; set; }
        public string MaGui { get; set; }
        public string MaNhan { get; set; }
        public bool? IsDelete { get; set; }
        public byte? MaTT { get; set; }
        public string AnhDaiDienGui { get; set; }
        public string AnhDaiDienNhan { get; set; }

        //
        public string HoTen { get; set; }
        public string ClassActive { get; set; }
    }

}
