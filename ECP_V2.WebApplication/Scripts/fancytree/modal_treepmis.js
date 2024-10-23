var modal_treepmis = {
    //#RegExp Tree
    load: 0,
    selectMode: 2,   
    time: null,
    tree: null,
    parrent: null,
    id_pmis: null,
    id_bb: null,
    show: function () {
        if (this.load == 0) {
        $("#tree").fancytree({
            clickFolderMode: 3,
            //minExpandLevel: 2,
            quicksearch: true,
            extensions: ["filter"],
            checkbox: true,
            autoCollapse: false, 
            selectMode: 1,    
            click: function (event, data) {
               // logEvent(event, data, ", targetType=" + data.targetType);
                var id = data.node.key;
                modal_treepmis.id_pmis = id;
                modal_treepmis.getBienBanVitri(id);
            },
            source: $.ajax({
                url: $('#UrlKTGS').val() + "api/v1/PMISTree/GetTreePMIS?dvi=" + $('#DonViID').val(),
                dataType: "json"
            }),
            filter: {
                autoApply: true,
                autoExpand: true,
                mode: "hide"
            },
        });
        modal_treepmis.tree = $("#tree").fancytree("getTree");
        $(".fancytree-container").addClass("fancytree-connectors");
         modal_treepmis.tree.activateKey("-1");
        }
        this.load = 1;
        $('#modal_treepmis').modal();

        $('#txttreesearch').keyup(function (e) {
            try {
                clearTimeout(this.time);
                this.time = setTimeout(function(){
                    var txtquery = $('#txttreesearch').val();
                    txtquery == "" ? modal_treepmis.tree.clearFilter() : modal_treepmis.tree.filterNodes(txtquery)
                }, 1000)              
            } catch (t) {
                alert(t)
            }
        });
    },

    chon: function () {
        this.id_bb = $('#cbobb').val();
        var selNodes = modal_treepmis.tree.getSelectedNodes();
        if (selNodes == null) {
            alert("Bạn chưa chọn");
            return;
        }
        if (modal_treepmis.parrent != null)
            modal_treepmis.parrent.chonEventCallBack(selNodes);
        $('#modal_treepmis').modal('hide');
        
    },
    huy: function () {
        $('#modal_treepmis').modal('hide');
    },


    // các funtion chọn biên bản thi công khi chọn vị trí

    getBienBanVitri:function(idvitri) {               
        url = "api/v1/ecp2021/GetBbanVitri?IDConect=" + $('#DonViID').val() + "&ID_VITRI=" + idvitri;
        service2.ExcuteAjaxtGet(url, null, (d)=>{
            console.log(d);
            var html = '';
            var data = d.Data;
            for (i = 0; i < data.length; i++) {
                html += '<option value="' + data[i].ID + '"> biên bản khảo sát:' + data[i].TEN + '</option>'
            }
            $('#cbobb').html(html);
            modal_treepmis.viewttbienban();
        });        
    },
   
    Init: function () {
        $('#cbobb').on('change', function () {
            modal_treepmis.viewttbienban();
        });
    },

    viewttbienban() {
        var idbb = $('#cbobb').val();
        url = "api/v1/ecp2021/GetTTinPA?IDConect=" + $('#DonViID').val() + "&ID_BBA=" + idbb + "&ID_VITRI_PMIS=" + modal_treepmis.id_pmis;

        service2.ExcuteAjaxtGet(url, null, (d) => {
            console.log(d);
            var pa = d.Data.pa;
            var cc = d.Data.cc;
            var html_pa = '<div><b>Phương án thi công :</b>';
            for (i = 0; i < pa.length; i++) {
                html_pa += pa[i].PAN_THIEN + "; ";
            }
            html_pa += "</div>";
            var html_cc = '<div><b>Công cụ sử dụng:</b>';
            for (i = 0; i < cc.length; i++) {
                html_cc += cc[i].TEN_CONGCU + "(" + cc[i].SOLUONG + ")";
            }
            html_cc += "</div>";
            $('#ttbb').html(html_pa + html_cc);


        });
    }
}
modal_treepmis.Init();

var service2 = {
    apiserver: $('#UrlKTGS').val(),
    token: '',
    ExcuteAjaxtGet: function ExcuteAjaxt(url, data, callback, async) {
        var asy = false;
        if (async != null)
            asy = async
        $.ajax({
            type: "GET",
            url: this.apiserver + url,
            data: data,            
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




};

