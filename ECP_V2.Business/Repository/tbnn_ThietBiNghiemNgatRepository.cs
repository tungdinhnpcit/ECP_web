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
using System.Globalization;

namespace ECP_V2.Business.Repository
{
    public class tbnn_ThietBiNghiemNgatRepository : RepositoryBase<tbnn_ThietBiNghiemNgat>
    {
        public string Connectstr { get; set; }
        public tbnn_ThietBiNghiemNgatRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public tbnn_ThietBiNghiemNgatRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.tbnn_ThietBiNghiemNgat.SingleOrDefault(o => o.ID == id);
                Context.tbnn_SoTheoDoiTBNN.RemoveRange(Context.tbnn_SoTheoDoiTBNN.Where(x => x.MaTB.HasValue && x.MaTB.Value == id).ToList());
                Context.tbnn_ThietBiNghiemNgat.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(tbnn_ThietBiNghiemNgat entity, ref string strError)
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

        public override tbnn_ThietBiNghiemNgat GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var model = Context.tbnn_ThietBiNghiemNgat.SingleOrDefault(p => p.ID == id);
                if (model.MaLoai != null)
                {
                    var loai = Context.tbnn_LoaiThietBi.SingleOrDefault(x => x.ID == model.MaLoai);
                    model.TenThietBi = loai.TenLoai;
                }
                return model;
            }
            catch { return null; }
        }

        public override List<tbnn_ThietBiNghiemNgat> List()
        {
            try
            {
                return Context.tbnn_ThietBiNghiemNgat.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<tbnn_ThietBiNghiemNgat> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(tbnn_ThietBiNghiemNgat entity, ref string strError)
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

        public override List<tbnn_ThietBiNghiemNgat> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public int CountByMaHieu(string MaHieu)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(ID) from tbnn_ThietBiNghiemNgat where MaNhom=1 and MaHieu='" + MaHieu;
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return 0; }
        }

        public int CountByMaHieu_Edit(string MaHieu, string ID)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(ID) from tbnn_ThietBiNghiemNgat where MaNhom=1 and MaHieu='" + MaHieu + " and ID!= " + ID;
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return 0; }
        }

        public List<ThietBiNghiemNgatModel> ListPaging(int page, int pageSize, string filter, string TuNgay, string DenNgay,
            string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT
            )
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

            /*
             1. Hết hạn
             2. 15 ngày
             3. 30 ngày
             4. Chưa đến hạn
            */

            List<ThietBiNghiemNgatModel> lstData = new List<ThietBiNghiemNgatModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY tb.[ID]) AS RowNum " +
                        ",tb.ID,tb.MaHieu,tb.QuyTacDanhMa " +
                        ",tb.HangSanXuat " +
                        ",tb.NuocSanXuat " +
                        ",TenThietBi=(select TenLoai from tbnn_LoaiThietBi where ID=tb.MaLoai) " +
                        ",tb.NamSX,tb.NgayDuaVaoSuDung " +
                        ",TenDonViQuanLy=(select TenDonVi from tblDonVi where Id=tb.DonViQuanLyId) " +
                        ",TenPhongBan=(select TenPhongBan from tblPhongBan where Id=tb.PhongBanId) " +
                        ",tb.PhongBanID,tb.DonViQuanLyId,tb.DonViId,tb.NgayTao,tb.NguoiTao,tb.NgaySua,tb.NguoiSua,tb.HanKiemDinh,tb.TaiTrongThietKe, tb.TaiTrongChoPhep " +
                        ",NgayKiemTra=(select top(1) NgayKiemTra from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",GhiChu=(select top(1) GhiChu from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",NgayKiemTraTiepTheo= (select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",DonViKiemTra= (select top(1) DonViKiemTra from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",TenTT=(select Name from tbnn_TrangThai where ID=tb.MaTT) " +
                        ",TrangThai= " +
                        "( " +
                        "case when " +
                        "(DATEDIFF(d, (select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB = tb.ID order by ID desc), getdate()) > 0) " +
                        "then 1 " +
                        "else " +
                        "(case when(DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=15 " +
                        "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >=0)  " +
                        "then 2 " +
                        "else " +
                        "(case when(DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=45 " +
                        "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >15)  " +
                        "then 3 " +
                        "else " +
                        "(case when DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >45 " +
                        "then 4 else 5 " +
                        "end) " +
                        "end) " +
                        "end) " +
                        "end " +
                        ") " +
                        "from tbnn_ThietBiNghiemNgat tb " +
                        "where " +
                        "((tb.TenThietBi like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (tb.MaHieu like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) " +
                        "and (tb.DonViQuanLyId=@DonViId or @DonViId='') " +
                        "and (CONVERT(date,tb.NgayTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,tb.NgayTao) <= CONVERT(date,@DenNgay) or @DenNgay='') " +
                        "and isnull(tb.IsDelete,0)=0 " +
                        "and (tb.MaNhom=@MaNhom or @MaNhom='') " +
                        "and (tb.MaLoai=@MaLoai or @MaLoai='') " +
                        "and (tb.MaTT=@MaTT or @MaTT='') "
                        ;

                    // het han
                    if (TrangThai == "hh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc),getdate()) >0 " +
                            ") "
                            ;
                    }
                    //sap het han 15 ngay
                    else if (TrangThai == "shh15")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=15 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >=0 " +
                            ") "
                            ;
                    }
                    //sap het han 30 ngay
                    else if (TrangThai == "shh30")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=45 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >15 " +
                            ") "
                            ;
                    }
                    //chua het han
                    else if (TrangThai == "cdh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >45 " +
                            ") "
                            ;
                    }

                    //trang thai kiem dinh dat
                    if (TrangThaiKiemDinh == "1")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =1 " +
                            ") "
                            ;
                    }
                    //trang thai kiem dinh khong dat
                    else if (TrangThaiKiemDinh == "2")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =0 " +
                            ") "
                            ;
                    }
                    //ko co kiem dinh
                    else if (TrangThaiKiemDinh == "3")
                    {
                        query = query + "and ( " +
                            "(select count(ID) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID) =0 " +
                            ") "
                            ;
                    }


                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN ((@page-1)*@pageSize)+1 and @page*@pageSize " +
                        "order by TrangThai "
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            page = page,
                            pageSize = pageSize,
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            //PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            MaLoai = MaLoai,
                            MaNhom = MaNhom,
                            MaTT = MaTT
                        }))
                    {
                        var q = multipleresult.Read<ThietBiNghiemNgatModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging(string filter, string TuNgay, string DenNgay,
            string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT
            )
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
                        "from tbnn_ThietBiNghiemNgat tb " +
                        "where " +
                        "((tb.TenThietBi like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (tb.MaHieu like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) " +
                        "and (tb.DonViQuanLyId=@DonViId or @DonViId='') " +
                        "and (CONVERT(date,tb.NgayTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,tb.NgayTao) <= CONVERT(date,@DenNgay) or @DenNgay='') " +
                        "and isnull(tb.IsDelete,0)=0 " +
                        "and (tb.MaNhom=@MaNhom or @MaNhom='') " +
                        "and (tb.MaLoai=@MaLoai or @MaLoai='') " +
                        "and (tb.MaTT=@MaTT or @MaTT='') "
                        ;

                    // het han
                    if (TrangThai == "hh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc),getdate()) >0 " +
                            ") "
                            ;
                    }
                    //sap het han 15 ngay
                    else if (TrangThai == "shh15")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=15 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >=0 " +
                            ") "
                            ;
                    }
                    //sap het han 30 ngay
                    else if (TrangThai == "shh30")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=45 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >15 " +
                            ") "
                            ;
                    }
                    //chua het han
                    else if (TrangThai == "cdh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >45 " +
                            ") "
                            ;
                    }

                    //trang thai kiem dinh dat
                    if (TrangThaiKiemDinh == "1")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =1 " +
                            ") "
                            ;
                    }
                    //trang thai kiem dinh khong dat
                    else if (TrangThaiKiemDinh == "2")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =0 " +
                            ") "
                            ;
                    }
                    //ko co kiem dinh
                    else if (TrangThaiKiemDinh == "3")
                    {
                        query = query + "and ( " +
                            "(select count(ID) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID) =0 " +
                            ") "
                            ;
                    }


                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            DonViId = DonViId,
                            MaLoai = MaLoai,
                            MaNhom = MaNhom,
                            MaTT = MaTT
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
                var entity = Context.tbnn_ThietBiNghiemNgat.Where(o => entityId.ToList().Contains(o.ID.ToString()));
                Context.tbnn_ThietBiNghiemNgat.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public string NextID(int nextID, string prefixID, string strnumber)
        {
            int lengthNumerID = strnumber.Length - nextID.ToString().Length;
            string zeroNumber = "";
            for (int j = 1; j <= lengthNumerID; j++)
            {
                zeroNumber += "0";
            }
            return prefixID + zeroNumber + nextID.ToString();
        }

        public string DinhDang(string QuyTacDanhMa, int nextID)
        {
            string str = "";
            try
            {
                string chuoi = QuyTacDanhMa.Substring(QuyTacDanhMa.IndexOf('{'), QuyTacDanhMa.IndexOf('}') - QuyTacDanhMa.IndexOf('{') + 1);
                string chuoi1 = QuyTacDanhMa.Replace(chuoi, "");
                string chuoi2 = QuyTacDanhMa.Substring(QuyTacDanhMa.IndexOf('{'), QuyTacDanhMa.IndexOf('}') - QuyTacDanhMa.IndexOf('{') + 1);
                str = NextID(nextID, chuoi1, chuoi2.Replace("{", "").Replace("}", ""));
            }
            catch (Exception ex)
            { }
            return str;
        }

        public string GetAutoMaPhieu(string QuyTacDanhMa)
        {
            int i = 1;
            string tmp = DinhDang(QuyTacDanhMa, Count() + i);
            while (CountByMaPhieu(tmp.ToString()) > 0)
            {
                i++;
                tmp = DinhDang(QuyTacDanhMa, Count() + i);
            }
            return tmp.ToString();

        }

        public int Count()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(tb.ID) from tbnn_ThietBiNghiemNgat tb " +
                        "where tb.MaNhom=1 ";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return 0; }
        }

        public int CountByMaPhieu(string MaHieu)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select count(ID) from tbnn_ThietBiNghiemNgat tb " +
                        "where MaNhom=1 and MaHieu='" + MaHieu + "' ";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return 0; }
        }

        public List<BieuDoTronModel> GetListBieuDoTron(string donviId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";
                    if (string.IsNullOrEmpty(donviId))
                        query = "select Id,TenDonVi from tblDonVi";
                    else
                        query = "select Id,TenDonVi from tblDonVi where Id='" + donviId + "'";
                    var q = db.Query<BieuDoTronModel>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<BieuDoTronModel>(); }
        }

        public List<ThietBiNghiemNgatModel> Export(string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string TrangThai, string MaLoai, string TrangThaiKiemDinh, string MaNhom, string MaTT
           )
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

            /*
             1. Hết hạn
             2. 15 ngày
             3. 30 ngày
             4. Chưa đến hạn
            */

            List<ThietBiNghiemNgatModel> lstData = new List<ThietBiNghiemNgatModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query =
                        "select tb.ID,tb.MaHieu,tb.QuyTacDanhMa " +
                        ",tb.HangSanXuat " +
                        ",tb.NuocSanXuat " +
                        ",TenThietBi=(select TenLoai from tbnn_LoaiThietBi where ID=tb.MaLoai) " +
                        ",tb.NamSX,tb.NgayDuaVaoSuDung " +
                        ",TenDonViQuanLy=(select TenDonVi from tblDonVi where Id=tb.DonViQuanLyId) " +
                         ",TenPhongBan=(select TenPhongBan from tblPhongBan where Id=tb.PhongBanId) " +
                        ",tb.PhongBanID,tb.DonViQuanLyId,tb.DonViId,tb.NgayTao,tb.NguoiTao,tb.NgaySua,tb.NguoiSua,tb.HanKiemDinh,tb.TaiTrongThietKe, tb.TaiTrongChoPhep " +
                        ",NgayKiemTra=(select top(1) NgayKiemTra from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",GhiChu=(select top(1) GhiChu from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",NgayKiemTraTiepTheo= (select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",DonViKiemTra= (select top(1) DonViKiemTra from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) " +
                        ",TenTT=(select Name from tbnn_TrangThai where ID=tb.MaTT) " +
                        ",TrangThai= " +
                        "( " +
                        "case when " +
                        "(DATEDIFF(d, (select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB = tb.ID order by ID desc), getdate()) > 0) " +
                        "then 1 " +
                        "else " +
                        "(case when(DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=15 " +
                        "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >=0)  " +
                        "then 2 " +
                        "else " +
                        "(case when(DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=45 " +
                        "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >15)  " +
                        "then 3 " +
                        "else " +
                        "(case when DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >45 " +
                        "then 4 else 5 " +
                        "end) " +
                        "end) " +
                        "end) " +
                        "end " +
                        ") " +
                        "from tbnn_ThietBiNghiemNgat tb " +
                        "where " +
                        "((tb.TenThietBi like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "or (tb.MaHieu like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'')) " +
                        "and (tb.DonViQuanLyId=@DonViId or @DonViId='') " +
                        "and (CONVERT(date,tb.NgayTao) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,tb.NgayTao) <= CONVERT(date,@DenNgay) or @DenNgay='') " +
                        "and isnull(tb.IsDelete,0)=0 " +
                        "and (tb.MaNhom=@MaNhom or @MaNhom='') " +
                        "and (tb.MaLoai=@MaLoai or @MaLoai='') " +
                        "and (tb.MaTT=@MaTT or @MaTT='') "
                        ;

                    // het han
                    if (TrangThai == "hh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc),getdate()) >0 " +
                            ") "
                            ;
                    }
                    //sap het han 15 ngay
                    else if (TrangThai == "shh15")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=15 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >=0 " +
                            ") "
                            ;
                    }
                    //sap het han 30 ngay
                    else if (TrangThai == "shh30")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) <=45 " +
                            "and DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >15 " +
                            ") "
                            ;
                    }
                    //chua het han
                    else if (TrangThai == "cdh")
                    {
                        query = query + "and ( " +
                            "DATEDIFF(d,getdate(),(select top(1) DATEADD(month, tb.HanKiemDinh, NgayKiemTra) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc)) >45 " +
                            ") "
                            ;
                    }

                    //trang thai kiem dinh dat
                    if (TrangThaiKiemDinh == "1")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =1 " +
                            ") "
                            ;
                    }
                    //trang thai kiem dinh khong dat
                    else if (TrangThaiKiemDinh == "2")
                    {
                        query = query + "and ( " +
                            "(select top(1) isnull(KetQua,0) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID order by ID desc) =0 " +
                            ") "
                            ;
                    }
                    //ko co kiem dinh
                    else if (TrangThaiKiemDinh == "3")
                    {
                        query = query + "and ( " +
                            "(select count(ID) from tbnn_SoTheoDoiTBNN where MaTB=tb.ID) =0 " +
                            ") "
                            ;
                    }


                    query = query +
                        "order by TrangThai "
                        ;

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            //PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            MaLoai = MaLoai,
                            MaNhom = MaNhom,
                            MaTT = MaTT
                        }))
                    {
                        var q = multipleresult.Read<ThietBiNghiemNgatModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

    }

    public class ThietBiNghiemNgatModel
    {
        public int ID { get; set; }
        public string TenThietBi { get; set; }
        public string MaHieu { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public Nullable<int> NamSX { get; set; }
        public Nullable<System.DateTime> NgayDuaVaoSuDung { get; set; }
        public Nullable<int> PhongBanID { get; set; }
        public string DonViId { get; set; }
        public string TenDonViQuanLy { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<int> HanKiemDinh { get; set; }
        public string QuyTacDanhMa { get; set; }
        public string TenTT { get; set; }
        public string SoCheTao { get; set; }
        public string SoDangKy { get; set; }
        public Nullable<decimal> TaiTrongThietKe { get; set; }
        public Nullable<decimal> TaiTrongChoPhep { get; set; }
        public string TenPhongBan { get; set; }

        //thong tin kiem dinh
        public Nullable<System.DateTime> NgayKiemTra { get; set; }
        public Nullable<System.DateTime> NgayKiemTraTiepTheo { get; set; }
        public string GhiChu { get; set; }
        public string DonViKiemTra { get; set; }

        public int TrangThai { get; set; }
    }

    public class tbnn_BieuDoTronModel
    {
        public string Id { get; set; }
        public string TenDonVi { get; set; }
        public int SLChuaDenHan { get; set; }
        public int SLDenHan30Ngay { get; set; }
        public int SLDenHan15Ngay { get; set; }
        public int SLQuaHan { get; set; }
    }
}
