var kt_atoan = {
    tree: null,
    init: function () {
        $('#btnaddbuoc').hide();
        $("#tree").fancytree({
            clickFolderMode: 4,
            //minExpandLevel: 2,
            quicksearch: true,
            extensions: ["filter"],
            checkbox: false,
            autoCollapse: true,
            selectMode: 1,
            source: $.ajax({
                url: service.apiserver + "api/v1/ecp2021/GetTreeKTAT",
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
                        return;
                    } else {
                        $('#btnaddbuoc').hide();
                    }
                    $('#lblbuoc').val(d.TEN);
                    $('#lblnoidung').val(d.NDUNG);
                    var f = d.File_Amthanh;
                    var html = '';                   
                    $('#divmedia').html(html);
                    console.log(event);
                    console.log(i);
                }
                catch {

                }
            },
        });
        kt_atoan.tree = $("#tree").fancytree("getTree");

    },
    reloadtree: function () {
        kt_atoan.tree.reload($.ajax({
            url: service.apiserver + "api/v1/ecp2021/GetTreeKTAT",
            dataType: "json"
        }));
    },
    add: function () {
        modal_addhl_ktat.p = this;
        modal_addhl_ktat.show();
    },
    addcallback: function (d) {        
        //var node = this.tree.getNodeByKey("-1");
        var node = $("#tree").fancytree("getRootNode");
        var inode = { title: d.TEN, key: d.ID, folder: true, expanded: true, data: d };
        node.addChildren(inode);
    },
    addbuoc: function () {
        var node = kt_atoan.tree.getActiveNode();
        if (node.folder == false) {
            alert('Đề nghị a chị chọn loại công việc để tạo bước');
            return;
        }
        modal_addbuoc_ktat.p = this;
        modal_addbuoc_ktat.data = node.data.ID;
        modal_addbuoc_ktat.show();

    },
    addbuocCallback: function (d) {
        this.reloadtree();
    }
}
kt_atoan.init();

var modal_addhl_ktat = {
    p: null,
    data: null,
    show: function () {
        $('#modal_addhl_ktat').modal();

    },
    submit: function () {
        var url = "api/v1/ecp2021/CreateKTAT_CDE";
        var data = { TEN: $('#txttencv_ktat').val(), };
        service.ExcuteAjaxtPost(url, JSON.stringify(data), (d) => {
            modal_addhl_ktat.p.addcallback(d.Data);
        });
        $('#modal_addhl_ktat').modal('hide');
    },

    cancel: function () {
        $('#modal_addhl_ktat').modal('hide');
    }

}

var modal_addbuoc_ktat = {
    data: null,
    p: null,
    show: function () {
        $('#modal_addbuoc_ktat').modal();
    },
    submit: function () {       
        var url = "api/v1/ecp2021/CreateKTAT_CAUHOI";
        var data = {
            ID_CDE: this.data,            
            TEN: $('#txtnoidung_ktat').val(),
            NDUNG: $('#txtnd_traloi_ktat').val(),            
        };
        service.ExcuteAjaxtPost(url, JSON.stringify(data), (d) => {                                 
                modal_addbuoc_ktat.p.addbuocCallback(d.Data);          
        });
        $('#modal_addbuoc_ktat').modal('hide');
    },
    cancel: function () {
        $('#modal_addbuoc_ktat').modal('hide');
    }
}
