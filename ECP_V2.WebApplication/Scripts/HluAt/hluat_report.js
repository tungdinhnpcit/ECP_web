function LoadDonvi() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC01/LoadDonvi',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboDonvi').empty();
            $('#cboDonvi').append($('<option>', {
                value: "ALL",
                text: '-- Chọn đơn vị --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboDonvi')
                        .append($('<option></option>', { text: value["tendvql"] })
                            .attr('value', value["madvql"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Đơn vị');
        }
    })
}
var obj;
function LoadNhansu() {
    let donvi = $('#cboDonvi').val();
    if (donvi === null) {
        $('#cboDonvi').val("PN");
        donvi = "PN";
    }
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC01/LoadNhansu',
        dataType: 'json',
        data: JSON.stringify({ donvi: donvi }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboNhansu').empty();
            $('#cboNhansu').append($('<option>', {
                value: "ALL",
                text: '-- Chọn nhân sự --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboNhansu')
                        .append($('<option></option>', { text: value["tenkhaisinh"] })
                            .attr('value', value["nsid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Nhân sự');
        }
    })
}

function LoadInfoNsById() {
    let nsid = $('#cboNhansu').val();
   
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC01/LoadNsById',
        dataType: 'json',
        data: JSON.stringify({ nsid: nsid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            obj = response[0];

            $('#donvi').text(obj.donvi);
            $('#hovaten').text(obj.tenkhaisinh);
            $('#chucvu').text(obj.chucdanh);
            $('#cccd').text(obj.cccd);
            $('#namsinh').text(obj.ngaysinh);
            $('#gioitinh').text(obj.gioitinh);
            $('#nhomantoan').text(obj.chucdanhatd);
            $('#chucdanh').text(obj.chucdanhatd);
            $('#bacat').text(obj.bacat);
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Nhân sự');
        }
    })
}


function LoadDsKquaThi() {
    let donvi = $('#cboDonvi').val();
    let nhansu = $('#cboNhansu').val();    

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC01/LoadDsKquaThi',
        dataType: 'json',
        data: JSON.stringify({ donvi: donvi, nhansu: nhansu }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblKquaThi').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblKquaThi').DataTable({
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
                    "columns": [{ 'data': 'nhomhluyen' },
                    { 'data': 'nam' },
                    { 'data': 'dlythuyet' },
                    { 'data': 'dthuchanh'},                    
                    ],
                    'order': [[1, 'asc']],
                    "select": {
                        style: 'single'
                    },
                });
            }
            
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách lớp học');
        }
    })
}

function SetValueFake() {
    $('#donvi').text('Điện lực Kim Sơn');
    $('#hovaten').text('Chu Văn Nam');
    $('#chucvu').text('Chuyên viên');
    $('#cccd').text('03687634567');
    $('#namsinh').text('1984');
    $('#gioitinh').text('Nam');
    $('#nhomantoan').text('ATVSLĐ - Nhóm Quản lý (20/01/2023 - LT: 9; TH: 9)       ATĐ - Nhóm nghề 2 - (22 / 01 / 2023 - LT: 8; TH: 10)');
    $('#chucdanh').text('Người giám sát ATĐ (QĐ 22/NBPC/2020)');
    $('#bacat').text('3/5');    
}

///Xuất báo cáo 02
function LoadKquaBC02() {
    let donvi = $('#cboDonvi').val();
    let nam = $('#cboNam').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC02/LoadDsKquaThi',
        dataType: 'json',
        data: JSON.stringify({ donvi: donvi, nam: nam }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblKquaHluyen').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblKquaHluyen').DataTable({
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
                    "columns": [{ 'data': 'donvi' },
                        { 'data': 'nhomhluyen' },
                        { 'data': 'lop' },
                        { 'data': 'ngaythien' },
                        { 'data': 'sohvien' },
                        { 'data': 'kqua' },
                    ],
                    'order': [[0, 'asc']],                    
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách lớp học');
        }
    })
}

///Xuất báo cáo 02
function LoadKquaBC03() {
    let donvi = $('#cboDvi03').val();
    let nam = $('#cboNam03').val();
    let loaidaotao = $('#cboLdtao03').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC03/LoadDs03KquaThi',
        dataType: 'json',
        data: JSON.stringify({ donvi: donvi, nam: nam, loaidaotao: loaidaotao }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblKqua03').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblKqua03').DataTable({
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
                    "columns": [{ 'data': 'nsid' },
                    { 'data': 'cty' },
                    { 'data': 'donvi' },
                    { 'data': 'phongban' },
                    { 'data': 'hoten' },
                    { 'data': 'ngaysinh' },
                    { 'data': 'chucdanh' },
                    { 'data': 'nhom_atd' },
                    { 'data': 'khoadtao_atd' },
                    { 'data': 'kqua_lt_atd' },
                    { 'data': 'kqua_th_atd' },
                    { 'data': 'ghichu_atd' },
                    { 'data': 'nhom_vsld' },
                    { 'data': 'khoadtao_vsld' },
                    { 'data': 'kqua_lt_vsld' },
                    { 'data': 'kqua_th_vsld' },
                    { 'data': 'ghichu_vsld' },
                    { 'data': 'nhom_hotline' },
                    { 'data': 'khoadtao_hotline' },
                    { 'data': 'kqua_lt_hotline' },
                    { 'data': 'kqua_th_hotline' },
                    { 'data': 'ghichu_hotline' },
                    ],
                    'order': [[0, 'asc']],
                });
            }

        },
        error: function (ex) {
            alert('Có lỗi trong quá trình lấy danh sách!');
        }
    })
}

///Lấy combo phần chọn CBO
function LoadDviBc03() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC01/LoadDonvi',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboDvi03').empty();
            $('#cboDvi03').append($('<option>', {
                value: "ALL",
                text: '-- Chọn đơn vị --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboDvi03')
                        .append($('<option></option>', { text: value["tendvql"].replace("fa fa-square-o","").replace("<i class=","").replace(">","") })
                            .attr('value', value["madvql"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Đơn vị');
        }
    })
}

//Lấy danh sách loại đào tạo.
function LoadCboLdaotao() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC03/LoadLdaotao',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboLdtao03').empty();
            $('#cboLdtao03').append($('<option>', {
                value: "ALL",
                text: '-- Loại đào tạo --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboLdtao03')
                        .append($('<option></option>', { text: value["groupdesc"] })
                            .attr('value', value["groupid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Loại đào tạo');
        }
    })
}

//Xuất excel báo cáo 03
function onExportExcelBc03() {
    let d = new Date();

    let filename = "HLAT_Kqua_" + d.toLocaleTimeString() + ".xlsx";

    let donvi = $('#cboDvi03').val();
    let nam = $('#cboNam03').val();
    let loaidaotao = $('#cboLdtao03').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATBC03/DownloadFileResult',
        data: { donvi: donvi, nam: nam, loaidaotao: loaidaotao },
        success: function (r) {
            //Convert Base64 string to Byte Array.
            var bytes = Base64ToBytes(r);

            //Convert Byte Array to BLOB.
            var blob = new Blob([bytes], { type: "application/octetstream" });

            //Check the Browser type and download the File.
            var isIE = false || !!document.documentMode;
            if (isIE) {
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var url = window.URL || window.webkitURL;
                link = url.createObjectURL(blob);
                var a = $("<a />");
                a.attr("download", filename);
                a.attr("href", link);
                $("body").append(a);
                a[0].click();
                $("body").remove(a);
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    })
}

function Base64ToBytes(base64) {
    var s = window.atob(base64);
    var bytes = new Uint8Array(s.length);
    for (var i = 0; i < s.length; i++) {
        bytes[i] = s.charCodeAt(i);
    }
    return bytes;
};