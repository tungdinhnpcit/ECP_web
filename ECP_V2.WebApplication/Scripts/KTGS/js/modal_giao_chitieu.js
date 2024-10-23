var modal_giao_chitieu = {
    p:null,
    init: function () {
        var html = "";       
        //for (i = 1; i < 13; ++i) {
        //    if (i == new Date().getMonth() + 1) {
        //        html += "<option selected='selected' value=" + i + ">" + i + "</option>";
        //    } else
        //        html += "<option value=" + i + ">" + i + "</option>";
        //}
        //$('#cbonammodal').html(html);

        html = "";
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvigiao').html(html);
    },
    show: function () {
        $('#modal_giao_chitieu').modal();
    },
    submit: function () {
        var ctieu = $('#txtctieuso').val();
        var donvi = $('#cbodonvigiao').val();
        var nam = $('#cbonamgiao').val();
        var url = "api/v1/ecp2021/GiaoChiTieu?nam=" + nam + "&madvi=" + donvi + "&soluong=" + ctieu + "&idnhom=" + ecp_2021_giaoct.ID_NhomLoaiCongViec;
        service.ExcuteAjaxtGet(url, null, (d) => {
            var data= d.Data;
            modal_giao_chitieu.p.addcalback(data);
        });

        $('#modal_giao_chitieu').modal('hide');
    },
    cancel: function () {
        $('#modal_giao_chitieu').modal('hide');
    },

}
modal_giao_chitieu.init();