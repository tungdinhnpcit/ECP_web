var user={
    DonViID: null,
    UserId: null,
    PhongBanId: null,
    UserName: null,
    HoTen:null
}
var util = {    
    init: function () {        
        $.ajax({
            url: "../scripts/bienbankhaosat/html/template.html", type: "get", async: false, success: function (d) {
                $('body').append(d);
            }           
        });
        user.DonViID = $('#DonViID').val();
        user.UserId = $('#UserId').val();
        user.PhongBanId = $('#PhongBanId').val();
        user.UserName = $('#UserName').val();
        user.HoTen = $('#HoTen').val();

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
            mm = mm + 1;
            if (mm < 10) mm = "0" + mm.toString();
            var strreturn = md + "/" + mm + "/" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes();
            return strreturn;
        } catch (e) {
            return "";
        }
    },
}

util.init();

