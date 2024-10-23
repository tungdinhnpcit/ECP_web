var tbController = function () {

    var cachedObj = {
        mahieu: [],
        nhasx: [],
        donvitinh: []
    }

    this.initialize = function () {

        $.when(loadMaHieu(),
            loadNhaSX(),
            loadDVT())
            .done(function () {

                $('html').addClass('sidebar-left-collapsed');
                loadData();
            });
        registerEvents();
    };

    
    function loadMaHieu() {
        $.ajax({
            type: 'GET',
            url: '/Admin/sc_ThietBiSuCo/GetAllMaHieu',
            dataType: 'json',
            async: false,
            success: function (response) {
                cachedObj.mahieu = response;
                var render = "<option value=''>-- Chọn mã hiệu--</option>";



                $.each(response, function (i, item) {
                    if (item.CapCha == null) {
                        render += "<option value='" + item.Id + "'>" + item.Ten + "</option>"

                        $.each(response, function (i2, item2) {
                            if (item2.CapCha == item.Id) {
                                render += "<option value='" + item2.Id + "'>" + "------  " + item2.Ten + "</option>"

                                $.each(response, function (i3, item3) {
                                    if (item3.CapCha == item2.Id) {
                                        render += "<option value='" + item3.Id + "'>" + "--------- " + item3.Ten + "</option>"


                                    }
                                });
                            }
                        });
                    }
                });
                //$('#ddlCategorySearch').html(render);
                $('#ddlMaHieu').html(render);
                $('#ddlMaHieu').chosen({ width: "100%" });
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải dữ liệu!', 'error');
            }
        });
    }
    
    function loadNhaSX() {
        $.ajax({
            type: 'GET',
            url: '/Admin/sc_ThietBiSuCo/GetAllNhaSX',
            dataType: 'json',
            async: false,
            success: function (response) {
                cachedObj.nhasx = response;
                var render = "<option value=''>-- Chọn nhà sản xuất--</option>";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.Id + "'>" + item.Ten + "</option>"
                });
                //$('#ddlCategorySearch').html(render);
                $('#ddlNhaSX').html(render);
                $('#ddlNhaSX').chosen({ width: "100%" });
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải dữ liệu!', 'error');
            }
        });
    }
    
    function loadDVT() {
        $.ajax({
            type: 'GET',
            url: '/Admin/sc_ThietBiSuCo/GetAllDonViTinh',
            dataType: 'json',
            async: false,
            success: function (response) {
                cachedObj.donvitinh = response;
                var render = "<option value=''>-- Chọn nhà sản xuất--</option>";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.Id + "'>" + item.Ten + "</option>"
                });
                //$('#ddlCategorySearch').html(render);
                $('#ddlDonViTinh').html(render);
                $('#ddlDonViTinh').chosen({ width: "100%" });
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải dữ liệu!', 'error');
            }
        });
    }

    function registerEvents() {

        $("#txtNgaySX").kendoDatePicker({
            start: "decade",
            depth: "decade",
            format: "yyyy",
            value: new Date()
        });
        
        $("#txtNgayDongDien").kendoDatePicker({
            start: "decade",
            depth: "decade",
            format: "yyyy",
            value: new Date()
        });

        //Init validation
        $('#frmMaintainance').validate({
            ignore: [],
            lang: 'vi',
            rules: {
               
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
                url: "/Admin/sc_ThietBiSuCo/GetById",
                data: { id: that },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#ddlMaHieu').val(data.MaHieuId).trigger("chosen:updated");
                    $('#ddlNhaSX').val(data.NhaSanXuatId).trigger("chosen:updated");
                    $('#txtSoHD').val(data.SoHD);
                    $('#txtBienban').val(data.BienBanNT);
                    $('#txtDVTC').val(data.TenDonViThiCong);
                    $('#txtNgaySX').val(data.NgaySanXuat != null ? javi.convertYearJS(data.NgaySanXuat) : "");
                    $('#txtLichSu').val(data.LichSuVanHanh);
                    $('#txtNgayDongDien').val(data.NgayDongDien != null ? javi.convertYearJS(data.NgayDongDien) : "");
                    $('#txtMoiTruong').val(data.MoiTruongVH);
                    $('#txtSoLuong').val(data.SoLuong);
                    $('#ddlDonViTinh').val(data.DonViTinhId).trigger("chosen:updated");
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
                    url: "/Admin/sc_ThietBiSuCo/DeleteEntity",
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
                var scId = $('#hidSuCoId').val();
                var id = $('#hidIdM').val();
                var mahieu = $('#ddlMaHieu').val();
                var nhasx = $('#ddlNhaSX').val();
                var sohd = $('#txtSoHD').val();
                var bienban = $('#txtBienban').val();
                var dvtc = $('#txtDVTC').val();
                if ($('#txtNgaySX').val() != null) 
                    var ngaysx = "01/01/" +  $('#txtNgaySX').val();
                var lichsu = $('#txtLichSu').val();
                if ($('#txtNgayDongDien').val() != null)
                    var ngaydd = "01/01/" + $('#txtNgayDongDien').val();
                var moitruong = $('#txtMoiTruong').val();
                var soluong = $('#txtSoLuong').val();
                var dvt = $('#ddlDonViTinh').val();

                if (mahieu == "" || mahieu == undefined || mahieu == null) {
                    javi.notify("Chưa chọn mã hiệu", 'error');
                    return false;
                }


                $.ajax({
                    type: "POST",
                    url: "/Admin/sc_ThietBiSuCo/SaveEntity",
                    data: {
                        Id: id,
                        SuCoId: scId,
                        MaHieuId: mahieu,
                        NhaSanXuatId: nhasx,
                        SoHD: sohd,
                        BienBanNT: bienban,
                        TenDonViThiCong: dvtc,
                        NgaySanXuat: ngaysx,
                        LichSuVanHanh: lichsu,
                        NgayDongDien: ngaydd,
                        MoiTruongVH: moitruong,
                        SoLuong: soluong,
                        DonViTinhId: dvt
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
        $('#frmMaintainance input').val('');
        $('#frmMaintainance select').val('').trigger("chosen:updated");
        $('#hidIdM').val(0);

    }


    function getTenMaHieu(id) {
        var st = $.grep(cachedObj.mahieu, function (element, index) {
            return element.Id == id;
        });
        if (st.length > 0)
            return st[0].Ten;
        else return '';
    }
    
    function getTenDVT(id) {
        var st = $.grep(cachedObj.donvitinh, function (element, index) {
            return element.Id == id;
        });
        if (st.length > 0)
            return st[0].Ten;
        else return '';
    }
    
    function getTenNhaSX(id) {
        var st = $.grep(cachedObj.nhasx, function (element, index) {
            return element.Id == id;
        });
        if (st.length > 0)
            return st[0].Ten;
        else return '';
    }


    //load dữ liệu
    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                scId: $('#hidSuCoId').val(),
                keyword: $('#txtKeyword').val(),
                page: javi.configs.pageIndex,
                pageSize: javi.configs.pageSize
            },
            url: '/admin/sc_ThietBiSuCo/GetAllPaging',
            dataType: 'json',
            success: function (response) {
                if (response.RowCount > 0) {
                    var d = 1;
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            STT: d,
                            Id: item.Id,
                            MaHieu: getTenMaHieu(item.MaHieuId),
                            NhaSX: getTenMaHieu(item.NhaSanXuatId),
                            SoHD: item.SoHD,
                            BienBan: item.BienBanNT,
                            DVTC: item.TenDonViThiCong,
                            NgaySX: item.NgaySanXuat != null ? javi.convertYearJS(item.NgaySanXuat) : "",
                            LichSu: item.LichSuVanHanh,
                            NgayDD: item.NgayDongDien != null ? javi.convertYearJS(item.NgayDongDien) : "",
                            MoiTruong: item.MoiTruongVH,
                            SoLuong: item.SoLuong,
                            DVT: getTenMaHieu(item.DonViTinhId),
                        });
                        d++;
                    });

                    if (render !== '') {
                        $('#tbl-content').html(render);
                    }
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="13">Không có dữ liệu</td></tr>');
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