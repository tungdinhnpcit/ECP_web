using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace ECP_V2.WebApplication.Helpers
{
    public static class MailHelper
    {
        public static void SendMail(string userName, string password, string receiver, string title, string html)
        {
            try
            {
                using (MailMessage mm = new MailMessage(userName, receiver))
                {
                    mm.Subject = title;
                    mm.Body = html;
                    mm.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(userName, password);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public static void SendMailWithAttachment(string userName, string password, string receiver, string title, string html, Byte[] file, string fileName)
        {
            try
            {
                using (MailMessage mm = new MailMessage(userName, receiver))
                {
                    using (MemoryStream ms = new MemoryStream(file))
                    {
                        mm.Subject = title;
                        mm.Body = html;
                        mm.IsBodyHtml = true;
                        mm.Attachments.Add(new Attachment(ms, fileName));

                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential NetworkCred = new NetworkCredential(userName, password);
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}