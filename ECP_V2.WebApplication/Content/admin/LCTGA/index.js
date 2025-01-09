var ndController = function () {

    var formatter = new Intl.NumberFormat('vi-VN', {
    });

    $('body').on('click', '#btnlayDL', function () {
        loadData();
    })



    function registerEvents() {

        $('body').on('click', '#checkall', function () {
            if ($(this).prop("checked") === true) {
                $.each($('#tbl-content tr'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', 'checked')
                    $(item).find('.ckChuyenDV:enabled').first().prop('checked', 'checked')
                });
            }

            else {
                $.each($('#tbl-content tr'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', '')
                    $(item).find('.ckChuyenDV:enabled').first().prop('checked', '')
                });
            }
        });


        $('#ddlTrangThai').on('change', function () {
            loadData();
        })

        $('#ddlDonVi').on('change', function () {
            loadData();
        })


        $('#GioBd').on('change', function () {
            loadData();
        })

        $('#GioKt').on('change', function () {
            loadData();
        })








        $('body').on('click', '.nd-donvi-collapse', function () {
            $(this).parent().toggleClass('expand').nextUntil('tr.header-collapse').slideToggle(100);
        })


        $('body').on('click', '.btn-view-history', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/bcbs_NoiDung/HistoryById",
                data: { id: that },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response.status) {

                        var template = $('#template-history').html();
                        var render = "";

                        $.each(response.data, function (i, item) {
                            render += Mustache.render(template, {
                                Id: item.Id,
                                NoiDung: item.NoiDung,
                                PhamVi: item.PhamVi,
                                KhoiLuong: item.KhoiLuongVTTB,
                                GiaTri: formatter.format(item.TongGiaTri),
                                HoanThanh: javi.convertDateJS(item.NgayHoanThanh),
                                NgaySua: javi.convertDateJS2(item.NgaySua),
                                NguoiSua: item.NguoiSua
                            });
                        });

                        $('#tbody-history').html(render);

                        $('#modal-history').modal('show');
                    }
                    else {
                        javi.notify(response.message, "error");
                    }


                },
                error: function (status) {
                    javi.notify('Có lỗi xảy ra', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });
        })

    }

    var dataTinhTrang = [
        {
            Id: 0,
            MoTa: 'Khởi tạo'
        },
        {
            Id: 1,
            MoTa: 'Nhận lênh'
        },
        {
            Id: 2,
            MoTa: 'Hoàn Thành'
        },
        {
            Id: 2,
            MoTa: 'Không nhận lệnh'
        },
    ]
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
        var template = $('#table-template').html();

        $.ajax({
            type: 'GET',
            url: '/admin/LCTGA/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                fromDate: $('#GioBd').val(),
                toDate: $('#GioKt').val(),
                status: $('#drlTrangThaiLCT').val(),
            },
            dataType: 'json',
            success: function (response) {

                $('#checkall').prop('checked', false);

                var header_template = $('#template-ds-header').html();
                var footer_template = $('#template-ds-footer').html();
                var data_template = $('#template-nd').html();
                var render = "";


                var data = response.data;
                var sum = 0;

                if (data.length > 0) {
                    var d = 1;
                    $.each(data, function (i, item) {
                        render += Mustache.render(template, {
                            STT: d,
                            Id: item.Id,
                            DViCPhieu: item.DViCPhieu,
                            SoPhieu: item.SoPhieu,
                            TenNguoiNhap: item.TenNguoiNhap,
                            TenNhanLenh: item.TenNhanLenh,
                            TrangThai: dataTinhTrang.find(x => x.Id == item.TrangThai)?.MoTa,
                            NgayNhap: item.NgayNhap,
                            NgayNhanLenh: item.NgayNhanLenh,
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



                $('#lblTong-string').html("Tổng: " + data.length + " LCT");


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


    $('body').on('click', '.btn-view-chitiet', function (e) {
        e.preventDefault();
        var that = $(this).data('id');
        $.ajax({
            type: "GET",
            url: "/Admin/LCTGA/GetChiTietLCT",
            data: { LCTGhiAmId: that },
            dataType: "json",
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                if (response.data.Id > 0) {

                    var template = $('#template-history').html();
                    var render = "";
                    var item = response.data;

                    render += Mustache.render(template, {
                        Id: item.Id,
                        NoiDung: item.NoiDung,
                        PhamVi: item.PhamVi,
                        KhoiLuong: item.KhoiLuongVTTB,
                        GiaTri: formatter.format(item.TongGiaTri),
                        HoanThanh: javi.convertDateJS(item.NgayHoanThanh),
                        NgaySua: javi.convertDateJS2(item.NgaySua),
                        NguoiSua: item.NguoiSua
                    });


                    $('#tbody-history').html(render);

                    $('#modal-history').modal('show');
                }
                else {
                    javi.notify(response.message, "error");
                }


            },
            error: function (status) {
                javi.notify('Có lỗi xảy ra', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    })
}