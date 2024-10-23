var ndController = function () {
    var today = kendo.toString(kendo.parseDate(new Date(), 'dd/MM/yyyy'), 'dd/MM/yyyy');
    var cachedObj = {
        donvi: []
    }

    var formatter = new Intl.NumberFormat('vi-VN', {
    });

    this.initialize = function () {
        $.when(loadData())
            .done(function () {

                $('html').addClass('sidebar-left-collapsed');
                registerEvents();
            });
    };

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

        $('body').on('click', '.ckChuyenDV', function () {

            var id = $(this).val();

            if ($(this).prop("checked") === true) {
                $.each($('#tbl-content tr[data-dvId="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', 'checked')
                });
            }

            else {
                $.each($('#tbl-content tr[data-dvId="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', '')
                });
            }
        });


        //$("#monthpicker").kendoDatePicker({
        //    start: "year",
        //    depth: "year",
        //    format: "MM/yyyy",
        //    dateInput: true
        //});

        //$(".txt-nd-hoanthanh").kendoDatePicker({
        //    start: "date",
        //    depth: "date",
        //    format: "dd/MM/yyyy",
        //    dateInput: true
        //});



        //todo: binding events to controls

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


        $('body').on('click', '#btn-add-nd', function (e) {
            e.preventDefault();
            var template = $('#template-add-nd').html();

            var last = $('.ndvs').last().find('.nd-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                NoiDung: "",
                PhamVi: "",
                KhoiLuong: "",
                Value: "0",
                HoanThanh: ""
            });

            if (stt > 1) {
                $('.ndvs').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

            $('.txt-nd-ht-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

        });

        $('body').on('click', '.btn-save-nd', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var stt = $(tr).find('.nd-stt').first().html();
            var noidung = $(tr).find('.txt-nd-noidung').first().val();
            var phamvi = $(tr).find('.txt-nd-phamvi').first().val();
            var khoiluong = $(tr).find('.txt-nd-khoiluong').first().val();
            var value = $(tr).find('.txt-nd-value').first().val();
            var hoanthanh = $(tr).find('input.txt-nd-hoanthanh').first().val();
            var donviId = $('#ddlDonVi').val();

            var lydo = ""
            if (id != 0) {
                var next = $(tr).next();
                lydo = $(tr).next().find('.txt-nd-lydo').val();
            }

            if (value < 0) {
                javi.notify("Giá trị không hợp lệ", "error");
                return false;
            }

            if (noidung == "") {
                javi.notify("Chưa nhập nội dung", "error");
                return false;
            }

            if (hoanthanh == "") {
                javi.notify("Chưa nhập ngày hoàn thành", "error");
                return false;
            }

            if (value >= 100) {

                if (confirm('Tổng giá trị bạn nhập là: ' + formatter.format((value * 1000000)) + ". Bạn có chắc chắn nhập đúng dữ liệu? \n Lưu ý:  dữ liệu đang được nhập theo đơn vị  (x triệu). VD: nhập 1 là 1 triệu đồng.")) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/bcbs_NoiDung/SaveNoiDung",
                        data: {
                            Id: id,
                            ThuTu: stt,
                            NoiDung: noidung,
                            PhamVi: phamvi,
                            KhoiLuongVTTB: khoiluong,
                            TongGiaTri: value,
                            NgayHoanThanh: hoanthanh,
                            DonViId: donviId,
                            lydo: lydo

                        },
                        dataType: "json",
                        beforeSend: function () {
                            javi.startLoading();
                        },
                        success: function (response) {
                            if (response.status) {
                                javi.notify(response.message, 'success');

                                //var data_template = $('#template-nd').html();
                                //var render = Mustache.render(data_template, {
                                //    STT: response.data.ThuTu,
                                //    Id: response.data.Id,
                                //    NoiDung: response.data.NoiDung,
                                //    PhamVi: response.data.PhamVi,
                                //    KhoiLuong: response.data.KhoiLuongVTTB,
                                //    Value: formatter.format(response.data.TongGiaTri),
                                //    HoanThanh: javi.convertDateJS(response.data.NgayHoanThanh),
                                //    IsNPC: response.data.IsChuyenNPC ? 'nd-isNPC' : ''
                                //});

                                //$(tr).after(render);
                                //$(tr).remove();

                                //var v = parseFloat($('#lblTong').html());
                                //v += response.value;
                                //$('#lblTong').html(formatter.format(v));

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
                }
                else {
                    $(tr).find('.txt-nd-value').first().val(0)
                    $(tr).find('.txt-nd-value').first().focus();
                }
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "/Admin/bcbs_NoiDung/SaveNoiDung",
                    data: {
                        Id: id,
                        ThuTu: stt,
                        NoiDung: noidung,
                        PhamVi: phamvi,
                        KhoiLuongVTTB: khoiluong,
                        TongGiaTri: value,
                        NgayHoanThanh: hoanthanh,
                        DonViId: donviId,
                        lydo: lydo

                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status) {
                            javi.notify(response.message, 'success');

                            //var data_template = $('#template-nd').html();
                            //var render = Mustache.render(data_template, {
                            //    STT: response.data.ThuTu,
                            //    Id: response.data.Id,
                            //    NoiDung: response.data.NoiDung,
                            //    PhamVi: response.data.PhamVi,
                            //    KhoiLuong: response.data.KhoiLuongVTTB,
                            //    Value: formatter.format(response.data.TongGiaTri),
                            //    HoanThanh: javi.convertDateJS(response.data.NgayHoanThanh),
                            //    IsNPC: response.data.IsChuyenNPC ? 'nd-isNPC' : ''
                            //});

                            //$(tr).after(render);
                            //$(tr).remove();

                            //var v = parseFloat($('#lblTong').html());
                            //v += response.value;
                            //$('#lblTong').html(formatter.format(v));

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
            }
        });

        $('body').on('click', '.btn-delete-add-nd', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-nd', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/bcbs_NoiDung/DeleteNoiDung",
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
                                //var v = parseFloat($('#lblTong').html());
                                //v -= response.value;
                                //$('#lblTong').html(formatter.format(v));
                                //divtr.remove();

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

        $('body').on('click', '.btn-edit-nd', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var id = $(tr).data('id');
            var stt = $(tr).find('.nd-stt').html();
            var nd = $(tr).find('.nd-noidung').html();
            var pv = $(tr).find('.nd-phamvi').html();
            var kl = $(tr).find('.nd-khoiluong').html();
            var ht = $(tr).find('.nd-hoanthanh').html();

            var temp = $(tr).find('.nd-value').html();
            var value = temp.replace('.', '');
            var value = value.replace(',', '.');


            var data_template = $('#template-edit-nd').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                NoiDung: nd,
                PhamVi: pv,
                KhoiLuong: kl,
                Value: value,
                HoanThanh: ht,
            });

            $(tr).after(render);
            $(tr).remove();

            var parts = ht.split('/');

            $('.txt-nd-ht-' + stt).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true,
                value: new Date(parts[2], parts[1] - 1, parts[0])
            });
        })

        $('body').on('click', '.btn-cancel-nd', function (e) {
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

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/bcbs_NoiDung/ChuyenNPC",
                    data: {
                        List: list
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            if (response.count > 0) {
                                alert("Có " + response.count + " bản ghi không hợp lệ!");
                            }
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

        $('body').on('click', '#btnDuyet', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn Duyệt?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/bcbs_NoiDung/Duyet",
                    data: {
                        List: list
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            if (response.count > 0) {
                                alert("Có " + response.count + " bản ghi không hợp lệ!");
                            }
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

        $('body').on('click', '#btnChuyenHoan', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn Chuyển hoàn?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/bcbs_NoiDung/ChuyenHoan",
                    data: {
                        List: list
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            if (response.count > 0) {
                                alert("Có " + response.count + " bản ghi không hợp lệ!");
                            }
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
                    action: "/Admin/bcbs_NoiDung/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        fromDate: $('#GioBd').val(),
                        toDate: $('#GioKt').val(),
                        stt: $('#ddlTrangThai').val()
                    },
                    downloadType: 'Progress',
                    ajaxLoadingSelector: '#loading'
                });
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
            url: '/admin/bcbs_NoiDung/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                fromDate: $('#GioBd').val(),
                toDate: $('#GioKt').val(),
                status: $('#ddlTrangThai').val(),
            },
            dataType: 'json',
            success: function (response) {

                $('#checkall').prop('checked', false);

                var header_template = $('#template-ds-header').html();
                var footer_template = $('#template-ds-footer').html();
                var data_template = $('#template-nd').html();
                var render = "";

                var listDV = response.listDV;
                var data = response.data;
                var sum = 0;

                if (listDV.length > 0) {
                    $.each(listDV, function (i, item) {

                        render += Mustache.render(header_template, {
                            Id: item.Id,
                            Name: item.TenDonVi,
                        });

                        let tempData = jQuery.grep(data, function (item2, i2) {
                            return item2.DonViId == item.Id;
                        })

                        var d = 1;
                        var sum2 = 0;
                        $.each(tempData, function (i3, item3) {

                            var s = item3.TrangThai;
                            var color = "#fff";
                            if (s == 2)
                                color = "#caffe7";
                            else if (s == 3)
                                color = "#c4c4c4"

                            render += Mustache.render(data_template, {
                                STT: d++,
                                Id: item3.Id,
                                dvId: item3.DonViId,
                                NoiDung: item3.NoiDung,
                                PhamVi: item3.PhamVi,
                                KhoiLuong: item3.KhoiLuongVTTB,
                                Value: formatter.format(item3.TongGiaTri),
                                HoanThanh: javi.convertDateJS(item3.NgayHoanThanh),
                                StatusColor: color,
                                hiddenCheck: item3.IsChuyenNPC ? 'display: none' : '',
                                hiddenCheck2: item3.IsChuyenNPC ? 'disabled readonly' : '',
                                IsNPC: item3.IsChuyenNPC ? 'imgNoiDung' : '',
                                color: item3.NguoiSua != null ? 'Color:#DC0000' : ''
                            });
                            sum2 += item3.TongGiaTri;
                        });

                        if (listDV.length != 1) {
                            let sumTemp = formatter.format(sum2);
                            let strSumTemp = sumTemp.replace(',', '.');
                            var floatSumTemp = parseFloat(strSumTemp);

                            render += Mustache.render(footer_template, {
                                TongText: "Tổng: " + DocTienBangChu(floatSumTemp * 1000000),
                                Sum: sumTemp
                            });
                        }

                        sum += sum2;
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="8">Không có dữ liệu</td></tr>');
                }

                let sumTemp2 = formatter.format(sum);
                let strSumTemp2 = sumTemp2.replace(',', '.');
                var floatSumTemp2 = parseFloat(strSumTemp2);
                $('#lblTong-string').html("Tổng: " + DocTienBangChu(floatSumTemp2 * 1000000));
                $('#lblTong').html(sumTemp2);

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