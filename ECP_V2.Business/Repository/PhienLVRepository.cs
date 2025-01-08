using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECP_V2.DataAccess;
using System.Data;
using ECP_V2.Business.UnitOfWork;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using ECP_V2.Common.Helpers;
using Dapper;
using System.Data.Entity.Validation;

namespace ECP_V2.Business.Repository
{
    public class PhienLVRepository : RepositoryBase<tblPhienLamViec>
    {
        public string connection;

        public PhienLVRepository()
            : base()
        {
            try
            {
                var con = new SqlConnection(Context.Database.Connection.ConnectionString);
                connection = con.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public PhienLVRepository(WorkUnit unit)
            : base(unit)
        {
        }


        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tblPhienLamViecs.FirstOrDefault(o => o.Id == id);
                Context.tblPhienLamViecs.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override tblPhienLamViec GetById(object entityId)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    var id = int.Parse(entityId.ToString());
                    return db.tblPhienLamViecs.FirstOrDefault(p => p.Id == id);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override List<tblPhienLamViec> List()
        {
            try
            {
                return Context.tblPhienLamViecs.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public tblPhienLamViec GetByMaPhieuCongTac(int maPhieuCongTac)
        {
            try
            {
                return Context.tblPhienLamViecs.Where(p => p.MaPCT == maPhieuCongTac).FirstOrDefault();
            }
            catch { return null; }
        }

        public List<tblPhienLamViec> GetListByMaPhieuCongTac(int maPhieuCongTac)
        {
            try
            {
                return Context.tblPhienLamViecs.Where(p => p.MaPCT == maPhieuCongTac).ToList();
            }
            catch { return null; }
        }

        #region AdvancedSearchPhienLv
        public IEnumerable<PhienLVModel> AdvancedSearchPhienLv(int page, int pagelength, string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhienLVNew @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Skip,@Take,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Skip", page.ToString()),
                       new SqlParameter("@Take", pagelength),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }
        #endregion



        #region AdvancedSearchPhienLvAll
        public IEnumerable<PhienLVModel> AdvancedSearchPhienLvAll( int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";

                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();

                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhienLVNewAll @TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }
        #endregion

        #region AdvancedSearchPhienLVNewAll
        public IEnumerable<PhienLVModel> AdvancedSearchPhienLVNewAll(int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";

                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();

                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhienLVNewAll @TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }
        #endregion

        #region sp_AdvancedSearchPhienLVNewAllCV
        public IEnumerable<PhienLVModel> sp_AdvancedSearchPhienLVNewAllCV(int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";

                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();

                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhienLVNewAllCV @TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }
        #endregion


        #region TongHopBaoCaoCuoiNgayPhienLV
        public IEnumerable<TongHopBaoCaoCuoiNgayPhienLV> TongHopBaoCaoCuoiNgayPhienLV(string DateFrom, string DateTo, string DonViID)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start = "";
                    string end = "";

                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    return db.Database.SqlQuery<TongHopBaoCaoCuoiNgayPhienLV>("EXEC sp_TongHopBaoCaoCuoiNgayPhienLV @DonViID,@NgayBD,@NgayKT",
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@NgayBD", start),
                       new SqlParameter("@NgayKT", end)).ToList();

                }
            }
            catch (Exception ex)
            { return new List<TongHopBaoCaoCuoiNgayPhienLV>(); }
        }
        #endregion

        #region CountTotalPhienLV
        public int CountTotalPhienLV(string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    var data = db.Database.SqlQuery<int>("EXEC sp_CountTotalPhienLVNew @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    if (data.Count > 0)
                        return int.Parse(data[0].ToString());
                    else return 0;
                }
            }
            catch { return 0; }
        }
        #endregion

        #region CountTotalPhienLVAll
        public int CountTotalPhienLVAll( int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";


                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    var data = db.Database.SqlQuery<int>("EXEC sp_CountTotalPhienLVNewAll @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    if (data.Count > 0)
                        return int.Parse(data[0].ToString());
                    else return 0;
                }
            }
            catch { return 0; }
        }
        #endregion

        #region CountTotalPhienLVNewAll
        public int CountTotalPhienLVNewAll(int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";


                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    var data = db.Database.SqlQuery<int>("EXEC sp_AdvancedSearchPhienLVNewAll @TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role",
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName)).ToList();

                    if (data.Count > 0)
                        return int.Parse(data[0].ToString());
                    else return 0;
                }
            }
            catch { return 0; }
        }
        #endregion

        #region GetListPhienLVOneWeek
        public IEnumerable<PhienLVModel> GetListPhienLVOneWeek(string DonViID, string type, string roleName, string PhongBanID)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";

                    //int offset = DateTime.Now.DayOfWeek - DayOfWeek.Monday;
                    //if (offset < 0) offset += 7; // Đảm bảo thứ Hai là đầu tuần

                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    //dts = DateTime.Now.AddDays(-offset).Date; // Ngày đầu tuần (thứ Hai)
                    //dte = dts.AddDays(6); // Ngày cuối tuần (Chủ Nhật)
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    if (type == "TT")
                    {
                        dts = dts.AddDays(-7);
                        dte = dte.AddDays(-7);
                    }
                    else if (type == "TK")
                    {
                        dts = dts.AddDays(7);
                        dte = dte.AddDays(7);
                    }

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);

                    var result = db.Database.SqlQuery<PhienLVModel>("EXEC sp_GetListPhienLVOneWeek @NgayBD,@NgayKT,@DonViID,@role,@PhongBanID",

                    new SqlParameter("@NgayBD", start1.ToString()),
                    new SqlParameter("@NgayKT", end1.ToString()),
                    new SqlParameter("@DonViID", DonViID.ToString()),
                    new SqlParameter("@role", roleName),
                    new SqlParameter("@PhongBanID", PhongBanID)
                    );
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region GetPhienLVById
        public PhienLVModel GetPhienLVById(int Id)
        {
            try
            {
                var data = Context.Database.SqlQuery<PhienLVModel>("EXEC sp_PhienLamViec_getbyId @Id",
               new SqlParameter("@Id", Id.ToString())).ToList();
                if (data.Count > 0)
                    return data[0];
                else
                    return new PhienLVModel();
            }
            catch { return new PhienLVModel(); }
        }
        public PhienLVModel GetPhienLVByIdV2(int Id)
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                using (IDbConnection db = new SqlConnection(connection.ConnectionString))
                {
                    string query = "select * " +
                        "from tblPhienLamViec " +
                        "where Id=" + Id;
                    var q = db.Query<PhienLVModel>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion

        #region PhienLamViec_Add
        public string PhienLamViec_Add(tblPhienLamViec plv)
        {
            string kt = "";
            try
            {
                Context.Database.ExecuteSqlCommand("EXEC sp_PhienLamViec_add @PhongBanID,@NoiDung,@DiaDiem,@NgayLamViec,@GioBd,@GioKt,@NguoiDuyet_SoPa,@NguoiChiHuy,@GiamSatVien,@NguoiKiemSoat,@NguoiKiemTraPhieu,@LanhDaoTrucBan,@NgayTao,@NguoiTao,@NgaySua,@NguoiSua,@TT_Phien,@TrangThai,@NguoiDuyet,@NgayDuyet,@LyDoThayDoi",
                   new SqlParameter("@PhongBanID", plv.PhongBanID),
                   new SqlParameter("@NoiDung", plv.NoiDung),
                   new SqlParameter("@DiaDiem", plv.DiaDiem),
                   new SqlParameter("@NgayLamViec", plv.NgayLamViec),
                   new SqlParameter("@GioBd", plv.GioBd),
                   new SqlParameter("@GioKt", plv.GioKt),
                   new SqlParameter("@NguoiDuyet_SoPa", plv.NguoiDuyet_SoPa),
                   new SqlParameter("@NguoiChiHuy", plv.NguoiChiHuy),
                   new SqlParameter("@GiamSatVien", plv.GiamSatVien),
                   new SqlParameter("@NguoiKiemSoat", plv.NguoiKiemSoat),
                   new SqlParameter("@NguoiKiemTraPhieu", plv.NguoiKiemTraPhieu),
                   new SqlParameter("@LanhDaoTrucBan", plv.LanhDaoTrucBan),
                   new SqlParameter("@NgayTao", plv.NgayTao),
                   new SqlParameter("@NguoiTao", plv.NguoiTao),
                   new SqlParameter("@NgaySua", DBNull.Value),
                   new SqlParameter("@NguoiSua", DBNull.Value),
                   new SqlParameter("@TT_Phien", plv.TT_Phien),
                   new SqlParameter("@TrangThai", plv.TrangThai),
                   new SqlParameter("@NguoiDuyet", DBNull.Value),
                   new SqlParameter("@NgayDuyet", DBNull.Value),
                   new SqlParameter("@LyDoThayDoi", plv.LyDoThayDoi)
                   );
            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }

        public int PhienLamViec_AddNew(tblPhienLamViec plv)
        {
            int kt = 0;
            //using (var db = new ECP_V2Entities())
            //{
            //    plv.IsChuyenNPC = false;
            //    db.tblPhienLamViecs.Add(plv);
            //    db.SaveChanges();
            //    kt = plv.Id;
            //}
            try
            {
                plv.IsChuyenNPC = false;
                Context.tblPhienLamViecs.Add(plv);
                Context.SaveChanges();
                kt = plv.Id;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

                kt = -1;
            }
            //finally
            //{
            //    Context.Dispose();     
            //}
            return kt;
        }
        #endregion

        #region PhienLamViec_Update
        public int PhienLamViec_UpdateNew(tblPhienLamViec plv)
        {
            int kt = 0;
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    tblPhienLamViec phienlv = db.tblPhienLamViecs.FirstOrDefault(x => x.Id == plv.Id);
                    if (phienlv != null)
                    {
                        phienlv.PhongBanID = plv.PhongBanID;
                        phienlv.NoiDung = plv.NoiDung;
                        phienlv.DiaDiem = plv.DiaDiem;
                        phienlv.NgayLamViec = plv.NgayLamViec;
                        phienlv.GioBd = plv.GioBd;
                        phienlv.GioKt = plv.GioKt;
                        phienlv.NguoiDuyet_SoPa = plv.NguoiDuyet_SoPa;
                        phienlv.NguoiChiHuy = plv.NguoiChiHuy;
                        phienlv.GiamSatVien = plv.GiamSatVien;
                        phienlv.NguoiKiemSoat = plv.NguoiKiemSoat;
                        phienlv.NguoiKiemTraPhieu = plv.NguoiKiemTraPhieu;
                        phienlv.LanhDaoTrucBan = plv.LanhDaoTrucBan;
                        phienlv.NgaySua = plv.NgaySua;
                        phienlv.NguoiSua = plv.NguoiSua;
                        phienlv.TrangThai = plv.TrangThai;
                        phienlv.LyDoThayDoi = plv.LyDoThayDoi;
                        phienlv.NguoiDuyet_SoPa_Id = plv.NguoiDuyet_SoPa_Id;
                        phienlv.NguoiChiHuy_Id = plv.NguoiChiHuy_Id;
                        phienlv.GiamSatVien_Id = plv.GiamSatVien_Id;
                        phienlv.NguoiKiemSoat_Id = plv.NguoiKiemSoat_Id;
                        phienlv.NguoiKiemTraPhieu_Id = plv.NguoiKiemTraPhieu_Id;
                        phienlv.LanhDaoTrucBan_Id = plv.LanhDaoTrucBan_Id;
                        phienlv.KinhDo = plv.KinhDo;
                        phienlv.ViDo = plv.ViDo;
                        phienlv.MaPCT = plv.MaPCT;

                        phienlv.LanhDaoCongViec_Id = plv.LanhDaoCongViec_Id;
                        phienlv.LanhDaoCongViec = plv.LanhDaoCongViec;

                        phienlv.NguoiCapPhieu_Id = plv.NguoiCapPhieu_Id;
                        phienlv.NguoiCapPhieu = plv.NguoiCapPhieu;

                        phienlv.PhongBanIDCreate = plv.PhongBanIDCreate;

                        db.SaveChanges();
                        kt = phienlv.Id;
                    }
                }

            }
            catch (Exception ex) { kt = -1; }
            return kt;
        }
        public string PhienLamViec_Update(tblPhienLamViec plv)
        {
            string kt = "";
            try
            {
                Context.Database.ExecuteSqlCommand("EXEC sp_PhienLamViec_update @Id,@PhongBanID,@NoiDung,@DiaDiem,@NgayLamViec,@GioBd,@GioKt,@NguoiDuyet_SoPa,@NguoiChiHuy,@GiamSatVien,@NguoiKiemSoat,@NguoiKiemTraPhieu,@LanhDaoTrucBan,@NgaySua,@NguoiSua,@TT_Phien,@LyDoThayDoi",
                   new SqlParameter("@Id", plv.Id),
                   new SqlParameter("@PhongBanID", plv.PhongBanID),
                   new SqlParameter("@NoiDung", plv.NoiDung),
                   new SqlParameter("@DiaDiem", plv.DiaDiem),
                   new SqlParameter("@NgayLamViec", plv.NgayLamViec),
                   new SqlParameter("@GioBd", plv.GioBd),
                   new SqlParameter("@GioKt", plv.GioKt),
                   new SqlParameter("@NguoiDuyet_SoPa", plv.NguoiDuyet_SoPa),
                   new SqlParameter("@NguoiChiHuy", plv.NguoiChiHuy),
                   new SqlParameter("@GiamSatVien", plv.GiamSatVien),
                   new SqlParameter("@NguoiKiemSoat", plv.NguoiKiemSoat),
                   new SqlParameter("@NguoiKiemTraPhieu", plv.NguoiKiemTraPhieu),
                   new SqlParameter("@LanhDaoTrucBan", plv.LanhDaoTrucBan),
                   new SqlParameter("@NgaySua", plv.NgaySua),
                   new SqlParameter("@NguoiSua", plv.NguoiSua),
                   new SqlParameter("@TT_Phien", plv.TT_Phien),
                   new SqlParameter("@LyDoThayDoi", plv.LyDoThayDoi)
                   );


            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }
        #endregion

        #region PhienLamViec_Delete
        public string PhienLamViec_Delete(int Id)
        {
            string kt = "";
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("EXEC sp_PhienLamViec_delete @Id",
                  new SqlParameter("@Id", Id)
                  );
                }
            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }
        #endregion

        #region TKPhienLVTheoTuan_KH
        public List<int> TKPhienLVTheoTuan_KH(string DonViID)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue("TKPhienLVTheoTuan_KH_" + DonViID);
                if (datacache != null)
                {
                    return (List<int>)datacache;
                }
                else
                {
                    int donviId = 0;
                    try
                    {
                        donviId = int.Parse(DonViID);
                    }
                    catch { }

                    List<int> lstData = new List<int>();

                    string start1 = "";
                    string end1 = "";

                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);


                    List<PhienLVTuan> data = new List<PhienLVTuan>();
                    if (donviId == 0 || donviId == 1)
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_KH @NgayBD,@NgayKT,@DonViID",
                     new SqlParameter("@NgayBD", start1.ToString()),
                     new SqlParameter("@NgayKT", end1.ToString()),
                     new SqlParameter("@DonViID", "")).ToList();
                    }
                    else
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_KH @NgayBD,@NgayKT,@DonViID",
                   new SqlParameter("@NgayBD", start1.ToString()),
                   new SqlParameter("@NgayKT", end1.ToString()),
                   new SqlParameter("@DonViID", donviId.ToString())).ToList();
                    }

                    // lấy ra danh sách tất cả các ngày trong tuần hiện tại
                    DateTime today = DateTime.Today.Date;
                    int currentDayOfWeek = (int)today.DayOfWeek;
                    DateTime sunday = today.AddDays(-currentDayOfWeek);
                    DateTime monday = sunday.AddDays(1);
                    // If we started on Sunday, we should actually have gone *back*
                    // 6 days instead of forward 1...
                    if (currentDayOfWeek == 0)
                    {
                        monday = monday.AddDays(-7);
                    }
                    var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();

                    // duyệt qua tất cả các ngày trong tuần hiện tại
                    foreach (var dt in dates)
                    {
                        // Nếu ngày này chưa được tạo lịch sẽ tự động thêm dòng trắng vào
                        var plv = data.FirstOrDefault(o => o.NgayLamViec.Date == dt);
                        if (plv == null)
                        {
                            data.Add(new PhienLVTuan()
                            {
                                NgayLamViec = dt,
                                SL = 0

                            });
                        }
                    }

                    //sắp xếp lại theo ngày tạo
                    data = data.OrderBy(o => o.NgayLamViec).ToList();

                    foreach (var item in data)
                    {
                        lstData.Add(item.SL);
                    }

                    MemoryCacheHelper.Add("TKPhienLVTheoTuan_KH_" + DonViID, lstData, DateTimeOffset.UtcNow.AddDays(1));
                    return lstData;
                }
            }
            catch (Exception ex) { return null; }
        }
        #endregion

        #region TKPhienLVTheoTuan_BS
        public List<int> TKPhienLVTheoTuan_BS(string DonViID)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue("TKPhienLVTheoTuan_BS_" + DonViID);
                if (datacache != null)
                {
                    return (List<int>)datacache;
                }
                else
                {
                    int donviId = 0;
                    try
                    {
                        donviId = int.Parse(DonViID);
                    }
                    catch { }

                    List<int> lstData = new List<int>();

                    string start1 = "";
                    string end1 = "";

                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);


                    List<PhienLVTuan> data = new List<PhienLVTuan>();
                    if (donviId == 0 || donviId == 1)
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_BS @NgayBD,@NgayKT,@DonViID",
                   new SqlParameter("@NgayBD", start1.ToString()),
                   new SqlParameter("@NgayKT", end1.ToString()),
                   new SqlParameter("@DonViID", "")).ToList();
                    }
                    else
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_BS @NgayBD,@NgayKT,@DonViID",
                   new SqlParameter("@NgayBD", start1.ToString()),
                   new SqlParameter("@NgayKT", end1.ToString()),
                   new SqlParameter("@DonViID", donviId.ToString())).ToList();
                    }
                    // lấy ra danh sách tất cả các ngày trong tuần hiện tại
                    DateTime today = DateTime.Today.Date;
                    int currentDayOfWeek = (int)today.DayOfWeek;
                    DateTime sunday = today.AddDays(-currentDayOfWeek);
                    DateTime monday = sunday.AddDays(1);
                    // If we started on Sunday, we should actually have gone *back*
                    // 6 days instead of forward 1...
                    if (currentDayOfWeek == 0)
                    {
                        monday = monday.AddDays(-7);
                    }
                    var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();

                    // duyệt qua tất cả các ngày trong tuần hiện tại
                    foreach (var dt in dates)
                    {
                        // Nếu ngày này chưa được tạo lịch sẽ tự động thêm dòng trắng vào
                        var plv = data.FirstOrDefault(o => o.NgayLamViec.Date == dt);
                        if (plv == null)
                        {
                            data.Add(new PhienLVTuan()
                            {
                                NgayLamViec = dt,
                                SL = 0

                            });
                        }
                    }

                    //sắp xếp lại theo ngày tạo
                    data = data.OrderBy(o => o.NgayLamViec).ToList();

                    foreach (var item in data)
                    {
                        lstData.Add(item.SL);
                    }

                    MemoryCacheHelper.Add("TKPhienLVTheoTuan_BS_" + DonViID, lstData, DateTimeOffset.UtcNow.AddDays(1));
                    return lstData;
                }
            }
            catch { return null; }
        }
        #endregion

        #region TKPhienLVTheoTuan_DX
        public List<int> TKPhienLVTheoTuan_DX(string DonViID)
        {
            try
            {
                var datacache = MemoryCacheHelper.GetValue("TKPhienLVTheoTuan_DX_" + DonViID);
                if (datacache != null)
                {
                    return (List<int>)datacache;
                }
                else
                {
                    int donviId = 0;
                    try
                    {
                        donviId = int.Parse(DonViID);
                    }
                    catch { }

                    List<int> lstData = new List<int>();

                    string start1 = "";
                    string end1 = "";

                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);


                    List<PhienLVTuan> data = new List<PhienLVTuan>();
                    if (donviId == 0 || donviId == 1)
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_DX @NgayBD,@NgayKT,@DonViID",
                   new SqlParameter("@NgayBD", start1.ToString()),
                   new SqlParameter("@NgayKT", end1.ToString()),
                   new SqlParameter("@DonViID", "")).ToList();
                    }
                    else
                    {
                        data = Context.Database.SqlQuery<PhienLVTuan>("EXEC sp_TKPhienLVOneWeek_DX @NgayBD,@NgayKT,@DonViID",
                   new SqlParameter("@NgayBD", start1.ToString()),
                   new SqlParameter("@NgayKT", end1.ToString()),
                   new SqlParameter("@DonViID", donviId.ToString())).ToList();
                    }
                    // lấy ra danh sách tất cả các ngày trong tuần hiện tại
                    DateTime today = DateTime.Today.Date;
                    int currentDayOfWeek = (int)today.DayOfWeek;
                    DateTime sunday = today.AddDays(-currentDayOfWeek);
                    DateTime monday = sunday.AddDays(1);
                    // If we started on Sunday, we should actually have gone *back*
                    // 6 days instead of forward 1...
                    if (currentDayOfWeek == 0)
                    {
                        monday = monday.AddDays(-7);
                    }
                    var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();

                    // duyệt qua tất cả các ngày trong tuần hiện tại
                    foreach (var dt in dates)
                    {
                        // Nếu ngày này chưa được tạo lịch sẽ tự động thêm dòng trắng vào
                        var plv = data.FirstOrDefault(o => o.NgayLamViec.Date == dt);
                        if (plv == null)
                        {
                            data.Add(new PhienLVTuan()
                            {
                                NgayLamViec = dt,
                                SL = 0

                            });
                        }
                    }

                    //sắp xếp lại theo ngày tạo
                    data = data.OrderBy(o => o.NgayLamViec).ToList();

                    foreach (var item in data)
                    {
                        lstData.Add(item.SL);
                    }

                    MemoryCacheHelper.Add("TKPhienLVTheoTuan_DX_" + DonViID, lstData, DateTimeOffset.UtcNow.AddDays(1));
                    return lstData;
                }

            }
            catch { return null; }
        }
        #endregion

        #region GetEmailByDonViID
        public IEnumerable<EmailSend> GetEmailByDonViID(string DonViID, string Role)
        {
            try
            {
                if (string.IsNullOrEmpty(Role))
                    Role = "";

                if (DonViID != null)
                {
                    return Context.Database.SqlQuery<EmailSend>("EXEC sp_Email_getbyDonViID @DonViID,@Role",
                    new SqlParameter("@DonViID", DonViID.ToString()),
                    new SqlParameter("@Role", Role)).ToList();
                }
                else
                {
                    return null;
                }

            }
            catch { return null; }
        }
        #endregion

        #region ExportPhienLv
        public IEnumerable<PhienLVModel> ExportPhienLv(string filter, int tcphien, int trangThai, string DateFrom, string DateTo, string DonViID, int PhongBanID)
        {
            try
            {

                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(filter))
                    filter = "";
                //if (string.IsNullOrEmpty(tcphien))
                //    tcphien = "";
                if (string.IsNullOrEmpty(DonViID))
                    DonViID = "";

                // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                return Context.Database.SqlQuery<PhienLVModel>("EXEC sp_ExportPhienLV @filter,@TCPhien,@TrangThai,@DonViID,@PhongBanID,@NgayBD,@NgayKT",
                   new SqlParameter("@filter", filter),
                   new SqlParameter("@TCPhien", tcphien),
                   new SqlParameter("@TrangThai", trangThai),
                   new SqlParameter("@DonViID", DonViID),
                   new SqlParameter("@PhongBanID", PhongBanID),
                   new SqlParameter("@NgayBD", start1),
                   new SqlParameter("@NgayKT", end1)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
                return new List<PhienLVModel>();
            }
        }
        #endregion

        #region PhienLamViec_Duyet
        public string PhienLamViec_Duyet(tblPhienLamViec plv)
        {
            string kt = "";
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Database.ExecuteSqlCommand("EXEC sp_PhienLamViec_duyet @Id,@NgayDuyet,@NguoiDuyet,@TrangThai",
                    new SqlParameter("@Id", plv.Id),
                    new SqlParameter("@NgayDuyet", plv.NgayDuyet),
                    new SqlParameter("@NguoiDuyet", plv.NguoiDuyet),
                    new SqlParameter("@TrangThai", plv.TrangThai)
                    );
                }
            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }

        public string PhienLamViec_UpdateTrangThai(tblPhienLamViec plv)
        {
            string kt = "";
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    db.Entry(plv).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { kt = ex.Message; }
            return kt;
        }

        #endregion
        #region
        public string GetCameraIdByPhienId(int phienid)
        {
            string cameraid = "";
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_get_cameraid_byphienid", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@id", phienid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cameraid = rdr["cameraid"].ToString();
                }
            }

            return cameraid;

        }

        public List<Camera> GetDsCamera(string madvql)
        {
            Camera cam = new Camera();
            List<Camera> listCam = new List<Camera>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_get_dscamera", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cam = new Camera();
                    cam.CamId = rdr["CAM_ID"].ToString();
                    cam.CamDesc = rdr["CAM_DESC"].ToString();
                    cam.MaDvql = rdr["MA_DVIQLY"].ToString();

                    listCam.Add(cam);
                }
            }

            return listCam;

        }

        public void addCameraToPhien(string madvql, string idphien, string idcamera)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("usp_add_camera_to_phien", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@madvql", SqlDbType.NVarChar).Value = madvql;
                sql_cmnd.Parameters.AddWithValue("@id", SqlDbType.Int).Value = int.Parse(idphien);
                sql_cmnd.Parameters.AddWithValue("@idcamera", SqlDbType.Int).Value = int.Parse(idcamera);
                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }

        }
        #endregion
        public override List<tblPhienLamViec> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public List<tblPhienLamViec> ListByPhongBanId(int phongbanid)
        {
            try
            {
                return Context.tblPhienLamViecs.Where(x => x.PhongBanID == phongbanid).OrderByDescending(x => x.Id).ToList();
            }
            catch { return null; }
        }

        public bool KiemTraTrung(string DiaDiem, DateTime TGBD, string NoiDung)
        {
            try
            {
                //return Context.tblPhienLamViecs.Count(x => x.DiaDiem.Trim().Contains(DiaDiem.Trim())
                //&& x.GioBd == TGBD && x.NoiDung.Trim().Contains(NoiDung)) > 0 ? true : false;

                return Context.tblPhienLamViecs.Any(x => x.DiaDiem.Trim().Contains(DiaDiem.Trim())
                && x.GioBd == TGBD && x.NoiDung.Trim().Contains(NoiDung));
            }
            catch { return false; }
        }

        public bool KiemTraTrung2(DateTime ngayLamViec, int phongBanId, string DiaDiem, DateTime TGBD, string NoiDung, int? PhienLVID)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    if (PhienLVID != null && PhienLVID > 0)
                    {
                        return db.tblPhienLamViecs.Any(x => x.Id != PhienLVID && x.NgayLamViec == ngayLamViec.Date && x.PhongBanID == phongBanId && x.GioBd == TGBD && x.NoiDung.Trim().ToLower().Contains(NoiDung.ToLower()) && x.DiaDiem.Trim().ToLower().Contains(DiaDiem.Trim().ToLower()));
                    }
                    else
                    {
                        return db.tblPhienLamViecs.Any(x => x.NgayLamViec == ngayLamViec.Date && x.PhongBanID == phongBanId && x.GioBd == TGBD && x.NoiDung.Trim().ToLower().Contains(NoiDung.ToLower()) && x.DiaDiem.Trim().ToLower().Contains(DiaDiem.Trim().ToLower()));
                    }
                }

            }
            catch { return false; }
            //finally
            //{
            //    Context.Dispose();
            //}
        }


        #region AutoAddDataInOneWeek
        public static void AutoAddDataInOneWeek(ref List<PhienLVModel> data, string type)
        {
            DateTime dts = new DateTime();
            DateTime dte = new DateTime();
            GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
            if (type == "TT")
            {
                dts = dts.AddDays(-7);
                dte = dte.AddDays(-7);
            }
            else if (type == "TK")
            {
                dts = dts.AddDays(7);
                dte = dte.AddDays(7);
            }

            var dates = Enumerable.Range(0, 1 + dte.Subtract(dts).Days).Select(offset => dts.AddDays(offset)).ToList();

            // duyệt qua tất cả các ngày trong tuần hiện tại
            foreach (var dt in dates)
            {
                // Nếu ngày này chưa được tạo lịch sẽ tự động thêm dòng trắng vào
                var plv = data.FirstOrDefault(o => o.NgayLamViec.Date == dt.Date);
                if (plv == null)
                {
                    data.Add(new PhienLVModel()
                    {
                        NgayLamViec = dt,//"CÔNG VIỆC THEO ĐĂNG KÝ KẾ HOẠCH"
                        TT_Phien = (int)TinhChatPhienLV2.CongViecKeHoach
                    });

                    data.Add(new PhienLVModel()
                    {
                        NgayLamViec = dt,
                        TT_Phien = (int)TinhChatPhienLV2.CongViecBoSung
                    });

                    data.Add(new PhienLVModel()
                    {
                        NgayLamViec = dt,
                        TT_Phien = (int)TinhChatPhienLV2.CongViecDotXuat
                    });
                }
                else
                {
                    // Nếu lịch này đã được tạo rồi nhưng chưa có mục công việc theo đăng ký kế hoạch thì tự động thêm dòng trắng vào
                    var plv2 = data.FirstOrDefault(o => o.TT_Phien == (int)TinhChatPhienLV2.CongViecKeHoach && o.NgayLamViec.Date == dt);
                    if (plv2 == null)
                    {
                        data.Add(new PhienLVModel()
                        {
                            NgayLamViec = dt,
                            TT_Phien = (int)TinhChatPhienLV2.CongViecKeHoach

                        });
                    }

                    var plv3 = data.FirstOrDefault(o => o.TT_Phien == (int)TinhChatPhienLV2.CongViecBoSung && o.NgayLamViec.Date == dt);
                    if (plv3 == null)
                    {
                        data.Add(new PhienLVModel()
                        {
                            NgayLamViec = dt,
                            TT_Phien = (int)TinhChatPhienLV2.CongViecBoSung

                        });
                    }

                    var plv4 = data.FirstOrDefault(o => o.TT_Phien == (int)TinhChatPhienLV2.CongViecDotXuat && o.NgayLamViec.Date == dt);
                    if (plv4 == null)
                    {
                        data.Add(new PhienLVModel()
                        {
                            NgayLamViec = dt,
                            TT_Phien = (int)TinhChatPhienLV2.CongViecDotXuat

                        });
                    }
                }
            }

            //sắp xếp lại theo ngày tạo
            data = data.OrderBy(o => o.NgayLamViec).ToList();


        }
        #endregion

        #region GetDayOfWeek
        public static string GetDayOfWeek(DateTime dt)
        {

            string kq = "";

            int dayI = ((int)dt.DayOfWeek);

            // Sun: 0 - Mon: 1 - Tue: 2 - Web: 3 - Thu: 4 - Fri: 5 - Sat: 6
            if (dayI == 1)
                kq = "Thứ 2, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 2)
                kq = "Thứ 3, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 3)
                kq = "Thứ 4, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 4)
                kq = "Thứ 5, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 5)
                kq = "Thứ 6, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 6)
                kq = "Thứ 7, ngày " + string.Format("{0:dd/MM/yyyy}", dt);
            else if (dayI == 0)
                kq = "Chủ nhật, ngày " + string.Format("{0:dd/MM/yyyy}", dt);

            return kq;
        }

        public static string GetDayOfWeekShort(DateTime dt)
        {

            string kq = "";

            int dayI = ((int)dt.DayOfWeek);

            // Sun: 0 - Mon: 1 - Tue: 2 - Web: 3 - Thu: 4 - Fri: 5 - Sat: 6
            if (dayI == 1)
                kq = "T2 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 2)
                kq = "T3 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 3)
                kq = "T4 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 4)
                kq = "T5 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 5)
                kq = "T6 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 6)
                kq = "T7 <br/> " + string.Format("{0:dd/MM}", dt);
            else if (dayI == 0)
                kq = "CN <br/> " + string.Format("{0:dd/MM}", dt);

            return kq;
        }
        #endregion

        public string GetTinhChatPhienNameById(int id)
        {
            try
            {
                return Context.plv_TinhChatPhien.Where(x => x.Id == id).FirstOrDefault().Name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region GetDateStartEnd_FromDateNow
        public static void GetDateStartEnd_FromDateNow(ref DateTime DateStart, ref DateTime DateEnd, DateTime date)
        {
            // Sun: 0 - Mon: 1 - Tue: 2 - Web: 3 - Thu: 4 - Fri: 5 - Sat: 6
            // lay tho igian tu thu 3 tuan nay den thu 2 tuan sau
            int dayI = ((int)date.DayOfWeek);

            if (dayI == 1)
            {
                DateStart = date;
                DateEnd = date.AddDays(6);
            }
            else if (dayI == 2)
            {
                DateStart = date.AddDays(-1);
                DateEnd = date.AddDays(5);
            }
            else if (dayI == 3)
            {
                DateStart = date.AddDays(-2);
                DateEnd = date.AddDays(4);
            }
            else if (dayI == 4)
            {
                DateStart = date.AddDays(-3);
                DateEnd = date.AddDays(3);
            }
            else if (dayI == 5)
            {
                DateStart = date.AddDays(-4);
                DateEnd = date.AddDays(2);
            }
            else if (dayI == 6)
            {
                DateStart = date.AddDays(-5);
                DateEnd = date.AddDays(1);
            }
            else if (dayI == 0)
            {
                DateStart = date.AddDays(-6);
                DateEnd = date.AddDays(7);
            }

        }
        public List<string> GetListWeekNow()
        {
            List<string> lstdate = new List<string>();
            DateTime today = DateTime.Today;
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime sunday = today.AddDays(-currentDayOfWeek);
            //DateTime tueday = sunday.AddDays(2);

            //if (currentDayOfWeek == 0)
            //{
            //    tueday = tueday.AddDays(-8);
            //}
            var dates = Enumerable.Range(1, 7).Select(days => sunday.AddDays(days)).ToList();
            foreach (var item in dates)
            {
                lstdate.Add(string.Format("{0:dd/MM/yyyy}", item));
            }

            return lstdate;
        }
        public override object Create(tblPhienLamViec entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override object Update(tblPhienLamViec entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();

                strError = "";

                return entity.Id;
            }
            catch (Exception ex)
            {
                strError = ex.Message;

                return 0;
            }
        }

        public override List<tblPhienLamViec> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetImageByPhienLVId
        public List<tblImage> GetImageByPhienLVId(int PhienLVId, string type)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    if (string.IsNullOrEmpty(type))
                        type = "";

                    return db.Database.SqlQuery<tblImage>("EXEC sp_Image_getbyPhienLVId @PhienLVId,@Type",
                   new SqlParameter("@PhienLVId", PhienLVId.ToString()),
                   new SqlParameter("@Type", type)).ToList();
                }


            }
            catch (Exception ex) { return null; }
        }
        #endregion

        #region ConvertSoSangLaMa
        public string ConvertSoSangLaMa(int so)
        {
            try
            {
                string strRet = string.Empty;
                decimal _Number = so;
                Boolean _Flag = true;
                string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
                int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
                int i = 0;
                while (_Flag)
                {
                    while (_Number >= ArrNumber[i])
                    {
                        _Number -= ArrNumber[i];
                        strRet += ArrLama[i];
                        if (_Number < 1)
                            _Flag = false;
                    }
                    i++;
                }
                return strRet;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region Get_KeHoachTuan
        public IEnumerable<Get_KeHoachTuan_Result> Get_KeHoachTuan(string DonViID)
        {
            try
            {

                var datacache = MemoryCacheHelper.GetValue("Get_KeHoachTuan_" + DonViID);
                if (datacache == null)
                {
                    //int donviId = 0;
                    //try
                    //{
                    //    donviId = int.Parse(DonViID);
                    //}
                    //catch { }

                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    var kq = Context.Get_KeHoachTuan(dts, dte, DonViID).ToList();

                    MemoryCacheHelper.Add("Get_KeHoachTuan_" + DonViID, kq, DateTimeOffset.UtcNow.AddDays(1));
                    return kq;
                }
                else
                    return (IEnumerable<Get_KeHoachTuan_Result>)datacache;
            }
            catch (Exception ex)
            { return null; }
        }

        #endregion

        public int CountImagePLV(int phienLv)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    return db.tblImages.Count(x => x.PhienLamViecId == phienLv && x.IsDelete == false);
                }
            }
            catch { return 0; }
        }

        public List<tblPhienLamViec> GetListPhienLVByDateRangeAndNhanVienId(string nhanVienId, string type)
        {
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                GetDateStartEnd_FromDateNow(ref startDate, ref endDate, DateTime.Now);

                if (type == "TT")
                {
                    startDate = startDate.AddDays(-7);
                    endDate = endDate.AddDays(-7);
                }
                else if (type == "TK")
                {
                    startDate = startDate.AddDays(7);
                    endDate = endDate.AddDays(7);
                }

                return Context.tbl_NhanVien_PhienLamViec.Where(x => x.NhanVienId == nhanVienId)
                                .Join(Context.tblPhienLamViecs.Where(x => x.NgayLamViec >= startDate && x.NgayLamViec <= endDate), x => x.PhienLamViecId, y => y.Id, (x, y) => y)
                                .ToList();
            }
            catch (Exception ex)
            {
                return new List<tblPhienLamViec>();
            }
        }

        public List<tblPhienLamViec> GetListPhienLVByDateRangeAndNhanVienIdSearch(string nhanVienId, string startDate, string endDate)
        {
            try
            {
                DateTime? beginDate;
                DateTime? stopDate;
                try
                {
                    beginDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    beginDate = null;
                }

                try
                {
                    stopDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    stopDate = null;
                }

                return Context.tbl_NhanVien_PhienLamViec.Where(x => x.NhanVienId == nhanVienId)
                                .Join(Context.tblPhienLamViecs.Where(x => (x.NgayLamViec >= beginDate || beginDate == null) && (x.NgayLamViec <= stopDate || stopDate == null)), x => x.PhienLamViecId, y => y.Id, (x, y) => y)
                                .ToList();
            }
            catch (Exception ex)
            {
                return new List<tblPhienLamViec>();
            }
        }

        public int UpdateKiemTraPLV(int plvId, ref string strError)
        {
            int kt = 0;
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    tblPhienLamViec phienlv = db.tblPhienLamViecs.FirstOrDefault(x => x.Id == plvId);
                    if (phienlv != null)
                    {
                        if (phienlv.IsKiemTra.HasValue)
                        {
                            if (!phienlv.IsKiemTra.Value)
                            {
                                phienlv.IsKiemTra = true;
                                phienlv.NgayGioKT = DateTime.Now;
                                db.SaveChanges();
                                kt = phienlv.Id;
                                strError = "";
                            }
                            else
                            {
                                strError = "Đã kiểm tra!";
                                return kt = 0;
                            }
                        }
                        else
                        {
                            phienlv.IsKiemTra = true;
                            phienlv.NgayGioKT = DateTime.Now;
                            db.SaveChanges();
                            kt = phienlv.Id;
                            strError = "";
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                kt = -1;
                strError = ex.Message;
            }
            return kt;
        }

        #region AdvancedSearchPhieuCongTac
        public IEnumerable<PhienLVModel> AdvancedSearchPhieuCongTac(int page, int pagelength, string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName, string loaiCV)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhieuCongTac @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Skip,@Take,@Duyet,@chuyenNPC,@phieuky, @role, @LoaiCongViec",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Skip", page.ToString()),
                       new SqlParameter("@Take", pagelength),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName),
                       new SqlParameter("@LoaiCongViec", loaiCV)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }
        #endregion

        #region CountTotalPhieuCongTac
        public int CountTotalPhieuCongTac(string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, int phieuky, string roleName, string loaiCV)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    var data = db.Database.SqlQuery<int>("EXEC sp_CountTotalPhieuCongTac @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@phieuky,@role,@LoaiCongViec",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@phieuky", phieuky),
                       new SqlParameter("@role", roleName),
                       new SqlParameter("@LoaiCongViec", loaiCV)).ToList();

                    if (data.Count > 0)
                        return int.Parse(data[0].ToString());
                    else return 0;
                }
            }
            catch { return 0; }
        }
        #endregion



        #region AdvancedSearchPhieuCongTac_His
        public IEnumerable<PhienLVModel> AdvancedSearchPhieuCongTac_His(int page, int pagelength, string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, string roleName, string loaiCV)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        dts = DateTime.Now;
                        dte = DateTime.Now;
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }


                    var x = db.Database.SqlQuery<PhienLVModel>("EXEC sp_AdvancedSearchPhieuCongTac_His @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Skip,@Take,@Duyet,@chuyenNPC,@role, @LoaiCongViec",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Skip", page.ToString()),
                       new SqlParameter("@Take", pagelength),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@role", roleName),
                       new SqlParameter("@LoaiCongViec", loaiCV)).ToList();

                    return x;
                }
            }
            catch (Exception ex)
            { return new List<PhienLVModel>(); }
        }


        public int CountTotalPhieuCongTac_His(string filter, int tcphien, int catdien, int tiepdia, int khac, string DateFrom, string DateTo, string DonViID, string PhongBanID, int Duyet, int chuyenNPC, string roleName, string loaiCV)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    string start1 = "";
                    string end1 = "";
                    if (string.IsNullOrEmpty(filter))
                        filter = "";
                    //if (string.IsNullOrEmpty(tcphien))
                    //    tcphien = "";
                    if (string.IsNullOrEmpty(DonViID))
                        DonViID = "";
                    if (string.IsNullOrEmpty(PhongBanID))
                        PhongBanID = "";
                    //if (string.IsNullOrEmpty(Duyet))
                    //    Duyet = "";

                    // neu khong loc theo thoi gian se mac dinh hien thi du lieu trong vong 1 tuan hien tai
                    if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                    {
                        DateTime dts = new DateTime();
                        DateTime dte = new DateTime();
                        //GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                    }
                    else
                    {
                        DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                        end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                    }

                    var data = db.Database.SqlQuery<int>("EXEC sp_CountTotalPhieuCongTac_His @filter,@TCPhien,@ListMaTT,@DonViID,@PhongBanID,@NgayBD,@NgayKT,@Duyet,@chuyenNPC,@role,@LoaiCongViec",
                       new SqlParameter("@filter", filter),
                       new SqlParameter("@TCPhien", tcphien),
                       new SqlParameter("@ListMaTT", catdien + "," + tiepdia + "," + khac),
                       new SqlParameter("@DonViID", DonViID),
                       new SqlParameter("@PhongBanID", PhongBanID),
                       new SqlParameter("@NgayBD", start1),
                       new SqlParameter("@NgayKT", end1),
                       new SqlParameter("@Duyet", Duyet),
                       new SqlParameter("@chuyenNPC", chuyenNPC),
                       new SqlParameter("@role", roleName),
                       new SqlParameter("@LoaiCongViec", loaiCV)).ToList();

                    if (data.Count > 0)
                        return int.Parse(data[0].ToString());
                    else return 0;
                }
            }
            catch { return 0; }
        }
        #endregion

        #region Get plv theo ngày
        //public List<tblNhanVien> Get_Plv_ChuaKiemTra_TheoNgay(string NgayLamViec)
        //{
        //    try
        //    {
        //        using (var connectionDB = new SqlConnection(connection))
        //        {
        //            await connectionDB.OpenAsync();

        //            var sql = @"SELECT p.* 
        //                FROM tblPhienLamViec p 
        //                LEFT JOIN plv_KeHoachLichLamViec k 
        //                ON p.Id = k.PhienLamViecId
        //                WHERE p.NgayLamViec = @NgayLamViec
        //                AND k.PhienLamViecId IS NULL
        //                ORDER BY p.Id DESC;";

        //            var result = await connectionDB.QueryAsync<PhienLVModel>(sql, new { NgayLamViec });

        //            return result.ToList(); // Trả về danh sách nếu có kết quả
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Ghi log lỗi
        //        Console.WriteLine($"Lỗi khi thực hiện Get_Plv_ChuaKiemTra_TheoNgay: {ex.Message}");
        //        Console.WriteLine(ex.StackTrace);

        //        return new List<PhienLVModel>(); // Trả về danh sách rỗng khi gặp lỗi
        //    }
        //}
        public List<PhienLVModel> Get_Plv_ChuaKiemTra_TheoNgay(string NgayLamViec)
        {
            try
            {

                using (var db = new ECP_V2Entities())
                {
                    var result = db.Database.SqlQuery<PhienLVModel>(@"SELECT distinct p.* 
                        FROM tblPhienLamViec p 
                        LEFT JOIN plv_KeHoachLichLamViec k 
                        ON p.Id = k.PhienLamViecId
                        WHERE p.NgayLamViec = @NgayLamViec
                        AND (k.PhienLamViecId IS NULL or k.TrangThai=0)
                        ORDER BY p.Id DESC;",
                new SqlParameter("@NgayLamViec", NgayLamViec)).ToList();

                    return result;
                }
                //}
            }
            catch (Exception ex) { return null; }
        }

        #endregion


        #region Get_TongSoPhienLV_ByTrangThai
        public decimal Get_TongSoPhienLV_ByTrangThai_ByLoaiCongViec(string DateFrom, string DateTo, string LoaiCongViec, string TrangThai, string DonViId)
        {
            try
            {
                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetTongSoPhien_ByTrangThai_ByLoaiCongViec(@TuNgay,@DenNgay,@LoaiCongViec,@TrangThai,@DonViId)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            TuNgay = start1,
                            DenNgay = end1,
                            LoaiCongViec = LoaiCongViec,
                            TrangThai = TrangThai,
                            DonViId = DonViId
                        }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        public int CountImagePLVByNhom(int phienLv, int nhom)
        {
            try
            {
                using (var db = new ECP_V2Entities())
                {
                    return db.tblImages.Count(x => x.PhienLamViecId == phienLv && x.IsDelete == false && x.GroupId == nhom);
                }
            }
            catch { return 0; }
        }

        #region Get_TongSoPhienLV_ByTC_ByDonVi
        public decimal Get_TongSoPhienLV_ByTC_ByDonVi(string DateFrom, string DateTo, string TinhChat, string MaDV)
        {
            try
            {
                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetTongSoPhien_ByTinhChat_ByDonVi(@TuNgay,@DenNgay,@TinhChat,@MaDV)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { TuNgay = start1, DenNgay = end1, TinhChat = TinhChat, MaDV = MaDV }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        #region Get_TongSoPhienLV_ByTrangThai_ByDonVi
        public decimal Get_TongSoPhienLV_ByTrangThai_ByDonVi(string DateFrom, string DateTo, string TinhChat, string MaDV, string TrangThai)
        {
            try
            {
                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetTongSoPhien_ByTrangThai_ByDonVi(@TuNgay,@DenNgay,@TinhChat,@MaDV,@TrangThai)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { TuNgay = start1, DenNgay = end1, TinhChat = TinhChat, MaDV = MaDV, TrangThai = TrangThai }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        #region Get_TongSoHinhAnh_ByDonVi
        public decimal Get_TongSoHinhAnh_ByDonVi(string DateFrom, string DateTo, string TinhChat, string MaDV, string TrangThai)
        {
            try
            {
                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetTongSoHinhAnh_ByDonVi(@TuNgay,@DenNgay,@TinhChat,@MaDV,@TrangThai)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { TuNgay = start1, DenNgay = end1, TinhChat = TinhChat, MaDV = MaDV, TrangThai = TrangThai }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        #region Get_TongSoPhienDaKiemTra_ByDonVi
        public decimal Get_TongSoPhienDaKiemTra_ByDonVi(string DateFrom, string DateTo, string TinhChat, string MaDV, string TrangThai)
        {
            try
            {
                string start1 = "";
                string end1 = "";
                if (string.IsNullOrEmpty(DateFrom) && string.IsNullOrEmpty(DateTo))
                {
                    DateTime dts = new DateTime();
                    DateTime dte = new DateTime();
                    GetDateStartEnd_FromDateNow(ref dts, ref dte, DateTime.Now);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dts.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dte.Date);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                    end1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtt.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetTongSoPhienDaKiemTra_ByDonVi(@TuNgay,@DenNgay,@TinhChat,@MaDV,@TrangThai)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { TuNgay = start1, DenNgay = end1, TinhChat = TinhChat, MaDV = MaDV, TrangThai = TrangThai }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        #region Get_PLVChuaKiemTraSauBC110_ByDonVi
        public decimal Get_PLVChuaKiemTraSauBC110_ByDonVi(string Date, string MaDV)
        {
            try
            {
                string start1 = "";
                if (string.IsNullOrEmpty(Date))
                {
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetPLVChuaKiemTraSauBC110_ByDonVi(@Ngay,@MaDV)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { Ngay = start1, MaDV = MaDV }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

        #region Get_PLVDaKiemTraSauBC110_ByDonVi
        public decimal Get_PLVDaKiemTraSauBC110_ByDonVi(string Date, string MaDV)
        {
            try
            {
                string start1 = "";
                if (string.IsNullOrEmpty(Date))
                {
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                }
                else
                {
                    DateTime dtf = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    start1 = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtf.Date);
                }

                using (IDbConnection db = new SqlConnection(connection))
                {
                    string query = "select dbo.NPC_GetPLVDaKiemTraSauBC110_ByDonVi(@Ngay,@MaDV)";
                    using (var multipleresult = db.QueryMultiple(query,
                        new { Ngay = start1, MaDV = MaDV }))
                    {
                        return multipleresult.Read<decimal>().FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            { return 0; }
        }
        #endregion

    }

    public class PhienLVModel
    {
        public int IdPhieuCongTac { get; set; }
        public string SoPhieu { get; set; }
        public string TrangThaiPhieu { get; set; }
        public int MaLP { get; set; }
        public string NguoiDuyetPCT { get; set; }
        public DateTime? NgayDuyetPCT { get; set; }

        public int Id { get; set; }
        public int PhongBanID { get; set; }
        public string TenPhongBan { get; set; }
        public string NoiDung { get; set; }
        public string DiaDiem { get; set; }
        public System.DateTime NgayLamViec { get; set; }
        public System.DateTime GioBd { get; set; }
        public System.DateTime GioKt { get; set; }
        public string NguoiDuyet_SoPa { get; set; }
        public string NguoiChiHuy { get; set; }
        public string GiamSatVien { get; set; }
        public string NguoiKiemSoat { get; set; }
        public string NguoiKiemTraPhieu { get; set; }
        public string LanhDaoTrucBan { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public int TT_Phien { get; set; }
        public int TrangThai { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string TenDonVi { get; set; }
        public int ViTri { get; set; }
        public string LyDoThayDoi { get; set; }
        public string SDT { get; set; }
        public int? MaPCT { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayDuyetNPC { get; set; }
        public string NguoiDuyetNPC { get; set; }
        public string NguoiDuyet_SoPa_Id { get; set; }
        public string NguoiChiHuy_Id { get; set; }
        public string GiamSatVien_Id { get; set; }
        public string NguoiKiemSoat_Id { get; set; }
        public string NguoiKiemTraPhieu_Id { get; set; }
        public string LanhDaoTrucBan_Id { get; set; }
        public Nullable<System.DateTime> NgayKetThuc { get; set; }
        public Nullable<bool> IsKiemTra { get; set; }
        public Nullable<System.DateTime> NgayGioKT { get; set; }
        public string DanhSachNhanVien { get; set; }
        public decimal? KinhDo { get; set; }
        public decimal? ViDo { get; set; }
        //thong tin ban do
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string htmlWindow { get; set; }

        public string LanhDaoCongViec { get; set; }
        public string LanhDaoCongViec_Id { get; set; }
        public string NguoiCapPhieu { get; set; }
        public string NguoiCapPhieu_Id { get; set; }
        public string NguoiKetThuc { get; set; }
        public bool? IsEndByWeb { get; set; }
        public int LoaiPhieu { get; set; }
        public string MaYeuCauCRM { get; set; }
        public Nullable<System.DateTime> NgayCNPCT { get; set; }
        public string NguoiCNPCT { get; set; }

        public int? IsCamera { set; get; }

        // Tham chiếu bảng plv_KeHoachLichLamViec
        public int? KeHoachLV_Id { set; get; }
        public int? HinhThucKiemTra { set; get; }
        public string NguoiDaiDienKT { set; get; }
        public string NguoiDaiDienKT_Id { set; get; }
        public string LyDoHoanHuy_KHLLV { set; get; }
        public int? TrangThai_KHLLV { set; get; }
        public Boolean isShowBtnHoanHuy { set; get; }
    }
    public class PhienLVTuan
    {
        public int SL { get; set; }
        public System.DateTime NgayLamViec { get; set; }
    }
    public class EmailSend
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string SoDT { get; set; }
    }
    public enum TinhChatPhienLV2
    {
        CongViecBoSung = 1,
        CongViecKeHoach = 2,
        CongViecDotXuat = 3
    }

    public class TongHopBaoCaoCuoiNgayPhienLV
    {
        public int Id { get; set; }
        public int PhongBanID { get; set; }
        public string TenPhongBan { get; set; }
        public string NoiDung { get; set; }
        public string DiaDiem { get; set; }
        public System.DateTime NgayLamViec { get; set; }
        public System.DateTime GioBd { get; set; }
        public System.DateTime GioKt { get; set; }
        public string NguoiDuyet_SoPa { get; set; }
        public string NguoiChiHuy { get; set; }
        public string GiamSatVien { get; set; }
        public string NguoiKiemSoat { get; set; }
        public string NguoiKiemTraPhieu { get; set; }
        public string LanhDaoTrucBan { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public int TT_Phien { get; set; }
        public int TrangThai { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string TenDonVi { get; set; }
        public string DonViId { get; set; }
        public int ViTri { get; set; }
        public string LyDoThayDoi { get; set; }
        public string SDT { get; set; }
        public int? MaPCT { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayDuyetNPC { get; set; }
        public string NguoiDuyetNPC { get; set; }
        public string NguoiDuyet_SoPa_Id { get; set; }
        public string NguoiChiHuy_Id { get; set; }
        public string GiamSatVien_Id { get; set; }
        public string NguoiKiemSoat_Id { get; set; }
        public string NguoiKiemTraPhieu_Id { get; set; }
        public string LanhDaoTrucBan_Id { get; set; }
        public Nullable<System.DateTime> NgayKetThuc { get; set; }
    }

    public class BangTongHopLoaiCongViec
    {
        public string TenLoai { get; set; }
        public string TenNhom { get; set; }
        public Nullable<decimal> SLChuaThucHien { get; set; }
        public Nullable<decimal> SLDangThucHien { get; set; }
        public Nullable<decimal> SLDaThucHienXong { get; set; }
        public Nullable<decimal> SLHoanHuy { get; set; }
        public int? STTLoai { get; set; }
        public int? STT { get; set; }
    }

    public class KeHoachTuanV2
    {
        public string DviCha { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string SDT { get; set; }
        public Nullable<decimal> KH { get; set; }
        public Nullable<decimal> BS { get; set; }
        public Nullable<decimal> DX { get; set; }
        public Nullable<decimal> HoanThanh { get; set; }
        public Nullable<decimal> ChuaXong { get; set; }
        public Nullable<decimal> HuyBo { get; set; }
        public Nullable<decimal> HinhAnh { get; set; }
        public Nullable<decimal> PLVKiemTra { get; set; }
        public Nullable<decimal> PLVChuaKiemTraSauBC110 { get; set; }
        public Nullable<decimal> PLVDaKiemTraSauBC110 { get; set; }
    }

    public class Camera
    {
        public string CamId { get; set; }
        public string CamDesc { get; set; }
        public string MaDvql { get; set; }
    }

}
