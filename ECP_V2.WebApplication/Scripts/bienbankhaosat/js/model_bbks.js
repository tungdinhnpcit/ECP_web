
var model_bbks = {
    parrent: null,
    idpmis: null,
    files_vatu: [],    
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
        //var id_bbks = $('#id_bbks').val();
        //var tenvitri = $('#txttagread').val();
       
            data = {
                id_bbks: $('#id_bbks').val(),
                id_vitri: $('#lblidpmis').html(),
                ten_vitri: $('#txtvitri').val(),
                loai_congviec: $('#txtloaicv').val(),
                ndung_congviec: $('#txtnoidungcv').val(),
                phuongan_thien: $('#txtphuonganth').val(),
                bienphap_at: $('#txtbienphapat').val(),          
            };
            //var fdata = new FormData();
            //fdata.append("id_bbks", $('#id_bbks').val());
            //fdata.append("id_vitri", $('#lblidpmis').html());
            //fdata.append("ten_vitri", $('#txtvitri').val());
            //fdata.append("loai_congviec", $('#txtloaicv').val());
            //fdata.append("ndung_congviec", $('#txtnoidungcv').val());
            //fdata.append("phuongan_thien", $('#txtphuonganth').val());
            //fdata.append("bienphap_at", $('#txtbienphapat').val());
            //fdata.append("congcu_dungcu", this.files_vatu);
        //var jsondata = JSON.stringify(data);
        service.ExcuteAjaxt("AddViTri", data, function (d) {
                       
            });
        for (i = 0; i < this.files_vatu.length; ++i) {
            data = {
                id_bbks: $('#id_bbks').val(),
                id_vitri: $('#lblidpmis').html(),
                ten_vitri: $('#txtvitri').val(),
                ID_CC: model_bbks.files_vatu[i].ID,
                TenLoai: model_bbks.files_vatu[i].TenLoai,
                SOLUONG: model_bbks.files_vatu[i].SOLUONG
            };
            service.ExcuteAjaxt("AddCongCu", data, function (d) {

            });
        };
        $('#modal_vitriBBKS').modal('hide');
        //service.ExcuteAjaxt("AddViTri" + id_bbks + "&id_vitri=" + tenvitri + "&ten_vitri=" + tenvitri, null, (d) => {
        //    model_bbks.parrent.addCallback(d.Data);
        //    $('#modal_addbbks').modal('hide');
        //});

    },
    gettreepmis: function () {
        modal_treepmis.parrent = this;
        modal_treepmis.show();
    },
    // get item tu pmis
    chonEventCallBack: function (d) {
        $('#txtvitri').val(d[0].title);
        $('#lblidpmis').html(d[0].key);
        this.idpmis = d[0].key;
    },
    inittable: function () {
        var oTable = $('#tblvattu').DataTable({
            "bLengthChange": false,
            "searching": true,
            //"scrollY": '40vh',
            //"scrollCollapse": true,
            "paging": true,
            "info": false,
            "language": {
                "emptyTable": "Không có vị trí trong biên bản"
            },
            "columns": [
                { "data": "ID_VITRI_PMIS", "width": "20%" },
                { "data": "TEN_VITRI", "width": "20%" },
                { "data": "NDUNG_CVIEC", "width": "15%" },
                { "data": "PAN_THIEN", "width": "15%" },
                { "data": "BPHAP_ATOAN", "width": "10%" },
                { "data": "CCU_DCU", "width": "10%" },
                {//Cột để báo cáo
                    "data": "Parent_ID", render: function (data, type, row) {
                        html = '<label style="width: 100%;"> <input name="form-field-checkbox" ID="nb' + row.ID + '"  onchange="model_bbks.checkchange(' + data + ',this,2)" class="check ace ace-checkbox-2 grnb_bc' + row.Parent_ID + ' row_bc" type="checkbox" ><span class="lbl"></span></label>';
                        return html;
                    }, "width": "10%", class: "dt-center"
                },
            ],
            "columnDefs": [

            ],
            "aaSortingFixed": [[0, 'desc']]
        });
    },
    inittableCCDC: function () {
        var oTable1 = $('#tblccdc').DataTable({
            "bLengthChange": false,
            "searching": true,
            //"scrollY": '40vh',
            //"scrollCollapse": true,
            "paging": true,
            "info": false,
            "language": {
                "emptyTable": "Không có Công cụ dụng cụ nào"
            },
            "columns": [
                { "data": "ID", "width": "20%" },
                { "data": "TenLoai", "width": "20%" },
                { "data": "NamSX", "width": "15%" },
                {//Cột để báo cáo
                    "data": "Parent_ID", render: function (data, type, row) {
                        html = '<label style="width: 100%;"> <input name="form-field-checkbox" ID="nb' + row.ID + '"  onchange="model_bbks.checkchange(' + data + ',this,2)" class="check ace ace-checkbox-2 grnb_bc' + row.Parent_ID + ' row_bc" type="checkbox" ><span class="lbl"></span></label>';
                        return html;
                    }, "width": "10%", class: "dt-center"
                },
            ],
            "columnDefs": [

            ],
            "aaSortingFixed": [[0, 'desc']]
        });
    },
    view_vitri: function () {
        var id_bbks = $('#id_bbks').val();
        this.inittable();
        service.ExcuteAjaxt("GetAllViTri?id_bbks=" + id_bbks, null, (d) => {
            var jsondata = JSON.stringify(d.Data);
            console.log("View Vi Tri" + jsondata);
            var oTable = $('#tblvattu').dataTable();
            oTable.fnClearTable();

            oTable.fnAddData(d.Data);
        });
        $('#modal_viewvitriBBKS').modal();

    },
    checkchange: function (data, e, type) {
        if (e.checked) {
            $(e).parent().parent().parent().addClass("selected");
        } else {
            $(e).parent().parent().parent().removeClass("selected");
        }
    },
    deletevitri: function () {
        var id_bbks = $('#id_bbks').val();
        var oTable = $('#tblvattu').DataTable();
        var dataTableRows = oTable.rows('.selected').data().toArray();

        var tblData = oTable.rows('.selected').data();
        var tmpData;
        $.each(tblData, function (i, val) {
            tmpData = tblData[i];
            //alert(tmpData.ID_VITRI_PMIS);
            data = {
                id_bbks: $('#id_bbks').val(),
                id_vitri: tmpData.ID_VITRI_PMIS,
            };
            service.ExcuteAjaxt("DelViTri", data, function (d) {
                //model_bbks.parent.addCallback(d.Data);
                var jsondata = JSON.stringify(d);
                $('#modal_viewvitriBBKS').modal('hide');
            });
        });
    },

    chonCCDC: function () {
        var id_bbks = $('#id_bbks').val();
        this.inittableCCDC();
        this.inittableKQ();
        service.ExcuteAjaxt("GetAllCCDC?id_bbks=" + id_bbks, null, (d) => {
            var jsondata = JSON.stringify(d.Data);
            //console.log("View Vi Tri" + jsondata);
            var oTable1 = $('#tblccdc').dataTable();
            oTable1.fnClearTable();

            oTable1.fnAddData(d.Data);
        });
        $('#modal_viewCCDC').modal();
    },
    // TẠO TEXTBOX KHI CLICK VÀO TEXT TABLE
    fnCreateTextBox: function (value, fieldprop) {

        return '<input  data-field="' + fieldprop + '" type="text" value="' + value + '" class="inputtext tableinput" onkeyUp="model_bbks.onkeyup_table(this,event);"></input>';
    },

    // RESET ALL CONTROL TRÊN TABLE
    fnResetControls: function () {
        var openedTextBox = $('#tblkq_vattu').find('input');
        $.each(openedTextBox, function (k, $cell) {
            $(openedTextBox[k]).closest('td').html($cell.value);
        })
    },

    fnResetControls_focus: function (editelement) {
        var data_focus = null;
        var openedTextBox = $('#tblkq_vattu').find('input');
        $.each(openedTextBox, function (k, $cell) {
            $(openedTextBox[k]).closest('td').html($cell.value);
            data_focus = $cell.value;
        })
    },

    onkeyup_table: function (textbox, e) {
        e.preventDefault();
        console.log($(textbox));
        console.log(e.keyCode);
        console.log("checkclass", $(textbox).parents('td').hasClass("SOLUONG"));
        console.log("checkclass", $(textbox).parents('td').hasClass("HSD"));

        var dataTable = $('#tblkq_vattu').DataTable();
        var rowIndex = dataTable.row($(textbox).closest('tr')).index();
        var rowdata = dataTable.row($(textbox).closest('tr')).data();
        console.log(rowIndex);
        console.log(rowdata);
        model_bbks.tinhlai_table(rowIndex, $(textbox).val(), textbox);
        if (e.keyCode == 13) {
            //model_bbks.fnResetControls_focus();
            model_bbks.fnResetControls();
        }
    },

    tinhlai_table: function (index, value, textbox) {
        var columedit = "SOLUONG";

        var soluong = model_bbks.files_vatu[index]["SOLUONG"];


        // NEU COLUMN EDIT LA SOLUONG
        if (value == "") value = "0";
        if (columedit == "SOLUONG") {
            soluong = parseFloat(value);
            var dataTable = $('#tblkq_vattu').DataTable();
            dataTable.rows().data()[index]["SOLUONG"] = soluong;
            // update source
            this.files_vatu[index]["SOLUONG"] = soluong;
            console.log("sửa index", index);
        }

    },
    xoa_VatTu: function (index) {
        console.log(index);
        this.files_vatu.splice(index, 1);
        this.oTable.fnClearTable();
        if (this.files_vatu.length > 0) {
            this.oTable.fnAddData(this.files_vatu);
        }
    },
    inittableKQ: function() {

        //  SƯ KIEN CLICK ROW TABLE VẬT TƯ
        $('#tblkq_vattu').on('click', 'a', function (e) {
            var tables = $('#tblkq_vattu').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var rowData = row.data();
            var index = row.index();



            // DELETE
            if ($(this).hasClass('del_method')) {
                e.preventDefault();
                model_bbks.xoa_VatTu(index);
            }


        });

$('#tblkq_vattu').on('click', 'td.editable', function () {
    console.log(this);
    if ($(this).find(".inputtext").length == 0) {
        var tables = $('#tblkq_vattu').DataTable();
        var tableData = tables.table($(this).parents('table'));
        var row = tableData.row($(this).parents('tr'));
        var rowData = row.data();

        model_bbks.fnResetControls ();

        if ($(this).hasClass('SOLUONG')) this._classEdit = "SOLUONG";
        else this._classEdit = "";

        this._rowEdit = row;


        var html = model_bbks.fnCreateTextBox (rowData["SOLUONG"], 'name');
        $(this).html($(html))
        console.log(this);
        var input = $(this).find("input");
        console.log(input);
        input.focus();
        //inputtext tableinput
        $('form input').keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return;
            }
        });

        $('.input').keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        });



    }
});

$('#tblkq_vattu').on('focusout', 'td.editable', function () {
    this.fnResetControls_focus(this);
});


this.oTable = $('#tblkq_vattu').DataTable({
    "bLengthChange": false,
    "searching": false,
    //"scrollY": '40vh',
    //"scrollCollapse": true,
    "paging": false,
    "info": false,
    "language": {
        "emptyTable": "Không có công cụ dụng cụ"
    },
    "columns": [
        { "data": "ID", "width": "20%" },
        { "data": "TenLoai", "width": "30%" },
        {
            "data": "SOLUONG", "width": "10%",
            "className": "editable SOLUONG"
        },
        { "data": "NamSX", "width": "10%" },
        {
            "data": "TYPE",
            render: function (data, type, row) {
                var btnhtml = '';
                btnhtml += '<a title = "Xóa" class="del_method m-10"><i class="glyph-icon icon-close" style="color:red">x</i></a>';

                return btnhtml;
            },
            "width": "30px"
        },

    ],
    "columnDefs": [

    ],
    "aaSortingFixed": [[0, 'desc']]
});

        },
    add_VT: function (LIST_IN) {
        this.files_vatu = LIST_IN;
        //console.log("md_tt_pdvb.files_vatu", vattu_dk.files_vatu);
        this.oTable = $('#tblkq_vattu').dataTable();
        this.oTable.fnClearTable();
        this.oTable.fnAddData(this.files_vatu);
    },
    okccdc: function () {
    const _this = this;
    var oTable = $('#tblccdc').DataTable();
    var dataTableRows = oTable.rows('.selected').data().toArray();

    var fuc = this.files_vatu;
    var tblData = oTable.rows('.selected').data();
    var tmpData;
    $.each(tblData, function (i, val) {
        tmpData = tblData[i];
        //alert(tmpData.ID);
        var newvt = tblData[i];
        newvt.SOLUONG = 1;
        fuc.push(newvt);
        _this.add_VT(fuc);       
    });
},


}