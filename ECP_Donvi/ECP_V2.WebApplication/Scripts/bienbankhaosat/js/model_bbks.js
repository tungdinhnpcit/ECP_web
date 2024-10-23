var model_bbks = {
    parrent: null,
    showmodel: function () {
        $('#modal_addbbks').modal();

    },
    addbienban: function () {           
        var tenbienban = $('#txtten').val();
        service.ExcuteAjaxt("AddBienBan?ten=" + tenbienban + "&nguoilap=" + user.HoTen, null, (d) => {
            model_bbks.parrent.addCallback(d.Data);
            $('#modal_addbbks').modal('hide');
        });

    },
    editbienban: function () {
        var tenbienban = $('#txtten').val();
        var id_bbks = $('#id_bbks').val();
        console.log("id_bbks" + id_bbks);
        service.ExcuteAjaxt("EditBienBan?ten=" + tenbienban + "&nguoilap=" + user.HoTen + "&idbb=" + id_bbks, null, (d) => {
            model_bbks.parrent.addCallback(d.Data);
            $('#modal_addbbks').modal('hide');
        });

    },
    showmodel_vitri: function () {
        var id_bbks = $('#id_bbks').val();
        service.ExcuteAjaxt("GetAllViTri?id_bbks=" + id_bbks, null, (d) => {  
            var jsondata = JSON.stringify(d.Data);
            //var result = JSON.parse(d);
            //$('#txttagread').val(d.Data.TEN_VITRI);
        //    //$('#txttagread').parent().find('.tag').remove();
        //    //var $tag_read = $('#txttagread').data('tag');
        //    //$tag_read.values = [];
            console.log("d.Data" + jsondata);
        //    //for (i = 0; i < d.Data.length; ++i) {
        //    //    $tag_read.add(d.Data[i].UserName);
        //    //}
        });
        $('#modal_vitriBBKS').modal();

    },
    addvitri: function () {
        var id_bbks = $('#id_bbks').val();
        var tenvitri = $('#txttagread').val();
        service.ExcuteAjaxt("AddViTri?id_bbks=" + id_bbks + "&id_vitri=" + tenvitri + "&ten_vitri=" + tenvitri, null, (d) => {
            model_bbks.parrent.addCallback(d.Data);
            $('#modal_addbbks').modal('hide');
        });

    },
}