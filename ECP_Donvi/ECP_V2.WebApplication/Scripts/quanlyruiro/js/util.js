var user = {
    DonViID: null,
    UserId: null,
    PhongBanId: null,
    UserName: null,
    HoTen: null
};
var TRANG_THAI = {
    CHUA_KP: "CHUA_KP",
    DA_KP: "DA_KP"
};
var util = {    
    init: function () {        
        $.ajax({
            url: "../scripts/quanlyruiro/html/template.html", type: "get", async: false, success: function (d) {
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
    }
}

util.init();

