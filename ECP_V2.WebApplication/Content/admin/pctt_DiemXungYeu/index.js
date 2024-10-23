var dxyController = function () {

    var cachedObj = {
        donvi: [],
        mucDo: [
            {
                Id: 1,
                Ten: 'Rất cao'
            },
            {
                Id: 2,
                Ten: 'Cao'
            },
            {
                Id: 3,
                Ten: 'Bình thường'
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

        //todo: binding events to controls

        $('#ddlDonVi').on('change', function () {
            loadData();
        })

        $('#ddlYear').on('change', function () {
            loadData();
        })

        $('body').on('click', '.dxy-donvi-collapse', function () {
            $(this).parent().toggleClass('expand').nextUntil('tr.header-collapse').slideToggle(100);
        })

        $('body').on('click', '#checkall', function () {
            if ($(this).prop("checked") === true) {
                $.each($('#tbl-content tr'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', 'checked')
                    $(item).find('.ckChooseDV:enabled').first().prop('checked', 'checked')
                    $(item).find('.ckChooseType:enabled').first().prop('checked', 'checked')
                });
            }

            else {
                $.each($('#tbl-content tr'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', '')
                    $(item).find('.ckChooseDV:enabled').first().prop('checked', '')
                    $(item).find('.ckChooseType:enabled').first().prop('checked', '')
                });
            }
        });

        $('body').on('click', '.ckChooseDV', function () {

            var id = $(this).val();

            if ($(this).prop("checked") === true) {
                $.each($('#tbl-content tr[data-dvid="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChooseType:enabled').first().prop('checked', 'checked')
                    $(item).find('.ckChoose:enabled').first().prop('checked', 'checked')
                });
            }

            else {
                $.each($('#tbl-content tr[data-dvid="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChooseType:enabled').first().prop('checked', '')
                    $(item).find('.ckChoose:enabled').first().prop('checked', '')
                });
            }
        });

        $('body').on('click', '.ckChooseType', function () {

            var id = $(this).val();

            if ($(this).prop("checked") === true) {
                $.each($('#tbl-content tr[data-type="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', 'checked')
                });
            }

            else {
                $.each($('#tbl-content tr[data-type="' + id + '"]'), function (i, item) {
                    $(item).find('.ckChoose:enabled').first().prop('checked', '')
                });
            }
        });




        $('body').on('click', '.btn-add-dxy', function (e) {
            e.preventDefault();
            var template = $('#template-add-dxy').html();
            var id = $(this).data('id');
            var dvId = $(this).data('dvid');

            var last = $('.pctt[data-dvid="' + dvId + '"][data-type="' + id + '"]').last().find('.dxy-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                TenDuongDay: "",
                TinhTrang: "",
                MucDo: Init_ddl_MucDo(),
                KHBD: "",
                KHKT: "",
                GhiChu: "",
                Id: 0,
                dvId: dvId,
                dxyType: id
            });

            if (stt > 1) {
                $('.pctt[data-dvid="' + dvId + '"][data-type="' + id + '"]').last().after(render);
            }
            else {
                $(this).parent().parent().after(render);
            }

            $(`.txt-dxy-khbd-${dvId}-${id}-${stt}`).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

            $(`.txt-dxy-khkt-${dvId}-${id}-${stt}`).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true
            });

        });

        $('body').on('click', '.btn-save-dxy', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var dvId = $(tr).data('dvid');
            var type = $(tr).data('type');
            var tendd = $(tr).find('.txt-dxy-tenduongday').first().val();
            var tinhtrang = $(tr).find('.txt-dxy-tinhtrang').first().val();
            var bd = $(tr).find('input.txt-dxy-khbd').first().val();
            var kt = $(tr).find('input.txt-dxy-khkt').first().val();
            var ghichu = $(tr).find('.txt-dxy-ghichu').first().val();
            var md = $(tr).find('.ddlMucDo').first().val();
            var nam = $('#ddlYear').val();

            if (tendd == '') {
                javi.notify("Chưa nhập tên đường dây", "error");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/pctt_DiemXungYeu/SaveNoiDung",
                data: {
                    Id: id,
                    DonViId: dvId,
                    Nam: nam,
                    LoaiDuongDayId: type,
                    TenDuongDay: tendd,
                    TinhTrang: tinhtrang,
                    MucDo: md,
                    KHXL_BD: bd,
                    KHXL_KT: kt,
                    GhiChu: ghichu

                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    javi.notify(response.message, 'success');
                    loadData();
                },
                error: function (err) {
                    javi.notify('Đã xảy ra lỗi khi cập nhật.', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });

        });

        $('body').on('click', '.btn-delete-add-dxy', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-dxy', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/pctt_DiemXungYeu/DeleteNoiDung",
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

        $('body').on('click', '.btn-edit-dxy', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.dxy-stt').html();
            var id = $(tr).data('id');
            var dvId = $(tr).data('id');
            var type = $(tr).data('id');
            var tenduongday = $(tr).find('.dxy-TenDuongDay').html();
            var tinhtrang = $(tr).find('.dxy-tinhtrang').html();
            var mucdo = $(tr).find('.dxy-mucdo').data('mdid');
            var khbd = $(tr).find('.dxy-kehoach-bd').html();
            var khkt = $(tr).find('.dxy-kehoach-kt').html();
            var ghichu = $(tr).find('.dxy-ghichu').html();

            var data_template = $('#template-edit-dxy').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                TenDuongDay: tenduongday,
                TinhTrang: tinhtrang,
                MucDo: Init_ddl_MucDo(mucdo),
                KHBD: khbd,
                KHKT: khkt,
                GhiChu: ghichu,
                Id: id,
                dvId: dvId,
                dxyType: type
            });

            $(tr).after(render);
            $(tr).remove();

            var part1 = khbd.split('/');

            $(`.txt-dxy-khbd-${dvId}-${type}-${stt}`).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true,
                value: part1.length > 1 ? new Date(part1[2], part1[1] - 1, part1[0]) : ""
            });

            var part2 = khkt.split('/');

            $(`.txt-dxy-khkt-${dvId}-${type}-${stt}`).kendoDatePicker({
                start: "date",
                depth: "date",
                format: "dd/MM/yyyy",
                dateInput: true,
                value: part2.length > 1 ? new Date(part2[2], part2[1] - 1, part2[0]) : ""
            });
        })

        $('body').on('click', '.btn-cancel-dxy', function (e) {
            e.preventDefault();

            loadData();
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

            if (confirm('Bạn có chắc chắn muốn Chuyển NPC?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/pctt_DiemXungYeu/ChuyenNPC",
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

        $('body').on('click', '#btnHuyChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn hủy Chuyển NPC?')) {
                var kybaocaoId = $('#hidKyBaoCao').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/HuyChuyenNPC",
                    data: {
                        kybaocaoId: kybaocaoId
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
            $.UnifiedExportFile(
                {
                    action: "/Admin/pctt_DiemXungYeu/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        nam: $('#ddlYear').val(),
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



    function Init_ddl_MucDo(select) {
        var render = '<select class="form-control ddlMucDo">';
        $.each(cachedObj.mucDo, function (i, item) {
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
            url: '/admin/pctt_DiemXungYeu/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                nam: $('#ddlYear').val(),
            },
            dataType: 'json',
            success: function (response) {
                $('#checkall').prop('checked', false);
                var donvi_template = $('#template-dxy-donvi').html();
                var type_template = $('#template-dxy-type').html();
                var data_template = $('#template-dxy').html();
                var render = "";

                var listDV = response.listDV;
                var listType = response.listType;
                var data = response.data;

                if (listDV.length > 0) {
                    var d = 1;
                    $.each(listDV, function (i, item) {
                        render += Mustache.render(donvi_template, {
                            STT: ConvertToRoman(d++),
                            Id: item.Id,
                            Name: item.TenDonVi,
                        });

                        var d2 = 0;
                        $.each(listType, function (i2, item2) {
                            render += Mustache.render(type_template, {
                                STT: ConvertToAnphabet(d2++),
                                Id: item2.Id,
                                dvId: item.Id,
                                Name: item2.TenLoai,
                            });

                            var tempData = jQuery.grep(data, function (item3, i3) {
                                return (item3.DonViId == item.Id && item3.LoaiDuongDayId == item2.Id);
                            })

                            var count = 1;
                            $.each(tempData, function (i4, item4) {

                                var md = "";
                                if (item4.MucDo == 1)
                                    md = "Rất cao";
                                else if (item4.MucDo == 2)
                                    md = "Cao";
                                else if (item4.MucDo == 3)
                                    md = "Bình thường";

                                render += Mustache.render(data_template, {
                                    Id: item4.Id,
                                    dvId: item4.DonViId,
                                    dxyType: item4.LoaiDuongDayId,
                                    STT: count++,
                                    TenDuongDay: item4.TenDuongDay,
                                    TinhTrang: item4.TinhTrang,
                                    MucDoId: item4.MucDo,
                                    MucDo: md,
                                    KHBD: javi.convertDateJS(item4.KHXL_BD),
                                    KHKT: javi.convertDateJS(item4.KHXL_KT),
                                    GhiChu: item4.GhiChu,
                                    hiddenCheck: item4.IsChuyenNPC ? 'display: none' : '',
                                    hiddenCheck2: item4.IsChuyenNPC ? 'disabled readonly' : '',
                                    IsNPC: item4.IsChuyenNPC ? 'imgNoiDung' : '',
                                });
                            })
                        })
                    })

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="8">Không có dữ liệu</td></tr>');
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