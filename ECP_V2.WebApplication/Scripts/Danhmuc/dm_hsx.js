///Hàm load nhóm danh mục
function loadDanhmuc() {
    $.ajax({
        type: 'POST',
        url: '/Dmhsxcam/GetListDmHSX',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#tblDanhmuc').dataTable().fnClearTable();
            if (response.length > 0) {
                $('#tblDanhmuc').DataTable({
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
                    paging: true,
                    paginate: true,
                    info: false,
                    searching: true,
                    select: true,
                    destroy: true,
                    data: response,
                    columns: [
                        { data: 'madm' },
                        { data: 'tendm', orderable: false },
                        { data: 'trangthai', orderable: false }
                    ],
                    "columnDefs": [{
                        'targets': 3,
                        'searchable': false,
                        'orderable': false,
                        'className': 'cell-center',
                        'render': function (data, type, full, meta) {
                            return '<input type="checkbox" name="id[]" value="' + full.madm + '">';
                        }
                    }],
                });
            }

        },
        error: function (ex) {
            alert('Không thể lấy được danh sách Nhóm danh mục.');
        }
    })
}

//hàm thêm danh mục
function onSave() {
    var formData = new FormData();
    formData.append("madmuc", $('#madmuc').val());
    formData.append("tendmuc", $('#tendmuc').val());//Lý thuyết hoặc thực hành
    formData.append("mahd", $('#mahd').val());
    formData.append("trangthai", $('#trangthai').is(':checked') == true ? 1 : 0);

    $.ajax({
        type: 'post',
        url: '/Dmhsxcam/OnSave',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            //Load lại form
            loadDanhmuc();
            alert('Thêm danh mục thành công.');
        },
        error: function (error) {
            alert('Có lỗi xảy ra trong quá trình thêm mới! Vui lòng xem lại thông tin nhập vào.' + error);
        }
    });
}

//Clear form input
function clearInfor() {
    $('#madmuc').val('');
    $('#tendmuc').val('');
    $('#trangthai').val('0');
    $('#mahd').val('');
}

function cmdAddFrm() {
    $('#mdAdd').modal('show');
}

//Hàm sửa danh mục
function cmdEdit() {
    var tblDm = $('#tblDanhmuc').DataTable();
    var data = tblDm.rows().data();
    let icount = 0;
    tblDm.$('input[type="checkbox"]').each(function () {
        if (this.checked) {
            icount++;
        }
    });

    if (icount > 1) {
        alert('Bạn chỉ được chọn 1 để sửa.');
        return;
    }

    if (icount == 0) {
        alert('Bạn chưa chọn?');
        return;
    }

    tblDm.$('input[type="checkbox"]').each(function () {
        // If checkbox is checked
        if (this.checked) {
            var valCheck = $(this).val();

            data.each(function (value, index) {
                if (valCheck === value.madm.toString()) {
                    //Fill value to modal edit
                    $('#madmuc').val(value.madm);
                    $('#tendmuc').val(value.tendm);
                    $('#mahd').val(value.madm);
                    if (value.trangthai == true) {
                        $("#trangthai").prop("checked", true);
                    } else {
                        $("#trangthai").prop("checked", false);
                    }
                }
            });
        }
    });

    $('#mdAdd').modal('show');
}

function cmdDelete() {
    var tblDm = $('#tblDanhmuc').DataTable();

    tblDm.$('input[type="checkbox"]').each(function () {
        // If checkbox is checked
        if (this.checked) {
            var valCheck = $(this).val();

            //Gọi hàm delete luôn
            var formData = new FormData();
            formData.append("madmuc", valCheck);

            $.ajax({
                type: 'post',
                url: '/Dmhsxcam/OnDelete',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    //Load lại form
                    loadDanhmuc();
                    alert('Xóa danh mục thành công.');
                },
                error: function (error) {
                    alert('Có lỗi xảy ra trong quá trình xóa. Vui lòng liên hệ quản trị!' + error);
                }
            });

        }
    });
}