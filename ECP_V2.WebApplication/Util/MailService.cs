using System.Net;
using System.Net.Mail;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ECP_V2.WebApplication.Util
{
    public class MailService
    {
        //Send mail: success return string.empty
        public string SendMail(string toEmail, string ccEmail, string bccEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail))
                return "Invalid to email. To email is required";
            //Get config email
            string fromEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password))
                return string.Format("Invalid fromm email, pass config: {0}||{1}", fromEmail, password);
            //Mail service
            const string smtpAddress = "smtp.gmail.com";
            const int portNumber = 587;
            const bool enableSsl = true;
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                if(!string.IsNullOrEmpty(ccEmail))
                    mail.CC.Add(ccEmail);
                if(!string.IsNullOrEmpty(bccEmail))
                    mail.Bcc.Add(bccEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.
                using (var smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, password);
                    smtp.EnableSsl = enableSsl;
                    smtp.Send(mail);
                }
            }
            //send success
            return string.Empty;
        }


        private string ZipFiles(List<string> files, string zipfilename)
        {
            try
            {
                string SaveAs = HttpContext.Current.Server.MapPath("~/Content/FilesAtt/Result");
                string sZipFileName = zipfilename;
                string sZipFileNameTemp = SaveAs + "\\" + zipfilename + "_Temp";

                if (!System.IO.Directory.Exists(sZipFileNameTemp))
                {
                    System.IO.Directory.CreateDirectory(sZipFileNameTemp);
                }
                string[] validExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".doc", ".docx", ".xls", ".xlsx" };

                // Process the files and move each into the target folder
                for (int i = 0; i < files.Count(); i++)
                {
                    string filePath = files[i];
                    FileInfo fi2 = new FileInfo(filePath);
                    if (fi2.Exists)
                    {
                        try
                        {
                            string fileExtension = fi2.Extension.ToLower();
                            if (!validExtensions.Contains(fileExtension))
                            {
                                System.Diagnostics.Debug.WriteLine("Invalid file format: " + filePath);
                                continue; 
                            }
                            fi2.CopyTo(sZipFileNameTemp + "\\" + fi2.Name, true);
                        }
                        catch
                        {
                            System.IO.Directory.Delete(sZipFileNameTemp);
                        }
                    }
                }

                try
                {
                    string[] filenames = Directory.GetFiles(sZipFileNameTemp);

                    // Zip up the files - From SharpZipLib Demo Code
                    using (ZipOutputStream s = new ZipOutputStream(File.Create(SaveAs + "\\" + sZipFileName + ".zip")))
                    {
                        s.SetLevel(9); // 0-9, 9 being the highest level of compression

                        byte[] buffer = new byte[4096];

                        foreach (string file in filenames)
                        {

                            ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                            entry.DateTime = DateTime.Now;
                            s.PutNextEntry(entry);

                            using (FileStream fs = File.OpenRead(file))
                            {
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);

                                } while (sourceBytes > 0);
                            }
                        }
                        s.Finish();
                        s.Close();
                    }

                    System.IO.Directory.Delete(sZipFileNameTemp, true);
                }
                catch
                {

                }
                return "/Content/FilesAtt/Result/" + sZipFileName + ".zip";
            }
            catch
            {
                return null;
            }
        }
    }
}