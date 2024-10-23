using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Areas.Admin.Models
{
    public class DonViChiTieuSuCoViewModels
    {
        public string MA_DVIQLY { get; set; }
        public string TEN_DVIQLY { get; set; }
        public int? ChiTieuNam { get; set; }
        public int? ChiTieuNamCongTy { get; set; }
        public int? NPCChiTieuNam { get; set; }

        #region Id
        public int? IdT1 { get; set; }
        public int? IdT2 { get; set; }
        public int? IdT3 { get; set; }
        public int? IdT4 { get; set; }
        public int? IdT5 { get; set; }
        public int? IdT6 { get; set; }
        public int? IdT7 { get; set; }
        public int? IdT8 { get; set; }
        public int? IdT9 { get; set; }
        public int? IdT10 { get; set; }
        public int? IdT11 { get; set; }
        public int? IdT12 { get; set; }
        #endregion

        #region Chi tieu
        public int? ChiTieuT1 { get; set; }
        public int? ChiTieuT2 { get; set; }
        public int? ChiTieuT3 { get; set; }
        public int? ChiTieuT4 { get; set; }
        public int? ChiTieuT5 { get; set; }
        public int? ChiTieuT6 { get; set; }
        public int? ChiTieuT7 { get; set; }
        public int? ChiTieuT8 { get; set; }
        public int? ChiTieuT9 { get; set; }
        public int? ChiTieuT10 { get; set; }
        public int? ChiTieuT11 { get; set; }
        public int? ChiTieuT12 { get; set; }
        #endregion

        #region Mo khoa
        public Nullable<bool> NPCIsOpenT1 { get; set; }
        public Nullable<bool> NPCIsOpenT2 { get; set; }
        public Nullable<bool> NPCIsOpenT3 { get; set; }
        public Nullable<bool> NPCIsOpenT4 { get; set; }
        public Nullable<bool> NPCIsOpenT5 { get; set; }
        public Nullable<bool> NPCIsOpenT6 { get; set; }
        public Nullable<bool> NPCIsOpenT7 { get; set; }
        public Nullable<bool> NPCIsOpenT8 { get; set; }
        public Nullable<bool> NPCIsOpenT9 { get; set; }
        public Nullable<bool> NPCIsOpenT10 { get; set; }
        public Nullable<bool> NPCIsOpenT11 { get; set; }
        public Nullable<bool> NPCIsOpenT12 { get; set; }
        #endregion

        #region Chi tieu sau dieu chuyen
        public int? ChiTieuT1SauDieuChuyen { get; set; }
        public int? ChiTieuT2SauDieuChuyen { get; set; }
        public int? ChiTieuT3SauDieuChuyen { get; set; }
        public int? ChiTieuT4SauDieuChuyen { get; set; }
        public int? ChiTieuT5SauDieuChuyen { get; set; }
        public int? ChiTieuT6SauDieuChuyen { get; set; }
        public int? ChiTieuT7SauDieuChuyen { get; set; }
        public int? ChiTieuT8SauDieuChuyen { get; set; }
        public int? ChiTieuT9SauDieuChuyen { get; set; }
        public int? ChiTieuT10SauDieuChuyen { get; set; }
        public int? ChiTieuT11SauDieuChuyen { get; set; }
        public int? ChiTieuT12SauDieuChuyen { get; set; }
        #endregion

        #region So vu sau mien tru
        public int? SoVuSauMTT1 { get; set; }
        public int? SoVuSauMTT2 { get; set; }
        public int? SoVuSauMTT3 { get; set; }
        public int? SoVuSauMTT4 { get; set; }
        public int? SoVuSauMTT5 { get; set; }
        public int? SoVuSauMTT6 { get; set; }
        public int? SoVuSauMTT7 { get; set; }
        public int? SoVuSauMTT8 { get; set; }
        public int? SoVuSauMTT9 { get; set; }
        public int? SoVuSauMTT10 { get; set; }
        public int? SoVuSauMTT11 { get; set; }
        public int? SoVuSauMTT12 { get; set; }
        #endregion

        #region So vu truoc mien tru
        public int? SoVuTruocMTT1 { get; set; }
        public int? SoVuTruocMTT2 { get; set; }
        public int? SoVuTruocMTT3 { get; set; }
        public int? SoVuTruocMTT4 { get; set; }
        public int? SoVuTruocMTT5 { get; set; }
        public int? SoVuTruocMTT6 { get; set; }
        public int? SoVuTruocMTT7 { get; set; }
        public int? SoVuTruocMTT8 { get; set; }
        public int? SoVuTruocMTT9 { get; set; }
        public int? SoVuTruocMTT10 { get; set; }
        public int? SoVuTruocMTT11 { get; set; }
        public int? SoVuTruocMTT12 { get; set; }
        #endregion

        public List<ChiTieuSuCoTheoLoaiModels> chitieutheoloais { get; set; }
    }

    public class ChiTieuSuCoTheoLoaiModels
    {
        public string Ten { get; set; }
        public int? ChiTieuNamCongTy { get; set; }

        #region Id
        public int? IdT1 { get; set; }
        public int? IdT2 { get; set; }
        public int? IdT3 { get; set; }
        public int? IdT4 { get; set; }
        public int? IdT5 { get; set; }
        public int? IdT6 { get; set; }
        public int? IdT7 { get; set; }
        public int? IdT8 { get; set; }
        public int? IdT9 { get; set; }
        public int? IdT10 { get; set; }
        public int? IdT11 { get; set; }
        public int? IdT12 { get; set; }
        #endregion

        #region Chi tieu
        public int? ChiTieuT1 { get; set; }
        public int? ChiTieuT2 { get; set; }
        public int? ChiTieuT3 { get; set; }
        public int? ChiTieuT4 { get; set; }
        public int? ChiTieuT5 { get; set; }
        public int? ChiTieuT6 { get; set; }
        public int? ChiTieuT7 { get; set; }
        public int? ChiTieuT8 { get; set; }
        public int? ChiTieuT9 { get; set; }
        public int? ChiTieuT10 { get; set; }
        public int? ChiTieuT11 { get; set; }
        public int? ChiTieuT12 { get; set; }
        public int? ChiTieuNam { get; set; }
        #endregion

        #region Chi tieu sau dieu chuyen
        public int? ChiTieuT1SauDieuChuyen { get; set; }
        public int? ChiTieuT2SauDieuChuyen { get; set; }
        public int? ChiTieuT3SauDieuChuyen { get; set; }
        public int? ChiTieuT4SauDieuChuyen { get; set; }
        public int? ChiTieuT5SauDieuChuyen { get; set; }
        public int? ChiTieuT6SauDieuChuyen { get; set; }
        public int? ChiTieuT7SauDieuChuyen { get; set; }
        public int? ChiTieuT8SauDieuChuyen { get; set; }
        public int? ChiTieuT9SauDieuChuyen { get; set; }
        public int? ChiTieuT10SauDieuChuyen { get; set; }
        public int? ChiTieuT11SauDieuChuyen { get; set; }
        public int? ChiTieuT12SauDieuChuyen { get; set; }
        #endregion

        #region So vu truoc mien tru
        public int? SoVuTruocMTT1 { get; set; }
        public int? SoVuTruocMTT2 { get; set; }
        public int? SoVuTruocMTT3 { get; set; }
        public int? SoVuTruocMTT4 { get; set; }
        public int? SoVuTruocMTT5 { get; set; }
        public int? SoVuTruocMTT6 { get; set; }
        public int? SoVuTruocMTT7 { get; set; }
        public int? SoVuTruocMTT8 { get; set; }
        public int? SoVuTruocMTT9 { get; set; }
        public int? SoVuTruocMTT10 { get; set; }
        public int? SoVuTruocMTT11 { get; set; }
        public int? SoVuTruocMTT12 { get; set; }
        #endregion
    }

}