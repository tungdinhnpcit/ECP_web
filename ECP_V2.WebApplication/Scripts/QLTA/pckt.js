//Hàm load cbo loai kiem tra
function loadCboLoaikt(typeid) {
    //type: tba or dz
    $.ajax({
        type: 'POST',
        url: '/Admin/Plan/getLoaiKtra',
        dataType: 'json',
        data: JSON.stringify({ typeid: typeid }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    if (value["typeid"] == 'DUONGDAY') {
                        $('#cbLoaiDZ')
                            .append($('<option></option>', { text: value["ten_loaiktr"] })
                                .attr('value', value["id_loaiktr"]));
                    } else {
                        $('#cbLoaiTBA')
                            .append($('<option></option>', { text: value["ten_loaiktr"] })
                                .attr('value', value["id_loaiktr"]));
                    }
                });
            }

        },
        error: function (ex) {
            alert('Không thể load danh mục Loại kiểm tra TBA hoặc ĐZ');
        }
    })
}

function loadDsDoi() {
    let pdonvi = $('#cbDvvh').val();
    $.ajax({
        type: 'POST',
        url: '/Admin/Plan/getDmDoi',
        data: JSON.stringify({
            vdonvi: pdonvi
        }),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $('#cbDoi').empty();
                $.each(response, function (i, value) {
                    $('#cbDoi')
                        .append($('<option></option>', { text: value["TenPhongBan"] })
                            .attr('value', value["Id"]));
                });

                $('#cbDoi option')[0].selected = true;
                LoadDsPhancong();
            }
        },
        error: function (ex) {
            alert('Không thể load danh mục Đội kiểm tra đường dây');
        }
    })
}

function LoadDmDonVi() {
    $.ajax({
        type: 'POST',
        url: '/Admin/ApprovePlan/GetDmDonvi',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cbDvvh')
                        .append($('<option></option>', { text: value["TenDonVi"] })
                            .attr('value', value["Id"]));
                });

                $('#cbDvvh option')[0].selected = true;
                //Load Dsach đội thuộc Đơn vị
                loadDsDoi();
            }


        },
        error: function (ex) {
            alert('Không thể load danh mục Đội vận hành');
        }
    })
}
//Load ds phân công
function LoadDsPhancong() {
    let doiql = $('#cbDoi').val();

    $.ajax({
        type: 'POST',
        url: '/Admin/Plan/getDsPhancong',
        dataType: 'json',
        data: JSON.stringify({ doiql: doiql }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            $('#tblPc').dataTable().fnClearTable();
            if (response.length > 0) {
                var table = $('#tblPc').DataTable({
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
                    "columns": [{ 'data': 'assetdesc' },
                    { 'data': 'ngaythmax' },
                    { 'data': 'tenloaikt' },
                    { 'data': 'typeid', 'visible': false },
                    ],
                    "columnDefs": [{
                        'targets': 4,
                        'searchable': false,
                        'orderable': false,
                        'className': 'td-body-center',
                        'render': function (data, type, full, meta) {
                            return '<input type="checkbox" name="id[]" value="' + full.assetid + '">';
                        }
                    }],
                    'order': [[1, 'asc']]
                });
            }

        },
        error: function (ex) {
            alert('Không thể load danh mục Loại kiểm tra TBA hoặc ĐZ');
        }
    })
}