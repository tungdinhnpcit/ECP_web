function LoadLoaiHinhDT() {
    //Load loại hình đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLAT/getTypeEdu',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboLhinhDt').empty();
            $('#cboLhinhDt').append($('<option>', {
                value: "ALL",
                text: '-- Chọn loại đào tạo --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboLhinhDt')
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

function LoadNhomDt() {
    let typeedu = $('#cboLhinhDt').val();
    $('#cboNhomDt').empty();

    if (typeedu == "ALL") {
        $('#cboNhomDt').append($('<option>', {
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

                    $('#cboNhomDt')
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

function LoadStatusPlan() {
    //Load trạng thái kế hoạch
    let kieu = "1";

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadStatusPlan',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboStatusPlan')
                        .append($('<option></option>', { text: value["statusdesc"] })
                            .attr('value', value["statusid"]));
                });
            }
        },
        error: function (ex) {
            alert('Không lấy danh sách trạng thái kế hoạch');
        }
    })
}
///Function Load danh sách kế hoạch: event btnXem
function LoadDsPlan() {
    let tungay = $('#dpTungay').val();
    let denngay = $('#dpDenngay').val();
    let typeEdu = $('#cboLhinhDt').val();
    let groupEdu = $('#cboNhomDt').val();
    let statusplan = $('#cboStatusPlan').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsPlan',
        dataType: 'json',
        data: JSON.stringify({ tungay: tungay, denngay: denngay, typeEdu: typeEdu, groupEdu: groupEdu, statusplan: statusplan }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblPlan').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblPlan').DataTable({
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
                    "columns": [{ 'data': 'planid', visible: false },
                        { 'data': 'plandesc' },
                    { 'data': 'categoryid' },
                    { 'data': 'so_hvien' },
                    { 'data': 'ngay_bdau' },
                    { 'data': 'ngay_kthuc' },
                    {
                        "data": null, render: function (data, type, row) {
                            var html = '';
                            html = '<button type="button" class="btn btn-primary btn-sm" onclick="editPlan(' + "'" + row.planid + "'" + ')"><i class="fa fa-edit"></i></button>' +
                                ' <button type="button" class="btn btn-danger btn-sm" onclick="confirmDel(' + "'" + row.planid + "','" + row.plandesc + "'" + ')"><i class="fa fa-trash"></i></button>';
                            return html;
                        }, 'width': "60px"
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
 
//Hàm edit class
function editPlan(planid) {
    //Load form by classid
    clearFromAddPlan();    
    $('#txtPlanId').val(planid);    
    
    //Fill form;
    LoadPlanById(planid);
    LoadLstFileByPlan();
    LoadDsOrgModal();
    LoadDsPersonEditByPlan(planid);

    var table = $('#tblPersonSelect').DataTable();
    //Set form Edit
    let icount = table.rows().data().count();
    $('#ttNut').val('EDIT');
    //if (icount > 1) {
    //    $('#ttNut').val('EDIT');
    //} else {
    //    $('#ttNut').val('ADD');
    //}
    //Load danh sách nhân sự tham gia;      
    $('#listFile').attr('style', 'display: inline');
    $('#mdAddPlan').modal('show');
}

//List danh sách file by classid
function LoadLstFileByPlan() {    
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadListFileByPlan',
        dataType: 'json',
        data: JSON.stringify({ planid: $('#txtPlanId').val() }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblListFile').dataTable().fnClearTable();
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
                    "columns": [{ 'data': 'fileid' },
                        { 'data': 'filename', sClass: 'dt-body-left' },
                        {
                            "data": null, render: function (data, type, row) {
                                var html = '';
                                html = '<button type="button" class="btn btn-success btn-sm" title="Xem nội dung file" onclick="viewFileByPlan(' + "'" + row.fileid + "','" + row.filename + "'" + ')"><i class="fa fa-download"></i></button>';
                                html = html + '  <button type="button" class="btn btn-danger btn-sm" title="Xoá file" onclick="delFileByPlan(' + "'" + row.fileid + "'" + ')"><i class="fa fa-trash"></i></button>';

                                return html;
                            }, sClass: 'dt-body-center'
                        }
                    ],
                    "columnDefs": [{
                        'targets': 0,
                        'visible': false,
                        searchable: false,
                    }, { "width": "60px", "targets": 2 }, { 'orderable': false, "target": 2 }],
                    'order': [[1, 'asc']],
                    "paging": false,
                    "bFilter": false, //hide Search bar
                    "bInfo": false,
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách File đính kèm');
        }
    })
}

//Load danh sách nhân sự thuộc lớp học và kết quả.
function LoadDsPersonByPlan(planid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsPersonalInPlan',
        dataType: 'json',
        data: JSON.stringify({ planid: planid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblStudentPlan').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblStudentPlan').DataTable({
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
                    "columns": [{ 'data': 'nsid', visible: false },
                    { 'data': 'orgid' },
                    { 'data': 'tenkhaisinh' },
                    { 'data': 'tinhtrang' },
                    ]
                    ,
                    "columnDefs": [{
                        'targets': 0,
                        'searchable': false,
                        'orderable': false,
                        'className': 'td-body-center',
                    }, { "width": "20px", "targets": 0 }, { "width": "30%", "targets": 3 }]
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự tham gia lớp học.');
        }
    })
}

//Load danh sách nhân sự thuộc lớp học và kết quả.
function LoadDsPersonEditByPlan(planid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsPersonalInPlan',
        dataType: 'json',
        data: JSON.stringify({ planid: planid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblPersonSelect').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblPersonSelect').DataTable({
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
                    "columns": [
                        {
                            "data": null, render: function (data, type, row) {
                                var html = '';
                                html = '<input type="checkbox" name="id[]" value="' + row.nsid + '">';

                                return html;
                            }, sClass: 'dt-body-center', orderable: false
                        },
                        { 'data': 'nsid' },
                    { 'data': 'orgid' },
                    { 'data': 'tenkhaisinh' },
                    { 'data': 'tinhtrang' },
                    ],                   
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhân sự tham gia lớp học.');
        }
    })
}

//Xoá lớp học
function confirmDel(planid, plandesc) {
    //
    $('#txtPlanId').val(planid);
    $('#lbMessage').text("(" + plandesc + ")");
    $('#confirmDel').modal('show');
}

function delPlan() {
    $('#confirmDel').modal('hide');
    if ($('#txtPlanId').val() != null) {
        //Gọi thủ tục xoá:
        $.ajax({
            type: 'POST',
            url: '/Admin/HLATKH/OnDeletePlan',
            dataType: 'json',
            data: JSON.stringify({ planid: $('#txtPlanId').val() }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response.length > 0) {
                    alert("Xoá thành công!");
                    //Xoá xong reset
                    $('#txtPlanId').val('');
                    //Load lại form
                    LoadDsPlan();
                }
            },
            error: function (ex) {
                alert('Xoá không thành công!');
            }
        })

    }
}

//Hàm load Class By Id
function LoadPlanById(planid) {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadPlanById',
        dataType: 'json',
        data: JSON.stringify({ planid: planid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0) {
                obj = response[0];
                //Fill thông tin vào form;
                LoadDmLDaotaoKH(obj.groupid);
                LoadDsNhomTheoLoaiDtaoKH(obj.groupid, obj.categoryid);
                FillDataEdit();
            }
        },
        error: function (ex) {
            alert('Không lấy được lớp học đã chọn!');
        }
    })
}

function FillDataEdit() {

    if (obj.ngay_bdau != null) {
        var day = obj.ngay_bdau.substr(0, 2);//("0" + ngaybd_kh.getDate()).slice(-2);
        var month = obj.ngay_bdau.substr(3, 2); //("0" + (ngaybd_kh.getMonth() + 1)).slice(-2);
        var year = obj.ngay_bdau.substr(6, 4);
        var bd_kh = (day) + "/" + (month) + "/" + year;
        $('#mdKhBatdau').datepicker({
            dateFormat: 'dd/mm/yy',
        }).val(bd_kh);

    }

    if (obj.ngay_kthuc != null) {
        var day = obj.ngay_kthuc.substr(0, 2);//("0" + ngaybd_kh.getDate()).slice(-2);
        var month = obj.ngay_kthuc.substr(3, 2); //("0" + (ngaybd_kh.getMonth() + 1)).slice(-2);
        var year = obj.ngay_kthuc.substr(6, 4);
        var kt_kh = (day) + "/" + (month) + "/" + year;

        $('#mdKhKthuc').datepicker({
            dateFormat: 'dd/mm/yy',
        }).val(kt_kh);

    }

    //$('#mdDonvi').value = obj.;
    $('#mdDonviDtao').val(obj.dvdtao);
    $('#mdLoaiDtao').val(obj.groupid);
    $('#mdNhomhl').val(obj.categoryid);
    $('#mdKhoach').val(obj.plandesc);
}

//Hàm reset form nhập kế hoạch
function clearFromAddPlan() {
    $('#mdDonviDtao').val('');
    $('#mdKhoach').val('');
    $('#mdLoaiDtao').val('');
    $('#mdNhomhl').val('');
    $('#mdKhBatdau').val('');
    $('#mdKhKthuc').val('');    
    //$('#formFileMultiple').val('');
    //$('#tblOrgAdd').value = "";
    $('#tblPerson').DataTable().clear().draw();
    $('#tblPersonSelect').DataTable().clear().draw();
    $('#tblPersonSelect').DataTable().destroy();
    //Load các loại danh mục;
    $('#listFile').css('display', 'none');
}

//Load danh mục Loại đào tạo Thêm mới.
function LoadDmLDaotaoKH(groupdefault) {
    $('#mdLoaiDtao').empty();
    $('#mdLoaiDtao').append($('<option>', {
        value: 'ALL',
        text: '-- Chọn loại đào tạo --'
    }));

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadLhinhDtao',
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
function LoadDsNhomTheoLoaiDtaoKH(groupdefault, nhomdefault) {
    //let typeedu = $('#mdLoaiDtao').val();
    $('#mdNhomhl').empty();
    //if (typeedu == "ALL") {
    $('#mdNhomhl').append($('<option></option>', {
        value: 'ALL',
        text: '-- Chọn nhóm huấn luyện --'
    }));
    //}
    //Load Nhóm đào tạo
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadLNhomDtao',
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

function LoadDsOrgModal() {
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsOrg',
        dataType: 'json',
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
                        }
                    },
                    { 'data': 'slns', visible: false }
                    ],
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

//Ghi dữ liệu Kế hoạch
function OnSavePlan() {

    var formData = new FormData();
    var totalFiles = document.getElementById("f1").files.length;

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

    ////String date ngày ký
    //let sngayky = "";
    //for (var i = 1; i < counter; i++) {
    //    sngayky = sngayky + $('#n' + i).val() + ";";
    //}

    //formData.append("ngaykys", sngayky);
    formData.append("dvdaotao", $('#mdDonviDtao').val());
    /*formData.append("loailop", $('#mdLoaiLop').val());*/
    formData.append("mota", $('#mdKhoach').val());
    formData.append("loaidaotao", $('#mdLoaiDtao').val());
    formData.append("nhomhluyen", $('#mdNhomhl').val());
    formData.append("khbatdau", $('#mdKhBatdau').val());
    formData.append("khketthuc", $('#mdKhKthuc').val());    
    formData.append("planid", $('#txtPlanId').val());

    //Danh sách nhân sự
    var table = $('#tblPersonSelect').DataTable();
    var data = table.rows().data();
    var dsnhansu = "";
    var ttnut = $('#ttNut').val();
    data.each(function (value, index) {
        //let res = $.isEmptyObject(value);
        if (ttnut == "EDIT"){
                dsnhansu = dsnhansu + ";" + value["nsid"].toString();
        } else {
            dsnhansu = dsnhansu + ";" + value[1].toString();
        //} else {
        //    dsnhansu = dsnhansu + ";" + value["nsid"].toString();
        }

    })

    formData.append("lstnhansu", dsnhansu);

    $.ajax({
        type: 'post',
        url: '/Admin/HLATKH/OnSavePlan',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //load lại form
            LoadDsPlan();
            alert('Thêm mới kế hoạch thành công!');
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
}

//Hàm lấy danh sách nhân sự theo đơn vị form Add
function LoadDsNhansuKHAddByOrg() {
    //Lấy đơn vị chọn
    var t = $('#tblOrgAdd').DataTable();
    var dvql = t.row('.selected').data()['madvql'];

    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/LoadDsNhansuByOrg',
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
                    "columns": [{ 'data': 'nsid', visible: false, orderable: false },
                    { 'data': 'orgid', visible: false, orderable: false },
                    { 'data': 'tenkhaisinh' },
                    { 'data': 'chucdanh' },
                    { 'data': 'kqua' },
                    ],
                    "columnDefs": [{
                        'targets': 5,
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

///Hiển thị file
function viewFileByPlan(fileid, filename) {
    //Gọi thủ tục xem file:
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/OnViewFileById',
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
///Xoá file
function delFileByPlan(fileid) {
    //Gọi thủ tục xoá:
    $.ajax({
        type: 'POST',
        url: '/Admin/HLATKH/OnDeleteFileByClass',
        dataType: 'json',
        data: JSON.stringify({ fileid: fileid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (response.length > 0 && response === 'OK') {
                //Load lại danh sách
                LoadLstFileByPlan();
                alert("Xoá thành công!");
                //Xoá xong reset                   
            }
        },
        error: function (ex) {
            alert('Xoá không thành công!');
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