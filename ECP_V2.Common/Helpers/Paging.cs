﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.Common.Helpers
{
    public class PagedData<T> where T : class
    {
        public List<T> Data { get; set; }
        public int NumberOfPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalRecords { get; set; }        
        public int CurrentPage { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
        public int Next { get; set; }
        public int Prev { get; set; }
        public string RecordsName { get; set; }

        // Tham chiếu bảng plv_KeHoachLichLamViec
        public int? KeHoachLV_Id { set; get; }
        public int? HinhThucKiemTra { set; get; }
        public string NguoiDaiDienKT { set; get; }
        public string NguoiDaiDienKT_Id { set; get; }
        public string LyDoHoanHuy { set; get; }
    }
    public class PageData<T> where T : class
    {
        public List<T> Data { get; set; }
        public Page Page { get; set; }
        
    }
    public class Page
    {
        public int NumberOfPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
        public int Next { get; set; }
        public int Prev { get; set; }
        public string RecordsName { get; set; }
    }
    
}