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
    },
    getUrlParameter: function getUrlParameter(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;
        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    },        
}

util.init();

