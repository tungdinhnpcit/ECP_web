

var aiplv = {
    currrent_row: null,
    data: null,
    init: function () {
        this.tables = $('#tblaiplv').DataTable(this.optiontable);
        // tao cbo thang
        var html = "";
        for (i = 1; i < 13; ++i) {
            if (i == new Date().getMonth() + 1) {
                html += "<option  value=" + i + ">" + i + "</option>";
            } else
                html += "<option value=" + i + ">" + i + "</option>";
        }
        $('#cbothang').html(html);

        html = "";
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvi').html(html);

        // row click
        $('#tblaiplv tbody').on('click', 'a', function () {
            var tables = $('#tblaiplv').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            aiplv.tr = $(this).parents('tr');
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
            aiplv.currrent_row = row;

            var strheader = $(this);
            if (strheader.hasClass('check')) {
                modal_face_ai.data = data;
                modal_face_ai.show();
            }

        });
    },
    // lay danh sach bien ban theo don vi
    loaddata: function () {
        var thang = $('#cbothang').val();
        var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();

//        madvi

        url = "api/v1/PLV/GetDsPhienLamViec_AI?donvid=" + madvi + "&thang=" + thang + "&nam=" + nam ;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            aiplv.data = d.Data;
            aiplv.oTable = $("#tblaiplv").dataTable();
            aiplv.oTable.fnClearTable();
            if (d.Data.length > 0)
                aiplv.oTable.fnAddData(d.Data);
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
                "data": "NoiDung", "width": "30%"
            },
            {
                "data": "DiaDiem", "width": "30%"
            },
            {
                "data": null, render: function (data, type, row) {
                    var html = util.FormatDateToVn(row.NgayLamViec);
                    return html;
                }
            },
            {
                "data":null, render: function(data, type, row) {
                    var html = '<a class="check">Điểm danh</a>';
                    return html;
                        }
            },
        ],

    },

  
}

aiplv.init();

var modal_face_ai = {
    parent: null,
    data: null,
    show: function () {
        $('#modal_face_ai').show();

        this.loaddata();
    },
    loaddata: function () {
        var url = "api/v1/AI/GetAlLImageFlv?donvid=" + this.data.DonViId + "&ID_PLV=" + this.data.Id;
        service.ExcuteAjaxtGet(url, null, (d) => {
            var html = "";
            var idconnect = d.IDConect;
            imgs = d.images;
            for (var i = 0; i < imgs.length; ++i) {
                html += "<div style='padding: 10px'><img src=" + service.apiserver + "api/v1/Image/GetFile?path=" + imgs[i].Url + "&MA_DVQL_CHA=" + idconnect + " style='width:400px' />";
                html += "<div id=i" + imgs[i].Id + " style='margin: 10px;'></div></div > ";
            }
            $('#ai_images').html(html);

            // faceapi
            for (var i = 0; i < imgs.length; ++i) {
                var url = "api/v1/AI/CheckFacePLV?urlimg=" + imgs[i].Url + "&id=" + imgs[i].Id + "&donvid=" + this.data.DonViId 
                service.ExcuteAjaxtGet(url, null, (d) => {
                    var str ='<b>Khuân mặt:</b></br>'+ d.str.join(';');
                    $("#i" + d.id).html(str);
                });
            }
            
        });
    },
    cancel: function () {
        $('#modal_face_ai').hide();
    }

}