using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class DonViChiTieuThoiGianSuCoViewModels
    {
        public int Id { get; set; }
        public int Nam { get; set; }
        public string MA_DVIQLY { get; set; }
        public string TEN_DVIQLY { get; set; }

        public Nullable<double> TrungBinhNam { get; set; }
        public Nullable<double> Thang1 { get; set; }
        public Nullable<double> Thang2 { get; set; }
        public Nullable<double> Thang3 { get; set; }
        public Nullable<double> Thang4 { get; set; }
        public Nullable<double> Thang5 { get; set; }
        public Nullable<double> Thang6 { get; set; }
        public Nullable<double> Thang7 { get; set; }
        public Nullable<double> Thang8 { get; set; }
        public Nullable<double> Thang9 { get; set; }
        public Nullable<double> Thang10 { get; set; }
        public Nullable<double> Thang11 { get; set; }
        public Nullable<double> Thang12 { get; set; }

        #region thoi gian
        public double? TGTBN { get; set; }
        public string TGT1 { get; set; }
        public string TGT2 { get; set; }
        public string TGT3 { get; set; }
        public string TGT4 { get; set; }
        public string TGT5 { get; set; }
        public string TGT6 { get; set; }
        public string TGT7 { get; set; }
        public string TGT8 { get; set; }
        public string TGT9 { get; set; }
        public string TGT10 { get; set; }
        public string TGT11 { get; set; }
        public string TGT12 { get; set; }
        #endregion

        #region thoi gian dieu chinh
        public double? TGTBNDC { get; set; }
        public string TGT1DC { get; set; }
        public string TGT2DC { get; set; }
        public string TGT3DC { get; set; }
        public string TGT4DC { get; set; }
        public string TGT5DC { get; set; }
        public string TGT6DC { get; set; }
        public string TGT7DC { get; set; }
        public string TGT8DC { get; set; }
        public string TGT9DC { get; set; }
        public string TGT10DC { get; set; }
        public string TGT11DC { get; set; }
        public string TGT12DC { get; set; }
        #endregion

        public string THTBN { get; set; }
        public string THT1 { get; set; }
        public string THT2 { get; set; }
        public string THT3 { get; set; }
        public string THT4 { get; set; }
        public string THT5 { get; set; }
        public string THT6 { get; set; }
        public string THT7 { get; set; }
        public string THT8 { get; set; }
        public string THT9 { get; set; }
        public string THT10 { get; set; }
        public string THT11 { get; set; }
        public string THT12 { get; set; }

        public decimal? THTGTBN { get; set; }
        public decimal? THTGT1 { get; set; }
        public decimal? THTGT2 { get; set; }
        public decimal? THTGT3 { get; set; }
        public decimal? THTGT4 { get; set; }
        public decimal? THTGT5 { get; set; }
        public decimal? THTGT6 { get; set; }
        public decimal? THTGT7 { get; set; }
        public decimal? THTGT8 { get; set; }
        public decimal? THTGT9 { get; set; }
        public decimal? THTGT10 { get; set; }
        public decimal? THTGT11 { get; set; }
        public decimal? THTGT12 { get; set; }

        public Nullable<double> TrungBinhNamDC { get; set; }
        public Nullable<double> Thang1DC { get; set; }
        public Nullable<double> Thang2DC { get; set; }
        public Nullable<double> Thang3DC { get; set; }
        public Nullable<double> Thang4DC { get; set; }
        public Nullable<double> Thang5DC { get; set; }
        public Nullable<double> Thang6DC { get; set; }
        public Nullable<double> Thang7DC { get; set; }
        public Nullable<double> Thang8DC { get; set; }
        public Nullable<double> Thang9DC { get; set; }
        public Nullable<double> Thang10DC { get; set; }
        public Nullable<double> Thang11DC { get; set; }
        public Nullable<double> Thang12DC { get; set; }
        public Nullable<bool> IsOpenNam { get; set; }
        public Nullable<bool> IsOpenT1 { get; set; }
        public Nullable<bool> IsOpenT2 { get; set; }
        public Nullable<bool> IsOpenT3 { get; set; }
        public Nullable<bool> IsOpenT4 { get; set; }
        public Nullable<bool> IsOpenT5 { get; set; }
        public Nullable<bool> IsOpenT6 { get; set; }
        public Nullable<bool> IsOpenT7 { get; set; }
        public Nullable<bool> IsOpenT8 { get; set; }
        public Nullable<bool> IsOpenT9 { get; set; }
        public Nullable<bool> IsOpenT10 { get; set; }
        public Nullable<bool> IsOpenT11 { get; set; }
        public Nullable<bool> IsOpenT12 { get; set; }
    }
}