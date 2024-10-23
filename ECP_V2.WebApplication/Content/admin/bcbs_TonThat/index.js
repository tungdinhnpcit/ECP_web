
var ttController = function () {

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
        $('#ddlDonVi').on('change', function () {
            loadData();
        })

        $('#ddlYear').on('change', function () {
            var year = $(this).val();
            var render = "";
            var currentY = (new Date()).getFullYear();

            if (year < currentY) {
                var d = new Date(year, 11, 31);

                var temp = getWeekNumber(d)[1];
                var countW = temp == 1 ? 53 : temp

                for (var i = countW; i >= 1; i--) {
                    if (i == countW) {
                        render += `<option value="${i}" selected="selected" >Tuần ${i}</option>`;
                    }
                    else {
                        render += `<option value="${i}">Tuần ${i}</option>`;
                    }
                }
            }
            else {
                var d = new Date(year, 11, 31);
                var temp = getWeekNumber(d)[1];
                var countW = temp == 1 ? 53 : temp

                var currentW = getWeekNumber(new Date());

                for (var i = countW; i >= 1; i--) {
                    if (i == currentW[1]) {
                        render += `<option value="${i}" selected="selected" >Tuần ${i}</option>`;
                    }
                    else {
                        render += `<option value="${i}">Tuần ${i}</option>`;
                    }
                }
            }
            $('#ddlWeek').html(render);

            loadData();
        })

        $('#ddlWeek').on('change', function () {
            loadData();
        })

        $('body').on('click', '.btn-save-nd', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var ctId = $(tr).data('chitieuid');
            var truoc = $(tr).find('.txt-nd-truoc').first().val();
            var sau = $(tr).find('.txt-nd-sau').first().val();
            var donviId = $('#ddlDonVi').val();
            var week = parseInt($('#ddlWeek').val());
            var year = parseInt($('#ddlYear').val());

            if (ctId < 1) {
                javi.notify("Giá trị không hợp lệ", "error");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/bcbs_TonThat/SaveEntity",
                data: {
                    Id: id,
                    ChiTieuId: ctId,
                    TruocXuLy: truoc,
                    SauXuLy: sau,
                    tuan: week,
                    thang: 1,
                    nam: year,
                    DonViId: donviId

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

        $('body').on('click', '.btn-edit-nd', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.nd-stt').html();
            var id = $(tr).data('id');
            var ctId = $(tr).data('chitieuid');
            var ctName = $(tr).find('.nd-noidung').first().html();
            var truoc = $(tr).find('.nd-truoc').first().html();
            var sau = $(tr).find('.nd-sau').first().html();
            var kdl = $(tr).data('kieudulieu');

            var data_template = ''
            if (kdl == 'string') {
                data_template = $('#template-edit-string-nd').html();
            }
            else {
                data_template = $('#template-edit-nd').html();
            }

            var render = Mustache.render(data_template, {
                Id: id,
                ctId: ctId,
                STT: stt,
                NoiDung: ctName,
                Truoc: truoc,
                Sau: sau,
            });

            $(tr).after(render);
            $(tr).remove();

        })

        $('body').on('click', '.btn-cancel-nd', function (e) {
            e.preventDefault();

            loadData();
        })


        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn chuyển Tổng công ty?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/bcbs_TonThat/ChuyenNPC",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        week: $('#ddlWeek').val(),
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
                    action: "/Admin/bcbs_TonThat/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        week: $('#ddlWeek').val(),
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
            url: '/admin/bcbs_TonThat/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                week: $('#ddlWeek').val(),
                year: $('#ddlYear').val()
            },
            dataType: 'json',
            success: function (response) {

                var data_template = $('#template-nd').html();
                var render = "";

                var data = response.data;
                if (data.length > 0) {
                    var d = 1;

                    $.each(data, function (i, item) {
                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item.Id,
                            ctId: item.ChiTieuId,
                            KieuDuLieu: item.KieuDuLieu,
                            NoiDung: item.TenChiTieu,
                            Truoc: item.TruocXuLy,
                            Sau: item.SauXuLy,
                            IsNPC: item.IsChuyenNPC ? 'nd-isNPC' : '',
                            hidden: item.IsChuyenNPC ? 'display:none' : ''
                        });
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="5">Không có dữ liệu</td></tr>');
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

    function getWeekNumber(d) {
        // Copy date so don't modify original
        d = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
        // Set to nearest Thursday: current date + 4 - current day number
        // Make Sunday's day number 7
        d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
        // Get first day of year
        var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        // Calculate full weeks to nearest Thursday
        var weekNo = Math.ceil((((d - yearStart) / 86400000) + 1) / 7);
        // Return array of year and week number
        return [d.getUTCFullYear(), weekNo];
    }

}