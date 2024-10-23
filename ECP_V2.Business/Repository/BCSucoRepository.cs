using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class BCSucoRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public BCSucoRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public BCSucoRepository()
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

        public BCSucoRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public DataTable GetDsSucoChitiet(string vdonvi, string vthang, string vnam)
        {           
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.usp_bcsuco_chitiet", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", vdonvi));
                cmd.Parameters.Add(new SqlParameter("@thang", vthang));
                cmd.Parameters.Add(new SqlParameter("@nam", vnam));
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);
                
                return dt;
            }

            
        }
        //Danh sách sự cố tổng hợp - BC02
        public DataTable GetDsSucoThop(string vdonvi, string vthang, string vnam)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.usp_bcsuco_tonghop", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", vdonvi));
                cmd.Parameters.Add(new SqlParameter("@thang", vthang));
                cmd.Parameters.Add(new SqlParameter("@nam", vnam));
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                return dt;
            }
        }

        //Danh sách sự cố Xuất tuyến - BC03
        public DataTable GetDsSucoXtuyen(string vdonvi, int vloai, DateTime vngay, int vtuan, int vthang, int vnam)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.usp_bcsuco_xtuyen", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", vdonvi));
                cmd.Parameters.Add(new SqlParameter("@loai", vloai));
                cmd.Parameters.Add(new SqlParameter("@ngay", vngay));
                cmd.Parameters.Add(new SqlParameter("@tuan", vtuan));
                cmd.Parameters.Add(new SqlParameter("@thang", vthang));
                cmd.Parameters.Add(new SqlParameter("@nam", vnam));
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);

                return dt;
            }
        }
    }
}
