

var ll_lviec = {
    idpmis: [],
    index:null,
    gettreepmis: function (index) {
        modal_treepmis.parrent = this;
        ll_lviec.index = index;
        modal_treepmis.show();
    },
    // get item tu pmis
    chonEventCallBack: function (d) {
        //$('#txtvitri').val(d[0].title);
        //$('#lblidpmis').html(d[0].key);
        //this.idpmis = d[0].key;        
        ll_lviec.idpmis[this.index] = {
            ID_PMIS: d[0].key,
            ID_BB: modal_treepmis.id_bb
        }
        $("#DiaDiem_" + this.index).val(d[0].title)
    },

    save: function (id_plv, index) {
       var data= ll_lviec.idpmis[index] 
        var url = "api/v1/ecp2021/UpdateBBanPLV?IDConect=" + $('#DonViID').val() + "&idplv=" + id_plv + "&ID_BBAN=" + data.ID_BB + "&ID_VITRI_PMIS=" + data.ID_PMIS;
        service.ExcuteAjaxtGet(url, null, (d) => { });
    }
}



var qlthietbi = {
    qltb: [],
    index: null,
    isload:null,
    addThietBi: function (index) {
        this.index = index;
        $('#modal_addTBi').modal();
        this.getAllTBi();
    },

    chon: function () {

    },

    huy: function () {
        $('#modal_addTBi').modal(Hide);
    },

    submit: function (index) {
        // ko có dữ liệu ko cần submit
        if (qltb[index] == null) return;
    },
    // load thiết bị
    getAllTBi: function () {
        var url = "api/v1/ecp2021/GetTBi?loaitb=1&ID_Connect=" + service.ID_CONNECT;
        service.ExcuteAjaxtGet(url, null,(d)=> {
            var html = ''
            var data = d.Data;
            for (i = 0; i < data.length; ++i) {
                html += '<option value=' + data[i].ID + '>' + data[i].TenLoai + '</option>';
            }
            $('#cboloaitb').html(html);

            });
    }
}



var service = {
    apiserver: $('#UrlKTGS').val(),
    ID_CONNECT: '',
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
        var madv = $('#DonViID').val();
        if (madv.length == 6)
            madv = madv.substr(0, 4);
        this.ID_CONNECT = madv;
        data = {
            "UserName": $('#UserName').val(),//,
            "ID_DV": this.ID_CONNECT,
        }

        this.ExcuteAjaxtPost("api/v1/Auth/ECPAuth", JSON.stringify(data), function (d) {
            service.token = d.Data.Token;
        });
        // get donvi
    }

};

service.init();

