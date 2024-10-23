using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,AdminDonVi,Manager,Master")]
    public class LogController : UTController
    {
        //
        // GET: /Admin/Log/
        private LogsRepository _faculty_ser = new LogsRepository();
        private MessagesRepository messagesRepository = new MessagesRepository();
        private NhanVienRepository nhanVienRepository = new NhanVienRepository();

        private void DisposeAll()
        {
            if (_faculty_ser != null)
            {
                _faculty_ser = null;
            }

            if (messagesRepository != null)
            {
                messagesRepository.Dispose();
                messagesRepository = null;
            }

            if (nhanVienRepository != null)
            {
                nhanVienRepository.Dispose();
                nhanVienRepository = null;
            }
        }

        #region Index
        [HasCredential(MenuCode = "NK")]
        public ActionResult Index()
        {
            try
            {
                tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);
                var model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id
                                        && x.MA_DVIQLY == Session["DonViID"].ToString()
                                        && x.TrangThai == (int)TrangThaiMessage.UnRead)
                                        .ToList();

                if (model != null && model.Count > 0)
                {
                    string strError = "";

                    foreach (var item in model)
                    {
                        item.TrangThai = (int)TrangThaiMessage.IsRead;

                        messagesRepository.Update(item, ref strError);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            DisposeAll();

            return View();
        }
        #endregion

        #region List
        [HttpGet]
        public ActionResult List(int page, int pageSize, string filter, string type, string DateFrom, string DateTo, string DonViId)
        {
            filter = filter.ToUpper();
            int page1 = (page - 1) * pageSize;
            int pagelength1 = page * pageSize;
            int Count = 0;

            string donviId = null;
            try
            {
                donviId = Session["DonViID"].ToString();
            }
            catch { }

            if (DonViId == "0")
                DonViId = "";

            List<Message> model = new List<Message>();

            DateTime? startDate;
            DateTime? endDate;

            try
            {
                startDate = DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                startDate = null;
            }

            try
            {
                endDate = DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                endDate = null;
            }

            if (donviId == null)
            {
                tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);
                model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id 
                                        && (x.NgayTao >= startDate || startDate == null) 
                                        && (x.NgayTao <= endDate || endDate == null)
                                        && x.MA_DVIQLY == DonViId)
                                        .OrderByDescending(x => x.NgayTao)
                                        .ToList();
                Count = model.Count;
            }
            else
            {
                tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);

                if (string.IsNullOrEmpty(DonViId))
                {
                    model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id
                                        && (x.NgayTao >= startDate || startDate == null)
                                        && (x.NgayTao <= endDate || endDate == null)
                                        && x.MA_DVIQLY == donviId)
                                        .OrderByDescending(x => x.NgayTao)
                                        .ToList();
                    Count = model.Count;
                }
                else
                {
                    model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id
                                            && (x.NgayTao >= startDate || startDate == null)
                                            && (x.NgayTao <= endDate || endDate == null)
                                            && x.MA_DVIQLY == DonViId)
                                            .OrderByDescending(x => x.NgayTao)
                                            .ToList();
                    Count = model.Count;
                }
            }


            var ListNewsPageSize = new PageData<Message>();
            if (model.Count() > 0)
            {
                ListNewsPageSize.Data = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Thông báo",
                    NumberOfPages = Convert.ToInt32(Math.Ceiling((double)Count / pageSize)),
                    RecordsPerPage = pageSize,
                    CurrentPage = page,
                    TotalRecords = Count
                };

            }
            else
            {
                ListNewsPageSize.Data = new List<Message>();
                ListNewsPageSize.Page = new Page()
                {
                    RecordsName = "Thông báo",
                    NumberOfPages = 0,
                    RecordsPerPage = 0,
                    CurrentPage = 0,
                    TotalRecords = 0
                };
            }

            DisposeAll();

            return PartialView("_List", ListNewsPageSize);
        }
        #endregion

        #region LoadLogAuto
        [HttpGet]
        public ActionResult LoadLogAuto()
        {
            ECP_V2.Business.Repository.LogsRepository lgr = new ECP_V2.Business.Repository.LogsRepository();
            try
            {
                ViewBag.LogInfo = lgr.AdvancedSearchLogs(0, 10, "", "Info", "", "", Session["DonViID"].ToString());
                ViewBag.LogError = lgr.AdvancedSearchLogs(0, 10, "", "Error", "", "", Session["DonViID"].ToString());
            }
            catch { }

            DisposeAll();

            return View();
        }
        #endregion

        [HttpPost]
        public JsonResult GetNewMessages(Dictionary<string, int> messageList)
        {
            try
            {
                if (messageList != null && messageList.Count > 0)
                {
                    string html = "";
                    List<Message> model = new List<Message>();
                    tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);
                    List<int> messageIdList = null;

                    try
                    {
                        messageIdList = messageList.Where(x => x.Key.ToLower().Equals(User.Identity.Name)).Select(x => x.Value).ToList();
                    }
                    catch (Exception e)
                    {
                        messageIdList = null;
                    }

                    if (messageIdList != null && messageIdList.Count > 0)
                    {
                        model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id && messageIdList.Contains(x.Id))
                                            .OrderByDescending(x => x.NgayTao)
                                            .ToList();

                        html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Areas/Admin/Views/Log/LoadNewMessagesPartial.cshtml", model);

                        DisposeAll();

                        return Json(new { success = true, responseText = html, countIsRead = model.Where(x => x.TrangThai == (int)TrangThaiMessage.UnRead).Count() }, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateMessage(int id = 0)
        {
            try
            {
                var entity = messagesRepository.GetById(id);

                if (entity != null)
                {
                    string error = "";

                    entity.TrangThai = (int)TrangThaiMessage.IsRead;
                    messagesRepository.Update(entity, ref error);
                }

                string html = "";

                tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);
                List<Message> model = messagesRepository.ListMessageByPosition(1).Where(x => x.MaTaiKhoan == nhanVien.Id)
                                        .OrderByDescending(x => x.NgayTao)
                                        .ToList();
                html = RenderViewHelper.RenderRazorViewToString(this.ControllerContext, "~/Areas/Admin/Views/Log/LoadMessagesPartial.cshtml", model);

                DisposeAll();

                return Json(new { success = true, responseText = html, countIsRead = model.Where(x => x.TrangThai == (int)TrangThaiMessage.UnRead).Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}