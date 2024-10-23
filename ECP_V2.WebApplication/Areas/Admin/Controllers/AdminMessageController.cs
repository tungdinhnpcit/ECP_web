using ECP_V2.Business.Repository;
using ECP_V2.Common.Helpers;
using ECP_V2.Common.Mvc;
using ECP_V2.DataAccess;
using ECP_V2.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminMessageController : UTController
    {
        private MessagesRepository messagesRepository = new MessagesRepository();
        private NhanVienRepository nhanVienRepository = new NhanVienRepository();

        private void DisposeAll()
        {
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

        // GET: Admin/AdminMessage
        [HasCredential(MenuCode = "ADMIN_MESSAGE")]
        public ActionResult Index()
        {
            var messagesList = messagesRepository.ListMessageByPosition(2);

            DisposeAll();

            return View(messagesList);
        }

        public ActionResult List(int page, int pageSize, string filter, string DateFrom, string DateTo)
        {
            filter = filter.ToUpper();
            int page1 = (page - 1) * pageSize;
            int pagelength1 = page * pageSize;
            int Count = 0;

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

            tblNhanVien nhanVien = nhanVienRepository.GetByUserName(User.Identity.Name);
            model = messagesRepository.ListMessageByPosition(2).Where(
                                        x => ((startDate.HasValue && x.NgayTao.Date >= startDate.Value.Date) || startDate == null) && ((endDate.HasValue && x.NgayTao.Date <= endDate.Value.Date) || endDate == null)
                                    )
                                    .OrderByDescending(x => x.NgayTao)
                                    .ToList();
            Count = model.Count;


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

        [HasCredential(MenuCode = "ADMIN_MESSAGE")]
        public ActionResult Add()
        {
            DisposeAll();

            return View();
        }

        [HttpPost]
        public ActionResult Add(Message model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NoiDung))
                {
                    DisposeAll();

                    return Json(new { success = false, responseText = "Nội dung thông báo không được để trống!" }, JsonRequestBehavior.AllowGet);
                }

                model.NgayTao = DateTime.Now;
                model.NguoiTao = User.Identity.Name;
                model.LoaiThongBao = 2;
                model.TrangThai = 1;
                model.MA_DVIQLY = Session["DonViID"].ToString();

                string strError = "";
                messagesRepository.Create(model, ref strError);

                if (string.IsNullOrEmpty(strError))
                {
                    DisposeAll();

                    return Json(new { success = true, responseText = "Tạo thông báo thành công!" }, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(new { success = false, responseText = "Tạo thông báo không thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false, responseText = "Tạo thông báo không thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetLastestMessageAdmin()
        {
            try
            {
                var message = messagesRepository.ListMessageByPosition(2).OrderByDescending(x => x.NgayTao).FirstOrDefault();

                DisposeAll();

                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMessageAdminNotify()
        {
            try
            {
                var message = messagesRepository.ListMessageByPosition(2).OrderByDescending(x => x.NgayTao).FirstOrDefault(x => x.TrangThai == 1);

                DisposeAll();

                return Json(new { success = true, message = message.NoiDung }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int Id)
        {
            try
            {
                string strError = "";
                messagesRepository.Delete(Id, ref strError);

                if (string.IsNullOrEmpty(strError))
                {
                    DisposeAll();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActiveUpdateStatus(int Id)
        {
            try
            {
                string strError = "";
                var message = messagesRepository.GetById(Id);

                if (message != null)
                {
                    message.TrangThai = 1;
                    messagesRepository.Update(message, ref strError);

                    if (string.IsNullOrEmpty(strError))
                    {
                        DisposeAll();

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeactiveUpdateStatus(int Id)
        {
            try
            {
                string strError = "";
                var message = messagesRepository.GetById(Id);

                if (message != null)
                {
                    message.TrangThai = 0;
                    messagesRepository.Update(message, ref strError);

                    if (string.IsNullOrEmpty(strError))
                    {
                        DisposeAll();

                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                DisposeAll();

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}