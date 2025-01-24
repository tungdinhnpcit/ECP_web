using ECP_V2.Business.UnitOfWork;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ECP_V2.Business.Repository
{
    public class SafeTrainRepository : RepositoryBase_V2
    {
        public string Connectstr { get; set; }
        public SafeTrainRepository(string strConn)
         : base()
        {
            this.Connectstr = strConn;
        }

        public SafeTrainRepository()
            : base()
        {
            try
            {
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbNpcContext"].ConnectionString);
                Connectstr = connection.ConnectionString;
            }
            catch (Exception ex)
            { }
        }

        public SafeTrainRepository(WorkUnit unit)
            : base(unit)
        {
        }

        //Lấy danh sách loại hình đào tạo
        public List<TypeEdu> getTypeEdu()
        {
            List<TypeEdu> typeEdus = new List<TypeEdu>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_typeedu", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    TypeEdu a = new TypeEdu();

                    a.groupid = rdr["groupid"].ToString();
                    a.groupdesc = rdr["groupdesc"].ToString();
                    a.note = rdr["note"].ToString();

                    typeEdus.Add(a);
                }
                conn.Close();
            }

            return typeEdus;
        }

        //Lấy danh mục Nhóm đào tạo theo loại hình đt
        public List<GroupEdu> LoadGroupEdu(string typeid)
        {
            List<GroupEdu> typeEdus = new List<GroupEdu>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_groupedu_bytype", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@typeid", typeid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GroupEdu a = new GroupEdu();

                    a.categoryid = rdr["categoryid"].ToString();
                    a.categorydesc = rdr["categorydesc"].ToString();
                    a.groupid = rdr["groupid"].ToString();
                    a.frequency_time = float.Parse(rdr["frequency_time"].ToString());
                    a.user_mdf_time = DateTime.Parse(rdr["user_mdf_time"].ToString());

                    typeEdus.Add(a);
                }
                conn.Close();
            }

            return typeEdus;
        }

        //Lấy trạng thái lớp học LoadStatusClass
        public List<StatusClass> LoadStatusClass(int typeid, string classid)
        {
            List<StatusClass> statusClasses = new List<StatusClass>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_status_class", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@type", typeid));
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StatusClass a = new StatusClass();

                    a.statusid = rdr["statusid"].ToString();
                    a.statusdesc = rdr["statusdesc"].ToString();

                    statusClasses.Add(a);
                }
                conn.Close();
            }

            return statusClasses;
        }

        //Lấy danh sách lớp học LoadDsClass
        public List<ClassTrain> LoadDsClass(string tungay, string denngay, string typeEdu, string groupEdu, string statusClass, string madvql)
        {
            List<ClassTrain> classTrains = new List<ClassTrain>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_class_all", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@tungay", tungay));
                cmd.Parameters.Add(new SqlParameter("@denngay", denngay));
                cmd.Parameters.Add(new SqlParameter("@typeEdu", typeEdu));
                cmd.Parameters.Add(new SqlParameter("@groupEdu", groupEdu));
                cmd.Parameters.Add(new SqlParameter("@statusClass", statusClass));
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClassTrain a = new ClassTrain();

                    a.classid = rdr["classid"].ToString();
                    a.classdesc = rdr["classdesc"].ToString();
                    a.classcode = rdr["classcode"].ToString();
                    a.ma_dviqly = rdr["ma_dviqly"].ToString();
                    a.ma_dvi_daotao = rdr["ma_dvi_daotao"].ToString();
                    a.categoryid = rdr["categoryid"].ToString();
                    a.statusid = rdr["statusid"] == null ? 0 : int.Parse(rdr["statusid"].ToString());
                    a.nguoilap_kh = rdr["nguoilap_kh"].ToString();
                    a.ngaybd_kh = rdr["ngaybd_kh"] == null ? null : rdr["ngaybd_kh"].ToString();
                    a.ngaykt_kh = rdr["ngaykt_kh"] == null ? null : rdr["ngaykt_kh"].ToString();
                    //a.ngaybd_kh = rdr["ngaybd_kh"] == null ? (DateTime?)null : DateTime.Parse(rdr["ngaybd_kh"].ToString());
                    //a.ngaykt_kh = rdr["ngaykt_kh"] == null ? (DateTime?)null : DateTime.Parse(rdr["ngaykt_kh"].ToString());
                    a.nguoiduyet_khc1 = rdr["nguoiduyet_khc1"].ToString();
                    a.nguoiduyet_khc2 = rdr["nguoiduyet_khc2"].ToString();
                    a.ngaybd_th = rdr["ngaybd_th"] == null ? null : rdr["ngaybd_th"].ToString();
                    a.ngaykt_th = rdr["ngaykt_th"] == null ? null : rdr["ngaykt_th"].ToString();
                    a.ghi_chu = rdr["ghi_chu"].ToString();
                    a.sohvien = rdr["sohvien"] == null ? 0 : int.Parse(rdr["sohvien"].ToString());
                    a.so_file = rdr["so_file"] == null ? 0 : int.Parse(rdr["so_file"].ToString());
                    a.ht = rdr["ht"].ToString();

                    classTrains.Add(a);
                }
                conn.Close();
            }

            return classTrains;

        }

        //Lấy danh sách cty
        public DataTable getDsPlvDonvi(string madvql, string kieudv)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(madvql)) madvql = "PA";
            if (string.IsNullOrEmpty(kieudv)) kieudv = "CTY";            

                using (SqlConnection conn = new SqlConnection(Connectstr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("usp_plv_getdviqly", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@madviqly", madvql));
                cmd.Parameters.Add(new SqlParameter("@kieudv", kieudv));

                //SqlDataReader dr = cmd.ExecuteReader();
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }

                    conn.Close();
                }
           

            return dt;
        }

        public DataTable getDsPlvNvien(string cty, string dluc)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(cty)) cty = "PA";
            if (string.IsNullOrEmpty(dluc)) dluc = "CTY";

            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_plv_getnvien_dvid", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@cty", cty));
                cmd.Parameters.Add(new SqlParameter("@dluc", dluc));

                //SqlDataReader dr = cmd.ExecuteReader();
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    sda.Fill(dt);
                }

                conn.Close();
            }


            return dt;
        }

        //Lấy danh sách nhân sự bởi ID
        public List<Personal> LoadDsPersonalById(string nsid)
        {
            List<Personal> lstPersional = new List<Personal>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from hluat_hrms_nhansu where nsid='" + nsid + "'", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Personal a = new Personal();
                    a.nsid = rdr["nsid"] == null ? "" : rdr["nsid"].ToString();
                    a.orgid = rdr["orgid"] == null ? "" : rdr["orgid"].ToString();
                    a.tenkhaisinh = rdr["tenkhaisinh"] == null ? "" : rdr["tenkhaisinh"].ToString();
                    a.quequan = rdr["quequan"] == null ? "" : rdr["quequan"].ToString();
                    a.email = rdr["email"] == null ? "" : rdr["email"].ToString();
                    a.sdt = rdr["sdt"] == null ? "" : rdr["sdt"].ToString();
                    a.phongban = rdr["phongban"] == null ? "" : rdr["phongban"].ToString();
                    a.chucdanh = rdr["chucdanh"] == null ? "" : rdr["chucdanh"].ToString();
                    a.chucdanhatd = rdr["chucdanhatd"] == null ? "" : rdr["chucdanhatd"].ToString();
                    a.bacat = rdr["bacat"] == null ? "" : rdr["bacat"].ToString();
                    a.cccd = rdr["cccd"] == null ? "" : rdr["cccd"].ToString();
                    a.bactho = rdr["bactho"] == null ? "" : rdr["bactho"].ToString();
                    a.ngaysinh = rdr["ngaysinh"] == null ? "" : rdr["ngaysinh"].ToString();
                    a.gioitinh = rdr["gioitinh"] == null ? "" : rdr["gioitinh"].ToString();
                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Lấy danh sách nhân sự thuộc đơn vị để chọn
        public List<Personal> LoadDsPersonal(string orgid)
        {
            List<Personal> lstPersional = new List<Personal>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_personal_byorg", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@orgid", orgid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Personal a = new Personal();
                    a.nsid = rdr["nsid"] == null ? "" : rdr["nsid"].ToString();
                    a.orgid = rdr["orgid"] == null ? "" : rdr["orgid"].ToString();
                    a.tenkhaisinh = rdr["tenkhaisinh"] == null ? "" : rdr["tenkhaisinh"].ToString();
                    a.quequan = rdr["quequan"] == null ? "" : rdr["quequan"].ToString();
                    a.email = rdr["email"] == null ? "" : rdr["email"].ToString();
                    a.sdt = rdr["sdt"] == null ? "" : rdr["sdt"].ToString();
                    a.phongban = rdr["phongban"] == null ? "" : rdr["phongban"].ToString();
                    a.chucdanh = rdr["chucdanh"] == null ? "" : rdr["chucdanh"].ToString();
                    a.chucdanhatd = rdr["chucdanhatd"] == null ? "" : rdr["chucdanhatd"].ToString();
                    a.bacat = rdr["bacat"] == null ? "" : rdr["bacat"].ToString();
                    a.cccd = rdr["cccd"] == null ? "" : rdr["cccd"].ToString();
                    a.bactho = rdr["bactho"] == null ? "" : rdr["bactho"].ToString();
                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Lấy danh sách đơn vị + số lượng nhân sự
        public List<Organization> LoadDsOrgByKh(string orgid, string khoach, string loaidaotao, string nhomhl)
        {
            List<Organization> organizations = new List<Organization>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_dsorg", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@orgid", orgid));
                cmd.Parameters.Add(new SqlParameter("@khoach", ""));
                cmd.Parameters.Add(new SqlParameter("@loaidaotao", 1));
                cmd.Parameters.Add(new SqlParameter("@nhomhl", 1));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Organization a = new Organization();
                    a.madvql = rdr["madvql"] == null ? "" : rdr["madvql"].ToString();
                    a.tendvql = rdr["tendvql"] == null ? "" : rdr["tendvql"].ToString();
                    a.slns = rdr["slns"] == null ? 0 : int.Parse(rdr["slns"].ToString());

                    organizations.Add(a);
                }
                conn.Close();
            }

            return organizations;
        }

        public List<Organization> LoadDsOrg(string orgid)
        {
            List<Organization> organizations = new List<Organization>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_dsorg_all", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@orgid", orgid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Organization a = new Organization();
                    a.madvql = rdr["madvql"] == null ? "" : rdr["madvql"].ToString();
                    a.tendvql = rdr["tendvql"] == null ? "" : rdr["tendvql"].ToString();
                    a.slns = rdr["slns"] == null ? 0 : int.Parse(rdr["slns"].ToString());

                    organizations.Add(a);
                }
                conn.Close();
            }

            return organizations;
        }

        //Insert class
        public string InsertClass(string dvi, string dvidaotao, string loailop, string mahieu
            , string loaidaotao, string nhomhluyen, string thbatdau
            , string thketthuc, string dsnhansu, string nguoitao, string classid)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_class", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@dvi", dvi));
                cmd.Parameters.Add(new SqlParameter("@dvidaotao", dvidaotao));
                cmd.Parameters.Add(new SqlParameter("@loailop", loailop));
                cmd.Parameters.Add(new SqlParameter("@mahieu", mahieu));
                cmd.Parameters.Add(new SqlParameter("@loaidaotao", loaidaotao));
                cmd.Parameters.Add(new SqlParameter("@nhomhluyen", nhomhluyen));
                cmd.Parameters.Add(new SqlParameter("@nguoitao", nguoitao));
                //cmd.Parameters.Add(new SqlParameter("@khbatdau", khbatdau));
                //cmd.Parameters.Add(new SqlParameter("@khketthuc", khketthuc));
                cmd.Parameters.Add(new SqlParameter("@thbatdau", thbatdau));
                cmd.Parameters.Add(new SqlParameter("@thketthuc", thketthuc));
                cmd.Parameters.Add(new SqlParameter("@dsnhansu", dsnhansu));
                cmd.Parameters.Add(new SqlParameter("@classid", classid));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        //Insert Plan
        public string InsertPlan(string dvi, string dvidaotao, string mota
            , string loaidaotao, string nhomhluyen, string khbatdau, string khketthuc
            , string dsnhansu, string nguoitao, string planid)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_plan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@dvi", dvi));
                cmd.Parameters.Add(new SqlParameter("@dvidaotao", dvidaotao));
                cmd.Parameters.Add(new SqlParameter("@mota", mota));
                cmd.Parameters.Add(new SqlParameter("@loaidaotao", loaidaotao));
                cmd.Parameters.Add(new SqlParameter("@nhomhluyen", nhomhluyen));
                cmd.Parameters.Add(new SqlParameter("@nguoitao", nguoitao));
                cmd.Parameters.Add(new SqlParameter("@khbatdau", khbatdau));
                cmd.Parameters.Add(new SqlParameter("@khketthuc", khketthuc));
                cmd.Parameters.Add(new SqlParameter("@dsnhansu", dsnhansu));
                cmd.Parameters.Add(new SqlParameter("@planid", planid));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        //Insert Exam File => Return examid        
        public string InsertExamFile(string filename, string fileurl, string filetype, string examid)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_exam_file", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@filename", filename));
                cmd.Parameters.Add(new SqlParameter("@fileurl", fileurl));
                cmd.Parameters.Add(new SqlParameter("@filetype", filetype));
                cmd.Parameters.Add(new SqlParameter("@examid", examid));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }
        //Insert Exam
        public string InsertExam(string classid, string categoryid, string examord)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_exam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@loaithi", categoryid));
                cmd.Parameters.Add(new SqlParameter("@examord", examord));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        public void UpdatePointExamByNsid(string examid, string nsid, string pointnew)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("update hluat_exam_result set score = " + pointnew + " where examid = '" + examid + "' and nsid = '" + nsid + "'", conn);
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public string InsertExamResult(string examid, string nsid, string score, string note)
        {
            string output = "OK";

            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_ins_result_exam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@examid", examid));
                cmd.Parameters.Add(new SqlParameter("@nsid", nsid));
                cmd.Parameters.Add(new SqlParameter("@score", score));
                cmd.Parameters.Add(new SqlParameter("@note", note));

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return output;
        }

        //Insert File thuộc Class
        public string InsertClassFile(string classid, string filename, string ngayky, string fileurl, string filetype)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_class_file", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@filename", filename));
                cmd.Parameters.Add(new SqlParameter("@ngayky", ngayky));
                cmd.Parameters.Add(new SqlParameter("@fileurl", fileurl));
                cmd.Parameters.Add(new SqlParameter("@filetype", filetype));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        //Insert File thuộc Plan
        public string InsertPlanFile(string planid, string filename, string fileurl, string filetype)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_add_plan_file", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@planid", planid));
                cmd.Parameters.Add(new SqlParameter("@filename", filename));
                cmd.Parameters.Add(new SqlParameter("@fileurl", fileurl));
                cmd.Parameters.Add(new SqlParameter("@filetype", filetype));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }
        //Lấy danh sách điểm thi lần thứ của kỳ thi
        public List<ExamPoint> LoadPointByExam(string examid)
        {
            List<ExamPoint> examFiles = new List<ExamPoint>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_get_result_byexam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@examid", examid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ExamPoint a = new ExamPoint();

                    a.diem = rdr["score"].ToString();
                    a.examid = rdr["examid"].ToString();
                    a.nhansu = rdr["nsid"].ToString();
                    a.tenkhaisinh = rdr["tenkhaisinh"].ToString();
                    a.chucdanh = rdr["chucdanh"].ToString();
                    a.donvi = rdr["donvi"].ToString();

                    examFiles.Add(a);
                }
                conn.Close();
            }

            return examFiles;
        }
        //Lấy danh sách điểm của bài thi theo combo chọn 
        public List<ExamPoint> LoadResultExamByHtthi(string classid, string htthi, string lanthi)
        {
            List<ExamPoint> examFiles = new List<ExamPoint>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_get_result_bycombo_class", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@htthi", htthi));
                cmd.Parameters.Add(new SqlParameter("@lanthi", lanthi));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ExamPoint a = new ExamPoint();

                    a.diem = rdr["score"].ToString();
                    a.examid = rdr["examid"].ToString();
                    a.nhansu = rdr["nsid"].ToString();
                    a.chucdanh = rdr["chucdanh"].ToString();
                    a.donvi = rdr["donvi"].ToString();

                    examFiles.Add(a);
                }
                conn.Close();
            }

            return examFiles;
        }

        //Lấy danh sách file thuộc Bài thi
        public List<ExamFile> LoadListFileByExam(string examid)
        {
            List<ExamFile> examFiles = new List<ExamFile>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select fileid, examid,filename, filetype, fileurl, convert(nvarchar(10),ngay_tao,103) ngay_tao from hluat_exam_file where examid = '" + examid + "'", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@examid", examid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ExamFile a = new ExamFile();

                    a.fileid = rdr["fileid"].ToString();
                    a.examid = rdr["examid"].ToString();
                    a.filename = rdr["filename"].ToString();
                    a.filetype = rdr["filetype"].ToString();
                    a.fileurl = rdr["fileurl"].ToString();
                    a.ngay_tao = rdr["ngay_tao"].ToString();

                    examFiles.Add(a);
                }
                conn.Close();
            }

            return examFiles;
        }

        //Delete class
        public string DeleteClass(string classid)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_delete_class", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        //Update status class
        public void UpldateStatusClass(string classid, string statusnew)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("update hluat_class set statusid = " + statusnew + " where classid = '" + classid + "'", conn);
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteScalar();

                conn.Close();
            }
        }

        //Delete class
        public string DeletePlan(string planid)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_delete_plan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@planid", planid));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        //Get file by id
        public string GetFileById(string fileid)
        {
            string fileurl = "";
            //Lấy bản ghi xoá file vật lý trước sau đó xoá trong db
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select fileurl from hluat_class_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                fileurl = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return fileurl;
        }

        //Delete File by Id
        public string DeleteFileByClass(string fileid)
        {
            //Lấy bản ghi xoá file vật lý trước sau đó xoá trong db
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select fileurl from hluat_class_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                string fileurl = (string)cmd.ExecuteScalar();
                //Delete file vật lý
                if (File.Exists(fileurl))
                {
                    try
                    {
                        File.Delete(fileurl);
                    }
                    catch (Exception ex)
                    {
                        //Do something
                    }
                }
                conn.Close();
            }

            string output = "OK";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from hluat_class_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return output;
        }

        //Load danh sách nhân sự tham gia lớp học
        public List<Personal> loadLstPersonalByClass(string classid)
        {
            List<Personal> list = new List<Personal>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_lst_personal_byclass", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Personal a = new Personal();

                    a.nsid = rdr["nsid"].ToString();
                    a.orgid = rdr["orgid"].ToString();
                    a.tenkhaisinh = rdr["tenkhaisinh"].ToString();
                    a.chucdanh = rdr["chucdanh"].ToString();
                    a.phongban = rdr["phongban"].ToString();
                    a.tendvi = rdr["ORG_DESC"].ToString();

                    list.Add(a);
                }


                conn.Close();
            }

            return list;
        }

        //Load danh sách nhân sự tham gia lớp học trong kế hoạch
        public List<Personal> loadLstPersonalByPlan(string planid)
        {
            List<Personal> list = new List<Personal>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_lst_personal_byplan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@planid", planid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Personal a = new Personal();

                    a.nsid = rdr["nsid"].ToString();
                    a.orgid = rdr["orgid"].ToString();
                    a.tenkhaisinh = rdr["tenkhaisinh"].ToString();
                    a.chucdanh = rdr["chucdanh"].ToString();
                    a.phongban = rdr["phongban"].ToString();

                    list.Add(a);
                }


                conn.Close();
            }

            return list;
        }

        //Lấy danh sách file theo classid
        public List<ClassFile> LoadListFileByClass(string classid)
        {
            List<ClassFile> lstClassFiles = new List<ClassFile>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_file_byclass", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClassFile a = new ClassFile();
                    a.fileid = rdr["fileid"].ToString();
                    a.classid = rdr["classid"] == null ? "" : rdr["classid"].ToString();
                    a.filename = rdr["filename"] == null ? "" : rdr["filename"].ToString();
                    a.ngay_ky = rdr["ngay_ky"] == null ? "" : rdr["ngay_ky"].ToString();
                    a.fileurl = rdr["fileurl"] == null ? "" : rdr["fileurl"].ToString();
                    a.filetype = rdr["filetype"] == null ? "" : rdr["filetype"].ToString();

                    lstClassFiles.Add(a);
                }
                conn.Close();
            }

            return lstClassFiles;
        }

        //Lấy danh sách file theo planid
        public List<ClassFile> LoadListFileByPlan(string planid)
        {
            List<ClassFile> lstClassFiles = new List<ClassFile>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_file_byplan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@planid", planid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClassFile a = new ClassFile();
                    a.fileid = rdr["fileid"].ToString();
                    a.classid = rdr["planid"] == null ? "" : rdr["planid"].ToString();
                    a.filename = rdr["filename"] == null ? "" : rdr["filename"].ToString();
                    a.ngay_ky = rdr["ngay_ky"] == null ? "" : rdr["ngay_ky"].ToString();
                    a.fileurl = rdr["fileurl"] == null ? "" : rdr["fileurl"].ToString();
                    a.filetype = rdr["filetype"] == null ? "" : rdr["filetype"].ToString();

                    lstClassFiles.Add(a);
                }
                conn.Close();
            }

            return lstClassFiles;
        }

        public List<ClassTrain> LoadClassById(String classid)
        {
            List<ClassTrain> classTrains = new List<ClassTrain>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select a.*,b.org_desc, g.groupdesc, h.categorydesc, s.statusdesc" +
                    " , (select count(*) from hluat_class_student where classid = '" + classid + "') sohvien" +
                    " from hluat_class a join hluat_hrms_donvi b on a.ma_dviqly = b.ma_dviqly " +
                    " join hluat_cate_gr g on a.groupid = g.groupid" +
                    " join hluat_cate h on a.categoryid = h.categoryid" +
                    " join hluat_class_status s on a.statusid = s.statusid" +
                    " where a.classid = '" + classid + "'", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClassTrain a = new ClassTrain();

                    a.classid = rdr["classid"].ToString();
                    a.classdesc = rdr["classdesc"].ToString();
                    a.classcode = rdr["classcode"].ToString();
                    a.ma_dviqly = rdr["ma_dviqly"].ToString();
                    a.ma_dvi_daotao = rdr["ma_dvi_daotao"].ToString();
                    a.loaidaotao = rdr["groupid"].ToString();
                    a.categoryid = rdr["categoryid"].ToString();
                    a.statusid = rdr["statusid"] == null ? 0 : int.Parse(rdr["statusid"].ToString());
                    a.nguoilap_kh = rdr["nguoilap_kh"].ToString();
                    a.ngaybd_kh = rdr["ngaybd_kh"] == null ? null : rdr["ngaybd_kh"].ToString();
                    a.ngaykt_kh = rdr["ngaykt_kh"] == null ? null : rdr["ngaykt_kh"].ToString();
                    a.nguoiduyet_khc1 = rdr["nguoiduyet_khc1"].ToString();
                    a.nguoiduyet_khc2 = rdr["nguoiduyet_khc2"].ToString();
                    a.ngaybd_th = rdr["ngaybd_th"] == null ? null : rdr["ngaybd_th"].ToString();
                    a.ngaykt_th = rdr["ngaykt_th"] == null ? null : rdr["ngaykt_th"].ToString();
                    a.ghi_chu = rdr["ghi_chu"].ToString();
                    a.tendvi = rdr["org_desc"].ToString();
                    a.sohvien = rdr["sohvien"] == null ? 0 : int.Parse(rdr["sohvien"].ToString());
                    a.groupdesc = rdr["groupdesc"].ToString();
                    a.categorydesc = rdr["categorydesc"].ToString();
                    a.statusdesc = rdr["statusdesc"].ToString();

                    classTrains.Add(a);
                }
                conn.Close();
            }

            return classTrains;
        }

        ///Lấy danh sách kế hoạch
        public List<PlanModel> LoadDsPlan(string tungay, string denngay, string typeEdu, string groupEdu, string statusplan, string madvql)
        {
            List<PlanModel> classTrains = new List<PlanModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_load_plan_all", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@tungay", tungay));
                cmd.Parameters.Add(new SqlParameter("@denngay", denngay));
                cmd.Parameters.Add(new SqlParameter("@typeEdu", typeEdu));
                cmd.Parameters.Add(new SqlParameter("@groupEdu", groupEdu));
                cmd.Parameters.Add(new SqlParameter("@statusplan", statusplan));
                cmd.Parameters.Add(new SqlParameter("@madvql", madvql));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PlanModel a = new PlanModel();

                    a.planid = rdr["planid"].ToString();
                    a.plandesc = rdr["plandesc"].ToString();
                    a.planstatus = rdr["planstatus"].ToString();
                    a.so_hvien = rdr["so_hvien"] == null ? 0 : int.Parse(rdr["so_hvien"].ToString());
                    a.ngay_bdau = rdr["ngay_bdau"].ToString();
                    a.ngay_kthuc = rdr["ngay_kthuc"].ToString();
                    a.groupid = rdr["groupid"] == null ? "" : rdr["groupid"].ToString();
                    a.categoryid = rdr["categoryid"].ToString();
                    a.ngaytao = rdr["ngaytao"] == null ? null : rdr["ngaytao"].ToString();
                    a.nguoitao = rdr["nguoitao"] == null ? null : rdr["nguoitao"].ToString();

                    classTrains.Add(a);
                }
                conn.Close();
            }

            return classTrains;

        }

        public List<PlanModel> LoadPlanById(String planid)
        {
            List<PlanModel> classTrains = new List<PlanModel>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select a.*,b.org_desc, g.groupdesc, h.categorydesc" +
                    " , (select count(*) from hluat_plan_student where planid = '" + planid + "') so_hvien" +
                    " from hluat_kehoach a join hluat_hrms_donvi b on a.madvql = b.ma_dviqly " +
                    " join hluat_cate_gr g on a.groupid = g.groupid" +
                    " join hluat_cate h on a.categoryid = h.categoryid" +
                    " where a.planid = '" + planid + "'", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PlanModel a = new PlanModel();

                    a.planid = rdr["planid"].ToString();
                    a.plandesc = rdr["plandesc"].ToString();
                    a.planstatus = rdr["planstatus"].ToString();
                    a.so_hvien = rdr["so_hvien"] == null ? 0 : int.Parse(rdr["so_hvien"].ToString());
                    a.ngay_bdau = rdr["ngay_bdau"].ToString();
                    a.ngay_kthuc = rdr["ngay_kthuc"].ToString();
                    a.groupid = rdr["groupid"] == null ? "" : rdr["groupid"].ToString();
                    a.categoryid = rdr["categoryid"].ToString();
                    a.ngaytao = rdr["ngaytao"] == null ? null : rdr["ngaytao"].ToString();
                    a.nguoitao = rdr["nguoitao"] == null ? null : rdr["nguoitao"].ToString();
                    a.dvdtao = rdr["dvi_dtao"] == null ? null : rdr["dvi_dtao"].ToString();
                    a.dvlkhoach = rdr["org_desc"] == null ? null : rdr["org_desc"].ToString();

                    classTrains.Add(a);
                }
                conn.Close();
            }

            return classTrains;
        }
        //Lấy danh sách kết quả thi của lớp học
        public List<ResultExam> LoadLastResultByClass(string classid)
        {
            List<ResultExam> lstResult = new List<ResultExam>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hlat_result_exam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ResultExam a = new ResultExam();
                    a.donvi = rdr["madvql"] == null ? null : rdr["madvql"].ToString();
                    a.nhansu = rdr["tenkhaisinh"] == null ? "" : rdr["tenkhaisinh"].ToString();
                    a.chucdanh = rdr["chucdanh"] == null ? "" : rdr["chucdanh"].ToString();
                    a.dlythuyet = rdr["dlythuyet"] == null ? "" : rdr["dlythuyet"].ToString();
                    a.dthuchanh = rdr["dthuchanh"] == null ? "" : rdr["dthuchanh"].ToString();

                    lstResult.Add(a);
                }
                conn.Close();
            }

            return lstResult;
        }

        //Báo cáo 01
        public List<BC01_KquaCnhan> BC01_KquaNhansu(string donvi, string nhansu)
        {
            List<BC01_KquaCnhan> lstPersional = new List<BC01_KquaCnhan>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_bc01_kquacnhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", donvi));
                cmd.Parameters.Add(new SqlParameter("@nhansu", nhansu));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BC01_KquaCnhan a = new BC01_KquaCnhan();
                    a.nhomhluyen = rdr["nhomhluyen"] == null ? null : rdr["nhomhluyen"].ToString();
                    a.nam = rdr["nam"] == null ? "" : rdr["nam"].ToString();
                    a.dlythuyet = rdr["dlythuyet"] == null ? "" : rdr["dlythuyet"].ToString();
                    a.dthuchanh = rdr["dthuchanh"] == null ? "" : rdr["dthuchanh"].ToString();

                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Báo cáo 02
        public List<BC02_Kqua> BC02_KquaThi(string donvi, string nam)
        {
            List<BC02_Kqua> lstPersional = new List<BC02_Kqua>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_bc02_kquathi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", donvi));
                cmd.Parameters.Add(new SqlParameter("@nam", nam));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BC02_Kqua a = new BC02_Kqua();
                    a.nhomhluyen = rdr["nhomhluyen"] == null ? null : rdr["nhomhluyen"].ToString();
                    a.donvi = rdr["donvi"] == null ? "" : rdr["donvi"].ToString();
                    a.lop = rdr["lop"] == null ? "" : rdr["lop"].ToString();
                    a.ngaythien = rdr["ngaythien"] == null ? "" : rdr["ngaythien"].ToString();
                    a.sohvien = rdr["sohvien"] == null ? "" : rdr["sohvien"].ToString();
                    a.kqua = rdr["kqua"] == null ? "" : rdr["kqua"].ToString();

                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Báo cáo 03
        public List<BC03_Kqua> BC03_KquaThi(string donvi, string nam, string loaidaotao)
        {
            List<BC03_Kqua> lstPersional = new List<BC03_Kqua>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_bc03_kquathi_new", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@donvi", donvi));
                //cmd.Parameters.Add(new SqlParameter("@nam", nam));
                //cmd.Parameters.Add(new SqlParameter("@loaidaotao", loaidaotao));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BC03_Kqua a = new BC03_Kqua();
                    a.nsid = rdr["nsID"] == null ? null : rdr["nsID"].ToString();
                    a.donvi = rdr["donvi"] == null ? null : rdr["donvi"].ToString();
                    a.cty = rdr["cty"] == null ? null : rdr["cty"].ToString();
                    a.hoten = rdr["hoten"] == null ? "" : rdr["hoten"].ToString();
                    a.chucdanh = rdr["chucdanh"] == null ? "" : rdr["chucdanh"].ToString();
                    a.phongban = rdr["phongban"] == null ? "" : rdr["phongban"].ToString();
                    a.lhdaotao = rdr["lhdaotao"] == null ? "" : rdr["lhdaotao"].ToString();
                    a.khoadaotao = rdr["khoadaotao"] == null ? "" : rdr["khoadaotao"].ToString();

                    a.ngaydaotao = rdr["ngaydaotao"] == null ? "" : rdr["ngaydaotao"].ToString();
                    a.kq_lthuyet = rdr["kq_lthuyet"] == null ? "" : rdr["kq_lthuyet"].ToString();
                    a.kq_thanh = rdr["kq_thanh"] == null ? "" : rdr["kq_thanh"].ToString();
                    a.ghichu = rdr["ghichu"] == null ? "" : rdr["ghichu"].ToString();
                    //ATD
                    a.nhom_atd = rdr["nhom_atd"] == null ? "" : rdr["nhom_atd"].ToString();
                    a.khoadtao_atd = rdr["khoadaotao_atd"] == null ? "" : rdr["khoadaotao_atd"].ToString();
                    a.kqua_lt_atd = rdr["kq_lthuyet_atd"] == null ? "" : rdr["kq_lthuyet_atd"].ToString();
                    a.kqua_th_atd = rdr["kq_thanh_atd"] == null ? "" : rdr["kq_thanh_atd"].ToString();
                    a.ghichu_atd = rdr["ghichu_atd"] == null ? "" : rdr["ghichu_atd"].ToString();
                    //ATVSLD
                    a.nhom_vsld = rdr["nhom_vs"] == null ? "" : rdr["nhom_vs"].ToString();
                    a.khoadtao_vsld = rdr["khoadaotao_vs"] == null ? "" : rdr["khoadaotao_vs"].ToString();
                    a.kqua_lt_vsld = rdr["kq_lthuyet_vs"] == null ? "" : rdr["kq_lthuyet_vs"].ToString();
                    a.kqua_th_vsld = rdr["kq_thanh_vs"] == null ? "" : rdr["kq_thanh_vs"].ToString();
                    a.ghichu_vsld = rdr["ghichu_vs"] == null ? "" : rdr["ghichu_vs"].ToString();
                    //HOTLINE
                    a.nhom_hotline = rdr["nhom_hl"] == null ? "" : rdr["nhom_hl"].ToString();
                    a.khoadtao_hotline = rdr["khoadaotao_hl"] == null ? "" : rdr["khoadaotao_hl"].ToString();
                    a.kqua_lt_hotline = rdr["kq_lthuyet_hl"] == null ? "" : rdr["kq_lthuyet_hl"].ToString();
                    a.kqua_th_hotline = rdr["kq_thanh_hl"] == null ? "" : rdr["kq_thanh_hl"].ToString();
                    a.ghichu_hotline = rdr["ghichu_hl"] == null ? "" : rdr["ghichu_hl"].ToString();



                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }
        //Get file và xoá file kế hoạch
        public string GetFileKhById(string fileid)
        {
            string fileurl = "";
            //Lấy bản ghi xoá file vật lý trước sau đó xoá trong db
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select fileurl from hluat_plan_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                fileurl = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return fileurl;
        }

        //Delete File by Id
        public string DeleteFileKhByClass(string fileid)
        {
            //Lấy bản ghi xoá file vật lý trước sau đó xoá trong db
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select fileurl from hluat_plan_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                string fileurl = (string)cmd.ExecuteScalar();
                //Delete file vật lý
                if (File.Exists(fileurl))
                {
                    try
                    {
                        File.Delete(fileurl);
                    }
                    catch (Exception ex)
                    {
                        //Do something
                    }
                }
                conn.Close();
            }

            string output = "OK";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from hluat_plan_file where fileid = '" + fileid + "'", conn);
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return output;
        }

        public string GetExamId(string classid, string loaidt, string loaithi, string lanthi)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select examid from hluat_exam where classid = '" + classid + "' and categoryid = '" + loaidt + "' and exam_order = " + lanthi + " and standardid = '" + loaithi + "'", conn);
                cmd.CommandType = CommandType.Text;

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }
        //Lấy điểm của kỳ thi gần nhất
        public List<ResultExam> getPointRecentExam(string classid, string standardid)
        {
            List<ResultExam> lstPersional = new List<ResultExam>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_exam_get_point_recentexam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@standardid", standardid));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ResultExam a = new ResultExam();
                    a.stt = rdr["stt"] == null ? 0 : int.Parse(rdr["stt"].ToString());
                    a.donvi = rdr["donvi"] == null ? "" : rdr["donvi"].ToString();
                    a.mans = rdr["mans"] == null ? "" : rdr["mans"].ToString();
                    a.nhansu = rdr["nhansu"] == null ? "" : rdr["nhansu"].ToString();
                    a.chucdanh = rdr["chucdanh"] == null ? "" : rdr["chucdanh"].ToString();
                    a.dlythuyet = rdr["dlythuyet"] == null ? "" : rdr["dlythuyet"].ToString();

                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        public List<Hluat_Cate_Standard> LoadCboLoaiThiByCate(string categoryid)
        {
            List<Hluat_Cate_Standard> lstPersional = new List<Hluat_Cate_Standard>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from hluat_cate_standard where categoryid = '" + categoryid + "'", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Hluat_Cate_Standard a = new Hluat_Cate_Standard();
                    a.standardid = rdr["standardid"] == null ? "" : rdr["standardid"].ToString();
                    a.categoryid = rdr["categoryid"] == null ? "" : rdr["categoryid"].ToString();
                    a.standard_desc = rdr["standard_desc"] == null ? "" : rdr["standard_desc"].ToString();
                    a.standard_ord = rdr["standard_ord"] == null ? "" : rdr["standard_ord"].ToString();

                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Lấy số lần thi dựa vào loại thi (categoryid)
        public List<Hluat_Exam> LoadCboSolanThiByClassStandard(string classid, string standardid)
        {
            List<Hluat_Exam> lstPersional = new List<Hluat_Exam>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from hluat_exam where classid = '" + classid + "' and standardid = '" + standardid + "'", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Hluat_Exam a = new Hluat_Exam();
                    a.examid = rdr["examid"] == null ? "" : rdr["examid"].ToString();
                    a.exam_order = rdr["exam_order"] == null ? "" : rdr["exam_order"].ToString();
                    a.standardid = rdr["standardid"] == null ? "" : rdr["standardid"].ToString();
                    a.categoryid = rdr["categoryid"] == null ? "" : rdr["categoryid"].ToString();
                    a.classid = rdr["classid"] == null ? "" : rdr["classid"].ToString();

                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }


        ///Cập nhật điểm cá nhân
        public string InsertPointPerson(string classid, string loaithi, string lanthi, string nsid, string diem, string notepoint)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_ins_point_person", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@loaithi", loaithi));
                cmd.Parameters.Add(new SqlParameter("@lanthi", lanthi));
                cmd.Parameters.Add(new SqlParameter("@nsid", nsid));
                cmd.Parameters.Add(new SqlParameter("@diem", diem));
                cmd.Parameters.Add(new SqlParameter("@notepoint", notepoint));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }
        //Khoá bài thi
        public string LockExam(string classid, string loaithi, string lanthi)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_lock_exam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@loaithi", loaithi));
                cmd.Parameters.Add(new SqlParameter("@lanthi", lanthi));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        public string CheckLockExam(string classid, string loaithi, string lanthi)
        {
            string output = "";
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_check_logexam", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@classid", classid));
                cmd.Parameters.Add(new SqlParameter("@loaithi", loaithi));
                cmd.Parameters.Add(new SqlParameter("@lanthi", lanthi));

                output = (string)cmd.ExecuteScalar();

                conn.Close();
            }

            return output;
        }

        public byte[] GetFileTmpSign(int stepSign,int loaiphieu)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select file_data from plv_template_sign where buoc_ky = "+ stepSign + " and loai_phieu = " + loaiphieu, conn);
                cmd.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                byte[] buffer = dt.AsEnumerable().Select(c => c.Field<byte[]>("file_data")).SingleOrDefault();

                conn.Close();
                return buffer;
            }

        }

        //Lấy danh sách danh mục cbo
        public List<GroupEdu> LoadCboWorker()
        {
            List<GroupEdu> lstPersional = new List<GroupEdu>();
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from hluat_dm_cdanh", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GroupEdu a = new GroupEdu();
                    a.categoryid = rdr["cdanh_id"] == null ? "" : rdr["cdanh_id"].ToString();
                    a.categorydesc = rdr["cdanh_desc"] == null ? "" : rdr["cdanh_desc"].ToString();
                   
                    lstPersional.Add(a);
                }
                conn.Close();
            }

            return lstPersional;
        }

        //Cập nhật thông tin nhân sự

        public void UpdateInforHr(string nsid, string bactho, string bacat, string cdanhatd, string nhomatd, string nhomvsld, string nhomhotline)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_hluat_update_infor_hr", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@nsid", nsid));
                cmd.Parameters.Add(new SqlParameter("@bactho", bactho));
                cmd.Parameters.Add(new SqlParameter("@bacat", bacat));
                cmd.Parameters.Add(new SqlParameter("@cdanhatd", cdanhatd));
                cmd.Parameters.Add(new SqlParameter("@nhomatd", nhomatd));
                cmd.Parameters.Add(new SqlParameter("@nhomvsld", nhomvsld));
                cmd.Parameters.Add(new SqlParameter("@nhomhotline", nhomhotline));

                string output = (string)cmd.ExecuteScalar();

                conn.Close();
            }
        }

        //Cập nhật người kiểm sát
        public void CapNhatNguoiKsoat(string macty, string idnhanvien, string idphien)
        {
            using (SqlConnection conn = new SqlConnection(Connectstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("usp_plv_update_nguoiksoat", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@macty", macty));
                cmd.Parameters.Add(new SqlParameter("@idnhanvien", idnhanvien));
                cmd.Parameters.Add(new SqlParameter("@idphien", idphien));

                string output = (string)cmd.ExecuteScalar();

                conn.Close();
            }
        }

        //Lấy danh sách đơn vị All
        public static string getAllDonviNPC(string madvi)
        {
            string html = "";
            var connection = ConfigurationManager.ConnectionStrings["DbNpcContext"].ConnectionString;
            List<tblDonVi> lstDonvi = new List<tblDonVi>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from v_donvi_khac", conn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tblDonVi a = new tblDonVi();
                    a.Id = rdr["Id"] == null ? "" : rdr["Id"].ToString();
                    a.TenDonVi = rdr["TenDonVi"] == null ? "" : rdr["TenDonVi"].ToString();
                    a.DviCha = rdr["DviCha"] == null ? "" : rdr["DviCha"].ToString();

                    lstDonvi.Add(a);
                }
                conn.Close();
            }

            foreach (var item in lstDonvi)
            {
                html += "<option value=" + item.Id + " " + (item.Id == madvi ? "selected" : "") + " >" + item.TenDonVi.Trim() + "</option>";
            }

            return html;
        }

    }
}
