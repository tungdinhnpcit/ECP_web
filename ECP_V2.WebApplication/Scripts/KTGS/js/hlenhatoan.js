var hlenhatoan = {
    tree:null,
    init: function () {        
        $('#btnaddbuoc').hide();
        $("#tree").fancytree({
            clickFolderMode: 3,
            minExpandLevel: 1,
            quicksearch: true,
            extensions: ["filter"],
            checkbox: false,
            //autoCollapse: true,
            selectMode: 1,
            source: $.ajax({
                url: service.apiserver + "api/v1/ecp2021/GetTree",
                dataType: "json"
            }),
            filter: {
                autoApply: true,
                autoExpand: true,
                mode: "hide"
            },
            click: function (i, event) {
                try {                    
                    var d = event.node.data;
                    if (event.node.folder == true) {
                        $('#btnaddbuoc').show();
                    } else {
                        $('#btnaddbuoc').hide();
                    }
                    $('#lblbuoc').val(d.BUOC);
                    $('#lblnoidung').val(d.NOI_DUNG);
                    var f = d.File_Amthanh;
                    var html = '';
                    if (f.indexOf(".mp4") > 0)
                        html = `<video width="400" height="400" controls>
                                <source src = "`+ service.apiserver + 'UploadMedia/' + d.File_Amthanh + `" type = "video/mp4">
                     
                        </video>`;
                    else if (f.indexOf(".mp3") > 0)
                        html = `<audio width="400" height="400" controls>
                                <source src = "`+ service.apiserver + 'UploadMedia/' + d.File_Amthanh + `" type = "audio/mpeg">
                     
                        </audio>`;
                    $('#divmedia').html(html);
                    console.log(event);
                    console.log(i);
                }
                catch {

                }
            },
        });
        hlenhatoan.tree = $("#tree").fancytree("getTree");

    },
    reloadtree: function(){
        hlenhatoan.tree.reload($.ajax({
            url: service.apiserver + "api/v1/ecp2021/GetTree",
            dataType: "json"
        }));
    },
    add: function () {
        modal_addhl.p = this;
        modal_addhl.show();
    },
    addcallback: function (d) {
        //
        //var node = this.tree.getNodeByKey("-1");
        var node = $("#tree").fancytree("getRootNode");
        var inode = { title: d.TEN, key: d.ID, folder: true, expanded: true, data: d };
        node.addChildren(inode);
    },
    addbuoc: function () {
        var node = hlenhatoan.tree.getActiveNode();
        if (node.folder == false) {
            alert('Đề nghị a chị chọn loại công việc để tạo bước');
            return;
        }
        modal_addbuoc.p = this;
        modal_addbuoc.data = node.data.ID;
        modal_addbuoc.show();
        
    },
    addbuocCallback: function (d) {
        this.reloadtree();
    }
}
hlenhatoan.init();

var modal_addhl = {
    p: null,
    data:null,
    show: function () {
        $('#modal_addhl').modal();

    },
    submit: function () {
        var url = "api/v1/ecp2021/CreateHLAT";
        var data = { TEN: $('#txttencv').val(), };
        service.ExcuteAjaxtPost(url, JSON.stringify(data), (d) => {
            modal_addhl.p.addcallback(d.Data);
        });
        $('#modal_addhl').modal('hide');
    },

    cancel: function () {
        $('#modal_addhl').modal('hide');
    }

}

var modal_addbuoc = {
    data: null,
    p: null,
    show: function () {
        $('#modal_addbuoc').modal();
    },
    submit: function () {
        if (file_hlat_upload.files.length == 0) {
            alert('Đề nghị upload file');
            return;
        }
        var url = "api/v1/ecp2021/CreateHLAT_CTIET";
        var data = {
            BUOC: $('#txtbuoc').val(),
            NOI_DUNG: $('#txtnoidung').val(),
            ID_HL:this.data
        };
        service.ExcuteAjaxtPost(url, JSON.stringify(data), (d) => {
            // upload file âm thanh
            var urlupload = "api/v1/ecp2021/Uploadfile";
            var formData = new FormData();
            formData.append('ID', d.Data.ID);
            formData.append('files', file_hlat_upload.files[0]);
            service.ExcuteAjaxtUpload(urlupload, formData, (d) => {
                modal_addbuoc.p.addbuocCallback(d.Data);
            });            
        });
        $('#modal_addbuoc').modal('hide');
    },
    cancel: function () {
        $('#modal_addbuoc').modal('hide');
    }
}