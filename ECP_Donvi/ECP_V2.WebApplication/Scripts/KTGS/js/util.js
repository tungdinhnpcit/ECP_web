var user = {
    baseurl: null,
    UserId: null,
    MA_DVIQLY: 'PA',
    UserName:null,
};
var TRANG_THAI = {
    CHUA_KP: "CHUA_KP",
    DA_KP: "DA_KP"
};
var util = {
    donvis:[],
    init: function () {
        $.ajax({
            url: "../scripts/ktgs/html/template.html", type: "get", async: false, success: function (d) {
                $('body').append(d);
            }
        });
        user.baseurl = $('#baseurl').val();
        user.UserId = $('#UserId').val();
        user.UserName = $('#UserName').val();  


        // lay danh sach nhan vien thuoc group                

        // get 
    },
     FormatDateToVn: function (dateStr) {
        try {
            // convert string to date  
            var a = dateStr.split(/[^0-9]/);
            var date = new Date(a[0], a[1] - 1, a[2], a[3], a[4], a[5]);
            var datenow = new Date();
            var md = date.getDate();
            if (md < 10) md = "0" + (md).toString();
            var mm = date.getMonth();
            if (mm < 9) mm = "0" + (mm + 1).toString()
            else mm = (mm + 1).toString();
            var strreturn = md + "/" + mm + "/" + date.getFullYear();
            return strreturn;
        } catch (e) {
            return "";
        }
    },

    GetNameDonvi: function (madvi) {
        var index = this.donvis.findIndex(d => d.ma_dviqly == madvi)
        if (index >= 0)
            return this.donvis[index].ten_donvi;
        return "";
        //switch (madvi) {
        //    case "PA":
        //        return "TỔNG CTY ĐIỆN LỰC MIỀN BẮC";
        //    case "PA01":
        //        return "CÔNG TY ĐIỆN LỰC NAM ĐỊNH";
        //    case "PA02":
        //        return "CÔNG TY ĐIỆN LỰC PHÚ THỌ";
        //    case "PA03":
        //        return "CÔNG TY ĐIỆN LỰC QUẢNG NINH";
        //    case "PA04":
        //        return "CÔNG TY ĐIỆN LỰC THÁI NGUYÊN";
        //    case "PA05":
        //        return "CÔNG TY ĐIỆN LỰC BẮC GIANG";
        //    case "PA07":
        //        return "CÔNG TY ĐIỆN LỰC THANH HÓA";
        //    case "PA09":
        //        return " CÔNG TY ĐIỆN LỰC THÁI BÌNH";
        //    case "PA10":
        //        return "CÔNG TY ĐIỆN LỰC YÊN BÁI";
        //    case "PA11":
        //        return "CÔNG TY ĐIỆN LỰC LẠNG SƠN";
        //    case "PA12":
        //        return "CÔNG TY ĐIỆN LỰC TUYÊN QUANG";
        //    case "PA13":
        //        return "CÔNG TY ĐIỆN LỰC NGHỆ AN";
        //    case "PA14":
        //        return "CÔNG TY ĐIỆN LỰC CAO BẰNG";
        //    case "PA15":
        //        return "CÔNG TY ĐIỆN LỰC SƠN LA";
        //    case "PA16":
        //        return "CÔNG TY ĐIỆN LỰC HÀ TĨNH";
        //    case "PA17":
        //        return "CÔNG TY ĐIỆN LỰC HOÀ BÌNH";
        //    case "PA18":
        //        return "CÔNG TY ĐIỆN LỰC LÀO CAI";
        //    case "PA19":
        //        return "CÔNG TY ĐIỆN LỰC ĐIỆN BIÊN";
        //    case "PA20":
        //        return " CÔNG TY ĐIỆN LỰC HÀ GIANG";
        //    case "PA22":
        //        return "CÔNG TY ĐIỆN LỰC BẮC NINH";
        //    case "PA23":
        //        return "CÔNG TY ĐIỆN LỰC HƯNG YÊN";
        //    case "PA24":
        //        return "CÔNG TY ĐIỆN LỰC HÀ NAM";
        //    case "PA25":
        //        return "CÔNG TY ĐIỆN LỰC VĨNH PHÚC";
        //    case "PA26":
        //        return "CÔNG TY ĐIỆN LỰC BẮC KẠN";
        //    case "PA29":
        //        return "CÔNG TY ĐIỆN LỰC  LAI CHÂU";
        //    case "PAT1":
        //        return "CÔNG TY THÍ NGHIỆM ĐIỆN MIỀN BẮC";
        //    case "PAX1":
        //        return "CÔNG TY DỊCH VỤ ĐIỆN LỰC MIỀN BẮC";
        //    case "PH":
        //        return "CÔNG TY ĐIỆN LỰC HẢI PHÒNG";
        //    case "PM":
        //        return "CÔNG TY ĐIỆN LỰC HẢI DƯƠNG";
        //    case "PN":
        //        return "CÔNG TY ĐIỆN LỰC NINH BÌNH";
        //    default:
        //        break;
        //}
        //return "";
    },
}

util.init();

