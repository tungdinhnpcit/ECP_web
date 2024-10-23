var khController = function () {
    var cachedObj = {
        donvi: []
    }

    this.initialize = function () {
        $.when(loadData())
            .done(function () {
                $('html').addClass('sidebar-left-collapsed');
                registerEvents();
            });
    };

    function registerEvents() {


        $('#ddlDonVi').on('change', function () {
            loadData();
        })


        $('#ddlYear').on('change', function () {
            loadData();
        })


        $('body').on('click', '.btn-save-hl', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var danhmucId = $(tr).data('dmid');
            var soluong = $(tr).find('.txt-hl-soluong').first().val();
            var ghichu = $(tr).find('.txt-hl-ghichu').first().val();

            var donviId = $('#ddlDonVi').val();
            var nam = $('#ddlYear').val();

            if (soluong == null || soluong == undefined || soluong == "") {
                javi.notify("Chưa nhập số lượng", 'error');
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/atvs_KeHoach/SaveNoiDung",
                data: {
                    Id: id,
                    DonViId: donviId,
                    Nam: nam,
                    DanhMucId: danhmucId,
                    SoLuong: soluong,
                    GhiChu: ghichu
                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response.status) {
                        javi.notify(response.message, 'success');

                        loadData();
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

        });

        $('body').on('click', '.btn-edit-hl', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var id = $(tr).data('id');
            var dmId = $(tr).data('dmid');
            var stt = $(tr).find('.hl-stt').html();
            var ten = $(tr).find('.hl-ten').html();
            var dvt = $(tr).find('.hl-donvitinh').html();
            var sl = $(tr).find('.hl-soluong').html();
            var gc = $(tr).find('.hl-ghichu').html();

            var data_template = $('#template-edit-hl').html();
            var render = Mustache.render(data_template, {
                Id: id,
                DMID: dmId,
                HLSTT: stt,
                HLTEN: ten,
                HLDVT: dvt,
                HLSL: sl,
                HLGC: gc,
            });

            $(tr).after(render);
            $(tr).remove();
        })

        $('body').on('click', '.btn-cancel-hl', function (e) {
            e.preventDefault();

            loadData();

        })

        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/atvs_KeHoach/ChuyenNPC",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        year: $('#ddlYear').val()
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            loadData();
                        }
                        else {
                            javi.notify(response.message, "error");
                        }
                    },
                    error: function (err) {
                        javi.notify('Có lỗi xảy ra.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            }
        })

        $('body').on('click', '#btnExport', function (e) {
            e.preventDefault();

            $.UnifiedExportFile(
                {
                    action: "/Admin/atvs_KeHoach/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        year: $('#ddlYear').val()
                    },
                    downloadType: 'Progress',
                    ajaxLoadingSelector: '#loading'
                });
        })

    }


    //load dữ liệu
    function loadData(isPageChanged) {

        $.ajax({
            type: 'GET',
            url: '/admin/atvs_KeHoach/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                year: $('#ddlYear').val()
            },
            dataType: 'json',
            success: function (response) {

                var template = $('#template-kh').html();
                var render = "";

                var data = response.data;

                var d = 1;

                if (data.length > 0) {
                    $.each(data, function (i, item) {

                        if (item.CapCha == null) {

                            if (item.IsRequired) {
                                render += Mustache.render(template, {
                                    Id: item.Id,
                                    DMID: item.DanhMucId,
                                    HLSTT: d++,
                                    HLTEN: item.TenDanhMuc,
                                    HLDVT: item.DonViTinh,
                                    HLSL: item.SoLuong,
                                    HLGC: item.GhiChu,
                                    Style: "font-weight:600",
                                    IsNPC: item.IsChuyenNPC ? 'nd-isNPC' : '',
                                    hidden: item.IsChuyenNPC ? 'display:none' : ''
                                });
                            }
                            else {
                                render += Mustache.render(template, {
                                    Id: item.Id,
                                    DMID: item.DanhMucId,
                                    HLSTT: d++,
                                    HLTEN: item.TenDanhMuc,
                                    Style: "font-weight:600",
                                    IsNPC: item.IsChuyenNPC ? 'nd-isNPC' : '',
                                    hidden: 'display:none'
                                });
                            }
                            

                            let tempData = jQuery.grep(data, function (item2, i2) {
                                return item2.CapCha == item.DanhMucId;
                            })

                            $.each(tempData, function (i3, item3) {

                                render += Mustache.render(template, {
                                    Id: item3.Id,
                                    DMID: item3.DanhMucId,
                                    HLSTT: "",
                                    HLTEN: item3.TenDanhMuc,
                                    HLDVT: item3.DonViTinh,
                                    HLSL: item3.SoLuong,
                                    HLGC: item3.GhiChu,
                                    IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                                    hidden: item3.IsChuyenNPC ? 'display:none' : ''
                                });

                            });
                        }

                    });

                    $('#tbl-content').html(render);

                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="6">Không có dữ liệu</td></tr>');
                }
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

}