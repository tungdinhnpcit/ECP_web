var dxkController = function () {
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

        $('#ddlYear').on('change', function () {
            loadData();
        })

        $('#btnSearch').on('click', function () {
            loadData();
        });

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which === 13) {
                loadData();
            }
        });

        $('body').on('click', '#btn-add-dxk', function (e) {
            e.preventDefault();
            var template = $('#template-add-dxk').html();

            var last = $('.pctt').last().find('.dxk-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                HoTen: "",
                DonViId: "",
                DonVi: "",
                ChucDanh: "",
                SoDienThoai: "",
            });

            if (stt > 1) {
                $('.pctt').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

        });

        $('body').on('click', '.btn-save-dxk', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.dxk-donvi').first().data('dvid');
            var donvi = $(tr).find('.txt-dxk-donvi').first().val();
            var hoten = $(tr).find('.txt-dxk-hoten').first().val();
            var chucdanh = $(tr).find('.txt-dxk-chucdanh').first().val();
            var sdt = $(tr).find('.txt-dxk-sdt').first().val();

            if (hoten == "") {
                javi.notify("Chưa nhập họ tên", "error");
                return false;
            }

            if (sdt != "" && (sdt.length < 10 || sdt.length > 11)) {
                javi.notify("Số điện thoại không hợp lệ", "error");
                return false;
            }


            $.ajax({
                type: "POST",
                url: "/Admin/pctt_DoiXungKich/SaveEntity",
                data: {
                    Id: id,
                    DonViId: donviId,
                    TenDonVi: donvi,
                    Nam: $('#ddlYear').val(),
                    HoTen: hoten,
                    ChucDanh: chucdanh,
                    SoDienThoai: sdt,
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

        $('body').on('click', '.btn-delete-add-dxk', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-dxk', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/pctt_DoiXungKich/DeleteEntity",
                        data: {
                            Id: id
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
                            javi.notify('Đã xảy ra lỗi khi xóa.', 'error');
                        },
                        complete: function () {
                            javi.stopLoading();
                        }
                    });
                }
                else {
                    javi.notify("Không tìm thấy bản ghi", "error");
                }
            }

        });

        $('body').on('click', '.btn-edit-dxk', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.dxk-stt').html();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.dxk-donvi').first().data('dvid');
            var donvi = $(tr).find('.dxk-donvi').first().html();
            var hoten = $(tr).find('.dxk-hoten').first().html();
            var chucdanh = $(tr).find('.dxk-chucdanh').first().html();
            var sdt = $(tr).find('.dxk-sdt').first().html();

            var data_template = $('#template-edit-dxk').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                HoTen: hoten,
                DonViId: donviId,
                DonVi: donvi,
                ChucDanh: chucdanh,
                SDT: sdt,
            });

            $(tr).after(render);
            $(tr).remove();

        })

        $('body').on('click', '.btn-cancel-dxk', function (e) {
            e.preventDefault();

            loadData();

        })

        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/pctt_DoiXungKich/ChuyenNPC",
                    data: {
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
                    action: "/Admin/pctt_DoiXungKich/Export",
                    data: {
                        year: $('#ddlYear').val(),
                        keyword: $('#txtKeyword').val()
                    },
                    downloadType: 'Progress',
                    ajaxLoadingSelector: '#loading'
                });
        })
    }

    function downloadURL(url) {
        var hiddenIFrameID = 'hiddenDownloader',
            iframe = document.getElementById(hiddenIFrameID);
        if (iframe === null) {
            iframe = document.createElement('iframe');
            iframe.id = hiddenIFrameID;
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
        }
        iframe.src = url;
    };

    //load dữ liệu
    function loadData(isPageChanged) {

        $.ajax({
            type: 'GET',
            url: '/admin/pctt_DoiXungKich/GetInfo',
            data: {
                nam: $('#ddlYear').val(),
                keyword: $('#txtKeyword').val()
            },
            dataType: 'json',
            success: function (response) {

                var data_template = $('#template-dxk').html();
                var render = "";

                var data = response.data;

                if (data.length > 0) {
                    var d = 1;  
                    $.each(data, function (i3, item3) {

                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item3.Id,
                            HoTen: item3.HoTen,
                            DonViId: item3.DonViId,
                            DonVi: item3.TenDonVi,
                            ChucDanh: item3.ChucDanh,
                            SoDienThoai: item3.SoDienThoai,
                            IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                            hidden: item3.IsChuyenNPC ? 'display: none' : '',
                        });
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