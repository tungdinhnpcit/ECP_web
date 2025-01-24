
var dkcamera = {
    init: function () {        
        var table = $('#tbldkcamera').DataTable(this.optiontable);

        var html = '';
        for (i = 0; i < util.donvis.length; ++i) {
            if (i == 0) {
                html += "<option select value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            } else {
                html += "<option value=" + util.donvis[i].ma_dviqly + ">" + util.donvis[i].ten_donvi + "</option>";
            }
        }
        $('#cbodonvi').html(html);

    },

    loaddata: function () {
        madvi = $('#cbodonvi').val();
        url = "api/v1/Camera/GetCameraDKDvi?madvi=" + madvi;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            oTable = $("#tbldkcamera").dataTable();
            oTable.fnClearTable();           
            if (d.Data.length > 0)
                oTable.fnAddData(d.Data);
        });
    },

    xoadk: function (camid) {
        console.log('xoadk');
        let text = "Anh chị có xóa camera dùng này khỏi đơn vị.";
        if (confirm(text) == true) {
            madvi = $('#cbodonvi').val();
            var url = "api/v1/Camera/RemoveCamDvi?camid=" + camid + "&madvi=" + madvi;
            service.ExcuteAjaxtGet(url, null, (d) => {
                //reload grid
                dkcamera.loaddata();
            });
        }
    },
    add: function () {
        $('#modal_dkcamera').modal('show');
        $('#dkcamera_txtID').val('');
        $('#dkcamera_txtdes').val('');

    },
    submitdk: function () {
        madvi = $('#cbodonvi').val();
        console.log('submitdk');
      var id=  $('#dkcamera_txtID').val();
        var des = $('#dkcamera_txtdes').val();
        if (id == "" || des == "") {
            alert('Anh chị chưa nhập đủ thông tin');
            return;
        }
        url = "api/v1/Camera/DKCamDvi?madvi=" + madvi + "&camid=" + id + "&camdes=" + des;
        service.ExcuteAjaxtGet(url, null, (d) => {
            if (d.State == false) {
                alert(d.Message);
                return;
            }
            dkcamera.loaddata();
            $('#modal_dkcamera').modal('hide');
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
                "data": "MA_DVIQLY"
            },
            {
                "data": "CAM_ID"
            },
            {
                "data": "CAM_DESC", "width": "15%"
            },
            {
                "data": "check", render: function (data, type, row) {
                    var html = '';                    
                        html = '<button type="button" class="btn btn-outline-primary" onclick="dkcamera.xoadk(' + row.CAM_ID+')">Xóa</button>';                   
                    return html;
                }, "width": "20%"
            },
        ],

    },

   
};


dkcamera.init();