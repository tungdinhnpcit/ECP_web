using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebImportNhanVien
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            //string donVi1 = "PA01,PA02,PA03,PA04,PA05,PA07,PA09,PA10,PA11,PA12,PA13,PA14,PA15,PA16,PA17,PA18,PA19,PA20,PA22,PA23,PA24,PA25,PA26,PA27,PA29,PM,PN,PH";
            //var listDonVi1 = donVi1.Split(',').ToList();
            //DataClasses1DataContext context1 = null;
            //foreach (var item1 in listDonVi1)
            //{
            //    context1 = new DataClasses1DataContext("data source=103.63.109.191;initial catalog=ECP_" + item1 + ";persist security info=True;user id=sa;password=Vnittech2018;");
            //    var donviCha = context1.tblDonVis.FirstOrDefault(x=>x.Id.Length <= 4);
            //    donviCha.DviCha = "PA";
            //    donviCha.CapDvi = 0;
            //    donviCha.TenDonVi = donviCha.TenDonVi.ToUpper();

            //    var donviCon = context1.tblDonVis.Where(x => x.Id.Length > 4);
            //    foreach (var item2 in donviCon)
            //    {
            //        item2.TenDonVi = item2.TenDonVi.ToUpper();
            //        item2.DviCha = donviCha.Id;
            //        item2.MaLP = 1;
            //        item2.CapDvi = 1;
            //    }
            //    context1.SubmitChanges();
            //}
            //return;

            foreach (string fName in Request.Files)
            {
                System.Web.HttpPostedFile f = Request.Files[fName];
                UploadExcel(f);
            }
        }

        void UploadExcel(HttpPostedFile file)
        {
            StringBuilder strErrorSum = new StringBuilder();
            DataSet dsFullTable = new DataSet();
            DataSet ds = new DataSet();
            int countDonVi = 0;
            int countPhongBan = 0;
            int countNhanVien = 0;
            int nullCount = 0;

            if (!string.IsNullOrEmpty(file.FileName))
            {
                string TenPhongBan = null;
                string TenDonVi = null;
                string MaDonVi = null;
                string DBName = null;
                int MaPhongBan = 0;
                string MaDonViCha = null;
                string fileExtension = System.IO.Path.GetExtension(file.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string strErrExcel = "";
                    this.ReadFileExcel(file, ref ds, out strErrExcel);
                    strErrorSum.Append(strErrExcel);
                }

                if (ds.Tables[0] != null)
                {
                    string roleID = "5";
                    int dem = 0;
                    string donVi = "PA01,PA02,PA03,PA04,PA05,PA07,PA09,PA10,PA11,PA12,PA13,PA14,PA15,PA16,PA17,PA18,PA19,PA20,PA22,PA23,PA24,PA25,PA26,PA27,PA29,PM,PN,PH";
                    string anToan = "KTAT,KTVAT,KT-AT,KTV-AT,KTVAT,AN TOÀN,AN TOAN,ANTOAN,ANTOÀN";
                    var listDonVi = donVi.Split(',').ToList();
                    var listAnToan = anToan.Split(',').ToList();

                    string donViPA = ds.Tables[0].Rows[0][2].ToString();
                    DataClasses1DataContext context = null;


                    for (int i = 4; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (i == 118)
                        {

                        }
                        dem++;
                        string firstText = ds.Tables[0].Rows[i][0].ToString();
                        string firstText1 = ds.Tables[0].Rows[i][1].ToString();
                        if (string.IsNullOrEmpty(firstText) || string.IsNullOrEmpty(firstText.Trim()))
                        {
                            //Lỗi hoặc kết thúc
                            nullCount++;
                            if (nullCount > 3)
                            {
                                Response.Write("Thành công! " + countDonVi + " đơn vị " + countPhongBan + " phòng ban " + countNhanVien + " nhân viên");
                                return;
                            }
                        }
                        firstText = firstText.Trim();

                        if (Regex.IsMatch(firstText, @"^\d+$"))
                        {
                            //Là số
                            if (dem == 1 && (MaDonVi == null || MaPhongBan == 0))
                            {
                                //Lỗi xác định Đơn vị, Phòng ban
                                Response.Write((i + 3) + " Lỗi xác định Đơn vị, Phòng ban");
                                return;
                            }
                            else
                            {
                                Random random = new Random();
                                int randomNumber = random.Next(1111, 9999);

                                tblNhanVien nhanVien = new tblNhanVien();
                                nhanVien.Id = "190546bf-0248-483d-" + MaDonVi + "-t" + randomNumber + DateTime.Now.ToString("mmssfff");
                                nhanVien.PhongBanId = MaPhongBan;
                                nhanVien.DonViId = MaDonVi;
                                nhanVien.TenNhanVien = ds.Tables[0].Rows[i][1].ToString().Trim();
                                string ngaysinh = ds.Tables[0].Rows[i][2].ToString();
                                if (!string.IsNullOrEmpty(ngaysinh))
                                {
                                    try
                                    {
                                        nhanVien.NgaySinh = DateTime.ParseExact(ngaysinh.Trim(), "dd/MM/yyyy", null);
                                    }
                                    catch
                                    { }
                                }
                                string email = ds.Tables[0].Rows[i][3].ToString();
                                if (!string.IsNullOrEmpty(email) && email.IndexOf("@") > 0)
                                {
                                    nhanVien.Email = email.Trim().ToLower();
                                }

                                string phone1 = ds.Tables[0].Rows[i][4].ToString();
                                if (!string.IsNullOrEmpty(phone1) && phone1.Trim().Length > 9)
                                {
                                    nhanVien.SoDT = phone1.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("-", "").Trim();
                                }

                                string chucvu = ds.Tables[0].Rows[i][7].ToString();
                                if (!string.IsNullOrEmpty(chucvu))
                                {
                                    nhanVien.ChucVu = chucvu.Trim();
                                }

                                string username = GenerateLink(MaDonViCha, nhanVien.TenNhanVien);
                                int usernameIndex = 0;
                                while (context.tblNhanViens.Any(x => x.Username == username))
                                {
                                    usernameIndex++;
                                    username = GenerateLink(MaDonViCha, nhanVien.TenNhanVien + usernameIndex);
                                }
                                nhanVien.Username = username;
                                if (nhanVien.NgaySinh != null && nhanVien.NgaySinh.Year < 1920)
                                {
                                    nhanVien.NgaySinh = new DateTime(1900, 1, 1);
                                }
                                //
                                AspNetUser aspUser = new AspNetUser();
                                aspUser.Id = nhanVien.Id;
                                aspUser.Email = nhanVien.Email;
                                aspUser.PhoneNumber = nhanVien.SoDT;
                                aspUser.UserName = nhanVien.Username;
                                aspUser.EmailConfirmed = false;
                                aspUser.PhoneNumberConfirmed = false;
                                aspUser.TwoFactorEnabled = false;
                                aspUser.LockoutEnabled = false;
                                aspUser.AccessFailedCount = 0;
                                aspUser.PasswordHash = "AEqW+gzb83PZaoZZGFjqfWV0vB61LQbGhV8mZ7z0qDpAr6vUdgGlwSRUdQ0rRtraYQ==";
                                aspUser.SecurityStamp = "dff6b794-bcac-40ca-a461-27d4ae577abc";

                                context.tblNhanViens.InsertOnSubmit(nhanVien);
                                context.AspNetUsers.InsertOnSubmit(aspUser);
                                context.SubmitChanges();
                                //add roles
                                AspNetUserRole aspUserRole = new AspNetUserRole();
                                aspUserRole.RoleId = roleID;
                                aspUserRole.UserId = nhanVien.Id;
                                context.AspNetUserRoles.InsertOnSubmit(aspUserRole);
                                context.SubmitChanges();

                                countNhanVien++;
                            }
                        }
                        else
                        {
                            //Là chữ
                            string Ma = ds.Tables[0].Rows[i][7].ToString();
                            if (!string.IsNullOrEmpty(Ma))
                            {
                                Ma = Ma.Replace("\n", "").Replace("\t", "").Trim();
                            }
                            TenDonVi = firstText;
                            if (dem == 1)
                            {
                                //Đơn vị                                
                                if ((!string.IsNullOrEmpty(Ma) && !string.IsNullOrEmpty(Ma.Trim())) && listDonVi.Any(x => x == Ma))
                                {
                                    Ma = Ma.ToUpper();
                                    MaPhongBan = 0;
                                    MaDonVi = Ma;
                                    DBName = "ECP_" + Ma;
                                    MaDonViCha = Ma;
                                }
                                else
                                {
                                    Ma = donViPA.ToUpper();
                                    MaPhongBan = 0;
                                    MaDonVi = Ma;
                                    DBName = "ECP_" + Ma;
                                    MaDonViCha = Ma;
                                }

                                context = new DataClasses1DataContext("data source=103.63.109.191;initial catalog=" + DBName + ";persist security info=True;user id=sa;password=Vnittech2018;");
                                //Xóa Data cũ
                                context.ExecuteCommand("DELETE FROM Messages");
                                context.ExecuteCommand("DELETE FROM tbl_NhanVien_PhienLamViec");
                                context.ExecuteCommand("DELETE FROM Logs");
                                context.ExecuteCommand("DELETE FROM [Logs]");
                                context.ExecuteCommand("DELETE FROM [AspNetUserRoles]");
                                context.ExecuteCommand("DELETE FROM [AspNetUsers]");
                                context.ExecuteCommand("DELETE FROM [tblDevice]");
                                context.ExecuteCommand("DELETE FROM [tblComment]");
                                context.ExecuteCommand("DELETE FROM [PhieuLamViec_Images]");
                                context.ExecuteCommand("DELETE FROM [plv_PhieuCongTac]");
                                context.ExecuteCommand("DELETE FROM [plv_TaiLieu]");
                                context.ExecuteCommand("DELETE FROM [tblBienBanKiemTra]");
                                context.ExecuteCommand("DELETE FROM [tblChiTietBaoCao]");
                                context.ExecuteCommand("DELETE FROM [tblBaoCao]");
                                context.ExecuteCommand("DELETE FROM [tblChiTietBaoCaoCuoiNgay]");
                                context.ExecuteCommand("DELETE FROM [tblBaoCaoCuoiNgay]");
                                context.ExecuteCommand("DELETE FROM [tblImage]");
                                context.ExecuteCommand("DELETE FROM [tblPhienLamViec_TheoDoi]");
                                context.ExecuteCommand("DELETE FROM [tblPhienLamViec_ThuocTinh]");
                                context.ExecuteCommand("DELETE FROM [tblPhienLamViec]");
                                context.ExecuteCommand("DELETE FROM [tblNhanVien]");
                                context.ExecuteCommand("DELETE FROM [tblPhongBan]");

                                #region Insert Admin
                                tblNhanVien nhanVien = new tblNhanVien();
                                nhanVien.Id = "1905f6bf-02d8-489d-" + MaDonVi + "-t" + DateTime.Now.ToString("ddHHmmssfff");
                                nhanVien.DonViId = MaDonVi;
                                nhanVien.TenNhanVien = "Administrator";
                                nhanVien.NgaySinh = new DateTime(1975, 1, 1);
                                nhanVien.Email = "admin@kiemsoatantoan.com";
                                nhanVien.SoDT = "0962588450";
                                nhanVien.ChucVu = "Administrator";
                                nhanVien.Username = MaDonViCha.ToLower() + "_admin";
                                //
                                AspNetUser aspUser = new AspNetUser();
                                aspUser.Id = nhanVien.Id;
                                aspUser.Email = nhanVien.Email;
                                aspUser.PhoneNumber = nhanVien.SoDT;
                                aspUser.UserName = nhanVien.Username;
                                aspUser.EmailConfirmed = false;
                                aspUser.PhoneNumberConfirmed = false;
                                aspUser.TwoFactorEnabled = false;
                                aspUser.LockoutEnabled = false;
                                aspUser.AccessFailedCount = 0;
                                aspUser.PasswordHash = "AEqW+gzb83PZaoZZGFjqfWV0vB61LQbGhV8mZ7z0qDpAr6vUdgGlwSRUdQ0rRtraYQ==";
                                aspUser.SecurityStamp = "dff6b794-bcac-40ca-a461-27d4ae577abc";

                                context.tblNhanViens.InsertOnSubmit(nhanVien);
                                context.AspNetUsers.InsertOnSubmit(aspUser);
                                context.SubmitChanges();
                                //add roles
                                AspNetUserRole aspUserRole = new AspNetUserRole();
                                aspUserRole.RoleId = "1";
                                aspUserRole.UserId = nhanVien.Id;
                                context.AspNetUserRoles.InsertOnSubmit(aspUserRole);
                                context.SubmitChanges();
                                #endregion

                            }
                            else
                            {
                                //Đơn vị Huyện
                                if (!string.IsNullOrEmpty(Ma) && !string.IsNullOrEmpty(Ma.Trim()))
                                {
                                    Ma = Ma.ToUpper();
                                    MaPhongBan = 0;
                                    MaDonVi = Ma;
                                    tblDonVi donViObj = context.tblDonVis.FirstOrDefault(x => x.Id == MaDonVi);
                                    if (donViObj != null)
                                    {
                                        TenDonVi = donViObj.TenDonVi;
                                        countDonVi++;
                                    }
                                    else
                                    {
                                        //Mã đơn vị MaDonVi không tồn tại
                                        if (MaDonVi.IndexOf("PX") > 0 || MaDonVi.IndexOf("TN") > 0)
                                        {
                                            tblDonVi donViAdd = new tblDonVi();
                                            donViAdd.Id = MaDonVi;
                                            donViAdd.TenDonVi = TenDonVi.Replace("\n", "").Replace("\t", "").Trim();
                                            donViAdd.ViTri = 30;
                                            donViAdd.MaLP = 1;
                                            donViAdd.CapDvi = 1;
                                            string getName = donViAdd.TenDonVi.ToLower().Replace("điện lực", "").Replace("cty", "").Replace("công ty", "").Replace("  ", " ").Replace("  ", " ").Trim();
                                            donViAdd.TenVietTat = new string(getName.Split(' ').Select(s => s[0]).ToArray());
                                            donViAdd.DviCha = MaDonViCha;

                                            context.tblDonVis.InsertOnSubmit(donViAdd);
                                            context.SubmitChanges();
                                        }
                                        else
                                        {
                                            Response.Write((i + 3) + " Mã đơn vị MaDonVi không tồn tại");
                                            return;
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(firstText))
                                {
                                    //Phòng Ban
                                    TenPhongBan = firstText.Replace("\n", "").Replace("\t", "").Trim().ToUpper();
                                    tblPhongBan phongBan = new tblPhongBan();
                                    phongBan.MaDVi = MaDonVi;
                                    phongBan.MoTa = TenDonVi;
                                    phongBan.TenPhongBan = TenPhongBan;
                                    string getName = TenPhongBan.ToLower().Replace("điện lực", "").Replace("cty", "").Replace("công ty", "").Replace("  ", " ").Replace("  ", " ").Trim();
                                    phongBan.TenVietTat = new string(getName.Split(' ').Select(s => s[0]).ToArray());

                                    context.tblPhongBans.InsertOnSubmit(phongBan);
                                    context.SubmitChanges();
                                    MaPhongBan = phongBan.Id;

                                    countPhongBan++;

                                    roleID = "5";
                                    if (phongBan.TenPhongBan.IndexOf("LÃNH ĐẠO") > -1 || phongBan.TenPhongBan.IndexOf("LANH DAO") > -1 || phongBan.TenPhongBan.IndexOf("GIÁM ĐỐC") > -1 || phongBan.TenPhongBan.IndexOf("GIAM DOC") > -1)
                                    {
                                        roleID = "2";
                                    }
                                    else if (listAnToan.Any(x => phongBan.TenPhongBan.Replace(" ", "").Contains(x)))
                                    {
                                        roleID = "4";
                                    }
                                }
                                else
                                {
                                    //Lỗi không xác định Phòng Ban
                                    Response.Write((i + 3) + " Lỗi không xác định Phòng Ban");
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Không đọc được
                }
            }
            Response.Write(" \n Thành công! " + countDonVi + " đơn vị " + countPhongBan + " phòng ban " + countNhanVien + " nhân viên");
        }

        public void ReadFileExcel(HttpPostedFile file, ref DataSet ds, out string strError)
        {
            strError = "";
            string fileExtension = System.IO.Path.GetExtension(file.FileName);

            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                string fileLocation = Server.MapPath("~/Content/") + file.FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    try
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    catch (Exception ex)
                    {
                        strError = "Thêm phiên làm việc không thành công: " + ex.Message;
                    }
                }
                file.SaveAs(fileLocation);
                string excelConnectionString = string.Empty;
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                if (fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                //connection String for xlsx file format.
                else if (fileExtension == ".xlsx")
                {

                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                //Create Connection to Excel work book and add oledb namespace
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                excelConnection.Close();
                if (dt == null)
                {
                    return;
                }

                List<string> excelSheets = new List<string>();

                //excel data saves in temp file here.
                foreach (DataRow row in dt.Rows)
                {
                    if (!row["TABLE_NAME"].ToString().ToLower().Contains("print"))
                    {
                        excelSheets.Add(row["TABLE_NAME"].ToString());
                    }
                }
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                foreach (string sheetItem in excelSheets)
                {
                    string queryData = string.Format("Select * from [{0}]", sheetItem);
                    DataTable tbl = new DataTable();
                    tbl.TableName = sheetItem.Substring(0, sheetItem.Length - 1);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(queryData, excelConnection1))
                    {
                        if (!ds.Tables.Contains(tbl.TableName) && !tbl.TableName.ToString().Contains("Print_Title"))
                        {
                            dataAdapter.Fill(tbl);
                            ds.Tables.Add(tbl);
                        }
                    }
                }
            }
        }
        public static string GenerateLink(string beurl, object Title)
        {
            string strTitle = Change_AV(Title.ToString());

            #region Generate SEO Friendly URL based on Title
            //Trim Start and End Spaces.
            strTitle = strTitle.Trim();

            //Trim "-" Hyphen
            strTitle = strTitle.Trim('-');

            strTitle = strTitle.ToLower();
            char[] chars = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
            strTitle = strTitle.Replace("c#", "C-Sharp");
            strTitle = strTitle.Replace("vb.net", "VB-Net");
            strTitle = strTitle.Replace("asp.net", "Asp-Net");

            //Replace . with - hyphen
            strTitle = strTitle.Replace(".", "-");

            //Replace Special-Characters
            for (int i = 0; i < chars.Length; i++)
            {
                string strChar = chars.GetValue(i).ToString();
                if (strTitle.Contains(strChar))
                {
                    strTitle = strTitle.Replace(strChar, string.Empty);
                }
            }
            //Replace all spaces with one "-" hyphen
            strTitle = strTitle.Replace(" ", "-");

            //Replace multiple "-" hyphen with single "-" hyphen.
            strTitle = strTitle.Replace("--", "");
            strTitle = strTitle.Replace("---", "");
            strTitle = strTitle.Replace("----", "");
            strTitle = strTitle.Replace("-----", "");
            strTitle = strTitle.Replace("----", "");
            strTitle = strTitle.Replace("---", "");
            strTitle = strTitle.Replace("--", "");
            strTitle = strTitle.Replace("-", "");

            //Run the code again...
            //Trim Start and End Spaces.
            strTitle = strTitle.Trim();

            //Trim "-" Hyphen
            strTitle = strTitle.Trim('-');
            #endregion

            //Append ID at the end of SEO Friendly URL
            strTitle = beurl.ToLower() + "_" + strTitle;

            return strTitle;
        }
        public static string Change_AV(string ip_str_change)
        {
            Regex v_reg_regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string v_str_FormD = ip_str_change.Normalize(NormalizationForm.FormD);
            return v_reg_regex.Replace(v_str_FormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}