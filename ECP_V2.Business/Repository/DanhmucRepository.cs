using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.Danhmuc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class DanhmucRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public DanhmucRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public DanhmucRepository()
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

        public DanhmucRepository(WorkUnit unit)
            : base(unit)
        {
        }

        //Danh mục biên bản type = 0
        public List<Danhmuc> GetDmBienban(string type)
        {
            List<Danhmuc> loaiKtras = new List<Danhmuc>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_dm_dungchung", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@type", type));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Danhmuc a = new Danhmuc();

                    a.madm = rdr["MADMUC"].ToString();
                    a.tendm = rdr["TENDMUC"].ToString();
                    a.trangthai = (bool)rdr["TRANGTHAI"];

                    loaiKtras.Add(a);
                }
            }

            return loaiKtras;
        }

        //Kieu: 0 - Add, 1 - Update
        public string InsertDmuc(string madmuc, string tendmuc, string trangthai, string kieu, string loaidmuc)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_dmuc_addedit", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madmuc", madmuc));
                cmd.Parameters.Add(new SqlParameter("@tendmuc", tendmuc));
                cmd.Parameters.Add(new SqlParameter("@trangthai", trangthai));                
                cmd.Parameters.Add(new SqlParameter("@kieu", kieu));
                cmd.Parameters.Add(new SqlParameter("@loaidmuc", loaidmuc));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        public string DeleteDmuc(string madmuc, string kieudmuc)
        {
            string output = "OK";
            
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_dmuc_delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madmuc", madmuc));
                cmd.Parameters.Add(new SqlParameter("@kieudmuc", kieudmuc));

                output = (string)cmd.ExecuteScalar();
                if (output == null)
                {
                    output = "OK";
                }
                conn.Close();
            }

            return output;
        }


    }
}
