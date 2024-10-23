var nsxController = function () {

    this.initialize = function () {

        $.when()
            .done(function () {

                loadData();
            });
        registerEvents();
    };


    function registerEvents() {

        //Init validation
        $('#frmMaintainance').validate({
            ignore: [],
            lang: 'vi',
            rules: {
                txtNameM: {
                    required: true,
                    maxlength:50
                },
            }
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
                url: "/Admin/NuocSanXuat/GetById",
                data: { id: that },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.ID);
                    $('#txtNameM').val(data.Name);
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
                    url: "/Admin/NuocSanXuat/Delete",
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
                var name = $('#txtNameM').val().trim();

                $.ajax({
                    type: "POST",
                    url: "/Admin/NuocSanXuat/SaveEntity",
                    data: {
                        ID: id,
                        Name: name,
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
        $('#txtNameM').val('');

    }

    //load dữ liệu
    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                keyword: $('#txtKeyword').val(),
                page: javi.configs.pageIndex,
                pageSize: javi.configs.pageSize
            },
            url: '/admin/NuocSanXuat/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                if (response.RowCount > 0) {
                    var d = 1;
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            STT: d,
                            Id: item.ID,
                            Name: item.Name
                        });
                        d++;
                    });

                    if (render !== '') {
                        $('#tbl-content').html(render);
                    }
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="7">Không có dữ liệu</td></tr>');
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