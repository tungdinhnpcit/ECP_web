var dmController = function () {

    var cachedObj = {
        parents: []
    }

    this.initialize = function () {

        $.when(loadParents())
            .done(function () {

                loadData();
            });
        registerEvents();
    };

    function loadParents() {
        $.ajax({
            type: 'GET',
            url: '/Admin/atvs_DanhMuc/GetAllParent',
            dataType: 'json',
            async: false,
            success: function (response) {
                cachedObj.parents = response;
                var render = "<option value=''>-- Chọn cấp cha--</option>";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.Id + "'>" + (item.Type == 1 ? "Kế hoạch - " : "Kết quả - ") + item.Ten + "</option>"
                });
                //$('#ddlCategorySearch').html(render);
                $('#ddlParents').html(render);
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải dữ liệu!', 'error');
            }
        });
    }

    function registerEvents() {

        //Init validation
        $('#frmMaintainance').validate({
            ignore: [],
            lang: 'vi',
            rules: {
                txtTen: {
                    required: true,
                    maxlength: 500
                },
                txtDonViTinh: {
                    maxlength: 50
                },
                txtThuTu: {
                    required: true,
                    minlength: 0
                }
            }
        });

        $('#ddlTypeFilter').on('change', function () {
            loadData(true);
        });


        //todo: binding events to controls
        $('#ddlShowPage').on('change', function () {
            javi.configs.pageSize = $(this).val();
            javi.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btnSearch').on('click', function () {
            loadData(true);
        });

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which === 13) {
                loadData(true);
            }
        });

        $("#btnCreate").on('click', function () {
            resetFormMaintainance();
            $('#modal-add-edit').modal('show');

        });


        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/atvs_DanhMuc/GetById",
                data: { id: that },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#txtTen').val(data.Ten);
                    $('#txtDonViTinh').val(data.DonViTinh);
                    $('#txtThuTu').val(data.ThuTu);
                    $('#txtType').val(data.Type);
                    $('#ddlParents').val(data.CapCha);
                    $('#ckIsRequỉed').prop('checked', data.IsRequired);
                    $('#modal-add-edit').modal('show');

                },
                error: function (status) {
                    javi.notify('Có lỗi xảy ra', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            javi.confirm('Bạn có chắc chắn muốn xóa không?', function () {
                $.ajax({
                    type: "POST",
                    url: "/Admin/atvs_DanhMuc/Delete",
                    data: { id: that },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        javi.notify('Xóa thành công.', 'success');
                        loadData(true);
                    },
                    error: function (status) {
                        javi.notify('Có lỗi xảy ra trong quá trình xóa.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            });
        });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidIdM').val();
                var ten = $('#txtTen').val().trim();
                var donvitinh = $('#txtDonViTinh').val().trim();
                var thutu = $('#txtThuTu').val();
                var parent = $('#ddlParents').val();
                var type = $('#txtType').val();
                var isRequired = $('#ckIsRequỉed').prop('checked');

                if (type == "" || type == undefined || type == null) {
                    javi.notify("Chưa chọn loại danh mục", 'error');
                    return false;
                }

                if (isRequired == true && (donvitinh == null || donvitinh == "" || donvitinh == undefined)) {
                    javi.notify("Chưa nhập đơn vị tính", 'error');
                    return false;
                }

                $.ajax({
                    type: "POST",
                    url: "/Admin/atvs_DanhMuc/SaveEntity",
                    data: {
                        Id: id,
                        ten: ten,
                        donvitinh: donvitinh,
                        ThuTu: thutu,
                        CapCha: parent,
                        IsRequired: isRequired,
                        Type: type
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status) {
                            javi.notify(response.message, 'success');
                            $('#modal-add-edit').modal('hide');
                            resetFormMaintainance();

                            loadData(true);
                            loadParents();
                        }
                        else {
                            javi.notify(response.message, "error");
                        }
                    },
                    error: function (err) {
                        javi.notify('Đã xảy ra lỗi khi cập nhật.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });

                return false;
            }
        });
    }


    //reset form
    function resetFormMaintainance() {
        $('#hidIdM').val(0);
        $('#txtTen').val('');
        $('#txtDonViTinh').val('');
        $('#txtThuTu').val('');
        $('#txtType').val(1);
        $('#ddlParents').val('');
        $('#ckIsRequỉed').prop('checked', true);

    }

    //load dữ liệu
    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                type: $('#ddlTypeFilter').val(),
                keyword: $('#txtKeyword').val(),
                page: javi.configs.pageIndex,
                pageSize: javi.configs.pageSize
            },
            url: '/admin/atvs_DanhMuc/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                if (response.RowCount > 0) {
                    var d = 1;
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            STT: d,
                            Id: item.Id,
                            Ten: item.Ten,
                            DonViTinh: item.DonViTinh,
                            ThuTu: item.ThuTu,
                        });
                        d++;
                    });

                    if (render !== '') {
                        $('#tbl-content').html(render);
                    }
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="5">Không có dữ liệu</td></tr>');
                }

                wrapPaging(response.RowCount > 0 ? response.RowCount : 1, function () {
                    loadData();
                }, isPageChanged);

                $('#lblTotalRecords').text(response.RowCount);
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu trang', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }
    // phân trang
    function wrapPaging(recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / javi.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            initiateStartPageClick: false,
            totalPages: totalsize,
            visiblePages: 7,
            first: 'Đầu',
            prev: 'Trước',
            next: 'Tiếp',
            last: 'Cuối',
            onPageClick: function (event, p) {
                javi.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    }
}