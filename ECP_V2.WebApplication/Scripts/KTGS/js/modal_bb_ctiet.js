var modal_bb_ctiet = {
    data: null,
    parrent: null,
    imgs: null,
    show: function () {
        $('#modal_ctbb').html($("#template_ktgs").tmpl(this.data));
        $('#modal_ctbb').modal();
        this.loaddata();
    },

    loaddata: function () {
        url = "api/v1/PLV/GetDsAnhKTGS?IDConect=PA&id_ktgs=" + this.data.ID;
        service.ExcuteAjaxtGet(url, null, function (d) {
            modal_bb_ctiet.imgs = d.Data;
            // bind image
            $("#imglager").attr("src", service.apiserver + "api/v1/Image/GetFile?path=" + modal_bb_ctiet.imgs[0].Url + "&MA_DVQL_CHA=" + modal_bb_ctiet.data.DonViCoSo);
            //$('#imglager').attr(src)
            var html = "";
            for (i = 0; i < modal_bb_ctiet.imgs.length; ++i) {
                html += "<img onclick='modal_bb_ctiet.imageclick("+i+")' class='imgthump' src='" + service.apiserver + "api/v1/Image/GetFileThump?path=" + modal_bb_ctiet.imgs[i].Url + "&MA_DVQL_CHA=" + modal_bb_ctiet.data.DonViCoSo +"' />";
            }
            $("#groupimg").html(html);

        });
        

    },
    imageclick: function (d) {
        $("#imglager").attr("src", service.apiserver + "api/v1/Image/GetFile?path=" + modal_bb_ctiet.imgs[d].Url + "&MA_DVQL_CHA=" + modal_bb_ctiet.data.DonViCoSo);
    },
    deleteimage: function () {
        var kt = confirm("Anh chị chắc xóa biên bản chứ");
        if (kt == true) {
            alert('Cập nhật thành công');
        }
    }
}