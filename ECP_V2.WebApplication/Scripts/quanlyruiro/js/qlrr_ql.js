
var qlrr_ql = {
    currrent_row: null,
    init: function () {
        this.tables = $('#tblruiro2').DataTable(this.optiontable);       
        $('#tblruiro2 tbody').on('click', 'a', function () {
            var tables = $('#tblruiro2').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            qlrr_ql.tr = $(this).parents('tr');
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
            qlrr_ql.currrent_row = row;            

            var strheader = $(this);

            if (strheader.hasClass('del')) {
                if (confirm("Anh/Chị có muốn xóa dữ liệu này không?") == true) {
                    service.ExcuteAjaxt("RemoveRuiRo?id=" + row.data().ID, null, (d) => {
                        row.remove().draw();
                    });
                }
            }
            if (strheader.hasClass('kphuc')) {
                modal_AddRuiRo.parent = qlrr_ql;
                modal_AddRuiRo.state = "kp";
                modal_AddRuiRo.data = row.data();
                modal_AddRuiRo.show();
            }
        });      
        this.load();
        this.loaddanhmuc();
    },

    changecbo: function () {
        this.load();
    },

    loaddanhmuc: function () {
        service.ExcuteAjaxt("GetAllDanhMuc", null, (d) => {
            var html = '';
            var data = d.Data;            
            for (i = 0; i < data.length; ++i) {
                //if(i==0)
                    html += "<option value='" + data[i].ID + "' selected>" + data[i].TEN + "</option>"
                //else
                //    html += "<option value='" + data[i].ID + "'>" + data[i].TEN + "</option>"
            }
            $('#idrrg').html(html);
           
        });
    },
    load: function () {
        trangthai = $('#drtrangthai').val();
        service.ExcuteAjaxt("GetAllData?madvi=" + user.DonViID + "&TRANG_THAI=" + trangthai , null, function (d) {
            qlrr_ql.oTable = $("#tblruiro2").dataTable();
            qlrr_ql.oTable.fnClearTable();
            if (d.Data.length > 0)
                qlrr_ql.oTable.fnAddData(d.Data);
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
            { "data": "NOI_DUNG", "width": "20%" },
            { "data": "TEN_VITRI", "width": "15%" },
            { "data": "BIEN_PHAP", "width": "15%" },
            { "data": "ND_KHAC_PHUC", "width": "20%" },
            { "data": "TRANG_THAI", "width": "10%" },
            { "data": null, render: function (data, type, row) {
                    var html = '';
                html = '<a class="btn btn-primary kphuc">Khắc phục</button>'+
                    '<a class="btn btn-danger del" style = "margin:10px"> Xóa</button>';
                       
                    return html;
                }, "width": "20%"
            },
        ],

    },

    Addrr: function () {
        modal_AddRuiRo.parent = this;
        modal_AddRuiRo.state = "add";
        modal_AddRuiRo.show();
    },
    EventAdd: function (d) {
        var oTable = $('#tblruiro2').DataTable();
        oTable.row.add(d).draw();
    },

    EventKhacPhuc: function (d) {
        qlrr_ql.currrent_row.remove().draw();
    }

}

qlrr_ql.init();