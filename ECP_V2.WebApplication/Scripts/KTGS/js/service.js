var service = {
    apiserver: $('#baseurl').val(),
    token: '',
    ExcuteAjaxtGet: function ExcuteAjaxt(url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            type: "GET",
            url: this.apiserver + url,
            data: data,
            headers: { "Authorization": 'Bearer ' + service.token },
            async: asy
        }).done(function (d) {
            try {
                if (d.toString().indexOf("error:") >= 0) {
                    alert(d);
                    if (d.toString().indexOf("authority") >= 0) {
                        window.location = "./login.aspx";
                    }

                } else {
                    if (callback != null)
                        callback(d)
                }
            } catch (e) {
                console.log(e);
            }
        });
    },

    ExcuteAjaxtPost: function ExcuteAjaxt(url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            type: "Post",
            url: this.apiserver + url,            
            data: data,            
            dataType: 'json',
            contentType: "application/json",   
            headers: { "Authorization": 'Bearer ' + service.token },
            async: asy
        }).done(function (d) {
            try {
                if (d.toString().indexOf("error:") >= 0) {
                    alert(d);
                    if (d.toString().indexOf("authority") >= 0) {
                        window.location = "./login.aspx";
                    }

                } else {
                    if (callback != null)
                        callback(d)
                }
            } catch (e) {
                console.log(e);
            }
        });
    },


    ExcuteAjaxtUpload: function (url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            type: "Post",
            url: this.apiserver + url,
            data: data,            
            contentType: false,
            processData: false,
            headers: { "Authorization": 'Bearer ' + service.token },
            async: asy
        }).done(function (d) {
            try {
                if (d.toString().indexOf("error:") >= 0) {
                    alert(d);
                    if (d.toString().indexOf("authority") >= 0) {
                        window.location = "./login.aspx";
                    }

                } else {
                    if (callback != null)
                        callback(d)
                }
            } catch (e) {
                console.log(e);
            }
        });
    },


    init: function () {
        //gettoken
        //"ECPAuth""
        data = {
            "UserName": user.UserName,
            "ID_DV": user.MA_DVIQLY
        }

        this.ExcuteAjaxtPost("api/v1/Auth/GetToken", JSON.stringify(data), function (d) {
            service.token = d.Data.Token;            
        });  
        // get donvi

        this.ExcuteAjaxtGet("api/v1/DanhMuc/GetDMDonviALL", null, function (d) {
            // log ra danh sach don vi cua minh
            var md = $('#DonViID').val();
            util.donvis = d.Data.filter(e => e.ma_dviqly == md || e.ma_dviqly_captren == md);
        });  
    }

};

service.init();