var model_dm_ruiro = {
    parrent: null,
    showmodel: function () {
        $('#modal_addruiro').modal();

    },
    addruiro: function () {
        var tenruiro = $('#txtten').val();
        service.ExcuteAjaxt("AdddanhMuc?ten=" + tenruiro, null, (d) => {
            model_dm_ruiro.parrent.addCallback(d.Data);
            $('#modal_addruiro').modal('hide');
        });

    }
}