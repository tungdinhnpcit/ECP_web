using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECP_V2.Business.Repository
{

    public class sc_TaiNanSuCoRepository : RepositoryBase<sc_TaiNanSuCo>
    {
        public string Connectstr { get; set; }
        public sc_TaiNanSuCoRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
        }

        public sc_TaiNanSuCoRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override object Create(sc_TaiNanSuCo entity, ref string strError)
        {
            try
            {
                Context.sc_TaiNanSuCo.Add(entity);
                //Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();

                return entity.Id;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }

                return 0;
            }
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.sc_TaiNanSuCo.SingleOrDefault(o => o.Id == id);
                Context.sc_TaiNanSuCo.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override sc_TaiNanSuCo GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.sc_TaiNanSuCo.SingleOrDefault(p => p.Id == id);
            }
            catch (Exception ex){ return null; }
        }

        public override List<sc_TaiNanSuCo> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public override List<sc_TaiNanSuCo> List()
        {
            try
            {
                return Context.sc_TaiNanSuCo.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<sc_TaiNanSuCo> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(sc_TaiNanSuCo entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SuCoModel> ListPaging(int page, int pageSize, string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string NguyenNhan, string TrangThaiNhap,
           string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            List<SuCoModel> lstData = new List<SuCoModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY a.[Id]) AS RowNum " +
                        ",a.Id, a.DonViId,a.CapDienAp,a.TomTat,a.TinhTrangBienBan,a.HinhAnhSuCo,a.ThoiGianXuatHien" +
                        ",a.ThoiGianBatDauKhacPhuc,a.ThoiGianKhacPhucXong,a.ThoiGianKhoiPhuc,a.T_XuatHienBatDauKhacPhuc" +
                        ",a.T_BatDauDenKhacPhucXong,a.T_KhacPhucXongDenKhoiPhuc,a.T_TongThoiGianMatDien,a.NgayTao,a.NguoiTao,a.PhieuCongTacId " +
                        ",LoaiSuCo =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.LoaiSuCoId) " +
                        ",LyDo =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.LyDoId) " +
                        ",NguyenNhan =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.NguyenNhanId) " +
                        ",TinhChat =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.TinhChatId) " +
                        ",TenDvi =(select TenDonVi from tblDonVi where Id = a.DonViId) " +
                        ",a.TrangThai, a.NgayDuyet, a.NguoiDuyet, a.NgaySua, a.NguoiSua, a.TenThietBi,a.DienBienSuCo,a.IsGianDoan, a.IsTaiSan " +
                        ",TrangThaiNhap=case when DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 then N'Trễ' else N'' end " +
                        ",a.IsChuyenNPC,a.NgayDuyetNPC,a.NguoiDuyetNPC, a.IsMienTru " +
                        ",a.NPCIsDuyetMT,a.NPCNgayDuyetMT,a.NPCNguoiDuyetMT,a.NPCTenNguoiDuyetMT,a.NPCCommentMT " +
                        ",NguoiKienNghi=(select top(1) NguoiTao from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",NgayKienNghi=(select top(1) NgayTao from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",NoiDungKienNghi=(select top(1) NoiDung from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",a.KienNghiId " +
                        ",lstDonViSuCoId=((SELECT  STUFF ((SELECT ',' + Rtrim( c.TenDV ) FROM sc_TaiNanSuCo_DonVi c WHERE c.SuCoId = a.Id FOR XML PATH ('')), 1,1 ,''))) " +
                        ",a.ThoiTiet,a.GhiChu, a.MaTbiSco,a.TenTbiSco,a.MaTbiTdong,a.TenTbiTdong,a.LoaiTB,a.HanhLangId,a.ThoiTietId, a.MaThietBi " + 
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (a.DonViId=@DonViId " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        //"or @DonViId in (select c.DonViId from tblNhanVien c where c.Username=a.NguoiTao) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiTB=@TinhChat ";

                    }

                    if (LyDo == "all" || string.IsNullOrEmpty(LyDo))
                    {

                    }
                    else
                    {
                        query = query + "and a.LyDoId=@LyDo ";
                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }
                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN((@page - 1) * @pageSize) + 1 and @page*@pageSize "
                        ;
                    #endregion


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
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            LyDo = LyDo,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        var q = multipleresult.Read<SuCoModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return lstData;
        }

        public int CountListPaging(string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string TrangThaiNhap,
           string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, bool IsThongKe, string TrangThaiChuyenNPC, string NguyenNhan)
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
                    string query = "";

                    #region query1
                    query =
                        "select Count(Id) " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    //if (IsThongKe)
                    //{
                        query = query + " and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        "or @DonViId='') ";
                    //}
                    //else
                    //{
                    //    query = query + " and (a.DonViId=@DonViId " +
                    //    "or @DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                    //    "or @DonViId in (select c.DonViId from tblNhanVien c where c.Username=a.NguoiTao) " +
                    //    "or @DonViId='') ";
                    //}

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (LyDo == "all" || string.IsNullOrEmpty(LyDo))
                    {

                    }
                    else
                    {
                        query = query + "and a.LyDoId=@LyDo ";
                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            LyDo = LyDo,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
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



        public List<SuCoModel> ListPaging_His(int page, int pageSize, string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
           string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            List<SuCoModel> lstData = new List<SuCoModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select * from ( " +
                        "select ROW_NUMBER() OVER (ORDER BY a.[Id]) AS RowNum " +
                        ",a.Id, a.DonViId,a.CapDienAp,a.TomTat,a.TinhTrangBienBan,a.HinhAnhSuCo,a.ThoiGianXuatHien" +
                        ",a.ThoiGianBatDauKhacPhuc,a.ThoiGianKhacPhucXong,a.ThoiGianKhoiPhuc,a.T_XuatHienBatDauKhacPhuc" +
                        ",a.T_BatDauDenKhacPhucXong,a.T_KhacPhucXongDenKhoiPhuc,a.T_TongThoiGianMatDien,a.NgayTao,a.NguoiTao,a.PhieuCongTacId " +
                        ",LoaiSuCo =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.LoaiSuCoId) " +
                        ",NguyenNhan =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.NguyenNhanId) " +
                        ",TinhChat =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.TinhChatId) " +
                        ",TenDvi =(select TenDonVi from tblDonVi where Id = a.DonViId) " +
                        ",a.TrangThai, a.NgayDuyet, a.NguoiDuyet, a.NgaySua, a.NguoiSua, a.TenThietBi,a.DienBienSuCo,a.IsGianDoan, a.IsTaiSan " +
                        ",TrangThaiNhap=case when DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 then N'Trễ' else N'' end " +
                        ",a.IsChuyenNPC,a.NgayDuyetNPC,a.NguoiDuyetNPC, a.IsMienTru " +
                        ",a.NPCIsDuyetMT,a.NPCNgayDuyetMT,a.NPCNguoiDuyetMT,a.NPCTenNguoiDuyetMT,a.NPCCommentMT " +
                        ",NguoiKienNghi=(select top(1) NguoiTao from sc_KienNghiMienTru_His where SuCoId=a.Id order by Id desc) " +
                        ",NgayKienNghi=(select top(1) NgayTao from sc_KienNghiMienTru_His where SuCoId=a.Id order by Id desc) " +
                        ",NoiDungKienNghi=(select top(1) NoiDung from sc_KienNghiMienTru_His where SuCoId=a.Id order by Id desc) " +
                        ",a.KienNghiId " +
                        ",lstDonViSuCoId=((SELECT  STUFF ((SELECT ',' + Rtrim( c.TenDV ) FROM sc_TaiNanSuCo_DonVi_His c WHERE c.SuCoId = a.Id FOR XML PATH ('')), 1,1 ,''))) " +
                        "from sc_TaiNanSuCo_His a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (a.DonViId=@DonViId " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi_His c where c.SuCoId = a.Id) " +
                        //"or @DonViId in (select c.DonViId from tblNhanVien c where c.Username=a.NguoiTao) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }
                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    query = query +
                        ") as kq " +
                        "where RowNum BETWEEN((@page - 1) * @pageSize) + 1 and @page*@pageSize "
                        ;
                    #endregion


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
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        var q = multipleresult.Read<SuCoModel>();
                        lstData = q.ToList();
                    }
                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public int CountListPaging_His(string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
           string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, bool IsThongKe, string TrangThaiChuyenNPC)
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
                    string query = "";

                    #region query1
                    query =
                        "select Count(Id) " +
                        "from sc_TaiNanSuCo_His a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    //if (IsThongKe)
                    //{
                    query = query + " and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi_His c where c.SuCoId = a.Id) " +
                    "or @DonViId='') ";
                    //}
                    //else
                    //{
                    //    query = query + " and (a.DonViId=@DonViId " +
                    //    "or @DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi_His c where c.SuCoId = a.Id) " +
                    //    "or @DonViId in (select c.DonViId from tblNhanVien c where c.Username=a.NguoiTao) " +
                    //    "or @DonViId='') ";
                    //}

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
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

        public decimal SumXuatHienBatDauKhacPhuc(string filter, string TuNgay, string DenNgay,
          string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
          string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            decimal count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select isnull(sum(isnull(a.T_XuatHienBatDauKhacPhuc,0)),0) " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<decimal>();
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

        public decimal SumBatDauDenKhacPhucXong(string filter, string TuNgay, string DenNgay,
          string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
          string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            decimal count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select isnull(sum(isnull(a.T_BatDauDenKhacPhucXong,0)),0) " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<decimal>();
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

        public decimal SumKhacPhucXongDenKhoiPhuc(string filter, string TuNgay, string DenNgay,
          string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string NguyenNhan, string TrangThaiNhap,
          string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            decimal count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select isnull(sum(isnull(a.T_KhacPhucXongDenKhoiPhuc,0)),0) " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<decimal>();
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

        public decimal SumTongThoiGianMatDien(string filter, string TuNgay, string DenNgay,
          string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string TrangThaiNhap,
          string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC, string NguyenNhan)
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

            decimal count = 0;
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        //"select T_TongThoiGianMatDien=isnull(sum(isnull(a.T_XuatHienBatDauKhacPhuc,0)),0)+isnull(sum(isnull(a.T_BatDauDenKhacPhucXong,0)),0)+isnull(sum(isnull(a.T_KhacPhucXongDenKhoiPhuc,0)),0) " +
                        "select T_TongThoiGianMatDien=isnull(sum(isnull(a.T_TongThoiGianMatDien,0)),0) " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (LyDo == "all" || string.IsNullOrEmpty(LyDo))
                    {

                    }
                    else
                    {
                        query = query + "and a.LyDoId=@LyDo ";
                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }

                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion

                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            LyDo = LyDo,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        try
                        {
                            var q = multipleresult.Read<decimal>();
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
                var entity = Context.sc_TaiNanSuCo.Where(o => entityId.ToList().Contains(o.Id.ToString()));
                Context.sc_TaiNanSuCo.RemoveRange(entity);

                var listTB = Context.sc_ThietBiSuCo.Where(x => entityId.ToList().Contains(x.SuCoId.ToString())).ToList();
                Context.sc_ThietBiSuCo.RemoveRange(listTB);

                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public string DuyetAll(string[] entityId, string nguoiDuyet, ref string strError)
        {
            try
            {
                var entity = Context.sc_TaiNanSuCo.Where(o => entityId.ToList().Contains(o.Id.ToString()));
                foreach (var item in entity)
                {
                    item.NgayDuyet = DateTime.Now;
                    item.NguoiDuyet = nguoiDuyet;
                    item.TrangThai = 2;
                }
                //Context.sc_TaiNanSuCo.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public List<SuCoModel> Export(string filter, string TuNgay, string DenNgay,
           string DonViId, string PhongBanId, string LoaiSuCo, string TinhChat, string LyDo, string NguyenNhan, string TrangThaiNhap,
           string MienTru, string KienNghi, string TCTDuyetMT, string CapDienAp, string LoaiTaiSan, string TrangThaiChuyenNPC)
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

            List<SuCoModel> lstData = new List<SuCoModel>();
            try
            {

                using (SqlConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";

                    #region query1
                    query =
                        "select ROW_NUMBER() OVER (ORDER BY a.[Id]) AS RowNum " +
                        ",a.Id, a.DonViId,a.CapDienAp,a.TomTat,a.TinhTrangBienBan,a.HinhAnhSuCo,a.ThoiGianXuatHien" +
                        ",a.ThoiGianBatDauKhacPhuc,a.ThoiGianKhacPhucXong,a.ThoiGianKhoiPhuc,a.T_XuatHienBatDauKhacPhuc" +
                        ",a.T_BatDauDenKhacPhucXong,a.T_KhacPhucXongDenKhoiPhuc,a.T_TongThoiGianMatDien,a.NgayTao,a.NguoiTao,a.PhieuCongTacId " +
                        ",LoaiSuCo =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.LoaiSuCoId) " +
                        ",LyDo =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.LyDoId) " +
                        ",NguyenNhan =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.NguyenNhanId) " +
                        ",TinhChat =(select TenLoaiSuCo from sc_LoaiSuCo where Id = a.TinhChatId) " +
                        ",TenDvi =(select TenDonVi from tblDonVi where Id = a.DonViId) " +
                        ",a.TrangThai, a.NgayDuyet, a.NguoiDuyet, a.NgaySua, a.NguoiSua, a.TenThietBi,a.DienBienSuCo,a.IsGianDoan, a.IsTaiSan " +
                        ",BienBan=case when (select count(Id) from sc_TaiLieu where SuCoId=a.Id and TypeObj=1)>0 then N'Có' else N'Không' end " +
                        ",HinhAnh=case when (select count(Id) from sc_TaiLieu where SuCoId=a.Id and TypeObj=2)>0 then N'Có' else N'Không' end " +
                        ",TaiSan=case when isnull(a.IsTaiSan,0)=1 then N'Điện lực' else N'Khách hàng' end " +
                        ",TrangThaiNhap=case when DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 then N'Trễ' else N'' end " +
                        ",strMienTru=case when isnull(a.IsMienTru,0)=1 then N'Có' else N'Không' end " +
                        ",strNPCIsDuyetMT=case when a.NPCIsDuyetMT=1 then N'Có' else (case when a.NPCIsDuyetMT=0 then N'Không' else '' end) end " +
                        ",strNPCNgayDuyetMT=convert(varchar, a.NPCNgayDuyetMT, 103)+' '+convert(varchar(8), a.NPCNgayDuyetMT, 108) " +
                        ",a.NPCTenNguoiDuyetMT,a.NPCCommentMT " +
                        ",NguoiKienNghi=(select top(1) NguoiTao from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",NgayKienNghi=(select top(1) NgayTao from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",HinhAnhKienNghi=case when (select count(Id) from sc_KienNghiMienTru_TaiLieu where KienNghiId=a.KienNghiId)>0 then N'Có' else N'' end " +
                        ",NoiDungKienNghi=(select top(1) NoiDung from sc_KienNghiMienTru where SuCoId=a.Id order by Id desc) " +
                        ",lstDonViSuCoId=((SELECT  STUFF ((SELECT ',' + Rtrim( c.TenDV ) FROM sc_TaiNanSuCo_DonVi c WHERE c.SuCoId = a.Id FOR XML PATH ('')), 1,1 ,''))) " +
                        ",a.ThoiTiet,a.GhiChu " +
                        "from sc_TaiNanSuCo a " +
                        "where " +
                        "(a.TomTat like REPLACE(N'%'+(N'" + filter + "')+N'%',' ',N'%') or N'" + filter + "'=N'') " +
                        //"and (a.DonViId=@DonViId " +
                        "and (@DonViId in (select c.DonViId from sc_TaiNanSuCo_DonVi c where c.SuCoId = a.Id) " +
                        //"or @DonViId in (select c.DonViId from tblNhanVien c where c.Username=a.NguoiTao) " +
                        "or @DonViId='') " +
                        //"and (tb.PhongBanID=@PhongBanId or @PhongBanId='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) >= CONVERT(date,@TuNgay) or @TuNgay='') " +
                        "and (CONVERT(date,a.ThoiGianXuatHien) <= CONVERT(date,@DenNgay) or @DenNgay='') "
                        //"and (a.LoaiSuCoId=@LoaiSuCo or @LoaiSuCo='' or a.LoaiSuCoId is null)"
                        ;

                    if (LoaiSuCo == "all" || string.IsNullOrEmpty(LoaiSuCo))
                    {

                    }
                    else
                    {

                        query = query + "and a.LoaiSuCoId=@LoaiSuCo ";

                    }

                    if (TinhChat == "all" || string.IsNullOrEmpty(TinhChat))
                    {

                    }
                    else
                    {

                        query = query + "and a.TinhChatId=@TinhChat ";

                    }

                    if (LyDo == "all" || string.IsNullOrEmpty(LyDo))
                    {

                    }
                    else
                    {
                        query = query + "and a.LyDoId=@LyDo ";
                    }

                    if (NguyenNhan == "all" || string.IsNullOrEmpty(NguyenNhan))
                    {

                    }
                    else
                    {
                        query = query + "and a.NguyenNhanId=@NguyenNhan ";
                    }

                    if (MienTru == "all" || string.IsNullOrEmpty(MienTru))
                    {

                    }
                    else
                    {
                        if (bool.Parse(MienTru))
                            query = query + "and a.IsMienTru=1 ";
                        else
                            query = query + "and isnull(a.IsMienTru,0)=0 ";
                    }

                    if (KienNghi == "all" || string.IsNullOrEmpty(KienNghi))
                    {

                    }
                    else if (bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is not null ";
                    }
                    else if (!bool.Parse(KienNghi))
                    {
                        query = query + "and a.KienNghiId is null ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiNhap))
                    {
                        if (TrangThaiNhap == "tre")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)>24 ";
                        else if (TrangThaiNhap == "dunghan")
                            query = query + "and DATEDIFF(hour, a.ThoiGianXuatHien, a.NgayTao)<=24 ";
                    }

                    if (TCTDuyetMT == "all" || string.IsNullOrEmpty(TCTDuyetMT))
                    {

                    }
                    else if (bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=1 ";
                    }
                    else if (!bool.Parse(TCTDuyetMT))
                    {
                        query = query + "and a.NPCIsDuyetMT=0 ";
                    }

                    if (!string.IsNullOrEmpty(CapDienAp))
                    {
                        query = query + "and a.CapDienAp=@CapDienAp ";
                    }
                    if (!string.IsNullOrEmpty(LoaiTaiSan))
                    {
                        if (LoaiTaiSan == "dienluc")
                            query = query + "and isnull(a.IsTaiSan,0)=1 ";
                        else if (LoaiTaiSan == "khachhang")
                            query = query + "and isnull(a.IsTaiSan,0)=0 ";
                    }

                    if (!string.IsNullOrEmpty(TrangThaiChuyenNPC))
                    {
                        if (TrangThaiChuyenNPC == "dachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=1 ";
                        else if (TrangThaiChuyenNPC == "chuachuyen")
                            query = query + "and isnull(a.IsChuyenNPC,0)=0 ";
                    }

                    #endregion


                    using (var multipleresult = db.QueryMultiple(query,
                        new
                        {
                            filter = DonViId,
                            TuNgay = start1,
                            DenNgay = end1,
                            PhongBanId = PhongBanId,
                            DonViId = DonViId,
                            LoaiSuCo = LoaiSuCo,
                            TinhChat = TinhChat,
                            LyDo = LyDo,
                            NguyenNhan = NguyenNhan,
                            MienTru = MienTru,
                            CapDienAp = CapDienAp,
                        }))
                    {
                        var q = multipleresult.Read<SuCoModel>();
                        lstData = q.ToList();
                    }


                }
            }
            catch (Exception ex) { }
            return lstData;
        }

        public string ChuyenNPCAll(string[] entityId, string nguoiDuyet, out List<string> lstChuaDuyet, ref string strError)
        {
            lstChuaDuyet = new List<string>();
            try
            {
                var entity = Context.sc_TaiNanSuCo.Where(o => entityId.ToList().Contains(o.Id.ToString()));
                foreach (var item in entity)
                {
                    if (item.TrangThai == 2)
                    {
                        item.NgayDuyetNPC = DateTime.Now;
                        item.NguoiDuyetNPC = nguoiDuyet;
                        item.IsChuyenNPC = true;
                    }
                    else
                    {
                        lstChuaDuyet.Add(item.Id.ToString());
                    }

                }
                //Context.sc_TaiNanSuCo.RemoveRange(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return "error";
            }
        }

        public List<BieuDoTronSuCoModel> GetListBieuDoTron(string donviId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "";
                    if (string.IsNullOrEmpty(donviId))
                        query = "select Id,TenDonVi=case when SDT is not null then TenDonVi+'('+RTRIM(LTRIM(SDT))+')' else TenDonVi end from tblDonVi";
                    else
                        query = "select Id,TenDonVi=case when SDT is not null then TenDonVi+'('+RTRIM(LTRIM(SDT))+')' else TenDonVi end from tblDonVi where Id='" + donviId + "'";
                    var q = db.Query<BieuDoTronSuCoModel>(query);
                    return q.ToList();
                }
            }
            catch (Exception ex) { return new List<BieuDoTronSuCoModel>(); }
        }

    }

    public class SuCoModel
    {
        public int Id { get; set; }
        public string DonViId { get; set; }
        public string lstDonViSuCoId { get; set; }
        public string CapDienAp { get; set; }
        public string TenThietBi { get; set; }
        public string DienBienSuCo { get; set; }
        public string TomTat { get; set; }
        public Nullable<bool> TinhTrangBienBan { get; set; }
        public Nullable<bool> HinhAnhSuCo { get; set; }
        public Nullable<System.DateTime> ThoiGianXuatHien { get; set; }
        public Nullable<System.DateTime> ThoiGianBatDauKhacPhuc { get; set; }
        public Nullable<System.DateTime> ThoiGianKhacPhucXong { get; set; }
        public Nullable<System.DateTime> ThoiGianKhoiPhuc { get; set; }
        public Nullable<double> T_XuatHienBatDauKhacPhuc { get; set; }
        public Nullable<double> T_BatDauDenKhacPhucXong { get; set; }
        public Nullable<double> T_KhacPhucXongDenKhoiPhuc { get; set; }
        public Nullable<double> T_TongThoiGianMatDien { get; set; }
        public Nullable<bool> IsGianDoan { get; set; }
        public Nullable<int> PhieuCongTacId { get; set; }
        public Nullable<int> LoaiSuCoId { get; set; }
        public Nullable<int> NguyenNhanId { get; set; }
        public Nullable<int> LyDoId { get; set; }
        public Nullable<int> TinhChatId { get; set; }

        public Nullable<System.DateTime> NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string NguoiDuyet { get; set; }
        public Nullable<System.DateTime> NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public Nullable<decimal> KinhDo { get; set; }
        public Nullable<decimal> ViDo { get; set; }
        public Nullable<bool> IsTaiSan { get; set; }
        public Nullable<bool> IsChuyenNPC { get; set; }
        public Nullable<System.DateTime> NgayDuyetNPC { get; set; }
        public string NguoiDuyetNPC { get; set; }

        public string LoaiSuCo { get; set; }
        public string NguyenNhan { get; set; }
        public string LyDo { get; set; }
        public string TinhChat { get; set; }
        public string TenDvi { get; set; }

        public string BienBan { get; set; }
        public string HinhAnh { get; set; }
        public string TaiSan { get; set; }
        public string TrangThaiNhap { get; set; }

        public Nullable<bool> IsMienTru { get; set; }
        public string strMienTru { get; set; }
        public bool? NPCIsDuyetMT { get; set; }
        public string strNPCIsDuyetMT { get; set; }
        public DateTime? NPCNgayDuyetMT { get; set; }
        public string strNPCNgayDuyetMT { get; set; }
        public string NPCNguoiDuyetMT { get; set; }
        public string NPCTenNguoiDuyetMT { get; set; }
        public string NPCCommentMT { get; set; }

        public int? KienNghiId { get; set; }
        public DateTime? NgayKienNghi { get; set; }
        public string NguoiKienNghi { get; set; }
        public List<sc_KienNghiMienTru_TaiLieu> lstTLKN { get; set; }
        public string HinhAnhKienNghi { get; set; }
        public string NoiDungKienNghi { get; set; }
        public string ThoiTiet { get; set; }
        public string GhiChu { get; set; }
        public string TenTBiSco { get; set; }
        public string TenTbiTdong { get; set; }
        public bool HanhLang { get; set; }
        public bool ThienTai { get; set; }
    }

    public class ThongKeSuCo
    {
        public int ID { get; set; }
        public string TenLoai { get; set; }
        public int SoLuong { get; set; }
    }

    public class BieuDoTronSuCoModel
    {
        public string Id { get; set; }
        public string TenDonVi { get; set; }
        public int SLCap04KV { get; set; }
        public int SLCap6KV { get; set; }
        public int SLCap10KV { get; set; }
        public int SLCap22KV { get; set; }
        public int SLCap35KV { get; set; }
        public int SLCap110KV { get; set; }
        public decimal XuatHienBatDauKhacPhuc { get; set; }
        public decimal BatDauDenKhacPhucXong { get; set; }
        public decimal KhacPhucXongDenKhoiPhuc { get; set; }
        public decimal TongThoiGianMatDien { get; set; }
        public int SLThoangQua { get; set; }
        public int SLKeoDai { get; set; }
        public int SLLoaiKhongXacDinh { get; set; }
        public int SLTSDienLuc { get; set; }
        public int SLTSKhachHang { get; set; }
        public int SLKhachQuan { get; set; }
        public int SLChuQuan { get; set; }
        public int SLNguyenNhanKhongXacDinh { get; set; }
        public int SLHanhLang { get; set; }
        public int SLThietBi { get; set; }
        public int SLMayBienAp { get; set; }
        public int SLDuongDay { get; set; }
        public int SLChuaXacDinh { get; set; }
        public int SLThienTai { get; set; }
        public int SLTinhChatKhongXacDinh { get; set; }
    }
}
