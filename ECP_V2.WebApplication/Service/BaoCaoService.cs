
using ECP_V2.WebApplication.Models;

using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;


namespace ECP_V2.WebApplication.BaoCaoService
{
    public interface IBaoCaoService
    {

        ActionResult ExportDocKeHoachLichLamViec(int tcphien, int catdien, int tiepdia,
            int khac, string DateFrom, string DateTo, string DonViId, string PhongBanId, int ttPhien, int? chuyenNPC, int? phieuky);
    }

    public class BaoCaoService : IBaoCaoService
    {

        public BaoCaoService()
        {
            //ComponentInfo.SetLicense("DLAP-Y2U7-5NKR-T5W1");
            //SpreadsheetInfo.SetLicense("ENC9-7SS5-JYS9-O0TK");
        }
        public ActionResult ExportDocKeHoachLichLamViec(int tcphien, int catdien, int tiepdia,
            int khac, string DateFrom, string DateTo, string DonViId, string PhongBanId, int ttPhien, int? chuyenNPC, int? phieuky)
        {
            try
            {
                int ngayht = DateTime.Now.Day;
                int thanght = DateTime.Now.Month;
                int namht = DateTime.Now.Year;

                string sWebRootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/");

                var path = Path.Combine(sWebRootFolder, "report_template", "ExportDocKeHoachLichLamViec.docx");
               // var document = DocumentModel.Load(path);
              //  document.DefaultCharacterFormat.FontName = "Times New Roman";
              //  document.DefaultCharacterFormat.Size = 11;
                //document.MailMerge.FieldMappings.Contains
                var Thongtingiaoca = new
                {
                    ngayht = ngayht,
                    thanght = thanght,
                    namht = namht,

                };

             //   document.MailMerge.Execute(Thongtingiaoca);



                //var model = new ExportModelDoc();
                //model.Format = "DOCX";
                //var stream = new MemoryStream();
           //     document.Save(stream, model.Options);

                // Download file.
                // return Task.FromResult<dynamic>(File(stream, model.Options.ContentType, $"BC_VTDRVHHT_{thanght}_{namht}.{model.Format.ToLower()}"));

           
                //// Save the Excel spreadsheet to a MemoryStream and return it to the client
                //using (var exportData = new MemoryStream())
                //{
                //    Response.Clear();
                //    workbook.Write(exportData);
                //    string strFileName = "";
                //    if (donviId == null)
                //    {
                //        strFileName = string.Format("Ctybc-LLV.Tuan_{0}.xlsx", DateTime.Now).Replace("/", "-");
                //    }
                //    else
                //    {
                //        strFileName = string.Format("Dvibc-LLV.Tuan_{0}.xlsx", DateTime.Now).Replace("/", "-");
                //    }
                //    string saveAsFileName = strFileName;

                //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                //    //Response.BinaryWrite(exportData.GetBuffer());
                //    Response.BinaryWrite(exportData.ToArray());
                //    Response.End();
                //}
                return null;

            }
            catch
            {
                return null;
            }
        }
    }

}