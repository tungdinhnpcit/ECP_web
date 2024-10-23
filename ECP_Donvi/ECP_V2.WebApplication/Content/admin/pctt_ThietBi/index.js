var tbController = function () {
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

        $('#btnSearch').on('click', function () {
            loadData();
        });

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which === 13) {
                loadData();
            }
        });


        $('body').on('click', '#btn-add-tb', function (e) {
            e.preventDefault();
            var template = $('#template-add-tb').html();

            var last = $('.pctt').last().find('.tb-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                Ten: "",
                DonViTinh: "",
                SoLuong: "",
                NoiDe: "",
                GhiChu: "",
            });

            if (stt > 1) {
                $('.pctt').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

        });

        $('body').on('click', '.btn-save-tb', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var type = $('#hidType').val();
            var ten = $(tr).find('.txt-tb-tentb').first().val();
            var donvitinh = $(tr).find('.txt-tb-donvitinh').first().val();
            var soluong = $(tr).find('.txt-tb-soluong').first().val();
            var noide = $(tr).find('.txt-tb-noide').first().val();
            var ghichu = $(tr).find('.txt-tb-ghichu').first().val();

            if (ten == "") {
                javi.notify("Chưa nhập tên thiết bị", "error");
                return false;
            }

            if (soluong != "" && (soluong < 0)) {
                javi.notify("Số lượng không hợp lệ", "error");
                return false;
            }


            $.ajax({
                type: "POST",
                url: "/Admin/pctt_ThietBi/SaveEntity",
                data: {
                    Id: id,
                    Nam: $('#ddlYear').val(),
                    DonViId: $('#ddlDonVi').val(),
                    Type: type,
                    Ten: ten,
                    DonViTinh: donvitinh,
                    SoLuong: soluong,
                    NoiDe: noide,
                    GhiChu: ghichu,
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

        $('body').on('click', '.btn-delete-add-tb', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-tb', function (e) {
            e.preventDefault();

            var id = $(this).data('id');

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/pctt_ThietBi/DeleteEntity",
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

        $('body').on('click', '.btn-edit-tb', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.tb-stt').html();
            var id = $(tr).data('id');
            var ten = $(tr).find('.tb-tentb').first().html();
            var donvitinh = $(tr).find('.tb-donvitinh').first().html();
            var soluong = $(tr).find('.tb-soluong').first().html();
            var noide = $(tr).find('.tb-noide').first().html();
            var ghichu = $(tr).find('.tb-ghichu').first().html();

            var data_template = $('#template-edit-tb').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                Ten: ten,
                DonViTinh: donvitinh,
                SoLuong: soluong,
                NoiDe: noide,
                GhiChu: ghichu,
            });

            $(tr).after(render);
            $(tr).remove();

        })

        $('body').on('click', '.btn-cancel-tb', function (e) {
            e.preventDefault();

            loadData();

        })

        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/pctt_ThietBi/ChuyenNPC",
                    data: {
                        year: $('#ddlYear').val(),
                        donviId: $('#ddlDonVi').val()
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
                    action: "/Admin/pctt_ThietBi/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        nam: $('#ddlYear').val(),
                        type: $('#hidType').val(),
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
            url: '/admin/pctt_ThietBi/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                nam: $('#ddlYear').val(),
                type: $('#hidType').val(),
                keyword: $('#txtKeyword').val()
            },
            dataType: 'json',
            success: function (response) {

                var data_template = $('#template-tb').html();
                var render = "";

                var data = response.data;

                if (data.length > 0) {
                    var d = 1;
                    $.each(data, function (i3, item3) {

                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item3.Id,
                            Ten: item3.Ten,
                            DonViTinh: item3.DonViTinh,
                            SoLuong: item3.SoLuong,
                            NoiDe: item3.NoiDe,
                            GhiChu: item3.GhiChu,
                            IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                            hidden: item3.IsChuyenNPC ? 'display: none' : '',
                        });
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="7">Không có dữ liệu</td></tr>');
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