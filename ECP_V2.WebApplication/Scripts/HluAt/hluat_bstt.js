//Load danh sách đơn vị
function LoadDsOrg() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsOrg',
        dataType: 'json',        
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblOrg').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblOrg').DataTable({
                    "destroy": true,
                    "data": response,
                    "columns": [{ 'data': 'madvql', visible: false, orderable: false },
                    {
                        "data": null,
                        render: function (data, type, row) {
                            var details = row.tendvql + "<font color='#3742fa'> (" + row.slns + ") </font>";
                            return details;
                        }
                    },
                    { 'data': 'slns', visible: false }
                    ],
                    scrollY:'335',
                    "paging": false,
                    "bFilter": false, //hide Search bar
                    "bInfo": false,
                    "select": {
                        style: 'single'
                    },
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Đơn vị');
        }
    })
}

function LoadDsNhansuByOrg() {
    //Lấy đơn vị chọn
    var t = $('#tblOrg').DataTable();
    var dvql = t.row('.selected').data()['madvql'];

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBSTT/LoadDsNhansuByOrg',
        dataType: 'json',
        data: JSON.stringify({ madvql: dvql }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblNhansu').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblNhansu').DataTable({
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
                        "processing": "ĐANG XỬ LÝ..."
                    },
                    "destroy": true,
                    "data": response,
                    "columns": [{ 'data': 'nsid', visible: false, orderable: false },
                    { 'data': 'orgid', visible: false, orderable: false },
                    { 'data': 'tenkhaisinh' },
                    { 'data': 'chucdanh' },
                        { 'data': 'phongban' },
                        { 'data': 'cccd' },
                        { 'data': 'bactho' },
                        { 'data': 'bacat' },
                        { 'data': 'chucdanhatd' }
                    ],                    
                    scrollY: 300,
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự');
        }
    })
}

///Load nhân sự bởi ID,Click vào row
function LoadNhansuById() {
    //Lấy đơn vị chọn
    var t = $('#tblNhansu').DataTable();
    var nsid = t.row('.selected').data()['nsid'];

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBSTT/LoadDsNhansuById',
        dataType: 'json',
        data: JSON.stringify({ nsid: nsid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //Load object 
            $('#hovaten').text(response[0].tenkhaisinh);
            $('#chucvu').text(response[0].chucvu);
            $('#phongban').text(response[0].phongban);
            $('#ngaysinh').text(response[0].ngay);
            $('#gioitinh').text(response[0].gioitinh);
            $('#cccd').text(response[0].cccd);
            $('#nsid').val(response[0].nsid);
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự');
        }
    })
}


///Load combobox thông tin Nhóm atd
function LoadGroup() {    

    $('#cboATD').empty();
    $('#cboATVSLD').empty();
    $('#cboHotline').empty();

    $('#cboATD').append($('<option>', {
        value: "",
        text: '-- Chọn nhóm An toàn Điện --'
    }));

    $('#cboATVSLD').append($('<option>', {
        value: "",
        text: '-- Chọn nhóm An toàn VSLĐ --'
    }));

    $('#cboHotline').append($('<option>', {
        value: "",
        text: '-- Chọn nhóm Hotline --'
    }));

    //Load Nhóm đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBSTT/LoadGroupByType',
        dataType: 'json',
        data: JSON.stringify({ typeid: 'All' }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    if (value.groupid == 'ATD') {
                        $('#cboATD')
                            .append($('<option></option>', { text: value["categorydesc"] })
                                .attr('value', value["categoryid"]));
                    }
                    
                    if (value.groupid == 'ATVSLD') {
                        $('#cboATVSLD')
                            .append($('<option></option>', { text: value["categorydesc"] })
                                .attr('value', value["categoryid"]));
                    }
                    
                    if (value.groupid == 'HOTLINE') {
                        $('#cboHotline')
                            .append($('<option></option>', { text: value["categorydesc"] })
                                .attr('value', value["categoryid"]));
                    }
                    
                });
            }
        },
        error: function (ex) {
            alert('Không lấy được danh sách Nhóm đào tạo');
        }
    })
}

///Load combobox danh mục Nhóm atd
function LoadGroupWorker() {

    $('#cdatd-select').empty();
    
    //Load Nhóm đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBSTT/LoadCboWorker',
        dataType: 'json',
        //data: JSON.stringify({ typeid: 'All' }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {                    
                    $('#cdatd-select')
                            .append($('<option></option>', { text: value["categorydesc"] })
                                .attr('value', value["categoryid"]));                    

                });
            }
        },
        error: function (ex) {
            alert('Không lấy được danh sách Nhóm đào tạo');
        }
    })
}

//Ghi thông tin nhân sự.
function OnSaveInforHR() {

    //Check trạng thái > 1 thì đc ghi
    let bactho = $('#bactho').val();
    let bacat = $('#bacat').val();
    let nhomcdatd = $('#cdatd-select').val();
    let cdanhatd = "";
    if (nhomcdatd.length == 0) {
        alert('Bạn chưa chọn nhóm chức danh an toàn điện.');
        return;
    } else {
        cdanhatd = nhomcdatd.toString();

    }

    let nhomatd = $('#cboATD').val();
    let nhomvsld = $('#cboATVSLD').val();
    let nhomhotline = $('#cboHotline').val();
    let nsid = $('#nsid').val();

    var formData = new FormData();
    
    formData.append("FileUpload", document.getElementById("multiplefileupload").files[0]);
    formData.append("bactho", bactho);
    formData.append("bacat", bacat);
    formData.append("cdanhatd", cdanhatd);
    formData.append("nsid", nsid);
    formData.append("nhomatd", nhomatd);
    formData.append("nhomvsld", nhomvsld);
    formData.append("nhomhotline", nhomhotline);

    $.ajax({
        type: 'post',
        url: '/Admin/HLATBSTT/OnUpdateInfor',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            alert('Cập nhật thông tin thành công!');
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình cập nhật thông tin.' + error);
        }
    });
}