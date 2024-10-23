using CKSource.FileSystem;
using ECP_V2.Business.Repository;
using ECP_V2.Business.ViewModels.HLAT;
using ECP_V2.Common;
using ECP_V2.DataAccess;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    public class HLATController : Controller
    {
        SafeTrainRepository safeTraninRepo = new SafeTrainRepository();
        private DonViRepository _dv_ser = new DonViRepository();
        // GET: Admin/HLAT
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getTypeEdu()
        {
            var lstTypeEdu = new List<TypeEdu>();
            lstTypeEdu = safeTraninRepo.getTypeEdu();

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadGroupEdu(string typeid)
        {
            var lstTypeEdu = new List<GroupEdu>();
            lstTypeEdu = safeTraninRepo.LoadGroupEdu(typeid);

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadStatusClass()
        {
            string classid = Request.Form["classid"];
            var lstTypeEdu = new List<StatusClass>();
            lstTypeEdu = safeTraninRepo.LoadStatusClass(0, classid);

            return Json(lstTypeEdu, JsonRequestBehavior.AllowGet);
        }
        //Get danh mục đơn vị
        public JsonResult LoadDsOrg(string khoach, string loaidaotao, string nhomhl)
        {
            string madvql = Session["DonViID"].ToString();
            var lstOrg = new List<Organization>();
            lstOrg = safeTraninRepo.LoadDsOrgByKh(madvql, khoach, loaidaotao, nhomhl);

            return Json(lstOrg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsClass(string tungay, string denngay, string typeEdu, string groupEdu, string statusClass)
        {
            string madvql = Session["DonViID"].ToString();
            List<ClassTrain> classTrains = new List<ClassTrain>();

            classTrains = safeTraninRepo.LoadDsClass(tungay, denngay, typeEdu, groupEdu, statusClass, madvql).ToList();
            return Json(classTrains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDsPersonalByOrg(string madvql)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.LoadDsPersonal(madvql);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadListFileByClass(string classid)
        {            
            List<ClassFile> clsFiles = new List<ClassFile>();
            clsFiles = safeTraninRepo.LoadListFileByClass(classid);

            return Json(clsFiles, JsonRequestBehavior.AllowGet);
        }

        //Lấy danh sách file scan bài thi
        public JsonResult LoadListFileByExam(string examid)
        {
            List<ExamFile> clsFiles = new List<ExamFile>();
            if(examid == null) { examid = ""; }
            clsFiles = safeTraninRepo.LoadListFileByExam(examid);

            return Json(clsFiles, JsonRequestBehavior.AllowGet);
        }


        //Save lớp học
        [HttpPost]
        public ActionResult OnSaveClass()
        {
            try
            {
                //Lấy các biến
                string classid = Request.Form["classid"];
                string donvi = Session["DonViID"].ToString();
                string dvdaotao = Request.Form["dvdaotao"];
                string loailop = Request.Form["loailop"];
                string mahieu = Request.Form["mahieu"];
                string loaidaotao = Request.Form["loaidaotao"];
                string nhomhluyen = Request.Form["nhomhluyen"];
                //string khbatdau = Request.Form["khbatdau"];
                //string khketthuc = Request.Form["khketthuc"];
                string thbatdau = Request.Form["thbatdau"];
                string thketthuc = Request.Form["thketthuc"];
                string lstnhansu = Request.Form["lstnhansu"];

                string nguoitao = Session["UserName"].ToString();
                string ngayky = Request.Form["ngaykys"];
                //1. Insert class nhé
                string x = safeTraninRepo.InsertClass(donvi, dvdaotao, loailop, mahieu, loaidaotao, nhomhluyen, thbatdau, thketthuc, lstnhansu, nguoitao, classid);

                //2. Có x thì insert file
                HttpFileCollectionBase files = Request.Files;
                string FilePath = "";


                if (ConfigurationManager.AppSettings.Get("URL_FILE_HLAT") != null)
                    FilePath = ConfigurationManager.AppSettings.Get("URL_FILE_HLAT").ToString();
                string[] arrNgay = ngayky.Split(';');
                string outx = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname = file.FileName;
                    fname = System.IO.Path.Combine(FilePath, fname);
                    file.SaveAs(fname);
                    if (files.Count == arrNgay.Length - 1)
                    {
                        outx = safeTraninRepo.InsertClassFile(x, file.FileName, arrNgay[i], fname, "DOC");
                    }
                    else
                    {
                        outx = safeTraninRepo.InsertClassFile(x, file.FileName, arrNgay[i], fname, "DOC");
                    }

                }

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult OnDeleteClass(string classid)
        {
            try
            {
                string delOut = safeTraninRepo.DeleteClass(classid);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult UpdateStatus()
        {
            try
            {
                string classid = Request.Form["classid"];
                string statusnew = Request.Form["statusnew"];
                safeTraninRepo.UpldateStatusClass(classid, statusnew);

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        

        [HttpPost]
        public ActionResult OnDeleteFileByClass(string fileid)
        {
            try
            {
                string delOut = safeTraninRepo.DeleteFileByClass(fileid);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ContentResult OnViewFileById(string fileid)
        {
            try
            {
                string urlFile = safeTraninRepo.GetFileById(fileid);
                byte[] bytes = System.IO.File.ReadAllBytes(urlFile);
                //System.IO.FileStream fs = System.IO.File.OpenRead(urlFile);
                //byte[] data = new byte[fs.Length];
                //int br = fs.Read(data, 0, data.Length);
                //if (br != fs.Length)
                //    throw new System.IO.IOException(urlFile);

                //byte[] fileBytes = data;

                //return File(
                //    fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, urlFile);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                return Content(base64);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ContentResult DownloadFileTmpResult()
        {
            try
            {
                string classid = Request.Form["classid"];
                string standardid = Request.Form["standardid"];
                //Lấy từ danh sách học viên chưa qua ở kỳ thi gần nhất.
                List <ResultExam> lstHvien = safeTraninRepo.getPointRecentExam(classid, standardid);
                //string urlFile = Server.MapPath("~/Content/") + "//MauNhapLieu//HLAT//Template_HLAT_Kqua.xlsx";
                //byte[] bytes = System.IO.File.ReadAllBytes(urlFile);

                //string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                //return Content(base64);
                string donViId = Session["DonViID"].ToString();
                tblDonVi donVi = null;                
                if (!string.IsNullOrEmpty(donViId))
                {
                    donVi = _dv_ser.GetById(donViId);                    
                }

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Bảng điểm");

                // Thay đổi kích thước từng cột
                sheet.SetColumnWidth(0, 4000);
                sheet.SetColumnWidth(1, 10000);
                sheet.SetColumnWidth(2, 6000);
                sheet.SetColumnWidth(3, 8000);
                sheet.SetColumnWidth(4, 6000);
                sheet.SetColumnWidth(5, 4000);
                sheet.SetColumnWidth(6, 10000);

                //gop cell               
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A1:C1"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D1:G1"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A2:C2"));
                sheet.AddMergedRegion(CellRangeAddress.ValueOf("D2:G2"));

                sheet.AddMergedRegion(CellRangeAddress.ValueOf("A4:G4"));

                IPrintSetup ps = sheet.PrintSetup;
                ps.Landscape = true;
                ps.PaperSize = (short)PaperSize.A4_Small;
                sheet.FitToPage = true;
                sheet.PrintSetup.FitWidth = 1;

                var rowIndex = 0;
                               
                ICellStyle styleHeader1 = workbook.CreateCellStyle();
                IFont font1 = workbook.CreateFont();
                font1.FontName = "Times New Roman";
                font1.Boldweight = (short)FontBoldWeight.Bold;
                font1.FontHeightInPoints = 13;
                styleHeader1.SetFont(font1);
                styleHeader1.VerticalAlignment = VerticalAlignment.Top;
                styleHeader1.Alignment = HorizontalAlignment.Center;
                styleHeader1.WrapText = true;

                IRow rowTerminal = sheet.CreateRow(rowIndex);
                
                rowTerminal.CreateCell(0).SetCellValue("TỔNG CÔNG TY ĐIỆN LỰC MIỀN BẮC");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(3).SetCellValue("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM");
                rowTerminal.Cells[1].CellStyle = styleHeader1;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                
                rowTerminal.CreateCell(0).SetCellValue(donVi.TenDonVi);
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader1;
                rowTerminal.CreateCell(3).SetCellValue("Độc lập – Tự do – Hạnh phúc");
                rowTerminal.Cells[1].CellStyle = styleHeader1;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                // So van ban
                ICellStyle styleHeader2 = workbook.CreateCellStyle();
                IFont font3 = workbook.CreateFont();
                font3.FontName = "Times New Roman";
                font3.FontHeightInPoints = 13;
                styleHeader2.SetFont(font3);
                styleHeader2.VerticalAlignment = VerticalAlignment.Top;
                styleHeader2.Alignment = HorizontalAlignment.Center;
                styleHeader2.WrapText = true;

                ICellStyle styleHeader3 = workbook.CreateCellStyle();
                IFont font4 = workbook.CreateFont();
                font4.FontName = "Times New Roman";
                font4.FontHeightInPoints = 13;
                font4.IsItalic = true;
                styleHeader3.SetFont(font4);
                styleHeader3.VerticalAlignment = VerticalAlignment.Top;
                styleHeader3.Alignment = HorizontalAlignment.Center;
                styleHeader3.WrapText = true;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                
                rowTerminal.CreateCell(0).SetCellValue("BẢNG CẬP NHẬT ĐIẾM");
                rowTerminal.Cells[0].Row.Height = 350;
                rowTerminal.Cells[0].CellStyle = styleHeader3;

                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);
                rowIndex++;
                rowTerminal = sheet.CreateRow(rowIndex);

                ICellStyle styleHeader = workbook.CreateCellStyle();
                IFont font = workbook.CreateFont();
                font.FontName = "Times New Roman";
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.FontHeightInPoints = 11;
                styleHeader.SetFont(font);
                styleHeader.VerticalAlignment = VerticalAlignment.Top;
                styleHeader.Alignment = HorizontalAlignment.Center;
                styleHeader.WrapText = true;
                styleHeader.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleHeader.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                rowTerminal.CreateCell(0).SetCellValue("STT");
                rowTerminal.Cells[0].Row.Height = 600;
                rowTerminal.Cells[0].CellStyle = styleHeader;

                rowTerminal.CreateCell(1).SetCellValue("Đơn vị");
                rowTerminal.Cells[1].CellStyle = styleHeader;

                rowTerminal.CreateCell(2).SetCellValue("Mã nhân sự");
                rowTerminal.Cells[2].CellStyle = styleHeader;

                rowTerminal.CreateCell(3).SetCellValue("Tên nhân sự");
                rowTerminal.Cells[3].CellStyle = styleHeader;

                rowTerminal.CreateCell(4).SetCellValue("Chức danh");
                rowTerminal.Cells[4].CellStyle = styleHeader;                

                rowTerminal.CreateCell(5).SetCellValue("Điểm");
                rowTerminal.Cells[5].CellStyle = styleHeader;

                rowTerminal.CreateCell(6).SetCellValue("Ghi chú");
                rowTerminal.Cells[6].CellStyle = styleHeader;
                //sheet.AddMergedRegion(new CellRangeAddress(9, 9, 5, 6));  

                rowIndex++;

                ICellStyle stylerow = workbook.CreateCellStyle();
                IFont fontr = workbook.CreateFont();
                fontr.FontName = "Times New Roman";
                fontr.FontHeightInPoints = 11;

                stylerow.SetFont(fontr);
                stylerow.VerticalAlignment = VerticalAlignment.Top;
                stylerow.Alignment = HorizontalAlignment.Center;
                stylerow.WrapText = true;
                stylerow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                stylerow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle styleCellText = workbook.CreateCellStyle();
                IFont fontF4 = workbook.CreateFont();
                fontr.FontName = "Times New Roman";
                fontr.FontHeightInPoints = 11;

                styleCellText.SetFont(fontr);
                styleCellText.VerticalAlignment = VerticalAlignment.Top;
                styleCellText.Alignment = HorizontalAlignment.Left;
                styleCellText.WrapText = true;
                styleCellText.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellText.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle styleCellNumber = workbook.CreateCellStyle();
                IFont fontNum = workbook.CreateFont();
                fontNum.FontName = "Times New Roman";
                fontNum.FontHeightInPoints = 11;

                styleCellNumber.SetFont(fontr);
                styleCellNumber.VerticalAlignment = VerticalAlignment.Top;
                styleCellNumber.Alignment = HorizontalAlignment.Right;
                styleCellNumber.WrapText = true;
                styleCellNumber.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleCellNumber.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                //Footer
                ICellStyle styleFooter1 = workbook.CreateCellStyle();
                IFont fontF1 = workbook.CreateFont();
                fontF1.FontName = "Times New Roman";
                fontF1.Boldweight = (short)FontBoldWeight.Bold;
                fontF1.FontHeightInPoints = 12;
                styleFooter1.SetFont(fontF1);
                styleFooter1.VerticalAlignment = VerticalAlignment.Top;
                styleFooter1.Alignment = HorizontalAlignment.Center;
                styleFooter1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                styleFooter1.WrapText = true;

                int i = 7;
                foreach (var item in lstHvien)
                {
                    i++;
                    rowTerminal = sheet.CreateRow(rowIndex);
                    rowTerminal.CreateCell(0).SetCellValue(item.stt);
                    rowTerminal.Cells[0].CellStyle = stylerow;

                    rowTerminal.CreateCell(1).SetCellValue(item.donvi);
                    rowTerminal.Cells[1].CellStyle = styleCellText;

                    rowTerminal.CreateCell(2).SetCellValue(item.mans);
                    rowTerminal.Cells[2].CellStyle = stylerow;


                    rowTerminal.CreateCell(3).SetCellValue(item.nhansu);
                    rowTerminal.Cells[3].CellStyle = styleCellText;
                    //rowTerminal.CreateCell(4).SetCellValue("");
                    //rowTerminal.Cells[4].CellStyle = stylerow;
                    //sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 3, 4));

                    rowTerminal.CreateCell(4).SetCellValue(item.chucdanh);
                    rowTerminal.Cells[4].CellStyle = styleCellText;

                    rowTerminal.CreateCell(5).SetCellValue("");
                    rowTerminal.Cells[5].CellStyle = styleCellNumber;

                    rowTerminal.CreateCell(6).SetCellValue("");
                    rowTerminal.Cells[6].CellStyle = styleCellText;
                    //rowTerminal.CreateCell(6).SetCellValue("");
                    //rowTerminal.Cells[6].CellStyle = stylerow;
                    //sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 5, 6));

                    rowIndex++;
                }

                // Save the Excel spreadsheet to a MemoryStream and return it to the client
                using (var exportData = new MemoryStream())
                {
                    workbook.Write(exportData);
                    //string strFileName = "";

                    //strFileName = string.Format("danh-sach-thi-sinh_{0}.xlsx", DateTime.Now).Replace("/", "-");

                    //string saveAsFileName = strFileName;
                    //Response.Clear();
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                    ////Response.BinaryWrite(exportData.GetBuffer());
                    //Response.BinaryWrite(exportData.ToArray());
                    //Response.End();

                    byte[] bytes = exportData.ToArray();
                    string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                    return Content(base64);
                }

                //string urlFile = Server.MapPath("~/Content/") + "//MauNhapLieu//HLAT//Template_HLAT_Kqua.xlsx";
                //byte[] bytes = System.IO.File.ReadAllBytes(urlFile);
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult LoadClassById(string classid)
        {
            List<ClassTrain> classTrains = new List<ClassTrain>();
            classTrains = safeTraninRepo.LoadClassById(classid);

            return Json(classTrains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadPointByExam(string examid)
        {
            List<ExamPoint> classTrains = new List<ExamPoint>();
            classTrains = safeTraninRepo.LoadPointByExam(examid);

            return Json(classTrains, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult LoadDsPersonalInClass(string classid)
        {
            //string madvql = Session["DonViID"].ToString();
            List<Personal> clsPersonals = new List<Personal>();
            clsPersonals = safeTraninRepo.loadLstPersonalByClass(classid);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        //Load danh sách kết quả cuối cùng theo lớp học
        public JsonResult LoadLastResultByClass(string classid)
        {
            List<ResultExam> clsPersonals = new List<ResultExam>();
            clsPersonals = safeTraninRepo.LoadLastResultByClass(classid);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        //Load danh sách điểm thi theo hình thức thi
        public JsonResult LoadResultExamByHtthi()
        {
            string htthi = Request.Form["htthi"];
            string lanthi = Request.Form["lanthi"];
            string classid = Request.Form["classid"];
            
            List<ExamPoint> clsPersonals = new List<ExamPoint>();
            clsPersonals = safeTraninRepo.LoadResultExamByHtthi(classid,htthi,lanthi);

            return Json(clsPersonals, JsonRequestBehavior.AllowGet);
        }

        //Save file kết quả kỳ thi -> return examid
        [HttpPost]
        public ActionResult OnSaveExamFile()
        {
            try
            {
                string filename = Request.Form["filename"];
                string fileurl = Request.Form["fileurl"];
                string filetype = Request.Form["filetype"];
                string examid = Request.Form["examid"];
                //2. Insert điểm thi đọc từ file Excel
                HttpFileCollectionBase files = Request.Files;
                
                string FilePath = "";

                if (ConfigurationManager.AppSettings.Get("URL_FILE_HLAT") != null)
                    FilePath = ConfigurationManager.AppSettings.Get("URL_FILE_HLAT").ToString();

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname = file.FileName;
                    fname = System.IO.Path.Combine(FilePath, fname);
                    file.SaveAs(fname);                    
                    string outx = safeTraninRepo.InsertExamFile(file.FileName, fname, "DOC",examid);
                    if (i == 0)
                    {
                        examid = outx;
                    }

                }                

                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        //Save Kỳ thi
        [HttpPost]
        public ActionResult OnSaveExam()
        {
            try
            {
                //Lấy các biến
                string classid = Request.Form["classid"];                
                string examord = Request.Form["lanthi"];
                string loaithi = Request.Form["loaithi"];

                //1. Insert exam nhé
                string examid = safeTraninRepo.InsertExam(classid, loaithi,  examord);


                return Json(examid);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult OnChangePoint()
        {
            try
            {
                //Lấy các biến
                string examid = Request.Form["examid"];
                string nsid = Request.Form["nsid"];
                string pointnew = Request.Form["pointnew"];

                //1. Update point exam
                safeTraninRepo.UpdatePointExamByNsid(examid, nsid, pointnew);


                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult GetExamID()
        {
            try
            {
                //Lấy các biến
                string classid = Request.Form["classid"];
                string loaithi = Request.Form["loaithi"];
                string lanthi = Request.Form["lanthi"];
                string loaidt = Request.Form["loaidt"];

                //1. Update point exam
                string examid = safeTraninRepo.GetExamId(classid, loaidt, loaithi, lanthi);

                return Json(examid);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        

        [HttpPost]
        public ActionResult OnUpdateExam()
        {
            try
            {
                ////2. Insert điểm thi đọc từ file Excel
                //string examid = Request.Form["examid"];
                //Chỉnh lại lấy examid từ db: input standardid, classid, categoryid, order exam
                string classid = Request.Form["classid"];
                string loaithi = Request.Form["loaithi"];
                string lanthi = Request.Form["lanthi"];
                string loaidt = Request.Form["loaidt"];

                string examid = safeTraninRepo.GetExamId(classid, loaidt, loaithi, lanthi);

                HttpFileCollectionBase files = Request.Files;
                DataSet ds = new DataSet();
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    ds = new DataSet();
                    string strErrExcel = "";
                    this.ReadFileExcel(ref ds, out strErrExcel, file);
                }

                if (ds.Tables[0] != null)
                {
                    for (int i = 6; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var row = ds.Tables[0].Rows[i];
                        safeTraninRepo.InsertExamResult(examid, ds.Tables[0].Rows[i][2].ToString(), ds.Tables[0].Rows[i][5].ToString(), ds.Tables[0].Rows[i][6].ToString());
                    }
                }

                return Json(examid);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult InputPointPerson()
        {
            try
            {
                ////2. Insert điểm thi đọc từ file Excel
                string classid = Request.Form["classid"];
                string kqLoaithi = Request.Form["kqLoaithi"];
                string kqLanthi = Request.Form["kqLanthi"];
                string cboNhansu = Request.Form["cboNhansu"];
                string pointperson = Request.Form["pointperson"];
                string notepoint = Request.Form["notepoint"];


                string output=  safeTraninRepo.InsertPointPerson(classid, kqLoaithi, kqLanthi, cboNhansu,pointperson, notepoint);
                
                return Json(output);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        //Khoá bài thi
        [HttpPost]
        public ActionResult LockExam()
        {
            try
            {
                ////2. Insert điểm thi đọc từ file Excel
                string classid = Request.Form["classid"];
                string kqLoaithi = Request.Form["kqLoaithi"];
                string kqLanthi = Request.Form["kqLanthi"];

                string output = safeTraninRepo.LockExam(classid, kqLoaithi, kqLanthi);

                return Json(output);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        //Hàm check lock kỳ thi
        
        [HttpPost]
        public ActionResult CheckLockExam()
        {
            try
            {
                ////2. Insert điểm thi đọc từ file Excel
                string classid = Request.Form["classid"];
                string kqLoaithi = Request.Form["kqLoaithi"];
                string kqLanthi = Request.Form["kqLanthi"];

                string output = safeTraninRepo.CheckLockExam(classid, kqLoaithi, kqLanthi);

                return Json(output);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        public JsonResult getCboLoaiThiResult(string categoryid)
        {
            try
            {
                List<Hluat_Cate_Standard> clsPersonals = new List<Hluat_Cate_Standard>();
                clsPersonals = safeTraninRepo.LoadCboLoaiThiByCate(categoryid);

                return Json(clsPersonals, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }

        public JsonResult LoadCboSolanThiByClassStandard(string classid, string standardid)
        {
            try
            {
                List<Hluat_Exam> clsPersonals = new List<Hluat_Exam>();
                clsPersonals = safeTraninRepo.LoadCboSolanThiByClassStandard(classid, standardid);

                return Json(clsPersonals, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("ERROR-" + ex.Message);
            }
        }
        public void ReadFileExcel(ref DataSet ds, out string strError, HttpPostedFileBase file)
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
                    ViewBag.ErrorExcel = "Không đọc được dữ liệu từ file bạn chọn!";
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

    }



}