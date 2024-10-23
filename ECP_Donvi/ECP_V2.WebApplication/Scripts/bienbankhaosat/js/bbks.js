var bbks = {
    currrent_row: null,
    load: function () {
        this.tables = $('#tbldanhmuc').DataTable(this.option_table);

        service.ExcuteAjaxt("GetAllBBKS", null, (d) => {
            bbks.oTable = $("#tbldanhmuc").dataTable();
            bbks.oTable.fnClearTable();
            if (d.Data.length > 0)
                bbks.oTable.fnAddData(d.Data);

        });

        $('#tbldanhmuc tbody').on('click', 'a', function () {
            var tables = $('#tbldanhmuc').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            bbks.tr = $(this).parents('tr');
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


            bbks.currrent_row = row;
            var data = row.data();

            var strheader = $(this);
            if (strheader.hasClass('del')) {
                if (confirm("Anh/Chị có muốn xóa dữ liệu này không?") == true) {
                    service.ExcuteAjaxt("DelDanhMuc?ID=" + row.data().ID, null, (d) => {
                        row.remove().draw();
                    });
                }
            }
            else if ($(this).hasClass('edit')) {
                console.log("edit action: ", data);
                $('#titBBKS').html("Hiệu chỉnh biên bản khảo sát hiện trường");  
                $('#btnAdd').hide();                
                $('#btnEdit').show();
                $('#txtten').val(data.TEN);
                $('#id_bbks').val(data.ID);
                model_bbks.parrent = this;                
                model_bbks.showmodel();
            }
            else if ($(this).hasClass('addViTri')) {
                console.log("add vitri action: ", data);               
                $('#id_bbks').val(data.ID);
                model_bbks.parrent = this;
                model_bbks.showmodel_vitri();
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
            { "data": "TEN", "width": "10%" },
            //{ "data": "TEN ", "width": "25%" },
            //{ "data": "TINH_TRANG ", "width": "15%" },
            //{ "data": "NGUOI_LAP ", "width": "10%" },
            //{ "data": "NGAY_LAP ", "width": "10%" },
            //{ "data": "NGUOI_DUYET ", "width": "10%" },
            //{ "data": "NGAY_DUYET ", "width": "10%" },
            {
                "data": null, render: function (data, type, row) {
                    var html = '';
                    html = '<a title="Xóa biên bản" class="del method"><img src="/Images/ImageDesign/xoa_icon.png" alt="del"></a>  <br/>'
                    html = html + '<a title="Hiệu chỉnh biên bản" class="edit method"><img src="/Images/ImageDesign/chinhsua_icon.png" style="width=17px;height:17px" alt="edit"></a> <br/>';
                    html = html + '<a title="Thêm vị trí vào biên bản" class="addViTri method"><img src="/Images/ImageDesign/xacnhan_icon.png" style="width=17px;height:17px" alt="edit"></a>';
                    return html;
                }, "width": "10%"
            },
        ],

    },

    Addbbks: function () {
        $('#titBBKS').html("Thêm mới biên bản khảo sát hiện trường");
        $('#btnAdd').show();
        $('#btnEdit').hide();
        model_bbks.parrent = this;
        model_bbks.showmodel();

    },
    addCallback: function (d) {
        var oTable = $('#tbldanhmuc').DataTable();
        oTable.row.add(d).draw();
    }



}
bbks.load();