var listbbks = {
    parrent: null,
    idpmis: null,
    currrent_row: null,
    init: function () {
        this.inittable();
        //this.tables = $('#tbllistbbks').DataTable(this.inittable);
        //$('#tbllistbbks tbody').on('click', 'a', function () {
        //    var tables = $('#tbllistbbks').DataTable();
        //    var tableData = tables.table($(this).parents('table'));
        //    var row = tableData.row($(this).parents('tr'));
        //    listbbks.tr = $(this).parents('tr');
        //    try {
        //        if ($($(this).parents('tr')).hasClass('selected')) {
        //            $($(this).parents('tr')).removeClass('selected');
        //        }
        //        else {
        //            $('tr.selected').removeClass('selected');
        //            $($(this).parents('tr')).addClass('selected');
        //        }
        //    } catch (e) {
        //        console.log(e);
        //    }
        //    listbbks.currrent_row = row;

        //    var strheader = $(this);

        //});
        this.loaddata();

    },
    loaddata: function () {
        trangthai = $('#drtrangthai').val();
        service.ExcuteAjaxt("GetAllBBKS_TT?TRANG_THAI=" + trangthai, null, function (d) {
            //var jsondata = JSON.stringify(d.Data);
            //console.log("GetAllBBKS_TT__" + jsondata);
            var oTable = $('#tbllistbbks').dataTable();
            oTable.fnClearTable();
            //tbllistbbks.oTable = $("#tbllistbbks").dataTable();
            //tbllistbbks.oTable.fnClearTable();
            //if (d.Data.length > 0)
            oTable.fnAddData(d.Data);
        });
    },
    changecbo: function () {
        this.loaddata();
    },
    inittable: function () {
        var oTable = $('#tbllistbbks').DataTable({
            "bLengthChange": false,
            "searching": true,
            //"scrollY": '40vh',
            //"scrollCollapse": true,
            "paging": true,
            "info": false,
            "language": {
                "emptyTable": "Không có biên bản khảo sát hiện trường nào"
            },
            "columns": [
                { "data": "TEN", "width": "30%" },
                { "data": "TINH_TRANG", "width": "20%" },
                { "data": "NGUOI_LAP", "width": "10%" },
                {
                    "data": "NGAY_LAP", "width": "10%", render: function (data, type, row) {
                        return util.FormatDateToVn(data);
                    }
                },
                { "data": "NGUOI_DUYET", "width": "10%" },
                {
                    "data": "NGAY_DUYET", "width": "10%", render: function (data, type, row) {
                        return util.FormatDateToVn(data);
                    }
                },
                {//Cột để báo cáo
                    "data": "Parent_ID", render: function (data, type, row) {
                        html = '<label style="width: 100%;"> <input name="form-field-checkbox" ID="nb' + row.ID + '"  onchange="listbbks.checkchange(' + data + ',this,2)" class="check ace ace-checkbox-2 grnb_bc' + row.Parent_ID + ' row_bc" type="checkbox" ><span class="lbl"></span></label>';
                        return html;
                    }, "width": "10%", class: "dt-center"
                },
            ],
            "columnDefs": [

            ],
            "aaSortingFixed": [[0, 'desc']]
        });


    },
    checkchange: function (data, e, type) {
        if (e.checked) {
            $(e).parent().parent().parent().addClass("selected");
        } else {
            $(e).parent().parent().parent().removeClass("selected");
        }
    },
    DuyetBB: function () {        
        var trangthai = $('#drtrangthai').val();
        var oTable = $('#tbllistbbks').DataTable();
        var dataTableRows = oTable.rows('.selected').data().toArray();

        var tblData = oTable.rows('.selected').data();
        var tmpData;
        $.each(tblData, function (i, val) {
            tmpData = tblData[i];
            //alert(tmpData.ID_VITRI_PMIS);
            data = {
                id_bbks: tmpData.ID,
                ten_bb: user.HoTen,
                trang_thai: trangthai,
            };
            service.ExcuteAjaxt("DuyetBBKS", data, function (d) {
                //var jsondata = JSON.stringify(d);   
                console.log(d);
            });
        });
        //load lại dữ liệu
        this.loaddata();
    },

}

listbbks.init();