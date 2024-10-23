//type: DZ-TBA
function getDsachTbiPmis(type) {
    //type: tba or dz
    //Trường hợp add type = 0, edit type = 1
    var tblName = "";
    var vloai = "KXD";
    if (type == 0) {
        tblName = "tblTbi";
    } else {
        tblName = "tblTbiEdit";     
    }

    if ($("#LoaiTB").val()==78) {
        vloai = 'DZ';
    }

    if ($("#LoaiTB").val()==79) {
        vloai = 'TBA';
    }

    $.ajax({
        type: 'POST',
        url: '/Admin/SuCo/GetAssetPmis',
        dataType: 'json',
        data: JSON.stringify({ loai: vloai, assetid_parent: null }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {            
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $('#' + tblName).DataTable().clear();
                
                $('#' + tblName).DataTable({
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
                    select: {
                        style: 'single'
                    },
                    "destroy": true,
                    "data": $.parseJSON(response),
                    //"data": response,
                    "columns": [
                        { 'data': 'Assetdesc' },
                        { 'data': 'Categorydesc' },
                        { 'data': 'Categoryid', 'visible': false },
                        { 'data': 'Ulevelid', 'visible': false },
                        { 'data': 'Assetid','visible': false },
                        { 'data': 'Duongdan' },
                    ],
                    "columnDefs": [{
                        'orderable': false,                      
                        'targets': 0                        
                    }],
                    'order': [[1, 'asc']]
                });
                if (type == 0) {
                    $('#modalDsTbi').modal('show');
                } else {
                    $('#modalDsTbiEdit').modal('show');
                }
                
            }

        },
        error: function (request, status, error) {
            alert('Không thể tải được danh sách TBA hoặc ĐZ');
        }
    })
}

//Set text for TenThietBi
//type = 0 là thêm mới, type=1 là edit
function fnSetTen(type) {
    var bName = "";
    if (type == 0) {
        bName = "tblTbi";
    } else {
        bName = "tblTbiEdit";
    }
    var oTable = $('#' + bName).DataTable();
    
    var data = oTable.rows('.selected').data();

    $('#TenThietBi').val(data[0]['Assetdesc']);
    $('#MaThietBi').val(JSON.stringify({ Assetid: data[0]['Assetid'], Categoryid: data[0]['Categorydesc'] }));

    if (type == 0) {
        //Close modal
        $('#modalDsTbi').modal('hide');
    } else {
        //Close modal
        $('#modalDsTbiEdit').modal('hide');
        //Clear mã và tên đường dây con - nếu có
        $('#TenTBSco').val('');
        $('#MaTBSco').val('');
    }
      
}

//Xử lý cha con
function getDsachByParent(type) {
    //type: tba or dz
    var tblName = "";
    var vloai = "";
    if ($("#LoaiTB").val() == 78) {
        vloai = 'DZ';
    }

    if ($("#LoaiTB").val() == 79) {
        vloai = 'TBA';
    }

    var assetid = $('#MaThietBi').val();
    if (assetid == '' || assetid == null) {
        alert('Bạn chưa chọn Đường dây/TBA');
        return;
    }
    //Get assetid from json
    var assets = JSON.parse(assetid);
    
    if (type == 0) {
        tblName = "tblTbbyParent";
    } else {
        tblName = "tblTbbyParentEdit";
    }
    $.ajax({
        type: 'POST',
        url: '/Admin/SuCo/GetAssetPmis',
        dataType: 'json',
        data: JSON.stringify({ loai: vloai, assetid_parent: assets.Assetid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $('#' + tblName).dataTable().fnClearTable();

                $('#' + tblName).DataTable({
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
                    select: {
                        style: 'multi'
                    },
                    "destroy": true,
                    "data": $.parseJSON(response),
                    "columns": [
                        { 'data': 'Assetdesc' },
                        { 'data': 'Categorydesc' },
                        { 'data': 'Categoryid', 'visible': false },
                        { 'data': 'Ulevelid', 'visible': false },
                        { 'data': 'Assetid', 'visible': false },
                        { 'data': 'Duongdan' },
                    ],
                    "columnDefs": [{
                        'orderable': false,
                        'targets': 0
                    }],
                    'order': [[1, 'asc']]
                });
                if (type == 0) {
                    $('#modalDstbByParent').modal('show');
                } else {
                    $('#modalDstbByParentEdit').modal('show');
                }
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách thiết bị');
        }
    })
}
//Set mã và tên thiết bị sự cố
function fnSetInfoTbiSco(type) {
    var tblName = "";
    if (type == 0) {
        tblName = "tblTbbyParent";
    } else {
        tblName = "tblTbbyParentEdit";
    }
    var oTable = $('#' + tblName).DataTable();

    var data = oTable.rows('.selected').data();
    var ten = "";
    var ma=[];
    for (let i = 0; i < data.length; i++) {
        ten += data[i]['Assetdesc'] + "(" + data[i]['Categorydesc'] + ");";
        var oma = { Assetid: data[i]['Assetid'], Categoryid: data[i]['Categorydesc']};
        ma.push(oma);
    }

    $('#TenTBSco').val(ten);
    $('#MaTBSco').val(JSON.stringify(ma));

    if (type == 0) {
        //Close modal
        $('#modalDstbByParent').modal('hide');
    } else {
        //Close modal
        $('#modalDstbByParentEdit').modal('hide');
    }
    
}

///Thao tác với thiết bị tác động
function getDsachTbiTdong(type) {
    //type: tba or dz
    var tblName = "";

    if (type == 0) {
        tblName = "tblTbTdong";
    } else {
        tblName = "tblTbTdongEdit";
    }
    $.ajax({
        type: 'POST',
        url: '/Admin/SuCo/GetAssetPmis',
        dataType: 'json',
        data: JSON.stringify({ loai: "DC", assetid_parent:null }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $('#' + tblName).dataTable().fnClearTable();

                $('#' + tblName).DataTable({
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
                    select: {
                        style: 'single'
                    },
                    "destroy": true,
                    "data": $.parseJSON(response),
                    "columns": [
                        { 'data': 'Assetdesc' },
                        { 'data': 'Categorydesc' },
                        { 'data': 'Categoryid', 'visible': false },
                        { 'data': 'Ulevelid', 'visible': false },
                        { 'data': 'Assetid', 'visible': false },
                        { 'data': 'Duongdan' },
                    ],
                    "columnDefs": [{
                        'orderable': false,
                        'targets': 0
                    }],
                    'order': [[1, 'asc']]
                });
                if (type == 0) {
                    $('#modalDstbTdong').modal('show');
                } else {
                    $('#modalDstbTdongEdit').modal('show');
                }
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách thiết bị');
        }
    })
}
//Set mã và tên thiết bị sự cố
function fnSetInfoTbiTdong(type) {
    //type: tba or dz
    var tblName = "";

    if (type == 0) {
        tblName = "tblTbTdong";
    } else {
        tblName = "tblTbTdongEdit";
    }
    var oTable = $('#' + tblName).DataTable();

    var data = oTable.rows('.selected').data();
    var ten = "";
    var ma = [];
    for (let i = 0; i < data.length; i++) {
        ten += data[i]['Assetdesc'] + "(" + data[i]['Categorydesc'] + ");";
        var oma = { Assetid: data[i]['Assetid'], Categoryid: data[i]['Categorydesc'] };
        ma.push(oma);
    }

    $('#TenTBTdong').val(ten);
    $('#MaTBTdong').val(JSON.stringify(ma));

    if (type == 0) {
        //Close modal
        $('#modalDstbTdong').modal('hide');
    } else {
        //Close modal
        $('#modalDstbTdongEdit').modal('hide');
    }
    
}