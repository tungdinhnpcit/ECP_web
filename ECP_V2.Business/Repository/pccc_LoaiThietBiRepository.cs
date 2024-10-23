using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class pccc_LoaiThietBiRepository : RepositoryBase<pccc_LoaiThietBi>
    {
        static string DBName { get; set; }
        public string Connectstr { get; set; }
        public pccc_LoaiThietBiRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public pccc_LoaiThietBiRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.pccc_LoaiThietBi.SingleOrDefault(o => o.ID == id);
                Context.pccc_LoaiThietBi.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(pccc_LoaiThietBi entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override pccc_LoaiThietBi GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.pccc_LoaiThietBi.SingleOrDefault(o => o.ID == id);
            }
            catch { return null; }
        }

        public override List<pccc_LoaiThietBi> List()
        {
            try
            {
                return Context.pccc_LoaiThietBi.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<pccc_LoaiThietBi> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(pccc_LoaiThietBi entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<pccc_LoaiThietBi> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public List<pccc_LoaiThietBi> GetListByMaNhom(string MaNhom)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from pccc_LoaiThietBi where MaNhom=" + MaNhom;
                    var q = db.Query<pccc_LoaiThietBi>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<pccc_LoaiThietBi>(); }
        }

        public pccc_LoaiThietBiModel GetObjByID(string ID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from pccc_LoaiThietBi where ID=" + ID;
                    var q = db.Query<pccc_LoaiThietBiModel>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new pccc_LoaiThietBiModel(); }
        }

        public List<pccc_LoaiThietBiModel> ListPaging(int page, int pageSize, string filter, string TuNgay, string DenNgay,
          string DonViId, string PhongBanId, string MaNhom)
        {
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(TuNgay))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(DenNgay))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }

            List<pccc_LoaiThietBiModel> lstData = new List<pccc_LoaiThietBiModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tb.[ID]) AS RowNum " +
                        ",tb.ID,tb.TenLoai " +
                        ",TenHangSX=(select Name from HangSanXuat where ID=tb.MaHSX) " +
                        ",TenNuocSX=(select Name from NuocSanXuat where ID=tb.MaNSX) " +
                        ",tb.NamSX,tb.NgayDuaVaoSuDung " +
                        //",TenPB=(select TenPhongBan from tblPhongBan where Id=tb.PhongBanID) " +
                        //",tb.PhongBanID,tb.DonViId "+
                        ",tb.NgayTao,tb.NguoiTao,tb.NgaySua,tb.NguoiSua,tb.HanKiemDinh,tb.TaiTrong,tb.HoSoThietBi,tb.QuyTacDanhMa " +
                        "from pccc_LoaiThietBi tb " +
                        "where " +
                        "(tb.TenLoai like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (tb.DonViId=@DonViId or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,tb.NgayTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,tb.NgayTao) <= CONVERT(date,@DenNgay) or @DenNgay='') " +
                        "and tb.MaNhom=@MaNhom "
                        ;



                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize"
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            MaNhom = MaNhom
                        }))
                    {
                        var q = multipleresult.Read<pccc_LoaiThietBiModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string filter, string TuNgay, string DenNgay,
            string DonViId, string PhongBanId, string MaNhom)
        {
            string start1 = "";
            string end1 = "";

            if (string.IsNullOrEmpty(TuNgay))
            {

            }
            else
            {
                DateTime dts = DateTime.ParseExact(TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
            }

            if (string.IsNullOrEmpty(DenNgay))
            {

            }
            else
            {
                DateTime dte = DateTime.ParseExact(DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
            }

            int count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(ID) " +
                        "from pccc_LoaiThietBi tb " +
                        "where " +
                        "(tb.TenLoai like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (tb.DonViId=@DonViId or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,tb.NgayTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,tb.NgayTao) <= CONVERT(date,@DenNgay) or @DenNgay='') " +
                        "and tb.MaNhom=@MaNhom "
                        ;



                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            MaNhom = MaNhom
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

        public string DeleteAll(string[] entityId, ref string strError)
        {
            try
            {
                var entity = Context.pccc_LoaiThietBi.Where(o => entityId.ToList().Contains(o.ID.ToString()));
                Context.pccc_LoaiThietBi.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public List<pccc_ThongKeoLoaiThietBiModel> GetListThongKe(string MaNhom)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select * from pccc_LoaiThietBi where MaNhom=" + MaNhom;
                    var q = db.Query<pccc_ThongKeoLoaiThietBiModel>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<pccc_ThongKeoLoaiThietBiModel>(); }
        }

        public int GetIdByName(string Name, string MaNhom)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from pccc_LoaiThietBi where MaNhom=" + MaNhom + " and UPPER(TenLoai) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

    }

    public class pccc_LoaiThietBiModel
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public Nullable<int> MaHSX { get; set; }
        public Nullable<int> MaNSX { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> MaNhom { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public Nullable<decimal> TaiTrong { get; set; }
        public string HoSoThietBi { get; set; }
        public string QuyTacDanhMa { get; set; }

        public string TenPB { get; set; }
        public string TenHangSX { get; set; }
        public string TenNuocSX { get; set; }
    }

    public class pccc_ThongKeoLoaiThietBiModel
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public int SoLuong { get; set; }
    }
}
