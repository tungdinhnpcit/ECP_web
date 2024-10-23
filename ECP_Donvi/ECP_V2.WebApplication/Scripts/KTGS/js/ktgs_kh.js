var ktgs_kh = {
    currrent_row: null,
    data: null,
    init: function () {
        this.tables = $('#tblktgs_kh').DataTable(this.optiontable);
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
        $('#tblktgs_kh tbody').on('click', 'a', function () {
            var tables = $('#tblktgs_kh').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            ktgs_kh.tr = $(this).parents('tr');
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
            ktgs_kh.currrent_row = row;

            var strheader = $(this);


            if (strheader.hasClass('tso')) {
                modal_ds_ktgs.data = data.CTIET;
                modal_ds_ktgs.show();
            }

        });
    },
    // lay danh sach bien ban theo don vi
    loaddata: function () {
        var thang = $('#cbothang').val();
        var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();
        url = "api/v1/PLV/GetTsoBienBanTheoChiTieu?madvi=" + madvi + "&madvics=-1&thang=" + thang + "&nam=" + nam;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            ktgs_kh.oTable = $("#tblktgs_kh").dataTable();
            ktgs_kh.oTable.fnClearTable();
            ktgs_kh.data = d.Data;
            if (d.Data.length > 0)
                ktgs_kh.oTable.fnAddData(d.Data);
        });
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
                "data": "iddonvi", render: function (data, type, row) {
                    var html = '';
                    html = row.iddonvi + '-' + util.GetNameDonvi(row.iddonvi);
                    return html;
                }, "width": "20%"
            },
            {
                "data": "ten_nhom", "width": "15%"
            },
            {
                "data": "dinh_muc_thang", "width": "15%"
            },
            {
                "data": "TSO", render: function (data, type, row) {
                    var html = '';
                    html = '<a class="tso">' + row.TSO + '</a>';
                    return html;
                }, "width": "20%"
            },
        ],

    },

    excel: function () {
        // call api export excel
        var thang = $('#cbothang').val();
        var nam = $('#cbonam').val();
        var url = "api/v1/PLV/Excel_KH?thang=" + thang + "&nam=" + nam;

        service.ExcuteAjaxtPost(url, JSON.stringify(ktgs_kh.data), (d) => {
            window.open(service.apiserver + "api/v1/PLV/DownloadExcel?path=" + d.Data);
            //console.log("ok");

        });
    },
}

ktgs_kh.init();