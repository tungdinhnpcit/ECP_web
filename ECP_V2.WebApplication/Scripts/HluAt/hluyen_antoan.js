function LoadDsOrg() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsOrg',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //$('#tblOrg').dataTable().fnClearTable();
            //if (response.length > 0) {
            //    var table = $('#tblOrg').DataTable({
            //        "destroy": true,
            //        "data": response,
            //        "columns": [{ 'data': 'madvql', visible: false, orderable: false },
            //            {
            //                "data": null,
            //                render: function (data, type, row) {
            //                    var details = row.tendvql + "<font color='#3742fa'> (" + row.slns + ") </font>";
            //                    return details;
            //                } },
            //            { 'data': 'slns', visible: false }
            //        ],
            //        "paging": false,
            //        "bFilter": false, //hide Search bar
            //        "bInfo": false,
            //        "select": {
            //            style: 'single'
            //        }, 
            //    });                
            //}            
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Đơn vị');
        }
    })
}

function LoadDsOrgModal() {
    //Chỉnh lại lấy danh sách theo
    //Bổ sung thêm tham số: kế hoạch, loại đào tạo (ATD, ATVSLD, HOTLINE), nhóm đào tạo
    let khoach = $('#mdKhoach').val();
    if (khoach == null) {
        khoach = "ALL";
    }
    let loaidaotao = $('#mdLoaiDtao').val();
    let nhomhl = $('#mdNhomhl').val();
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsOrg',
        dataType: 'json',
        data: JSON.stringify({ khoach: khoach, loaidaotao: loaidaotao, nhomhl: nhomhl }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblOrgAdd').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblOrgAdd').DataTable({
                    "destroy": true,
                    "data": response,
                    "columns": [{ 'data': 'madvql', visible: false, orderable: false },
                        {
                            "data": null,
                            render: function (data, type, row) {
                                var details = row.tendvql + "<font color='#3742fa'> (" + row.slns + ") </font>";
                                return details;
                            }, orderable: false},
                    { 'data': 'slns', visible: false }
                    ],
                    "paging": false,
                    "bFilter": false, //hide Search bar
                    "bInfo": false,
                    "select": {
                        style: 'single',
                    },
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Đơn vị');
        }
    })
}

//var t = $('#tblOrgAdd').DataTable();

//t.off("select").on("select", function (e, dt, type, indexes) {
//    alert("Selected");
//    alert(e, dt, type, indexes);
//    if (type === 'row') {
//        var data = t.rows(indexes).data().pluck('id');
//        console.dir(data);
//        // do something with data
//    }
//});

function LoadTypeEdu() {
    //Load loại hình đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/getTypeEdu',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboTypeEdu').empty();       
            $('#cboTypeEdu').append($('<option>', {
                value: "ALL",
                text: '-- Chọn loại đào tạo --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboTypeEdu')
                        .append($('<option></option>', { text: value["groupdesc"] })
                            .attr('value', value["groupid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Loại hình đào tạo');
        }
    })
}

function LoadGroupEdu() {
    let typeedu = $('#cboTypeEdu').val();
    $('#cboGroupEdu').empty();
    
    if (typeedu == "ALL") {
        $('#cboGroupEdu').append($('<option>', {
            value: "ALL",
            text: '-- Chọn loại đào tạo --'
        }));
    }
    
    //Load Nhóm đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadGroupEdu',
        dataType: 'json',
        data: JSON.stringify({ typeid: typeedu }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {                   
                   
                    $('#cboGroupEdu')
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

function LoadStatusClass() {
    //Load trạng thái lớp học
    var formData = new FormData();

    formData.append("classid", "ALL");
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadStatusClass',
        dataType: 'json',        
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
           
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboStatusClass')
                        .append($('<option></option>', { text: value["statusdesc"] })
                            .attr('value', value["statusid"]));                   
                });
            }
        },
        error: function (ex) {
            alert('Không lấy danh sách trạng thái lớp học');
        }
    })
}

///Function Load danh sách lớp học: event btnXem
function LoadDsClass() {
    let tungay = $('#dpTungay').val();
    let denngay = $('#dpDenngay').val();
    let typeEdu = $('#cboTypeEdu').val();
    let groupEdu = $('#cboGroupEdu').val();
    let statusClass = $('#cboStatusClass').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsClass',
        dataType: 'json',
        data: JSON.stringify({ tungay: tungay, denngay: denngay, typeEdu: typeEdu, groupEdu: groupEdu, statusClass: statusClass }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {            
            $('#tblClass').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblClass').DataTable({
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
                    "columns": [{ 'data': 'classdesc' },
                        { 'data': 'categoryid' },
                        { 'data': 'sohvien' },
                        { 'data': 'ngaybd_kh', visible:false },
                        { 'data': 'ngaybd_th' },
                        { 'data': 'ht' },
                        { 'data': 'so_file' },
                        {
                            "data": null, render: function (data, type, row) {
                                var html = '';
                                html = '<button type="button" class="btn btn-primary btn-sm" title="Sửa lớp" onclick="editClass(' + "'" + row.classid + "'" + ')"><i class="fa fa-edit"></i></button>' + 
                                    ' <button type="button" class="btn btn-danger btn-sm" title="Xoá lớp học" onclick="confirmDel(' + "'" + row.classid + "','" + row.classdesc + "'" + ')"><i class="fa fa-trash"></i></button>' +
                                    ' <button type="button" class="btn btn-success btn-sm" title="Cập nhật kết quả" onclick="addResult(' + "'" + row.classid + "','" + row.classdesc + "'" + ')"><i class="fa fa-code-fork"></i></button>';
                                    
                                return html;
                            }, 'width': "90px", orderable:false
                        }
                    /*{ 'data': 'typeid', 'visible': false },*/
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

//Hàm clear form modal thêm mới class
function clearFromAddClass() {
    //$('#mdDonvi').val('');
    $('#mdDonviDtao').val('');
    $('#mdLoaiLop').val('');
    $('#mdMaHieu').val('');
    $('#mdLoaiDtao').val('');
    $('#mdNhomhl').val('');
    //$('#mdKhBatdau').val('');
    //$('#mdKhKthuc').val('');
    $('#mdThBatdau').val('');
    $('#mdThKthuc').val('');
    $('#formFileMultiple').val('');
    //$('#tblOrgAdd').value = "";
    $('#tblPerson').DataTable().clear().draw();
    $('#tblPersonSelect').DataTable().clear().draw();
    $('#tblPersonSelect').DataTable().destroy();
    //Load các loại danh mục;
    $('#listFile').css('display', 'none');
    $('#mdStatus').html('Tạo mới');
    $('#f1').val('');
}

//Lấy danh sách nhân sự của lớp học
function LoadDsPersonalInClass(classid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsPersonalInClass',
        dataType: 'json',
        data: JSON.stringify({ classid: classid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblStudentClass').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblStudentClass').DataTable({
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
                        { 'data': 'tendvi' },
                        { 'data': 'tenkhaisinh' },
                        { 'data': 'chucdanh' },
                        { 'data': 'kqua' },
                        { 'data': 'tinhtrang' },                        
                    ],
                    //"columnDefs": [{
                    //    'targets': 5,
                    //    'searchable': false,
                    //    'orderable': false,
                    //    'className': 'td-body-center',
                    //    'render': function (data, type, full, meta) {
                    //        return '<input type="checkbox" name="id[]" value="' + full.nsid + '">';
                    //    }
                    //}],
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự');
        }
    })
}

//Hàm lấy danh sách nhân sự theo đơn vị
function LoadDsPersonalByOrg() {
    //Lấy đơn vị chọn
    var t = $('#tblOrgAdd').DataTable();
    var dvql = t.row('.selected').data()['madvql'];

    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsPersonalByOrg',
        dataType: 'json',
        data: JSON.stringify({ madvql: dvql }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblPerson').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblPerson').DataTable({
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
                    "columns": [{ 'data': 'nsid' ,visible: false, orderable: false },
                        { 'data': 'orgid', visible: false, orderable: false },
                        { 'data': 'tenkhaisinh' },
                        { 'data': 'chucdanh' },
                        { 'data': 'kqua' },
                    ],
                    "columnDefs": [{
                        targets: 5,
                        searchable: false,
                        orderable: false,
                        className: 'dt-body-center',
                        'render': function (data, type, full, meta) {
                            return '<input type="checkbox" name="id[]" value="' + full.nsid + '">';
                        }
                    }],
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự');
        }
    })
}

//Hàm lấy danh sách nhân sự theo đơn vị form Add
function LoadDsPersonalAddByOrg() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsPersonal',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblPerson').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblPerson').DataTable({
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
                        { 'data': 'chucdanh' },
                        { 'data': 'tenkhaisinh', 'visible': false },
                    ],
                    "columnDefs": [{
                        'targets': 3,
                        'searchable': false,
                        'orderable': false,                        
                        'className': 'td-body-center',
                        'render': function (data, type, full, meta) {
                            return '<input type="checkbox" name="id[]" value="' + full.nsid + '">';
                        }
                    }],
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự');
        }
    })
}

//Function SaveClass
function OnSaveClass() {

    //Check trạng thái > 1 thì đc ghi
    let tthai = $('#txtStatus').val();

    if (tthai > 1) {
        alert('Trạng thái không cho phép sửa!');
        return;
    }
    
    var formData = new FormData();
    var totalFiles = counter - 1;//document.getElementById("f1").files.length;

    for (var i = 0; i < totalFiles; i++) {
        if (i == 0) {
            var file = document.getElementById("f1").files[0];
        } else if (i == 1) {
            var file = document.getElementById("f2").files[0];
        } else if (i == 2) {
            var file = document.getElementById("f3").files[0];
        } else if (i == 3) {
            var file = document.getElementById("f4").files[0];
        } else if (i == 4) {
            var file = document.getElementById("f5").files[0];
        }
        formData.append("FileUpload", file);
    }

    //String date ngày ký
    let sngayky = "";
    for (var i = 1; i < counter; i++) {        
        sngayky = sngayky + $('#n' + i).val() + ";";
    }

    formData.append("ngaykys", sngayky);
    formData.append("dvdaotao", $('#mdDonviDtao').val());
    formData.append("loailop", $('#mdLoaiLop').val());
    formData.append("mahieu", $('#mdMaHieu').val());
    formData.append("loaidaotao", $('#mdLoaiDtao').val());
    formData.append("nhomhluyen", $('#mdNhomhl').val());
    //formData.append("khbatdau", $('#mdKhBatdau').val(''));
    //formData.append("khketthuc", $('#mdKhKthuc').val(''));
    formData.append("thbatdau", $('#mdThBatdau').val());
    formData.append("thketthuc", $('#mdThKthuc').val());
    formData.append("classid", $('#txtClassId').val());

    //Danh sách nhân sự
    var table = $('#tblPersonSelect').DataTable();
    var data = table.rows().data();
    var dsnhansu = "";    
    var ttnut = $('#ttNut').val();
    data.each(function (value, index) {
        //let res = $.isEmptyObject(value);
        if (ttnut == 'ADD') {
            dsnhansu = dsnhansu + ";" + value[1].toString();            
        } else {
            dsnhansu = dsnhansu + ";" + value["nsid"].toString();
        }
                   
    })

    formData.append("lstnhansu", dsnhansu);

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/OnSaveClass',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Load lại danh sách
            LoadDsClass();
            alert('Thêm mới lớp học thành công!');
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
}

//Load danh mục Loại đào tạo Thêm mới.
function LoadDmLDaotao(groupdefault) {
    $('#mdLoaiDtao').empty();
    $('#mdLoaiDtao').append($('<option>', {
        value: '',
        text: '-- Chọn loại đào tạo --'
    }));

    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/getTypeEdu',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {                       
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#mdLoaiDtao')
                        .append($('<option></option>', { text: value["groupdesc"] })
                            .attr('value', value["groupid"]));
                });
            }
            $("#mdLoaiDtao").val(groupdefault);
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Loại hình đào tạo');
        }
    })
}

//Load danh mục Nhóm đào tạo theo Loại đào tạo (Thêm mới)
function LoadDsNhomTheoLoaiDaotao(groupdefault,nhomdefault) {
    //let typeedu = $('#mdLoaiDtao').val();
    $('#mdNhomhl').empty();
    //if (typeedu == "ALL") {
        $('#mdNhomhl').append($('<option></option>', {
            value: "",
            text: '-- Chọn nhóm huấn luyện --'
        }));
    //}
    //Load Nhóm đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadGroupEdu',
        dataType: 'json',
        data: JSON.stringify({ typeid: groupdefault }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#mdNhomhl')
                        .append($('<option></option>', { text: value["categorydesc"] })
                            .attr('value', value["categoryid"]));
                });

                $('#mdNhomhl').val(nhomdefault);
            }
        },
        error: function (ex) {
            alert('Không lấy được danh sách Nhóm đào tạo');
        }
    })
}
//Xoá lớp học
function confirmDel(classid,classdesc) {
    //
    $('#txtClassId').val(classid);
    $('#lbMessage').text("(" + classdesc + ")");    
    $('#confirmDel').modal('show');
}

function delClass() {
    $('#confirmDel').modal('hide');
    if ($('#txtClassId').val() != null) {
        //Gọi thủ tục xoá:
        $.ajax({
            type: 'POST',
            url: '/Admin/HLAT/OnDeleteClass',
            dataType: 'json',
            data: JSON.stringify({ classid: $('#txtClassId').val() }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response.length > 0) {
                    alert("Xoá thành công!");
                    //Xoá xong reset
                    $('#txtClassId').val('');
                    //Load lại danh sách;
                    LoadDsClass();
                }
            },
            error: function (ex) {
                alert('Xoá không thành công!');
            }
        })
                
    }
}

//Load danh sách nhân sự thuộc lớp học và kết quả.
function LoadDsPersonByClass() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsPersonalInClass',
        dataType: 'json',
        data: JSON.stringify({ classid: $('#txtClassId').val() }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //$('#tblPersonSelect').dataTable().fnClearTable();
            if (response.length > 0) {
                $('#tblPersonSelect').DataTable({
                    "language": {
                        "lengthMenu": "Hiển thị _MENU_",
                        "zeroRecords": "Không có dữ liệu",
                        "info": "Hiển thị _START_ đến _END_ trong _TOTAL_ dòng",
                        "infoEmpty": "",
                        "paginate": {
                            "next": ">>",
                            "previous": "<<"
                        },
                        search: "",
                        searchPlaceholder: "Tìm kiếm..."
                    },
                    "destroy": true,
                    "data": response,
                    "columns": [{
                        "data": null, render: function (data, type, row) {
                            var html = '';
                            html = '<input type="checkbox" style="text-align:center;" name="id[]" value="' + row.nsid + '">';

                            return html;
                        }, sClass: 'dt-body-center', orderable: false
                    }, { 'data': 'nsid' , visible:false},
                        { 'data': 'tendvi' },
                    { 'data': 'tenkhaisinh' },
                    { 'data': 'tinhtrang' },
                    ]
                    ,
                    "columnDefs": [{
                        targets: 0,
                        searchable: false,
                        orderable: false,
                        className: 'td-body-center',
                        width: "20px",
                    }, { "width": "50px", "targets": 3 }]
                });
            } else {
                ///Khởi tạo table
                $('#tblPersonSelect').DataTable({
                    "language": {
                        "lengthMenu": "Hiển thị _MENU_",
                        "zeroRecords": "Không có dữ liệu",
                        "info": "Hiển thị _START_ đến _END_ trong _TOTAL_ dòng",
                        "infoEmpty": "",
                        "paginate": {
                            "next": ">>",
                            "previous": "<<"
                        },
                        search: "",
                        searchPlaceholder: "Tìm kiếm..."
                    },
                    "columnDefs": [{
                        "className": "dt-center", "targets": 0, orderable: false
                    }]
                });
            }


        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự tham gia lớp học.');
        }
    })
}

//List danh sách file by classid
function LoadLstFileByClass() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadListFileByClass',
        dataType: 'json',
        data: JSON.stringify({ classid: $('#txtClassId').val() }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //$('#tblListFile').dataTable().fnClearTable();   
            $('#tblListFile').DataTable().fnClearTable(); 
            if (response.length > 0) {
                var table = $('#tblListFile').DataTable({
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
                    "columns": [{ 'data': 'fileid'},
                        { 'data': 'filename', sClass: 'dt-body-left' },
                        { 'data': 'ngay_ky', sClass: 'dt-body-left' },                        
                        {
                            "data": null, render: function (data, type, row) {
                                var html = '';
                                html = '<button type="button" class="btn btn-success btn-sm" title="Xem nội dung file" onclick="viewFileByClass(' + "'" + row.fileid + "','" + row.filename + "'" + ')"><i class="fa fa-download"></i></button>';
                                html = html + '  <button type="button" class="btn btn-danger btn-sm" title="Xoá file" onclick="delFileByClass(' + "'" + row.fileid + "'" + ')"><i class="fa fa-trash"></i></button>';
                                
                                return html;
                            },sClass: 'dt-body-center'
                        },
                    ],                    
                    "columnDefs": [{
                        'targets': 0,
                        'visible': false,   
                        searchable: false,
                    }, {
                        'targets': 3,
                        'orderable': false,                       
                        }, { "width": "50px", "targets": 3 }, { "width": "100px", "targets": 2 }, { "width": "20px", "targets": 0 }],
                    'order': [[1, 'asc']],
                    //"paging": false,
                    //"bFilter": false, //hide Search bar
                    //"bInfo": false,
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy được danh sách File đính kèm');
        }
    })
}
var obj;
//Hàm load Class By Id
function LoadClassById(classid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadClassById',
        dataType: 'json',
        data: JSON.stringify({ classid: classid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0) {
                obj = response[0];
                //Fill thông tin vào form;
                LoadDmLDaotao(obj.loaidaotao);
                LoadDsNhomTheoLoaiDaotao(obj.loaidaotao, obj.categoryid);
                FillDataEdit();
            }
        },
        error: function (ex) {
            alert('Không lấy được lớp học đã chọn!');
        }
    })
}

//Fill data edit
function FillDataEdit() {

    //if (obj.ngaybd_kh != null) {
    //    var day = obj.ngaybd_kh.substr(0,2);//("0" + ngaybd_kh.getDate()).slice(-2);
    //    var month = obj.ngaybd_kh.substr(3, 2); //("0" + (ngaybd_kh.getMonth() + 1)).slice(-2);
    //    var year = obj.ngaybd_kh.substr(6, 4);
    //    var bd_kh = (month) + "/" + (day) + "/" + year;
    //    $('#mdKhBatdau').datepicker({
    //        dateFormat: 'dd/mm/yy',
    //    }).val(bd_kh);

    //}

    //if (obj.ngaykt_kh != null) {
    //    let ngaykt_kh = new Date(obj.ngaykt_kh);
    //    var daykt = ("0" + ngaykt_kh.getDate()).slice(-2);
    //    var monthkt = ("0" + (ngaykt_kh.getMonth() + 1)).slice(-2);

    //    var bd_khkt = (daykt) + "/" + (monthkt) + "/" + ngaykt_kh.getFullYear();
    //    $('#mdKhKthuc').datepicker({
    //        dateFormat: 'dd/mm/yy',
    //    }).val(bd_khkt);

    //}

    $('#mdStatus').html(obj.statusdesc);
    $('#txtStatus').val(obj.statusid);

    if (obj.ngaybd_th != null) {       

        var dayth = obj.ngaybd_th.substr(0, 2);//("0" + ngaybd_th.getDate()).slice(-2);
        var monthth = obj.ngaybd_th.substr(3, 2);//("0" + (ngaybd_th.getMonth() + 1)).slice(-2);
        var year = obj.ngaybd_th.substr(6, 4);
        var bd_th = (dayth) + "/" + (monthth) + "/" + year;
        $('#mdThBatdau').datepicker({
            dateFormat: 'dd/mm/yy',
        }).val(bd_th);

    }

    if (obj.ngaykt_th != null) {
        
        var daythkt = obj.ngaykt_th.substr(0, 2);//("0" + ngaykt_th.getDate()).slice(-2);
        var monththkt = obj.ngaykt_th.substr(3, 2);//("0" + (ngaykt_th.getMonth() + 1)).slice(-2);
        var year = obj.ngaykt_th.substr(6, 4);
        var bd_thkt = (daythkt) + "/" + (monththkt) + "/" + year;
        $('#mdThKthuc').datepicker({
            dateFormat: 'dd/mm/yy',
        }).val(bd_thkt);

    }

    //$('#mdDonvi').value = obj.;
    $('#mdDonviDtao').val(obj.ma_dvi_daotao);
    $('#mdLoaiLop').val(obj.classdesc);
    $('#mdMaHieu').val(obj.classcode);    
    $('#f1').val('');   
}
//Hàm edit class
function editClass(classid) {   
    //Load form by classid
    clearFromAddClass();
    $('#txtClassId').val(classid);
    //Fill form;
    LoadClassById(classid);    
    LoadLstFileByClass();
    LoadDsOrgModal();
    LoadDsPersonByClass();

    $('#ttNut').val('EDIT');

    //Load danh sách nhân sự tham gia;      
    $('#listFile').attr('style', 'display: inline');  
    $('#mdAddClass').modal('show');
}

//Xoá file thuộc lớp
function delFileByClass(fileid) {    
        //Gọi thủ tục xoá:
        $.ajax({
            type: 'POST',
            url: '/Admin/HLAT/OnDeleteFileByClass',
            dataType: 'json',
            data: JSON.stringify({ fileid: fileid }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response.length > 0 && response==='OK') {
                    alert("Xoá thành công!");
                    //Xoá xong reset                   
                }
            },
            error: function (ex) {
                alert('Xoá không thành công!');
            }
        })
    
}

//Gọi modal cập nhật kết quả
function addResult(classid, classdesc) {
    //Lấy thông tin chung class
    LoadClassByIdResult(classid);
    //Lấy danh sách file 
    LoadLstFileResultByExam($('#examid').val());
    //Lấy kết quả thi gần nhất
    LoadLastResultByClass(classid);
    //Lấy combo loại thi theo nhóm (categoryid)    
    //Lấy kết quả theo combo
    $('#hdClassid').val(classid);
    $('#mdAddResult').modal('show');
}

//Lấy combo loại thi
function FillCboLoaiThi(categoryid) {
    //let categoryid = $('#categoryid').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/getCboLoaiThiResult',
        dataType: 'json',
        data: JSON.stringify({ categoryid: categoryid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#kqLoaithi').empty();           
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#kqLoaithi')
                        .append($('<option></option>', { text: value["standard_desc"] })
                            .attr('value', value["standardid"]));

                    //set val
                    $('#kqLoaithi').val(value["standardid"]);
                });

                FillCboLanThi();
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Loại hình đào tạo');
        }
    })
}

//Lấy combo lần thi từ loại thi
function FillCboLanThi() {
    let loaithi = $('#kqLoaithi').val();
    let classid = $('#hdClassid').val();
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadCboSolanThiByClassStandard',
        dataType: 'json',
        data: JSON.stringify({ classid: classid, standardid: loaithi }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#kqLanthi').empty();
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#kqLanthi')
                        .append($('<option></option>', { text: "Lần " + value["exam_order"] })
                            .attr('value', value["exam_order"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Loại hình đào tạo');
        }
    })
}

//Kết quả:
//1. Load thông tin class
//Hàm load Class By Id
function LoadClassByIdResult(classid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadClassById',
        dataType: 'json',
        data: JSON.stringify({ classid: classid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0) {
                obj = response[0];
                //Fill thông tin vào form;                
                FillDataResult(obj);
            }
        },
        error: function (ex) {
            alert('Không lấy được lớp học đã chọn!');
        }
    })
}
//2. Filldata Class
function FillDataResult(obj) {
    $('#kqDonvi').text(obj.tendvi);
    if (obj.ma_dvi_daotao == 'CDN') {
        $('#kqDvDtao').text("Cao đẳng Nghề");
    } else {
        $('#kqDvDtao').text("Tự thực hiện");
    }
    
    $('#kqLop').text(obj.classdesc);
    $('#kqMahieu').text(obj.classcode);
    $('#kqLoaiDtao').text(obj.groupdesc);
    $('#kqNhomhl').text(obj.categorydesc);
    $('#kqNgayth').text(obj.ngaybd_th + ' - ' + obj.ngaykt_th);
    $('#kqSohvien').text(obj.sohvien);
    $('#categoryid').val(obj.categoryid);
    FillCboLoaiThi(obj.categoryid);
}

//Load list File Result
function LoadLstFileResultByExam(examid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadListFileByExam',
        dataType: 'json',
        data: JSON.stringify({ examid: examid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblListFileResult').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblListFileResult').DataTable({  
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
                    scrollY: '100px',
                    scrollX: '100px',
                    scrollCollapse: true,
                    "data": response,
                    "columns": [{ 'data': 'fileid', visible: false, orderable: false },
                        { 'data': 'filename', sClass: 'dt-body-left' },
                        { 'data': 'ngay_tao', sClass: 'dt-body-left' },
                        //{ 'data': 'fileurl', visible: false },
                        //{ 'data': 'filetype', visible: false },
                        {
                            "data": null, render: function (data, type, row) {
                                var html = '';
                                html = '<button type="button" class="btn btn-success btn-sm" title="Xem nội dung file" onclick="viewFileByClass(' + "'" + row.fileid + "','" + row.filename + "'" + ')"><i class="fa fa-download"></i></button>';

                                return html;
                            }, sClass: 'dt-body-center'
                        },
                    ],
                    "columnDefs": [{
                        'targets': 0,
                        'visible': false,
                        searchable: false,
                    }, {
                        target: 2,
                        order: false,
                    }, { "width": "40px", "targets": 2 }],
                    'order': [[1, 'asc']],
                    "paging": false,
                    "bFilter": false, //hide Search bar
                    "bInfo": false,
                });
            }

        },
        error: function (ex) {
            //alert('Không có File đính kèm');
        }
    })
}

//Mục kết quả:
//3. Load danh sách điểm gần nhất của các nhân sự theo lớp vừa chọn
function LoadLastResultByClass(classid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadLastResultByClass',
        dataType: 'json',
        data: JSON.stringify({ classid: classid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblResultExam').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblResultExam').DataTable({
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
                    scrollY: '250px',
                    "destroy": true,                    
                    "data": response,
                    "columns": [{ 'data': 'donvi' }, //Đơn vị
                    { 'data': 'nhansu', sClass: 'dt-body-left' },//Nhân sự
                        { 'data': 'chucdanh', sClass: 'dt-body-left' },//Chức danh
                        { 'data': 'dlythuyet', sClass: 'dt-body-left' },//Điểm lý thuyết
                        { 'data': 'dthuchanh', sClass: 'dt-body-left' },//Điểm thực hành                 
                    ],                    
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách học viên');
        }
    })
}
//Lấy kết quả theo combo chọn hình thức thi (Thực hành, lý thuyết) và lần thi
function LoadResultExamByHtthi(classid) {
    let htthi = $('#kqLoaithi').val();
    let lanthi = $('#kqLanthi').val();
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadResultExamByHtthi',
        dataType: 'json',
        data: JSON.stringify({ classid: classid, htthi: htthi, lanthi:lanthi }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblResultExam').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblResultExam').DataTable({
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
                    scrollY: '250px',
                    "destroy": true,
                    "data": response,
                    "columns": [{ 'data': 'examid' }, //Đơn vị
                    { 'data': 'donvi', sClass: 'dt-body-left' },//Nhân sự
                    { 'data': 'nsid', sClass: 'dt-body-left' },//Chức danh
                    { 'data': 'chucdanh', sClass: 'dt-body-left' },//Điểm lý thuyết
                    { 'data': 'diem', sClass: 'dt-body-left' },//Điểm thực hành                 
                    ],
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách File đính kèm');
        }
    })
}
//View file by classid
function viewFileByClass(fileid,filename) {
    //Gọi thủ tục xem file:
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/OnViewFileById',        
        data: { fileid: fileid },        
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


function DownloadFileTmpResult() {
    let d = new Date();

    let filename = "Template_HLAT_Kqua" + d.toLocaleTimeString() + ".xlsx";
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/DownloadFileTmpResult',
        data: { classid: $('#hdClassid').val(), standardid: $('#kqLoaithi').val() },
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

//add file to csdl -> return examid
function addImgExam(examid) {
    //Add exam => examid    
    //let examid = $('#examid').val();
    var formData = new FormData();
    var totalFiles = document.getElementById("uploadImgExam").files.length;
    if (totalFiles > 0) {
        var file = document.getElementById("uploadImgExam").files[0];
        formData.append("FileUpload", file);
    }

    formData.append("examid", examid);

    //tạo id kỳ thi
    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/OnSaveExamFile',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Gọi hảm hiển thị table file upload
            LoadLstFileResultByExam(examid);
            
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
    //var tblFileResult = $('#tblListFileResult').DataTable();
    //let dateNow = new Date();
    //for (var i = 0; i < fileUpload.length; i++)
    //{
    //    let objFile = {
    //        fileid: i, filename: fileUpload[0].name, ngay_tao: dateNow, x:''
    //        }
    //    tblFileResult.row.add(objFile);
    //}
}

function addResultExam() {
    var formData = new FormData();
    formData.append("classid", $('#hdClassid').val());
    formData.append("loaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("lanthi", $('#kqLanthi').val());
    
    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/OnSaveExam',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Trả ra examid
            $('#examid').val(response);
            addImgExam(response);
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
}
//Update kết quả từ file vào csdl
function updateResultExam() {
    //let examid = $('#examid').val();
    var formData = new FormData();
    var totalFiles = document.getElementById("uploadFile").files.length;
    if (totalFiles > 0) {
        var file = document.getElementById("uploadFile").files[0];
        formData.append("FileUpload", file);
    }
    //formData.append("examid", examid);

    formData.append("classid", $('#hdClassid').val());
    formData.append("loaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("lanthi", $('#kqLanthi').val());
    formData.append("loaidt", $('#categoryid').val());

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/OnUpdateExam',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Trả ra examid
            //GetPointByExam(examid);
            GetViewKqua();
            LoadLastResultByClass($('#hdClassid').val());
            alert('Cập nhật thành công!');
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình cập nhật! Vui lòng xem lại file nhập vào.' + error);
        }
    });
}

//Lấy examid bởi classid + categoryid (loại thi) + lần thi (1,2,3)

function GetPointByClassIdTimes() {
    let classid = $('#hdClassid').val();
    let categoryid = $('#hdClassid').val();
}

//Hàm lấy danh sách điểm thi theo form lựa chọn
function GetPointByExam(examid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadPointByExam',
        dataType: 'json',
        data: JSON.stringify({ examid: examid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblResultUpdate').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblResultUpdate').DataTable({
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
                    "columns": [{ 'data': 'examid', visible:false }, //Đơn vị
                        { 'data': 'nhansu', visible:false },
                        { 'data': 'donvi', visible:false },//Nhân sự
                        { 'data': 'tenkhaisinh', sClass: 'dt-body-left' },//Chức danh
                        { 'data': 'chucdanh', sClass: 'dt-body-left' },//Điểm lý thuyết
                        { 'data': 'diem', sClass: 'dt-body-left' },//Điểm thực hành                       
                        {
                            'data': null, render: function (data, type, row) {
                                var html = '';
                                html = '<button type="button" class="btn btn-primary btn-sm" title="Sửa điểm thi" onclick="editPoint(' + "'" + row.examid + "','" + row.nhansu + "'" + ')"><i class="fa fa-edit"></i></button>';

                                return html;
                            }, 'width': "20px"
                        }
                    ],
                    "columnDefs": [{
                        'targets': 0,
                        'visible': false,
                        searchable: false,
                    }, {
                        'targets': 6,
                        orderable: false,
                        },{
                            'targets': 5,
                            orderable: false,
                        }]    
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách File đính kèm');
        }
    })
}

function editPoint(examid, nsid) {
    $('#txtExamid').val(examid);
    $('#txtnsid').val(nsid);
    $('#mdUpdatePoint').modal('show');
}


function updatePoint() {
    //Add exam => examid    
    let examid = $('#txtExamid').val();
    let nsid = $('#txtnsid').val();
    let pointnew = $('#txtPoint').val();

    var formData = new FormData();   
    let classid = $('#hdClassid').val();

    let i = checkLockExam(1);
    if (i < 1) {
        formData.append("examid", examid);
        formData.append("nsid", nsid);
        formData.append("pointnew", pointnew);

        //tạo id kỳ thi
        $.ajax({
            type: 'post',
            url: '/Admin/HLAT/OnChangePoint',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                //Gọi hảm hiển thị table file upload
                GetPointByExam(examid);
                LoadLastResultByClass(classid);
                alert('Cập nhật thành công!');
            },
            error: function (error) {
                alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
            }
        });
    }    
    //var tblFileResult = $('#tblListFileResult').DataTable();
    //let dateNow = new Date();
    //for (var i = 0; i < fileUpload.length; i++)
    //{
    //    let objFile = {
    //        fileid: i, filename: fileUpload[0].name, ngay_tao: dateNow, x:''
    //        }
    //    tblFileResult.row.add(objFile);
    //}
}

//Hàm view kết quả thi theo lớp và lựa chọn kỳ thi
function GetViewKqua() {
    let examid = '';
    //Lấy examid theo classid và 
    var formData = new FormData();
    formData.append("classid", $('#hdClassid').val());
    formData.append("loaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("lanthi", $('#kqLanthi').val());
    formData.append("loaidt", $('#categoryid').val());

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/GetExamID',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Trả ra examid
            let examid = response;
            GetPointByExam(examid);
            LoadLstFileResultByExam(examid);            
        },
        error: function (error) {
            alert('Không lấy được danh sách! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
};

///Load trạng thái lớp để change
function LoadStatusChangeClass(classid) {
    //Load trạng thái lớp học
    var formData = new FormData();   

    formData.append("classid", classid);

    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadStatusClass',
        dataType: 'json',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {

            if (response.length > 0) {
                $('#cboStatusChange').empty();  
                $.each(response, function (i, value) {
                    $('#cboStatusChange')
                        .append($('<option></option>', { text: value["statusdesc"] })
                            .attr('value', value["statusid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không lấy danh sách trạng thái lớp học');
        }
    })
}

function updateStatus() {
    var formData = new FormData();
    formData.append("classid", $('#txtClassId').val());
    formData.append("statusnew", $('#cboStatusChange').val());//Lý thuyết hoặc thực hành
    let trangthaimoi = $('#cboStatusChange option:selected').text();
    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/UpdateStatus',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Trả ra examid
            $('#mdStatus').html(trangthaimoi);
            $('#mdUpdateStatus').modal('hidden');
            alert('Cập nhật trạng thái thành công;');
        },
        error: function (error) {
            alert('Không lấy được danh sách! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
}

///Load trạng thái lớp để change
function LoadCboNhansuByClassId(classid) {
    //Load trạng thái lớp học
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/LoadDsPersonalInClass',
        dataType: 'json',
        data: JSON.stringify({ classid: classid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0) {
                $('#cboNhansu').empty();
                $.each(response, function (i, value) {
                    $('#cboNhansu')
                        .append($('<option></option>', { text: value["tenkhaisinh"] + " - " + value["chucdanh"] })
                            .attr('value', value["nsid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không lấy danh sách trạng thái lớp học');
        }
    })
}

function saveResultPerson() {
    var formData = new FormData();
    formData.append("classid", $('#hdClassid').val());
    formData.append("kqLoaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("kqLanthi", $('#kqLanthi').val());//Lý thuyết hoặc thực hành
    formData.append("cboNhansu", $('#cboNhansu').val());//Lý thuyết hoặc thực hành
    formData.append("pointperson", $('#txtPointPerson').val()); //Điểm
    formData.append("notepoint", $('#txtNotePoint').val());

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/InputPointPerson',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            alert('Cập nhật điểm thành công;');
            $('#mdUpdatePointPerson').modal('toggle');
        },
        error: function (error) {
            alert('Nhập điểm không thành công.Vui lòng liên hệ quản trị để kiểm tra lỗi!');
        }
    });
}

function lockExam() {
    var formData = new FormData();
    formData.append("classid", $('#hdClassid').val());
    formData.append("kqLoaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("kqLanthi", $('#kqLanthi').val());//Lý thuyết hoặc thực hành    

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/LockExam',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            alert('Khoá điểm thành công;');
            $('#confirmLockExam').modal('toggle');
        },
        error: function (error) {
            alert('Khoá điểm không thành công.Vui lòng liên hệ quản trị để kiểm tra lỗi!');
        }
    });
}
//Hàm check lock Exam
function checkLockExam(thaotac) {
    var formData = new FormData();
    formData.append("classid", $('#hdClassid').val());
    formData.append("kqLoaithi", $('#kqLoaithi').val());//Lý thuyết hoặc thực hành
    formData.append("kqLanthi", $('#kqLanthi').val());//Lý thuyết hoặc thực hành    

    $.ajax({
        type: 'post',
        url: '/Admin/HLAT/CheckLockExam',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            if (parseInt(response) > 0) {                
                $('#msgLockExam').modal('show');                
                return 1;
            } else {
                if (thaotac == 0) {
                    $('#mdUpdatePointPerson').modal('show');
                }
                return 0;
            }
           
        },
        error: function (error) {
            alert('Khoá điểm không thành công.Vui lòng liên hệ quản trị để kiểm tra lỗi!');
        }
    });
}