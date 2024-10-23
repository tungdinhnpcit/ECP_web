var modal_ds_ktgs = {
    data: null,
    parrent: null,
    currrent_row: null,
    load: false,
    show: function(){
               
        $('#modal_ds_ktgs').modal();
        if (this.load == false) 
            this.init();
            this.load = true;
        

        this.oTable.fnClearTable();
        if (modal_ds_ktgs.data.length > 0)
            this.oTable.fnAddData(modal_ds_ktgs.data);
    },
    init: function () {
        this.tables = $('#tblktgs_ctiet').DataTable(this.optiontable);
        this.oTable = $("#tblktgs_ctiet").dataTable();       

        $('#tblktgs_ctiet tbody').on('click', 'a', function () {
            var tables = $('#tblktgs_ctiet').DataTable();
            var tableData = tables.table($(this).parents('table'));
            var row = tableData.row($(this).parents('tr'));
            modal_ds_ktgs.tr = $(this).parents('tr');
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
            modal_ds_ktgs.currrent_row = row;

            var strheader = $(this);

            if (strheader.hasClass('ctiet')) {
                modal_bb_ctiet.data = row.data();
                modal_bb_ctiet.show();
            }

            if (strheader.hasClass('khacphuc')) {
                modal_kptt.data = row.data();
                modal_kptt.show();
            }

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
            { "data": "MA_PCT", "width": "10%" },
            {
                "data": "Loai_KTGS", render: function (data, type, row) {
                    if (row.Loai_KTGS =="KTPLV")
                        return 'Kiểm tra phiên làm việc hiện trường';    
                    else 
                        return 'Kiểm tra đơn vị cơ sở';    
                }, "width": "10%" },
            { "data": "DiaDiem", "width": "25%" },
            { "data": "NoiDungCongViec", "width": "25%" },
            {
                "data": "CoTonTai", render: function (data, type, row) {
                    if (row.CoTonTai == true)
                        return "<span style='color:red'> Có tồn tại</span></br><a  style='color:blue' class='khacphuc'>Khắc phục</a>";
                    else 
                        return "<span > không tồn tại</span>";
                }, "width": "10%"
            },
            { "data": "Nd_Kphuc", "width": "10%" },
            {
                "data": null, render: function (data, type, row) {                    
                        return "<a class='ctiet' style='color:blue'> Chi tiết</span>";                    
                },"width": "10%" },

        ],

    },
}