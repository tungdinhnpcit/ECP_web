using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.PagingModel;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class ApprovePlanReponsitory : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public ApprovePlanReponsitory(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public ApprovePlanReponsitory()
            : base()
        {
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                Connectstr = connection.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public ApprovePlanReponsitory(WorkUnit unit)
            : base(unit)
        {
        }

        //public List<ApprovePlanViewModel> GetAssetByTeam(String madvql, String teamid, string thang, string nam, string nhomkt)
        public List<ApprovePlanViewModel> GetAssetByTeam(String madvql, String teamid, string thang, string nam)
        {
            List<ApprovePlanViewModel> appPlan = new List<ApprovePlanViewModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_get_asset_by_teamid", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));
                cmd.Parameters.Add(new SqlParameter("@teamid", teamid));
                cmd.Parameters.Add(new SqlParameter("@thang", thang));
                cmd.Parameters.Add(new SqlParameter("@nam", nam));
                //cmd.Parameters.Add(new SqlParameter("@nhomkt", nhomkt));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ApprovePlanViewModel a = new ApprovePlanViewModel();

                    a.Id = int.Parse(rdr["id"].ToString());
                    a.AssetId = rdr["assetid"].ToString();
                    a.AssetDesc = rdr["assetdesc"].ToString();
                    a.IdLoaiKt = rdr["ID_LOAIKTR"].ToString();
                    a.TenLoaiKt = rdr["TEN_LOAIKTR"].ToString();
                    a.MaLoaiKtra = rdr["MA_LOAI_KTR"].ToString();
                    a.SlDangcho =int.Parse(rdr["sl_dangcho"].ToString());
                    a.SlDath = int.Parse(rdr["sl_dath"].ToString());

                    appPlan.Add(a);
                }
            }

            return appPlan;
        }

        //Lấy danh sách combobox loại kieemtra tra
        public List<LoaiKtra> GetLoaiKtras(string type)
        {
            List<LoaiKtra> loaiKtras = new List<LoaiKtra>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_get_loai_ktras", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@type", type));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoaiKtra a = new LoaiKtra();

                    a.id_loaiktr = rdr["id_loaiktr"].ToString();
                    a.ten_loaiktr = rdr["ten_loaiktr"].ToString();
                    a.typeid  = rdr["typeid"].ToString();

                    loaiKtras.Add(a);
                }
            }

            return loaiKtras;
        }

        //Lấy danh sách phân công
        public List<DsPhancong> GetDsPhancongByDoiId(string doiql)
        {
            List<DsPhancong> dsPhancongs = new List<DsPhancong>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                string oStr = "SELECT a.doi_id, a.assetid, a.assetdesc , a.typeid, max(k.TEN_LOAIKTR) TEN_LOAIKTR" +
                              ", max(b.NGAY_TH_MAX) NGAY_TH_MAX FROM KTDK_PCKT a INNER JOIN KTDK_TBI_LOAI b " +
                              "ON a.assetid = b.ASSETID INNER JOIN KTDK_LOAIKTR k ON b.ID_LOAIKTR = k.ID_LOAIKTR WHERE a.doi_id = @doiql " +
                              " GROUP BY a.doi_id, a.assetid, a.assetdesc, a.typeid";
                SqlCommand cmd = new SqlCommand(oStr, conn);
                cmd.Parameters.Add(new SqlParameter("@doiql", doiql));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DsPhancong a = new DsPhancong();

                    a.doiid = rdr["doi_id"].ToString();
                    a.assetid = rdr["assetid"].ToString();
                    a.assetdesc = rdr["assetdesc"].ToString();
                    a.typeid = rdr["typeid"].ToString();
                    a.tenloaikt = rdr["TEN_LOAIKTR"].ToString();
                    a.ngaythmax = rdr["NGAY_TH_MAX"].ToString();

                    dsPhancongs.Add(a);
                }
            }

            return dsPhancongs;
        }

        //Thu tuc tao phien
        public void CrSession(string PhongBanID, string giobd, string giokt, string nguoitao, string donvi, string dskt)
        {
            List<ApprovePlanViewModel> appPlan = new List<ApprovePlanViewModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("at_ktdk_add_phien", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@PhongBanID", SqlDbType.Int).Value = int.Parse(PhongBanID);
                sql_cmnd.Parameters.AddWithValue("@GioBd", SqlDbType.DateTime).Value = giobd;
                sql_cmnd.Parameters.AddWithValue("@GioKt", SqlDbType.DateTime).Value = giokt;
                sql_cmnd.Parameters.AddWithValue("@NguoiTao", SqlDbType.NVarChar).Value = nguoitao;
                sql_cmnd.Parameters.AddWithValue("@donvi", SqlDbType.NVarChar).Value = donvi;
                sql_cmnd.Parameters.AddWithValue("@ds_dt", SqlDbType.NVarChar).Value = dskt;
                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }

        }

        //Lấy danh sách phân công theo đơn vị
        public List<DsPhancong> GetDsPhancongByMadvql(string madvql)
        {
            List<DsPhancong> dsPhancongs = new List<DsPhancong>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_get_dsphancong_bymadvql", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DsPhancong a = new DsPhancong();

                    a.doiid = rdr["doi_id"].ToString();
                    a.assetid = rdr["assetid"].ToString();
                    a.assetdesc = rdr["assetdesc"].ToString();
                    a.typeid = rdr["typeid"].ToString();
                    a.teamdesc = rdr["TenPhongBan"].ToString();
                    dsPhancongs.Add(a);
                }
            }

            return dsPhancongs;
        }

        //remove phan cong
        public void RemoveAssignment(string madvql, string doiid, string dskt)
        {
            List<ApprovePlanViewModel> appPlan = new List<ApprovePlanViewModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("pkg_at_ktdk_remove_pckt", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@madvql", SqlDbType.NVarChar).Value = madvql;
                sql_cmnd.Parameters.AddWithValue("@doiid", SqlDbType.Int).Value = int.Parse(doiid);
                sql_cmnd.Parameters.AddWithValue("@dskt", SqlDbType.NVarChar).Value = dskt;
                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }

        }

        //Thu tuc phan cong
        public void PhancongKtra(string madvql, string doiid, string dskt)
        {
            List<ApprovePlanViewModel> appPlan = new List<ApprovePlanViewModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("at_ktdk_add_pckt", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@donvi", SqlDbType.NVarChar).Value = madvql;
                sql_cmnd.Parameters.AddWithValue("@PhongBanID", SqlDbType.Int).Value = int.Parse(doiid);
                sql_cmnd.Parameters.AddWithValue("@ds_dt", SqlDbType.NVarChar).Value = dskt;
                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }

        }

        //Lấy danh sách combobox loại kieemtra tra
        public List<NhomLoaiKtra> GetDmNhomKtra()
        {
            List<NhomLoaiKtra> dmNhomKtras = new List<NhomLoaiKtra>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_at_dm_nhomktra", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    NhomLoaiKtra a = new NhomLoaiKtra();

                    a.ma_loai_ktr = rdr["ma_loai_ktr"].ToString();
                    a.ten_loai_ktr = rdr["ten_loai_ktr"].ToString();

                    dmNhomKtras.Add(a);
                }
            }

            return dmNhomKtras;
        }

        //Lấy danh sách phòng ban thuộc đơn vị
        //Typeid = "" tất cả. = 1 là điện lực, 2 là bên ngoài
        public List<PhongBan> GetDmPhongBan(string orgid, int typeid)
        {
            List<PhongBan> dmPhongban = new List<PhongBan>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                string oStr = "select * from tblPhongBan pb join tblDonVi dv on dv.Id = pb.MaDVi " +
                              "where 1 = 1 ";
                if(orgid != "")
                {
                    oStr = oStr + " and dv.Id = @madv";
                }
                    
                if(typeid > 0)
                {
                    oStr = oStr + " and pb.LoaiPB = @loaipb";
                }
                    
                SqlCommand cmd = new SqlCommand(oStr, conn);
                cmd.Parameters.AddWithValue("@madv", orgid);
                cmd.Parameters.AddWithValue("@loaipb", typeid);

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PhongBan a = new PhongBan();

                    a.Id = rdr["Id"].ToString();
                    a.TenPhongBan = rdr["TenPhongBan"].ToString();

                    dmPhongban.Add(a);
                }
            }

            return dmPhongban;
        }

        //Lấy danh sách báo cáo KTGS
        public List<BcKtgs> GetDsBcKtgs(string vdonvi, string vloai, string vthang, string vnam)
        {
            List<BcKtgs> dsPhancongs = new List<BcKtgs>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.get_bc_tt_ktdx", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", vdonvi));
                cmd.Parameters.Add(new SqlParameter("@loai_bc", vloai));
                cmd.Parameters.Add(new SqlParameter("@thang", vthang));
                cmd.Parameters.Add(new SqlParameter("@nam", vnam));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BcKtgs a = new BcKtgs();

                    a.MaDvql = rdr["ma_dviqly"].ToString();
                    a.IdNhom= rdr["id_nhom"]==null?0: int.Parse(rdr["id_nhom"].ToString());
                    a.TenDvql = rdr["TenDonVi"].ToString();
                    a.CapDv = rdr["capdv"] == null ? 0 : int.Parse(rdr["capdv"].ToString());
                    a.TenNhom = rdr["ten_nhom"].ToString();
                    a.HoVaTen = rdr["hovaten"].ToString();
                    a.ChucVu = rdr["chucvu"].ToString();
                    a.Nam = rdr["nam"] == null ? 0 : int.Parse(rdr["nam"].ToString());
                    a.Thang = rdr["thang"] == null ? 0 : int.Parse(rdr["thang"].ToString());
                    a.ChiTieu = rdr["chi_tieu"] == null ? 0 : int.Parse(rdr["chi_tieu"].ToString());
                    a.SlDaKtra = rdr["sl_da_ktr"] == null ? 0 : int.Parse(rdr["sl_da_ktr"].ToString());
                    a.SlCoTt = rdr["sl_co_tt"] == null ? 0 : int.Parse(rdr["sl_co_tt"].ToString());
                    a.SlDaKphuc = rdr["sl_da_kp"] == null ? 0 : int.Parse(rdr["sl_da_kp"].ToString());
                    a.ToMau = rdr["to_mau"] == null ? 0 : int.Parse(rdr["to_mau"].ToString());
                    dsPhancongs.Add(a);
                }
            }

            return dsPhancongs;
        }

        public List<DonVi> GetDmDonVi(string madvql)
        {
            List<DonVi> dsDonvi = new List<DonVi>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.usp_get_dvql", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DonVi a = new DonVi();

                    a.Id = rdr["id"].ToString();
                    a.TenDonVi = rdr["tendonvi"].ToString();
                    a.DviCha = rdr["DviCha"].ToString();

                    dsDonvi.Add(a);
                }
            }

            return dsDonvi;
        }

        //Lấy ảnh ký
        public byte[] imgSignByUserid(string username)
        {
            byte[] imgSignByUserid = null;
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select ChuKySo from tblNhanVien where username = '" + username + "'", conn);
                cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                imgSignByUserid = dt.AsEnumerable().Select(c => c.Field<byte[]>("ChuKySo")).SingleOrDefault();

                conn.Close();                
            }

            return imgSignByUserid;
        }

        //Insert file ký vào csdl
        public void insFileSignToDb(int idphien, int stepsign, string usersign, byte[] buffer)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("pkg_plv_ins_filesign", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@idphien", SqlDbType.NVarChar).Value = idphien;
                sql_cmnd.Parameters.AddWithValue("@stepsign", SqlDbType.Int).Value = stepsign;
                sql_cmnd.Parameters.AddWithValue("@usersign", SqlDbType.NVarChar).Value = usersign;
                sql_cmnd.Parameters.AddWithValue("@datasign", SqlDbType.VarBinary).Value = buffer;
                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }
        }

        //Insert file ký BBKS vào csdl
        public void InsFileSignBBKSPATC(int idphien, string usersign, string urlfile, DateTime datesign, int loaibb)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand sql_cmnd = new SqlCommand("pkg_plv_ins_file_bbks_patc", conn);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@idphien", SqlDbType.NVarChar).Value = idphien;
                sql_cmnd.Parameters.AddWithValue("@usersign", SqlDbType.NVarChar).Value = usersign;
                sql_cmnd.Parameters.AddWithValue("@datesign", SqlDbType.NVarChar).Value = datesign;
                sql_cmnd.Parameters.AddWithValue("@urlfile", SqlDbType.NVarChar).Value = urlfile;
                sql_cmnd.Parameters.AddWithValue("@loaibb", SqlDbType.Int).Value = loaibb;

                sql_cmnd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public byte[] getFileSign(string id, int step)
        {
            byte[] imgSignByUserid = null;
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select max(DataSign) DataSign from plv_Phieucongtac_Sign where PhienlamviecID = '" + id + "'", conn);
                cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                imgSignByUserid= dt.AsEnumerable().Select(c => c.Field<byte[]>("DataSign")).SingleOrDefault();

                conn.Close();
            }

            return imgSignByUserid;
        }

        #region Hàm lấy tham số
        public DataTable GetParamSignPlv(int idphien, int loaiphieu, int buoc)
        {            
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_sign_get_param", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idphien", idphien));
                cmd.Parameters.Add(new SqlParameter("@loaiphieu", loaiphieu));
                cmd.Parameters.Add(new SqlParameter("@step", buoc));

                //SqlDataReader dr = cmd.ExecuteReader();
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {                    
                    sda.Fill(dt);                    
                }

                conn.Close();
            }

            return dt;
        }
        #endregion
        #region 'Hàm lấy urlfile Sign'
        public string GetUrlFileSignBB(string idphien, int loaibb)
        {
            string urlfile = "";
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("pkg_sign_get_filebb", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idphien", idphien));
                cmd.Parameters.Add(new SqlParameter("@loaibb", loaibb));

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    sda.Fill(dt);
                }

                foreach (var row in dt.AsEnumerable())  // AsEnumerable() returns IEnumerable<DataRow>
                {
                    urlfile = row.Field<string>("UrlFileSign");

                }
            }

            return urlfile;
        }
        #endregion
    }

}

public class ApprovePlanViewModel
    {
        public int Id { get; set; }
        public string AssetId { get; set; }
        public string AssetDesc { get; set; }
        public string IdLoaiKt { get; set; }
        public string TenLoaiKt { get; set; }
        public int SlDangcho { get; set; }
        public int SlDath { get; set; }
        public string MaLoaiKtra { get; set; }
    }

    public class AssetAudit
    {
        public string ASSETID { get; set; }
    }

    public class LoaiKtra
    {
        public string id_loaiktr
        { get; set; }

        public string ten_loaiktr { get; set; }

        public string typeid { get; set; }

    }

    public class NhomLoaiKtra
    {
        public string ma_loai_ktr { get; set; }
        public string ten_loai_ktr { get; set; }
        public int stt { get; set; }
        public string mota { get; set; }
    }

    //Model ds phân công
    public class DsPhancong
    {
        public string madvql { get; set; }
        public string doiid  { get; set; }
        public string assetid { get; set; }
        public string assetdesc { get; set; }
        public string typeid { get; set; }
        public string tenloaikt { get; set; }
        public string ngaythmax  {get;set;}
        public string teamdesc { get; set; }
    }

    //Model phòng ban
    public class PhongBan
    {
        public string Id { get; set; }
        public string TenPhongBan { get; set; }
    }

    //Model Đơn vị quản lý
    public class DonVi
    {
        public string Id { get; set; }
        public string TenDonVi { get; set; }
        public string DviCha { get; set; }
        public string MoTa { get; set; }
    }

    //Model Báo cáo kiểm tra giám sat
    public class BcKtgs
    {
        public string MaDvql { get; set; }
        public int IdNhom { get; set; }
        public string TenDvql { get; set; }
        public int CapDv { get; set; }
        public string TenNhom { get; set; }
        public string HoVaTen { get; set; }
        public string ChucVu { get; set; }
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int ChiTieu { get; set; }
        public int SlDaKtra { get; set; }
        public int SlCoTt { get; set; }
        public int SlDaKphuc { get; set; }
        public List<BcKtgs> LstBcKtgs { get; set; }
        public int ToMau { get; set; }
}