var ktgs = {
    currrent_row: null,
    data:null,
    init: function () {
        this.tables = $('#tblktgs').DataTable(this.optiontable);  
        // tao cbo thang
        var html = "";
        for (i = 1; i < 13; ++i) {
            if (i == new Date().getMonth() + 1) {
                html += "<option selected='selected' value=" + i + ">" + i + "</option>";
            } else
                html += "<option value=" + i + ">" + i + "</option>";
        }
        $('#cbothang').html(html);
        // row click
        $('#tblktgs tbody').on('click', 'a', function () {
            var tables = $('#tblktgs').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            ktgs.tr = $(this).parents('tr');
            try {
                if ($($(this).parents('tr')).hasClass('selected')) {
                    $($(this).parents('tr')).removeClass('selected');
                }
                else {
                    $('tr.selected').removeClass('selected');
                    $($(this).parents('tr')).addClass('selected');
                }
            } catch (e) {
                console.log(e);
            }
            ktgs.currrent_row = row;

            var strheader = $(this);

            if (strheader.hasClass('tso')) {
                modal_ds_ktgs.data = data.CTIET;
                modal_ds_ktgs.show();
            }

            if (strheader.hasClass('tsott')) {
                modal_ds_ktgs.data = data.CTIET_TT;
                modal_ds_ktgs.show();
            }

            if (strheader.hasClass('tsokp')) {
                modal_ds_ktgs.data = data.CTIET_TT_DAKPHUC;
                modal_ds_ktgs.show();
            }
          
        });      
    },
// lay danh sach bien ban theo don vi
    loaddata: function () {
        var thang = $('#cbothang').val();
        var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();

        var cbotontai = $('#cbotontai').val();
        var cbokpngay = $('#cbokpngay').val();


        url = "api/v1/PLV/GetTsoBienBanTheoDonvi?madvi=-1&madvics=" + madvi + "&thang=" + thang + "&nam=" + nam + "&cbotontai=" + cbotontai + "&cbokpngay=" + cbokpngay;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            ktgs.data = d.Data;
            ktgs.oTable = $("#tblktgs").dataTable();
            ktgs.oTable.fnClearTable();
            if(d.Data.length > 0)
               ktgs.oTable.fnAddData(d.Data);
        });
    },

    loadgroup2: function (t) {
        try {
            var tables = $('#tblktgs').DataTable();
            var tableData = tables.table($(t).parents('table'));
            var row = tableData.row($(t).parents('tr'));
            try {
                if ($(t).parents('tr').hasClass('selected')) {
                    $(t).parents('tr').removeClass('selected');
                }
                else {
                    tableData.$('tr.selected').removeClass('selected');
                    $(t).parents('tr').addClass('selected');
                }
            } catch (e) {
                console.log(e);
            }
            if ($(t).find('#iconclick').hasClass('glyphicon-plus')) {
                $(t).find('#iconclick').removeClass('glyphicon-plus');
                $(t).find('#iconclick').addClass('glyphicon-minus');
            } else {
                $(t).find('#iconclick').removeClass('glyphicon-minus');
                $(t).find('#iconclick').addClass('glyphicon-plus');
            }

            var tr = $(t).closest('tr');
            var d = row.data();
            var rowt = tables.row(tr);
            if (rowt.child.isShown()) {
                // This row is already open - close it
                rowt.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                var html = '';
                html = ktgs.fomattb(d.GROUP_L2);
                rowt.child(html).show();
                tr.addClass('shown');
            }
            this.load_ct = 1;
        } catch (e) {
            console.log(e);
        }
    },
    fomattb: function (d) {
        var html = '<table id="TB_CTIET" cellspacing = "0" border = "0" style = "padding-left:50px; width:100%;" >';
        var body = '';
        var edit = '';
        for (var i = 0; i < d.length; i++) {
            body = body + '<tr style="border:none">' +
                '<td style="width: 50%">' + d[i].DONVI +'-'+util.GetNameDonvi(d[i].DONVI) + '</td>' +
                '<td style="width: 15%">' + d[i].TSO + '</td>' +
                '<td style="width: 15%">' + d[i].TSO_TT + '</td>' +
                '<td style="width: 15%">' + d[i].TSO_TT_DAKPHUC + '</td>';
               
                //'<td style="display:none" class="CV_ID">' + d[i].CV_ID + '</td>' +
                //'<td style="display:none" class="MA_TIEU_CHI">' + d[i].MA_TIEU_CHI + '</td>' +
                //'<td style="display:none" class="PB_ID">' + d[i].PB_ID + '</td>' +
                
            body = body + '</tr>';
        }
        html = html + body + '</table>';
        return html;
    },

    optiontable: {
        "bLengthChange": true,
        "searching": true,
        "paging": true,
        "info": true,
        "scrollY": '70vh',
        "scrollCollapse": true,
        "pageLength": 50,
        "lengthMenu": [[50, 100, 500, 1000], [50, 100, 500, 1000]],
        "language": {
            "emptyTable": "Bảng chưa có dữ liệu",
            "info": "<b>Tổng số: _START_-_END_/_TOTAL_ </b>",
            "lengthMenu": "Xem _MENU_ bản ghi",
            "search": "Tìm nội dung:",
            "paginate": {
                "first": "Đầu",
                "last": "Cuối",
                "next": "Trang tiếp",
                "previous": "Trang trước"
            },
        },
        //"order": [[2, "desc"]],
        "columns": [
            {
                "data": "DONVI", render: function (data, type, row) {
                    
                    var html = '';
                    if (row.DONVI == "PA")
                       return html = '<b> Cơ quan tổng công ty </b>';
                    else
                        html = '<a href="#" style="margin-left:5px" onclick="ktgs.loadgroup2(this)"><span id="iconclick" class="glyphicon glyphicon-plus"></span></a><b>';
                    return html + row.DONVI + "-"+util.GetNameDonvi(row.DONVI) +'</b>';
                }, "width": "20%"

               //"data": "DONVI", render: function (data, type, row) {
               //     var html '';
               //     return html + row.DONVI + "-" + util.GetNameDonvi(row.IDDVI_LAPBB) +';
               // }, "width": "20%"
            },
            {
                "data": "TSO", render: function (data, type, row) {
                    var html = '';
                    html = '<a class="tso">' + row.TSO +'</a>';
                    return html;
                }, "width": "15%" },
            {
                "data": "TSO_TT", render: function (data, type, row) {
                    var html = '';
                    html = '<a class=" tsott">' + row.TSO_TT + '</a>';                        
                    return html;
                }, "width": "15%" },
            {
                "data": "TSO_TT_DAKPHUC", render: function (data, type, row) {
                    var html = '';
                    html = '<a class="tsokp">' + row.TSO_TT_DAKPHUC+'</a>';                        
                    return html;
                }, "width": "20%" },          
        ],

    },

    excel: function () {
        // call api export excel
        var thang = $('#cbothang').val();
        var nam = $('#cbonam').val();
        var url = "api/v1/PLV/Excel?thang=" + thang + "&nam="+nam;
        service.ExcuteAjaxtPost(url, JSON.stringify(ktgs.data), (d) => {
            window.open(service.apiserver + "api/v1/PLV/DownloadExcel?path=" + d.Data);
            //console.log("ok");

        });

        //$.ajax({
        //    type: "Post",
        //    url: url,
        //    data: JSON.stringify(ktgs.data),
        //    dataType: 'json',
        //    contentType: "application/json",                        
        //}).done(function (d) {
       
        //});     
    }
}

ktgs.init();