using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Web.Configuration;

namespace ECP_V2.Common.Helpers
{
    public class BackUpRestoreDataBaseHelper
    {
        static string strcon = WebConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;
        public BackUpRestoreDataBaseHelper(string strconnect)
        {
            strcon = strconnect;
        }

        static SqlConnection sqlcon;
        static SqlCommand sqlcommand;

        static private void open()
        {
            if (sqlcon == null)
            {
                sqlcon = new SqlConnection(strcon);
            }
            if (sqlcon.State != ConnectionState.Open)
                sqlcon.Open();
        }

        static private void close()
        {
            sqlcon.Close();
        }

        static private bool exec(string strcommand)
        {
            try
            {
                open();
                sqlcommand = new SqlCommand(strcommand, sqlcon);
                sqlcommand.ExecuteNonQuery();
                close();
                sqlcommand.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                close();
                return false;
            }
        }

        public static bool AttachDataBase(string DataBaseName, string path)
        {
            try
            {
                ServerConnection conn = new ServerConnection();
                conn.ServerInstance = ConfigurationManager.ConnectionStrings["IdentityDbContext"].ConnectionString;
                conn.LoginSecure = false;
                conn.Login = ConfigurationManager.ConnectionStrings["user"].ConnectionString;
                conn.Password = ConfigurationManager.ConnectionStrings["password"].ConnectionString;
                //khoi tao đoi tương Server
                Server sQLServer = new Server(conn);
                GrantAccessFile(path + DataBaseName + "_Data.mdf");
                GrantAccessFile(path + DataBaseName + "_log.ldf");
                //atach
                System.Collections.Specialized.StringCollection strcolect = new System.Collections.Specialized.StringCollection();
                strcolect.Add(path + DataBaseName + "_Data.mdf");
                strcolect.Add(path + DataBaseName + "_log.ldf");
                sQLServer.AttachDatabase(DataBaseName, strcolect, AttachOptions.None);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DetachDataBase(string DataBaseName)
        {
            try
            {
                ServerConnection conn = new ServerConnection(WebConfigurationManager.ConnectionStrings["server"].ConnectionString);
                conn.LoginSecure = false;
                conn.Login = WebConfigurationManager.ConnectionStrings["user"].ConnectionString;
                conn.Password = WebConfigurationManager.ConnectionStrings["password"].ConnectionString;
                //khoi tao đoi tương Server
                Server sQLServer = new Server(conn);
                //huy tat cac cac dich vu dang hoat dong tren DataBase
                try
                {
                    sQLServer.KillAllProcesses(DataBaseName);
                }
                catch (Exception ex) { };
                sQLServer.DetachDatabase(DataBaseName, false);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Boolean CopyFile_In_Forder(string SFolder, string targetFolder)
        {
            try
            {
                string[] filePaths = System.IO.Directory.GetFiles(SFolder);
                foreach (String fileName in filePaths)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
                    fi.CopyTo(System.IO.Path.Combine(targetFolder, fi.Name), true);
                }
                return true;
            }
            catch { }
            return false;
        }
        static public bool CoppyFile(string sourceFile, string destFile)
        {
            try
            {
                GrantAccessFile(sourceFile);
                System.IO.FileInfo fi = new System.IO.FileInfo(destFile);
                if (fi.Exists)
                    GrantAccessFile(destFile);
                System.IO.File.Copy(sourceFile, destFile, true);
                return true;
            }
            catch { return false; }
        }
        static public bool CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                string name;
                string dest;
                if (!System.IO.Directory.Exists(destFolder))
                    System.IO.Directory.CreateDirectory(destFolder);
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    name = System.IO.Path.GetFileName(file);
                    dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);
                }
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    name = System.IO.Path.GetFileName(folder);
                    dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
                return true;
            }
            catch { return false; }
        }

        public static void GrantAccessFile(string FullPathFile)
        {
            System.Security.AccessControl.FileSecurity fs = new System.Security.AccessControl.FileSecurity(FullPathFile, System.Security.AccessControl.AccessControlSections.None);
            System.Security.AccessControl.FileSystemAccessRule fr = new System.Security.AccessControl.FileSystemAccessRule("everyone", System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.InheritanceFlags.None, System.Security.AccessControl.PropagationFlags.None, System.Security.AccessControl.AccessControlType.Deny);
            fs.RemoveAccessRuleAll(fr);
            System.IO.FileInfo fi = new System.IO.FileInfo(FullPathFile);
            fi.SetAccessControl(fs);
        }

        public static bool Backup(string DataBaseName, string path_Src, string pathContainDB_coppy)
        {
            try
            {
                bool kt = DetachDataBase(DataBaseName);

                GrantAccessFile(path_Src + "App_Data\\" + DataBaseName + "_Data.mdf");
                GrantAccessFile(path_Src + "App_Data\\" + DataBaseName + "_log.ldf");

                // copy 2file DataBase sang thư mục BackUp          
                System.IO.File.Copy(path_Src + "App_Data\\" + DataBaseName + "_Data.mdf", pathContainDB_coppy + "DataBaseFile\\" + DataBaseName + "_Data.mdf");
                System.IO.File.Copy(path_Src + "App_Data\\" + DataBaseName + "_log.ldf", pathContainDB_coppy + "DataBaseFile\\" + DataBaseName + "_log.ldf");
                //
                kt = AttachDataBase(DataBaseName, path_Src + "\\App_Data\\");
                //coppy file anh vào thư mục BackUp
                //Copy Image files
                CopyFolder(path_Src + "\\student\\Images\\Questions", pathContainDB_coppy + "Images");
                //setting.xml
                return kt;
            }
            catch (Exception ex)
            {
                AttachDataBase(DataBaseName, path_Src + "\\App_Data\\");
                return false;
            }
        }

        public static bool Backup2(string DataBaseName, string path_Src, string pathContainDB_coppy, bool LoadImage)
        {
            try
            {
                string strcomamnd = "BACKUP DATABASE " + DataBaseName + "  TO DISK = '" + pathContainDB_coppy + "DataBaseFile\\" + DataBaseName + ".bak';";
                bool kt = exec(strcomamnd);
                //Coppy file setting.xml
                CoppyFile(path_Src + "\\setting.xml", pathContainDB_coppy + "SettingXML\\setting.xml");
                //Copy Image files
                if (LoadImage)
                    CopyFolder(path_Src + "\\student\\Images\\Questions", pathContainDB_coppy + "Images");
                return kt;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Backup3(string DataBaseName, string pathContainDB_coppy, string pathCompress)
        {
            try
            {
                string fileName = DataBaseName + "_" + DateTime.Now.ToString("dd-MM-yyyy_hhmmss_tt");
                string filePath = pathContainDB_coppy + "DataBaseFile\\" + fileName + ".bak";

                string strcomamnd = "BACKUP DATABASE " + DataBaseName + "  TO DISK = '" + filePath + "';";
                bool kt = exec(strcomamnd);

                if (kt)
                {
                    compressDirectory(pathContainDB_coppy, fileName, pathCompress);
                }

                return kt;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Restore(string DataBaseName, string path_Src, string pathContainDB_coppy)
        {
            try
            {
                bool kt = DetachDataBase(DataBaseName);

                GrantAccessFile(path_Src + "\\App_Data\\" + DataBaseName + "_Data.mdf");
                GrantAccessFile(path_Src + "\\App_Data\\" + DataBaseName + "_log.ldf");

                // copy 2file DataBase sang thư mục BackUp          
                System.IO.File.Copy(pathContainDB_coppy + "\\DataBaseFile\\" + DataBaseName + "_Data.mdf", path_Src + "\\App_Data\\" + DataBaseName + "_Data.mdf", true);
                System.IO.File.Copy(pathContainDB_coppy + "\\DataBaseFile\\" + DataBaseName + "_log.ldf", path_Src + "\\App_Data\\" + DataBaseName + "_log.ldf", true);
                //
                kt = AttachDataBase(DataBaseName, path_Src + "\\App_Data\\");
                //coppy file anh vào thư mục BackUp
                //Copy Image files
                CopyFolder(pathContainDB_coppy + "\\Images", path_Src + "\\student\\Images\\Questions");

                return kt;
            }
            catch (Exception ex)
            {
                AttachDataBase(DataBaseName, path_Src + "\\App_Data\\");
                return false;
            }
        }

        public static bool Restore2(string DataBaseName, string path_Src, string PathContainForderRestore)
        {
            try
            {
                ServerConnection conn = new ServerConnection(ConfigurationManager.ConnectionStrings["server"].ConnectionString);
                conn.LoginSecure = false;
                conn.Login = ConfigurationManager.ConnectionStrings["user"].ConnectionString;
                conn.Password = ConfigurationManager.ConnectionStrings["password"].ConnectionString;
                //khoi tao đoi tương Server
                Server sQLServer = new Server(conn);
                //huy tat cac cac dich vu dang hoat dong tren DataBase
                try
                {
                    string strAlter = "alter database " + DataBaseName + " set single_user with rollback immediate";
                    exec(strAlter);
                    sQLServer.KillAllProcesses(DataBaseName);
                }
                catch { };

                //System.Threading.Thread.Sleep(5000);

                string strcomamnd = "Use master; RESTORE DATABASE " + DataBaseName + "  FROM DISK = '" + PathContainForderRestore + "\\DataBaseFile\\" + DataBaseName + ".bak' WITH REPLACE;";
                bool kt = exec(strcomamnd);
                //if (kt)
                //    CopyFolder(PathContainForderRestore + "\\Images", path_Src + "\\student\\Images\\Questions");
                //Coppy file setting.xml
                //CoppyFile(PathContainForderRestore + "\\SettingXML\\setting.xml", path_Src + "\\setting.xml");
                return kt;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Restore3(string DataBaseName, string path_SrcFile)
        {
            try
            {
                ServerConnection conn = new ServerConnection(WebConfigurationManager.ConnectionStrings["server"].ConnectionString);
                conn.LoginSecure = false;
                conn.Login = WebConfigurationManager.ConnectionStrings["user"].ConnectionString;
                conn.Password = WebConfigurationManager.ConnectionStrings["password"].ConnectionString;
                //khoi tao đoi tương Server
                Server sQLServer = new Server(conn);
                //huy tat cac cac dich vu dang hoat dong tren DataBase

                //bool kt = DetachDataBase(DataBaseName);

                try
                {
                    sQLServer.KillAllProcesses(DataBaseName);
                }
                catch { };
                string strcomamnd = "Use master; RESTORE DATABASE " + DataBaseName + "  FROM DISK = '" + path_SrcFile + "' WITH REPLACE;";
                bool kt = exec(strcomamnd);
                return kt;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool compressForder(string pathForder, string NameFileZip, string compress_Forder)
        {
            //Viet lenh nen file vao tep comand.cmd
            // đặt tên file mong muốn
            try
            {
                string filezipName = NameFileZip + ".zip";
                //tạo file command.cmd trong cùng thư muc nén
                System.IO.StreamWriter w = new System.IO.StreamWriter(pathForder + "\\command.cmd");
                //lấy tên ổ đĩa
                w.WriteLine(pathForder.Substring(0, 2));
                //
                w.WriteLine("cd " + pathForder + "\\");
                w.WriteLine("nenfile a " + filezipName + " " + compress_Forder);
                w.Close();
                System.Diagnostics.ProcessStartInfo i = new System.Diagnostics.ProcessStartInfo(pathForder + "\\command.cmd");
                i.CreateNoWindow = true;
                i.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(i);
                p.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UncompressForder(string pathForder, string NameFileZip, string compress_Forder)
        {
            try
            {
                string fileName = NameFileZip + ".zip";
                System.IO.StreamWriter w = new System.IO.StreamWriter(pathForder + "\\command.cmd");
                w.WriteLine(pathForder.Substring(0, 2));
                w.WriteLine("cd " + pathForder);
                w.WriteLine("nenfile x " + fileName);
                w.Close();
                System.Diagnostics.ProcessStartInfo i = new System.Diagnostics.ProcessStartInfo(pathForder + "\\command.cmd");
                i.CreateNoWindow = true;
                i.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(i);
                p.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool compressDirectory(string folder, string fileName, string otherFolder)
        {
            try
            {
                string zipFile = otherFolder + @"\" + fileName + ".zip";
                ZipFile.CreateFromDirectory(folder, zipFile, CompressionLevel.Optimal, true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool uncompressDirectory(string zipPath, string extractPath)
        {
            try
            {
                //string ZipPath = @"c:\my\data.zip";
                //string extractPath = @"d:\\myunzips";
                ZipFile.ExtractToDirectory(zipPath, extractPath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

