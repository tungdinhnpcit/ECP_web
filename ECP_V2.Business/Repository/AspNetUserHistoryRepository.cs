using Dapper;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class AspNetUserHistoryRepository : RepositoryBase_V2
    {
        private ECP_V2Entities _context;
        public AspNetUserHistoryRepository()
        {
            _context = new ECP_V2Entities();
        }
        public int Add(AspNetUserHistory model)
        {
            try
            {
                _context.AspNetUserHistories.Add(model);
                _context.SaveChanges();
                return model.Id;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return -1;
            }
        }
        public List<AspNetUserHistoryViewModel> List()
        {
            try
            {
                var connection = new SqlConnection(_context.Database.Connection.ConnectionString);
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select * " +
                                   "from AspNetUserHistory"
                        ;
                    return db.Query<AspNetUserHistoryViewModel>(query).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<AspNetUserHistoryViewModel> ListPaging(int page, int pageSize, string beginDate,
            string endDate, string trangThai, string filter, string PhongBanId, string DonViId)
        {
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(beginDate))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(endDate))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }

            List<AspNetUserHistoryViewModel> lstData = new List<AspNetUserHistoryViewModel>();
            try
            {
                using (SqlConnection db = new SqlConnection(_context.Database.Connection.ConnectionString))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY anuh.[Id]) AS RowNum " +
                        ",* " +
                        "from AspNetUserHistory anuh " +
                        "where " +
                        "((anuh.TaiKhoan like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) <= CONVERT(date,@DenNgay) or @DenNgay=''))"
                        ;
                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        query = query + "and anuh.TrangThai = @TrangThai ";
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.DonViId=@DonViId) ) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.PhongBanId=@PhongBanId) ) ";
                    }

                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize"
                        ;
                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            TrangThai = trangThai,
                            TuNgay = start1,
                            DenNgay = end1,
                            DonViId = DonViId,
                            PhongBanId = PhongBanId
                        }))
                    {
                        var q = multipleresult.Read<AspNetUserHistoryViewModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string beginDate, string endDate, string trangThai,
            string filter, string PhongBanId, string DonViId)
        {

            int count = 0;
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(beginDate))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(endDate))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }
            try
            {
                using (SqlConnection db = new SqlConnection(_context.Database.Connection.ConnectionString))
                {
                    string query = "select count(Id) " +
                        "from AspNetUserHistory anuh " +
                        "where " +
                        "((anuh.TaiKhoan like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) <= CONVERT(date,@DenNgay) or @DenNgay=''))"
                        ;
                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        query = query + "and anuh.TrangThai = @TrangThai ";
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.DonViId=@DonViId) ) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.PhongBanId=@PhongBanId) ) ";
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            TrangThai = trangThai,
                            TuNgay = start1,
                            DenNgay = end1,
                            DonViId = DonViId,
                            PhongBanId = PhongBanId
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<int>();
                            count = q.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            count = 0;
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return count;
        }

        public List<AspNetUserHistoryViewModel> Export(string beginDate, string endDate,
            string trangThai, string filter, string PhongBanId, string DonViId)
        {
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(beginDate))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(endDate))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }

            List<AspNetUserHistoryViewModel> lstData = new List<AspNetUserHistoryViewModel>();
            try
            {
                using (SqlConnection db = new SqlConnection(_context.Database.Connection.ConnectionString))
                {
                    string query = "select * " +
                        ",TenDV=(select (select TenDonVi from tblDonVi dv where dv.Id=nv.DonViId) from tblNhanVien nv where nv.Username=anuh.TaiKhoan) " +
                        "from AspNetUserHistory anuh " +
                        "where " +
                        "((anuh.TaiKhoan like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) <= CONVERT(date,@DenNgay) or @DenNgay=''))"
                        ;

                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        query = query + "and anuh.TrangThai = @TrangThai ";
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.DonViId=@DonViId) ) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.PhongBanId=@PhongBanId) ) ";
                    }

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            trangThai = trangThai,
                            TuNgay = start1,
                            DenNgay = end1,
                            DonViId = DonViId,
                            PhongBanId = PhongBanId
                        }))
                    {
                        var q = multipleresult.Read<AspNetUserHistoryViewModel>();
                        lstData = q.ToList();
                    }

                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public List<AspNetUserHistoryViewModel> ExportTongHop(string beginDate, string endDate,
          string trangThai, string filter, string PhongBanId, string DonViId)
        {
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(beginDate))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(endDate))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }

            List<AspNetUserHistoryViewModel> lstData = new List<AspNetUserHistoryViewModel>();
            try
            {
                using (SqlConnection db = new SqlConnection(_context.Database.Connection.ConnectionString))
                {
                    string query = "select anuh.TaiKhoan " +
                        ",TenDV=(select (select TenDonVi from tblDonVi dv where dv.Id=nv.DonViId) from tblNhanVien nv where nv.Username=anuh.TaiKhoan) " +
                        "from AspNetUserHistory anuh " +
                        "where " +
                        "((anuh.TaiKhoan like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,anuh.ThoiGianTao) <= CONVERT(date,@DenNgay) or @DenNgay=''))"
                        ;

                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        query = query + "and anuh.TrangThai = @TrangThai ";
                    }

                    if (!string.IsNullOrEmpty(DonViId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.DonViId=@DonViId) ) ";
                    }

                    if (!string.IsNullOrEmpty(PhongBanId))
                    {
                        query = query + "and (anuh.TaiKhoan in (select Username from tblNhanVien nv where nv.PhongBanId=@PhongBanId) ) ";
                    }

                    query = query + "group by anuh.TaiKhoan";

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            trangThai = trangThai,
                            TuNgay = start1,
                            DenNgay = end1,
                            DonViId = DonViId,
                            PhongBanId = PhongBanId
                        }))
                    {
                        var q = multipleresult.Read<AspNetUserHistoryViewModel>();
                        lstData = q.ToList();


                        if (lstData != null)
                        {
                            foreach(var item in lstData)
                            {
                                item.SL = CountListPaging(beginDate, endDate, trangThai, item.TaiKhoan, PhongBanId, DonViId);
                                item.SLLK = CountListPaging("", endDate, trangThai, item.TaiKhoan, PhongBanId, DonViId);
                            }
                        }
                    }

                }
            }
            catch (Exception ex) { }
            return lstData;
        }
    }

    public class AspNetUserHistoryViewModel
    {
        public int Id { get; set; }
        public string TaiKhoan { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string TrangThai { get; set; }
        public string IP { get; set; }
        public string TenDV { get; set; }
        public int? SL { get; set; }
        public int? SLLK { get; set; }
    }



}
