var giao_kh ={

    currrent_row: null,
    data: null,
    init: function () {
        this.tables = $('#tblg_kh').DataTable(this.optiontable);
        var html = '';
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvi').html(html);
        // tao cbo thang     
        // row click
        $('#tblg_kh tbody').on('click', 'a', function () {
            var tables = $('#tblg_kh').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            giao_kh.tr = $(this).parents('tr');
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
            giao_kh.currrent_row = row;

            var strheader = $(this);

            if (strheader.hasClass('adduser')) {
                modal_adduser.data = data;
                modal_adduser.parrent = giao_kh;
                modal_adduser.IDConnect = data.iddonvi;
                modal_adduser.show();
            }
           
        });
    },

    // lay danh sach bien ban theo don vi
    loaddata: function () {
        //var thang = $('#cbothang').val();
        //var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();
        url = "api/v1/danhmuc/GetDanhMucNhom?madvi=" + madvi ;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            giao_kh.data = d.Data;
            giao_kh.oTable = $("#tblg_kh").dataTable();
            giao_kh.oTable.fnClearTable();
            if (d.Data.length > 0)
                giao_kh.oTable.fnAddData(d.Data);
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
                    return html + row.iddonvi + "-" + util.GetNameDonvi(row.iddonvi);
                }, "width": "20%"
            },
            {
                "data": "ten_nhom", "width": "15%"
            },
            {
                "data": "dinh_muc_thang", "width": "15%"
            },
            {
                "data": null, render: function (data, type, row) {
                    var html = '';
                    for (i = 0; i < row.USERS.length; ++i) {
                        html += row.USERS[i].hovaten + '-' + row.USERS[i].chucvu + ' <a onclick="giao_kh.delUserkh(' + row.USERS[i].ID +')"><i class="fa fa-trash-o" style="font-size: 25px; cursor: pointer;color: red"></i></a></br>';
                    }
                    return html;
                }, "width": "35%"
            },
            {
                "data": null, render: function (data, type, row) {
                    var html = '';
                    html = '<a class="adduser">Thêm thành viên</a>';
                    return html;
                }, "width": "10%"
            },
        ],

    },
    delUserkh(id) {
        let text = "Anh chị có xóa người dùng này khỏi nhóm.";
        if (confirm(text) == true) {
            var url = "api/v1/danhmuc/DeleteUserDV?IDConnect=" + data.iddonvi+"&ID="+id;            
            service.ExcuteAjaxtGet(url, null, (d)=>{
                //reload grid
                giao_kh.loaddata();
            });
        } 
    }
}
giao_kh.init();
