var kqController = function () {

    var cachedObj = {
        NhanViens: [],
        NhomHLs: [
            {
                Id: 1,
                Ten: "Nhóm 1"
            },
            {
                Id: 2,
                Ten: "Nhóm 2"
            },
            {
                Id: 3,
                Ten: "Nhóm 3"
            },
            {
                Id: 4,
                Ten: "Nhóm 4"
            },
            {
                Id: 5,
                Ten: "Nhóm 5"
            },
            {
                Id: 6,
                Ten: "Nhóm 6"
            },
        ],
        KetQuas: [
            {
                Id: true,
                Ten: "Đạt"
            },
            {
                Id: false,
                Ten: "Không đạt"
            }
        ]
    }

    this.initialize = function () {
        $.when(loadNhanVien(),
            loadData())
            .done(function () {

                $('html').addClass('sidebar-left-collapsed');
                registerEvents();
            });
    };

    function loadNhanVien() {
        return $.ajax({
            type: "GET",
            url: "/Admin/cthl_KetQuaThi/GetAllNhanVien",
            dataType: "json",
            success: function (response) {
                cachedObj.NhanViens = response;
            },
            error: function () {
                javi.notify('Có Lỗi Xảy Ra', 'error');
            }
        });
    }

    function loadAnhThe(id) {
        var template = $('#image-template').html();
        var render = "";
        $.ajax({
            type: 'GET',
            data: {
                id: $('#hid-cthl-Id').val()
            },
            url: '/admin/cthl_KetQuaThi/GetAllAnhTheById',
            dataType: 'json',
            success: function (response) {
                if (response.data.length > 0) {
                    $.each(response.data, function (i, item) {
                        render += Mustache.render(template, {
                            Id: item.Id,
                            Image: item.Url,
                            Date: javi.convertDateJS2(item.NgayTao)
                        });
                    });

                    if (render !== '') {
                        $('#list-AnhThe').html(render);
                    }
                }
                else {
                    $('#list-AnhThe').html('');
                }

                $('#modal-anh-the').modal('show');
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu ảnh thẻ', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }

    function registerEvents() {


        $('#ddlLoaiKT').on('change', function () {
            loadData();
        })

        $('#ddlYear').on('change', function () {
            loadData();
        })

        $('body').on('click', '.btn-anh-the', function (e) {
            e.preventDefault();
            var id = $(this).parent().parent().data('id');
            $('#hid-cthl-Id').val(id);

            loadAnhThe(id);
        });

        $('#btn-add-anh-the').on('click', function () {
            $('#fileInputImage').click();
        });

        $("#fileInputImage").on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }
            var cthlId = $('#hid-cthl-Id').val();
            data.append("cthlId", cthlId);

            $.ajax({
                type: "POST",
                url: "/Admin/cthl_KetQuaThi/UploadAnhThe",
                contentType: false,
                processData: false,
                data: data,
                success: function (response) {
                    if (response.status) {
                        javi.notify(response.message, 'success');
                        loadAnhThe(cthlId);
                    }
                    else {
                        jvai.notify(response.message, 'error');
                    }
                },
                error: function () {
                    javi.notify('Đã xảy ra lỗi khi tải tệp lên!', 'error');
                },
                complete: function () {
                    $('#fileInputImage').val('');
                }
            });
        });


        $('body').on('click', '.btn-deleteImage', function (e) {
            e.preventDefault();
            var cthlId = $('#hid-cthl-Id').val();
            var id = $(this).data('id');

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/cthl_KetQuaThi/DeleteAnhThe",
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
                                loadAnhThe(cthlId);
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



        $('body').on('click', '#btn-add-hl', function (e) {
            e.preventDefault();
            var template = $('#template-add-hl').html();

            var last = $('.cthl').last().find('.hl-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                NhanVien: Init_ddl_NhanVien(),
                NhomHL: Init_ddl_NhomHL(),
                HL3: "",
                HL4: "",
                HL5: "",
                HL6: "",
                KQ: Init_ddl_KetQua(),
                HL8: "",
                HL9: "",
                HL10: "",
                HL11: "",
                HL12: ""
            });

            if (stt > 1) {
                $('.cthl').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

            $('.txt-hl-4-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

            $('.txt-hl-5-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

            $('.txt-hl-6-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

            $('.ddlNhanVien').chosen({ width: "100%" });

        });

        $('body').on('click', '.btn-save-hl', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var nvId = $(tr).find('.ddlNhanVien').first().val();
            var nhom = $(tr).find('.ddlNhomHL').first().val();
            var bat = $(tr).find('.txt-hl-3').first().val();
            var ngaybd = $(tr).find('input.txt-hl-4').first().val();
            var ngaykt = $(tr).find('input.txt-hl-5').first().val();
            var ngaysh = $(tr).find('input.txt-hl-6').first().val();
            var kq = $(tr).find('.ddlKetQua').first().val();
            var ngaycap = $(tr).find('.txt-hl-8').first().val();
            var donviTC = $(tr).find('.txt-hl-9').first().val();
            var kyTT = $(tr).find('.txt-hl-10').first().val();
            var ghichu = $(tr).find('.txt-hl-11').first().val();

            if (nvId == "") {
                javi.notify("Chưa chọn nhân viên", "error");
                return false;
            }

            if (ngaybd == "" || ngaykt == "") {
                javi.notify("Chưa nhập ngày huấn luyện", "error");
                return false;
            }

            if (ngaysh == "") {
                javi.notify("Chưa nhập ngày sát hạch", "error");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/cthl_KetQuaThi/SaveNoiDung",
                data: {
                    Id: id,
                    Nam: $('#ddlYear').val(),
                    LoaiKyThiId: $('#ddlLoaiKT').val(),
                    NhanVienId: nvId,
                    NhomHL: nhom,
                    BacAnToan: bat,
                    NgayHL_BD: ngaybd,
                    NgayHL_KT: ngaykt,
                    NgaySH: ngaysh,
                    KetQuaSH: kq,
                    CapGCN: ngaycap,
                    DonViHL: donviTC,
                    KySHTiepTheo: kyTT,
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
                        url: "/Admin/cthl_KetQuaThi/DeleteNoiDung",
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
            var nvId = $(tr).find('.hl-1').first().data('nvid');
            var hl2 = $(tr).find('.hl-2').first().data('nhomid');
            var hl3 = $(tr).find('.hl-3').first().data('bat');
            var hl4 = $(tr).find('.hl-4').first().html();
            var hl5 = $(tr).find('.hl-5').first().html();
            var hl6 = $(tr).find('.hl-6').first().html();
            var hl7 = $(tr).find('.hl-7').first().data('kq');
            var hl8 = $(tr).find('.hl-8').first().html();
            var hl9 = $(tr).find('.hl-9').first().html();
            var hl10 = $(tr).find('.hl-10').first().html();
            var hl11 = $(tr).find('.hl-11').first().html();


            var data_template = $('#template-edit-hl').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                NhanVien: Init_ddl_NhanVien(nvId),
                NhomHL: Init_ddl_NhomHL(hl2),
                HL3: hl3,
                HL4: hl4,
                HL5: hl5,
                HL6: hl6,
                KQ: Init_ddl_KetQua(hl7),
                HL8: hl8,
                HL9: hl9,
                HL10: hl10,
                HL11: hl11
            });

            $(tr).after(render);
            $(tr).remove();

            if (hl4 != "") {
                var parts1 = hl4.split('/');
                $('.txt-hl-4-' + stt).kendoDatePicker({
                    start: "date",
                    depth: "date",
                    format: "dd/MM/yyyy",
                    dateInput: true,
                    value: new Date(parts1[2], parts1[1] - 1, parts1[0])
                });
            }

            if (hl5 != "") {
                var parts2 = hl5.split('/');
                $('.txt-hl-5-' + stt).kendoDatePicker({
                    start: "date",
                    depth: "date",
                    format: "dd/MM/yyyy",
                    dateInput: true,
                    value: new Date(parts2[2], parts2[1] - 1, parts2[0])
                });
            }

            if (hl6) {
                var parts3 = hl6.split('/');
                $('.txt-hl-6-' + stt).kendoDatePicker({
                    start: "date",
                    depth: "date",
                    format: "dd/MM/yyyy",
                    dateInput: true,
                    value: new Date(parts3[2], parts3[1] - 1, parts3[0])
                });
            }

            $('.ddlNhanVien').chosen({ width: "100%" });
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
                    url: "/Admin/cthl_KetQuaThi/ChuyenNPC",
                    data: {
                        year: $('#ddlYear').val(),
                        loaiKT: $('#ddlLoaiKT').val()
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
                    action: "/Admin/cthl_KetQuaThi/Export",
                    data: {
                        nam: $('#ddlYear').val(),
                        loaiKT: $('#ddlLoaiKT').val()
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

    function Init_ddl_NhomHL(select) {
        var render = '<select class="form-control ddlNhomHL" style="text-align:center">';
        $.each(cachedObj.NhomHLs, function (i, item) {
            if (select == item.Id)
                render += `<option value="${item.Id}" selected="selected">${item.Ten}</option>`;
            else
                render += `<option value="${item.Id}">${item.Ten}</option>`;
        })

        render += ' </select>';
        return render;
    }

    function Init_ddl_KetQua(select) {
        var render = '<select class="form-control ddlKetQua" style="text-align:center">';
        $.each(cachedObj.KetQuas, function (i, item) {
            if (select == item.Id)
                render += `<option value="${item.Id}" selected="selected">${item.Ten}</option>`;
            else
                render += `<option value="${item.Id}">${item.Ten}</option>`;
        })

        render += ' </select>';
        return render;
    }

    function Init_ddl_NhanVien(select) {
        var render = '<select class="form-control ddlNhanVien" style="text-align:left">';
        $.each(cachedObj.NhanViens, function (i, item) {
            if (select == item.Id)
                render += `<option value="${item.Id}" selected="selected">${item.TenNhanVien} - ${item.SoDT}</option>`;
            else
                render += `<option value="${item.Id}">${item.TenNhanVien} - ${item.SoDT}</option>`;
        })

        render += ' </select>';
        return render;
    }

    //load dữ liệu
    function loadData(isPageChanged) {

        $.ajax({
            type: 'GET',
            url: '/admin/cthl_KetQuaThi/GetInfo',
            data: {
                nam: $('#ddlYear').val(),
                loaiKT: $('#ddlLoaiKT').val()
            },
            dataType: 'json',
            success: function (response) {

                var data_template = $('#template-hl').html();
                var render = "";

                var data = response.data;
                var d = 1;

                if (data.length > 0) {
                    $.each(data, function (i3, item3) {


                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item3.Id,
                            HL1: item3.HoTenNV,
                            NhanVienId: item3.NhanVienId,
                            HL2: item3.NhomHL != null ? "Nhóm " + item3.NhomHL : "",
                            NhomHL: item3.NhomHL,
                            BAT: item3.BacAnToan,
                            HL3: item3.BacAnToan,
                            HL4: item3.NgayHL_BD != null ? javi.convertDateJS(item3.NgayHL_BD) : "",
                            HL5: item3.NgayHL_KT != null ? javi.convertDateJS(item3.NgayHL_KT) : "",
                            HL6: item3.NgaySH != null ? javi.convertDateJS(item3.NgaySH) : "",
                            HL7: item3.KetQuaSH ? "Đạt" : "Không đạt",
                            KQ: item3.KetQuaSH,
                            HL8: item3.CapGCN,
                            HL9: item3.DonViHL,
                            HL10: item3.KySHTiepTheo,
                            HL11: item3.GhiChu,
                            IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                            hidden: item3.IsChuyenNPC ? 'display: none' : '',
                        });
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="13">Không có dữ liệu</td></tr>');
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