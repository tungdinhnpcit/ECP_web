var modal_AddRuiRo = {
    data: null,
    parent: null,
    state: null, //'add','update','kpruiro'
    show: function () {
        $('#modal_AddRuiRo').modal();
        if (this.state != 'add') {
            $('#txtvitri').val(modal_AddRuiRo.data.TEN_VITRI);
            $('#txtnoidung').val(modal_AddRuiRo.data.NOI_DUNG);
            $('#txtbp').val(modal_AddRuiRo.data.BIEN_PHAP);
            $('#txtndkp').val(modal_AddRuiRo.data.ND_KHAC_PHUC);
            $('#txtbp').val(modal_AddRuiRo.data.ID_LOAI)
            $('#divckp').show();
            $('#divkp').show();
            if (modal_AddRuiRo.data.TRANG_THAI == "DA_KP")
                $('#txtcheckkp').prop('checked', true); // Unchecks it.check;
        } else {
            $('#divckp').hide();
            $('#divkp').hide();           
        }
    },

    save: function () {
        if (this.state == 'add') {
            data = {
                TEN_VITRI : $('#txtvitri').val(),
                NOI_DUNG : $('#txtnoidung').val(),
                BIEN_PHAP: $('#txtbp').val(),
                MADVI: user.DonViID,
                ID_LOAI: $('#idrrg').val(),
            };
                        
            service.ExcuteAjaxt("AddRuiRo", data, function (d) {
                modal_AddRuiRo.parent.EventAdd(d.Data); 
                $('#modal_AddRuiRo').modal('hide');            
            });
        };
        if (this.state == 'kp') {            
            var TRANG_THAI = '';
            if ($('#txtcheckkp').is(':checked')) {
                TRANG_THAI = "DA_KP";
            } else {
                TRANG_THAI = "CHUA_KP";
            }

            var data = {
                ND_KHAC_PHUC: $('#txtndkp').val(),
                TRANG_THAI: TRANG_THAI,
                ID: modal_AddRuiRo.data.ID,
            };


            service.ExcuteAjaxt("UpdateKhacPhuc", data, function (d) {
                modal_AddRuiRo.parent.EventKhacPhuc(d.Data);
                $('#modal_AddRuiRo').modal('hide');
            
            });

        };
    },

    close: function () {
        $('#modal_AddRuiRo').modal('hide');
    }

}