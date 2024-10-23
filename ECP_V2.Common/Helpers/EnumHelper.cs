using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECP_V2.Common.Helpers
{
    public class EnumHelper
    {
    }

    public enum NotificationEnumeration
    {
        Success,
        Error,
        Warning
    }

    public class StatusEnumeration
    {
        public static int Published = 1;
        public static int Waiting = 2;
        public static int Unpublished = 3;
    }
    public enum VaiTroPhienLamViec
    {
        NguoiDuyet_SoPa = 1,
        NguoiChiHuy = 2,
        GiamSatVien = 3,
        NguoiKiemSoat = 4,
        NguoiKiemTraPhieu = 5,
        LanhDaoTrucBan = 6
    }

    public enum LoaiThongBao
    {
        TaoCongViec = 1,
        DuyetCongviec = 2,
        XoaCongViec = 3
    }

    public enum TrangThaiMessage
    {
        UnRead = 0,
        IsRead = 1
    }

    public enum TrangThaiPhienLV
    {
        VuaTao = 1,
        DaDuyet = 2,
        DaXong = 3,
        ChuyenNPC = 4,
        ChuyenHoan = 5,
        DaXoa = 6,
        HuyBo = 7
    }
    public enum TinhChatPhienLV
    {
        CongViecBoSung = 1,
        CongViecKeHoach = 2,
        CongViecDotXuat = 3
    }

    public static class PhanHe
    {
        public const string PhanHeImage = "HA";
        public const string PhanHePhienLamViec = "LV";
        public const string PhanHePhieuCongTac = "CT";
        public const string PhanHeSuCo = "SC";
    }
}