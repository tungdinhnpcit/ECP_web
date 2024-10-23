var tnController = function () {
    var cachedObj = {
        donvi: [],
        capDienAp: [
            {
                Id: 1,
                TenCapDienAp: 110
            },
            {
                Id: 2,
                TenCapDienAp: 35
            },
            {
                Id: 3,
                TenCapDienAp: 22
            },
            {
                Id: 4,
                TenCapDienAp: 10
            },
            {
                Id: 5,
                TenCapDienAp: 6
            }
        ],
        tinhTrang: [
            {
                Id: 0,
                Ten: 'Không xác định'
            },
            {
                Id: 1,
                Ten: 'Nặng'
            },
            {
                Id: 2,
                Ten: 'Nhẹ'
            },
            {
                Id: 3,
                Ten: 'Chết'
            }
        ]
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

        $('#ddlQuarter').on('change', function () {
            loadData();
        })

        $('#ddlYear').on('change', function () {
            loadData();
        })


        $('body').on('click', '#btn-add-hl', function (e) {
            e.preventDefault();
            var template = $('#template-add-hl').html();

            var last = $('.hltn').last().find('.hl-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                DonViId: $('#ddlDonVi :selected').val(),
                DonVi: $('#ddlDonVi :selected').text(),
                ddlCapDienAp: Init_ddl_CapDienAp(),
                SoLuong: "",
                HoTen: "",
                Tuoi: "",
                NgheNghiep: "",
                NgayXayRa: "",
                NguyenNhan: "",
                TinhTrang: Init_ddl_TinhTrang(),
                GhiChu: ""
            });

            if (stt > 1) {
                $('.hltn').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

            $('.txt-hl-ngayxayra-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

        });

        $('body').on('click', '.btn-save-hl', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.hl-donvi').first().data('dvid');
            var cda = $(tr).find('.ddlCapDienAp').first().val();
            var soluong = $(tr).find('.txt-hl-soluong').first().val();
            var hoten = $(tr).find('.txt-hl-hoten').first().val();
            var tuoi = $(tr).find('.txt-hl-tuoi').first().val();
            var nghenghiep = $(tr).find('.txt-hl-nghenghiep').first().val();
            var ngayxayra = $(tr).find('input.txt-hl-ngayxayra').first().val();
            var nguyennhan = $(tr).find('.txt-hl-nguyennhan').first().val();
            var tinhtrang = $(tr).find('.ddlTinhTrang').first().val();
            var ghichu = $(tr).find('.txt-hl-ghichu').first().val();

            if (hoten == "") {
                javi.notify("Chưa nhập họ tên nạn nhân", "error");
                return false;
            }

            if (ngayxayra == "") {
                javi.notify("Chưa nhập ngày xảy ra tai nạn", "error");
                return false;
            }

            if (nguyennhan == "") {
                javi.notify("Chưa nhập nguyên nhân", "error");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/hlat_TaiNan/SaveNoiDung",
                data: {
                    Id: id,
                    DonViId: donviId,
                    Quy: $('#ddlQuarter').val(),
                    Nam: $('#ddlYear').val(),
                    CapDienApId: cda,
                    SoLuongVu: soluong,
                    HoTenNN: hoten,
                    TuoiNN: tuoi,
                    NgheNghiepNN: nghenghiep,
                    NgayXayRa: ngayxayra,
                    NguyenNhan_DienBien: nguyennhan,
                    TinhTrang: tinhtrang,
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

        $('body').on('click', '.btn-delete-add-hl', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-hl', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/hlat_TaiNan/DeleteNoiDung",
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

        $('body').on('click', '.btn-edit-hl', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.hl-stt').html();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.hl-donvi').first().data('dvid');
            var donvi = $(tr).find('.hl-donvi').first().html();
            var cda = $(tr).find('.hl-cda').first().data('cda');
            var soluong = $(tr).find('.hl-soluong').first().html();
            var hoten = $(tr).find('.hl-hoten').first().html();
            var tuoi = $(tr).find('.hl-tuoi').first().html();
            var nghenghiep = $(tr).find('.hl-nghenghiep').first().html();
            var ngayxayra = $(tr).find('.hl-ngayxayra').first().html();
            var nguyennhan = $(tr).find('.hl-nguyennhan').first().html();
            var tinhtrang = $(tr).find('.hl-tinhtrang').first().data('tinhtrangid');
            var ghichu = $(tr).find('.hl-ghichu').first().html();


            var data_template = $('#template-edit-hl').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                DonViId: donviId,
                DonVi: donvi,
                ddlCapDienAp: Init_ddl_CapDienAp(cda),
                SoLuong: soluong,
                HoTen: hoten,
                Tuoi: tuoi,
                NgheNghiep: nghenghiep,
                NgayXayRa: ngayxayra,
                NguyenNhan: nguyennhan,
                TinhTrang: Init_ddl_TinhTrang(tinhtrang),
                GhiChu: ghichu
            });

            $(tr).after(render);
            $(tr).remove();

            var parts = ngayxayra.split('/');

            $('.txt-hl-ngayxayra-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true,
                value: new Date(parts[2], parts[1] - 1, parts[0])
            });

        })

        $('body').on('click', '.btn-cancel-hl', function (e) {
            e.preventDefault();

            loadData();

            //var id = $(this).data('id');
            //var tr = $(this).parent().parent();
            //$.ajax({
            //    type: "POST",
            //    url: "/Admin/bcbs_NoiDung/GetNoiDungById",
            //    data: {
            //        Id: id
            //    },
            //    dataType: "json",
            //    beforeSend: function () {
            //        javi.startLoading();
            //    },
            //    success: function (response) {
            //        if (response != null) {

            //            var data_template = $('#template-nd').html();
            //            var render = Mustache.render(data_template, {
            //                STT: response.ThuTu,
            //                Id: response.Id,
            //                NoiDung: response.NoiDung,
            //                PhamVi: response.PhamVi,
            //                KhoiLuong: response.KhoiLuongVTTB,
            //                Value: formatter.format(response.TongGiaTri),
            //                HoanThanh: javi.convertDateJS(response.NgayHoanThanh),
            //                IsNPC: response.IsChuyenNPC ? 'nd-isNPC' : ''
            //            });
            //            $(tr).after(render);
            //            $(tr).remove();


            //        }
            //        else {
            //            javi.notify("Có lỗi xảy ra", "error");
            //        }
            //    },
            //    error: function (err) {
            //        javi.notify('Có lỗi xảy ra.', 'error');
            //    },
            //    complete: function () {
            //        javi.stopLoading();
            //    }
            //});
        })

        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/hlat_TaiNan/ChuyenNPC",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        quarter: $('#ddlQuarter').val(),
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
                    action: "/Admin/hlat_TaiNan/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        quarter: $('#ddlQuarter').val(),
                        year: $('#ddlYear').val()
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
    
    function Init_ddl_CapDienAp(select) {
        var render = '<select class="form-control ddlCapDienAp">';
        $.each(cachedObj.capDienAp, function (i, item) {
            if (select == item.Id)
                render += `<option value="${item.Id}" selected="selected">${item.TenCapDienAp}</option>`;
            else
                render += `<option value="${item.Id}">${item.TenCapDienAp}</option>`;
        })

        render += ' </select>';
        return render;
    }

    function Init_ddl_TinhTrang(select) {
        var render = '<select class="form-control ddlTinhTrang">';
        $.each(cachedObj.tinhTrang, function (i, item) {
            if (select == item.Id)
                render += `<option value="${item.Id}" selected="selected">${item.Ten}</option>`;
            else
                render += `<option value="${item.Id}">${item.Ten}</option>`;
        })

        render += ' </select>';
        return render;
    }

    //load dữ liệu
    function loadData(isPageChanged) {

        $.ajax({
            type: 'GET',
            url: '/admin/hlat_TaiNan/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                quy: $('#ddlQuarter').val(),
                nam: $('#ddlYear').val()
            },
            dataType: 'json',
            success: function (response) {

                var data_template = $('#template-hl').html();
                var render = "";

                var listDV = response.listDV;
                var data = response.data;
                var d = 1;
                var d0 = 0;
                var d1 = 0;
                var d2 = 0;
                var d3 = 0;

                if (listDV.length > 0) {
                    $.each(data, function (i3, item3) {

                        var tt = "";
                        if (item3.TinhTrang == 0) {
                            d0 += item3.SoLuongVu;
                            tt = "Không xác định";
                        }
                        else if (item3.TinhTrang == 1) {
                            d1 += item3.SoLuongVu;
                            tt = "Nặng";
                        }
                        else if (item3.TinhTrang == 2) {
                            d2 += item3.SoLuongVu;
                            tt = "Nhẹ";
                        }
                        else if (item3.TinhTrang == 3) {
                            d3 += item3.SoLuongVu;
                            tt = "Chết";
                        }

                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item3.Id,
                            DonViId: item3.DonViId,
                            DonVi: item3.TenDonVi,
                            CapDienAp: item3.TenCapDienAp,
                            CapDienApId: item3.CapDienApId,
                            SoLuong: item3.SoLuongVu,
                            HoTenNN: item3.HoTenNN,
                            TuoiNN: item3.TuoiNN,
                            NgheNghiepNN: item3.NgheNghiepNN,
                            NgayXayRa: javi.convertDateJS(item3.NgayXayRa),
                            NguyenNhan: item3.NguyenNhan_DienBien,
                            TinhTrangId: item3.TinhTrang,
                            TinhTrang: tt,
                            GhiChu: item3.GhiChu,
                            IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                            hidden: item3.IsChuyenNPC ? 'display: none' : '',
                        });
                    });

                    $('#tbl-content').html(render);

                    $('#table-hl .lblTong_SoLuong_KXD').html(d0);
                    $('#table-hl .lblTong_SoLuong_Nang').html(d1);
                    $('#table-hl .lblTong_SoLuong_Nhe').html(d2);
                    $('#table-hl .lblTong_SoLuong_Chet').html(d3);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="12">Không có dữ liệu</td></tr>');
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