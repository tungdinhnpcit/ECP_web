//ID_NHOMLOAICONGVIEC
// 1 thi nghiệm điện
// 10 chinh trang cáp viễn thông
// 11 kiểm tra định kỳ lưới điện và thiết bị điện


var ecp_2021_giaoct =
{
    ID_NhomLoaiCongViec: 0,//= util.getUrlParameter("ID_NHOM");
    data:null,
    // id nhom khai bao cứng trên giao diện
    init: function () {
        this.tables = $('#tblgiao').DataTable(this.optiontable);  
        var html = "";
        // cho đơn vị không được chọn tất cả
        //if (DonViID)
          //  html = "<option select value=-1 selec>Tất cả</option>";
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvi').html(html);

        this.ID_NhomLoaiCongViec = util.getUrlParameter("idnhom");

        if (this.ID_NhomLoaiCongViec == 1)
            $('#divtitle').html('Giao chỉ tiêu thi nghiệm định kỳ');
        if (this.ID_NhomLoaiCongViec == 10)
            $('#divtitle').html('Giao chỉ tiêu chỉnh trang cáp viễn thông');
        if (this.ID_NhomLoaiCongViec == 11)
            $('#divtitle').html('Giao chỉ tiêu kiểm tra lưới điện và thiết bị điện');

        this.loaddata();

        $('#tblgiao tbody').on('click', 'a', function () {
            var tables = $('#tblgiao').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            ecp_2021_giaoct.tr = $(this).parents('tr');
           
            ecp_2021_giaoct.currrent_row = row;

            var strheader = $(this);

            if (strheader.hasClass('del')) {
                if (confirm("Anh/Chị có muốn xóa dữ liệu này không?") == true) {
                    service.ExcuteAjaxtGet("api/v1/ecp2021/DelGiaoCT?id=" + row.data().ID, null, (d) => {
                        row.remove().draw();
                    });
                }
            }

        });  

        // thêm       
    },
    add() {
        modal_giao_chitieu.p = this;
        modal_giao_chitieu.show();
    },
    addcalback(d) {
        var oTable = $('#tblgiao').DataTable();
        oTable.row.add(d).draw();
    },
    loaddata() {
  

        var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();
        var url = "api/v1/ecp2021/layDanhSachGiao?nam=" + nam + "&madvi=" + madvi + "&ID_NhomLoaiCongViec=" + this.ID_NhomLoaiCongViec;
        service.ExcuteAjaxtGet(url, null, (d) => {
            ecp_2021_giaoct.data = d.Data;
            ecp_2021_giaoct.oTable = $("#tblgiao").dataTable();
            ecp_2021_giaoct.oTable.fnClearTable();
            if (d.Data.length > 0)
                ecp_2021_giaoct.oTable.fnAddData(d.Data);
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
                "data": "donvi_id", render: function (data, type, row) {
                    var html = '';                    
                    return html + row.donvi_id + "-" + util.GetNameDonvi(row.donvi_id) ;
                }, "width": "20%"             
            },
            {
                "data": "DinhMuc", render: function (data, type, row) {
                    var html = '';
                    html =  row.DinhMuc ;
                    return html;
                }, "width": "15%"
            },
            {
                "data": "", render: function (data, type, row) {
                    var html = '';
                    html = '<a class="del">Xóa</a>';
                    return html;
                }, "width": "15%"
            },
        ],

    },

}

var ecp_2021_thongke =
{

    data: null,
    init: function () {
        
        this.tables = $('#tblgiaotk').DataTable(ecp_2021_thongke.optiontable);
        var html = "";

        html = "<option select value=-1 selec>Tất cả</option>";
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvi').html(html);

        this.ID_NhomLoaiCongViec = util.getUrlParameter("idnhom");

        if (this.ID_NhomLoaiCongViec == 1)
            $('#divtitle').html('Giao chỉ tiêu thi nghiệm định kỳ');
        if (this.ID_NhomLoaiCongViec == 10)
            $('#divtitle').html('Giao chỉ tiêu chỉnh trang cáp viễn thông');
        if (this.ID_NhomLoaiCongViec == 11)
            $('#divtitle').html('Giao chỉ tiêu kiểm tra lưới điện và thiết bị điện');

        this.loaddata();

        $('#tblgiaotk tbody').on('click', 'a', function () {
            var tables = $('#tblgiaotk').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            var data = row.data();
            ecp_2021_thongke.tr = $(this).parents('tr');

            ecp_2021_thongke.currrent_row = row;

            var strheader = $(this);

            if (strheader.hasClass('ct')) {
               // show modal chi tiết

            }

        });  
    },
    loaddata: function () {
        var nam = $('#cbonam').val();
        var madvi = $('#cbodonvi').val();
        var url = "api/v1/ecp2021/TKBaoCao?nam=" + nam + "&madvi=" + madvi + "&idnhom=" + this.ID_NhomLoaiCongViec;
        service.ExcuteAjaxtGet(url, null, (d) => {
            ecp_2021_thongke.data = d.Data;
            ecp_2021_thongke.oTable = $("#tblgiaotk").dataTable();
            ecp_2021_thongke.oTable.fnClearTable();
            if (d.Data.length > 0)
                ecp_2021_thongke.oTable.fnAddData(d.Data);
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
                "data": "MA_DVI", render: function (data, type, row) {
                    var html = '';
                    return html + row.MA_DVI + "-" + util.GetNameDonvi(row.MA_DVI);
                }, "width": "20%"
            },
            {
                "data": "DinhMuc", render: function (data, type, row) {
                    var html = '';
                    html = row.DinhMuc;
                    return html;
                }, "width": "15%"
            },
            {
                "data": "TSO", render: function (data, type, row) {
                    var html = '';
                    html = row.TSO;
                    return html;
                }, "width": "15%"
            },
            {
                "data": "", render: function (data, type, row) {
                    var html = '';
                    html = '<a class="ct">Chi tiết</a>';
                    return html;
                }, "width": "15%"
            },
        ],

    },
}

ecp_2021_giaoct.init();

ecp_2021_thongke.init();