var bc_ktgs = {
    currrent_row: null,
    data: null,
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
        $('#cboThang').html(html);
        html = "";
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }

        //Lấy danh mục đơn vị
        $.ajax({
            type: 'POST',
            url: '/Admin/BCKTGS/getDmDvql',
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                //if type=tba then build select tba, dz build dz
                if (response.length > 0) {
                    $.each(response, function (i, value) {
                        $('#cboDonvi')
                            .append($('<option></option>', { text: value["TenDonVi"] })
                                .attr('value', value["Id"]));
                    });
                }
            },
            error: function (ex) {
                alert('Không thể load danh mục Đơn vị');
            }
        })        

        // row click
        //$('#tblktgs tbody').on('click', 'a', function () {
        //    var tables = $('#tblktgs').DataTable();
        //    var tableData = tables.table($(this).parents('table'));
        //    var row = tableData.row($(this).parents('tr'));
        //    var data = row.data();
        //    ktgs.tr = $(this).parents('tr');
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
        //    ktgs.currrent_row = row;

        //    var strheader = $(this);

        //    if (strheader.hasClass('tso')) {
        //        modal_ds_ktgs.data = data.CTIET;
        //        modal_ds_ktgs.show();
        //    }

        //    if (strheader.hasClass('tsott')) {
        //        modal_ds_ktgs.data = data.CTIET_TT;
        //        modal_ds_ktgs.show();
        //    }

        //    if (strheader.hasClass('tsokp')) {
        //        modal_ds_ktgs.data = data.CTIET_TT_DAKPHUC;
        //        modal_ds_ktgs.show();
        //    }

        //});
        
    },

    hideMonth: function () {
        var vLoai = $('#cboLoai').val();
        var cbThang = document.getElementById('cboThang');
        var lbThang = document.getElementById('lbThang');

        if (vLoai == '2') {
            cbThang.style.display = 'none';
            lbThang.style.display = 'none';
        } else {
            cbThang.style.display = 'block';
            lbThang.style.display = 'block';
        }
        
        
    },

    loadData: function () {
        var thang = $('#cboThang').val();
        var nam = $('#cboNam').val();
        var madvi = $('#cboDonvi').val();
        var loai = $('#cboLoai').val();

        $.ajax({
            type: 'POST',
            url: '/Admin/BCKTGS/getDsKtgs',
            dataType: 'json',
            data: JSON.stringify({ vdonvi: madvi,vloai: loai,vthang:thang,vnam:nam }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                //if type=tba then build select tba, dz build dz
                $('#tblKtgs').dataTable().fnClearTable();
                if (response.length > 0) {
                    $('#tblKtgs').DataTable({
                        "language": {
                            "lengthMenu": "Hiển thị _MENU_ dòng mỗi trang",
                            "zeroRecords": "Không có dữ liệu",
                            "info": "Hiển thị _START_ đến _END_ trong _TOTAL_ dòng",
                            "infoEmpty": "",
                            "paginate": {
                                "next": ">>",
                                "previous": "<<"
                            },
                            search: "",
                            searchPlaceholder: "Tìm kiếm...",
                            "processing": "ĐANG XỮ LÝ..."
                        },
                        "destroy": true,
                        "data": response,
                        "columns": [
                            {
                                'data': 'MaDvql', render: function (data, type, row) {

                                    var html = '';
                                    if (row.MaDvql == "PA")
                                        return html = '<b> Cơ quan tổng công ty </b>';
                                    else
                                        html = '<a href="#" style="margin-left:5px;font-size:20px;padding-right:5px;" onclick="bc_ktgs.loadgroup2(this)"><span id="iconclick" class="glyphicon glyphicon-plus"></span></a><b>';
                                    return html + row.MaDvql + "-" + row.TenDvql + '</b>';
                                }, "width": "20%"
                            },
                            { 'data': 'Thang' },
                            { 'data': 'ChiTieu' },
                            { 'data': 'SlDaKtra' },
                            { 'data': 'SlCoTt' },
                            { 'data': 'SlDaKphuc' }
                        ],
                        //"columnDefs": [{
                        //    visible: false,
                        //    'targets': 1                           
                        //}],
                        //'order': [[1, 'asc']],
                        //displayLength: 25,
                        //drawCallback: function (settings) {
                        //    var api = this.api();
                        //    var rows = api.rows({ page: 'current' }).nodes();
                        //    var last = null;

                        //    api.column(1, { page: 'current' })
                        //        .data()
                        //        .each(function (group, i) {
                        //            if (last !== group) {
                        //                $(rows)
                        //                    .eq(i)
                        //                    .before('<tr class="group"><td colspan="8">' + group + '</td></tr>');

                        //                last = group;
                        //            }
                        //        });
                        //},
                    });
                }

            },
            error: function (ex) {
                alert('Không thể lấy được danh sách');
            }
        })
    },
    loadgroup2: function (t) {
        try {
            var tables = $('#tblKtgs').DataTable();
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
                html = bc_ktgs.fomattb(d.LstBcKtgs);
                rowt.child(html).show();
                tr.addClass('shown');
            }
            this.load_ct = 1;
        } catch (e) {
            console.log(e);
        }
    },
    fomattb: function (d) {
        var html = '<table id="TB_CTIET" class="table table-bordered table-responsive table-striped" cellspacing = "0" border = "1" style = "padding-left:100px; width:100%;" >';
        var body = '';
        var style = '';
        var bg = '';
        var  icon = '';
        for (var i = 0; i < d.length; i++) {
            if (d[i].ToMau === 1) {
                style = "style='background-color:#3dc1d3;color:white'";
                bg = 'background-color:#3dc1d3;color:white';
                icon = '<i class="fa fa-check"></i>'
            } else {
                icon = ' ';
            }

            body = body + '<tr ' + style + '>' +
                '<td style="width: 22%;padding-left:5px;' + bg + '">' + icon + d[i].TenNhom + '(' + d[i].HoVaTen + ')' + '</td>' +
                '<td style="width: 10%;' + bg + '">' + d[i].Thang + '</td>' +
                '<td style="width: 10%;' + bg + '">' + d[i].ChiTieu + '</td>' +
                '<td style="width: 10%;' + bg + '">' + d[i].SlDaKtra + '</td>' +
                '<td style="width: 10%;' + bg + '">' + d[i].SlCoTt + '</td>' +
                '<td style="width: 10%;' + bg + '">' + d[i].SlDaKphuc + '</td>';

            //'<td style="display:none" class="CV_ID">' + d[i].CV_ID + '</td>' +
            //'<td style="display:none" class="MA_TIEU_CHI">' + d[i].MA_TIEU_CHI + '</td>' +
            //'<td style="display:none" class="PB_ID">' + d[i].PB_ID + '</td>' +

            body = body + '</tr>';
            style = '';
            bg = '';
        }
        html = html + body + '</table>';
        return html;
    }
}
$('#cboLoai').change(bc_ktgs.hideMonth);

function Export() {
    loading('Đang tải dữ liệu...', 1);
    $("#preloader").unbind("click");
    $('#preloader').click(function () {
        unloading();
    })

    var thang = $('#cboThang').val();
    var nam = $('#cboNam').val();
    var madvi = $('#cboDonvi').val();
    var loai = $('#cboLoai').val();

    $.UnifiedExportFile(
        {
            action: "/Admin/BCKTGS/Export",
            data: {
                vdonvi: madvi,
                vloai: loai,
                vthang: thang,
                vnam: nam
            },
            downloadType: 'Progress',
            ajaxLoadingSelector: '#loading'
        });
    unloading();
}

bc_ktgs.init();