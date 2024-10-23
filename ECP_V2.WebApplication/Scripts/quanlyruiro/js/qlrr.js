var qlrr = {
    currrent_row:null,
    load: function () {
        this.tables = $('#tbldanhmuc').DataTable(this.option_table);

        service.ExcuteAjaxt("GetAllDanhMuc", null,(d) => {
            qlrr.oTable = $("#tbldanhmuc").dataTable();
            qlrr.oTable.fnClearTable();
            if (d.Data.length > 0)
                qlrr.oTable.fnAddData(d.Data);

        });

        $('#tbldanhmuc tbody').on('click', 'a', function () {
            var tables = $('#tbldanhmuc').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            qlrr.tr = $(this).parents('tr');
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
            qlrr.currrent_row = row;
            var data = row.data();
        
            var strheader = $(this);

            if (strheader.hasClass('del')) {
                if (confirm("Anh/Chị có muốn xóa dữ liệu này không?") == true) {
                    service.ExcuteAjaxt("DelDanhMuc?ID="+row.data().ID, null, (d) => {
                        row.remove().draw();
                    });
                }
            }     
            
        });
    },


     option_table: {
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
             { "data": "TEN", "width": "80%"            },
             { "data": null, render: function (data, type, row) {
                 var html = '';
                 html = '<a class="del">Xóa</button>'
                 return html;
                },"width": "20%"
             },            
        ],

    },

    Addrr: function () {
        model_dm_ruiro.parrent = this;
        model_dm_ruiro.showmodel();

    },
    addCallback: function (d) {
        var oTable = $('#tbldanhmuc').DataTable();
        oTable.row.add(d).draw();
    }

}
qlrr.load();