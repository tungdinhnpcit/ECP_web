var qtController = function () {
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

        $('body').on('click', '#btn-add-qt', function (e) {
            e.preventDefault();
            var template = $('#template-add-qt').html();

            var last = $('.qtmt').last().find('.qt-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html()) + 1;
            else
                stt = 1;

            var render = Mustache.render(template, {
                STT: stt,
                Id: 0,
                DonViId: "",
                DonVi: "",
                PL1: "",
                PL2: "",
                PL3: "",
                PL4: "",
                PL5: "",
                PL6: "",
                PL7: "",
                PL8: "",
                PL9: "",
                PL10: "",
                PL11: "",
                PL12: "",
                KQ1: "",
                KQ2: "",
                KQ3: "",
                KQ4: "",
                KQ5: "",
                KQ6: "",
                KQ7: "",
                KQ8: "",
            });

            if (stt > 1) {
                $('.qtmt').last().after(render);
            }
            else {
                $('#tbl-content').html(render);
            }

        });

        $('body').on('click', '.btn-save-qt', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.qt-donvi').first().data('dvid');
            var donvi = $(tr).find('.txt-qt-donvi').first().val();
            var pl1 = $(tr).find('.txt-qt-pl1').first().val();
            var pl2 = $(tr).find('.txt-qt-pl2').first().val();
            var pl3 = $(tr).find('.txt-qt-pl3').first().val();
            var pl4 = $(tr).find('.txt-qt-pl4').first().val();
            var pl5 = $(tr).find('.txt-qt-pl5').first().val();
            var pl6 = $(tr).find('.txt-qt-pl6').first().val();
            var pl7 = $(tr).find('.txt-qt-pl7').first().val();
            var pl8 = $(tr).find('.txt-qt-pl8').first().val();
            var pl9 = $(tr).find('.txt-qt-pl9').first().val();
            var pl10 = $(tr).find('.txt-qt-pl10').first().val();
            var pl11 = $(tr).find('.txt-qt-pl11').first().val();
            var pl12 = $(tr).find('.txt-qt-pl12').first().val();
            var kq1 = $(tr).find('.txt-qt-kq1').first().val();
            var kq2 = $(tr).find('.txt-qt-kq2').first().val();
            var kq3 = $(tr).find('.txt-qt-kq3').first().val();
            var kq4 = $(tr).find('.txt-qt-kq4').first().val();
            var kq5 = $(tr).find('.txt-qt-kq5').first().val();
            var kq6 = $(tr).find('.txt-qt-kq6').first().val();
            var kq7 = $(tr).find('.txt-qt-kq7').first().val();
            var kq8 = $(tr).find('.txt-qt-kq8').first().val();

            if (donvi == "") {
                javi.notify("Chưa nhập đơn vị", "error");
                return false;
            }


            $.ajax({
                type: "POST",
                url: "/Admin/vsld_QuanTrac/SaveEntity",
                data: {
                    Id: id,
                    DonViId: donviId,
                    DonVi: donvi,
                    Nam: $('#ddlYear').val(),
                    PLSK_Tong_Nam: pl1,
                    PLSK_Tong_Nu: pl2,
                    PLSK_Loai1_Nam: pl3,
                    PLSK_Loai1_Nu: pl4,
                    PLSK_Loai2_Nam: pl5,
                    PLSK_Loai2_Nu: pl6,
                    PLSK_Loai3_Nam: pl7,
                    PLSK_Loai3_Nu: pl8,
                    PLSK_Loai4_Nam: pl9,
                    PLSK_Loai4_Nu: pl10,
                    PLSK_Loai5_Nam: pl11,
                    PLSK_Loai5_Nu: pl12,
                    KQDK_TongMau: kq1,
                    KQDK_VuotMuc: kq2,
                    KQDK_BD1: kq3,
                    KQDK_BD2: kq4,
                    KQDK_BD3: kq5,
                    KQDK_BD4: kq6,
                    KQDK_ChiPhiDK: kq7,
                    KQDK_DonViThucHien: kq8,
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

        $('body').on('click', '.btn-delete-add-qt', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-qt', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/vsld_QuanTrac/DeleteEntity",
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

        $('body').on('click', '.btn-edit-qt', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var stt = $(tr).find('.qt-stt').html();

            var id = $(tr).data('id');
            var donviId = $(tr).find('.qt-donvi').first().data('dvid');
            var donvi = $(tr).find('.qt-donvi').first().html();
            var pl1 = $(tr).find('.qt-pl1').first().html();
            var pl2 = $(tr).find('.qt-pl2').first().html();
            var pl3 = $(tr).find('.qt-pl3').first().html();
            var pl4 = $(tr).find('.qt-pl4').first().html();
            var pl5 = $(tr).find('.qt-pl5').first().html();
            var pl6 = $(tr).find('.qt-pl6').first().html();
            var pl7 = $(tr).find('.qt-pl7').first().html();
            var pl8 = $(tr).find('.qt-pl8').first().html();
            var pl9 = $(tr).find('.qt-pl9').first().html();
            var pl10 = $(tr).find('.qt-pl10').first().html();
            var pl11 = $(tr).find('.qt-pl11').first().html();
            var pl12 = $(tr).find('.qt-pl12').first().html();
            var kq1 = $(tr).find('.qt-kq1').first().html();
            var kq2 = $(tr).find('.qt-kq2').first().html();
            var kq3 = $(tr).find('.qt-kq3').first().html();
            var kq4 = $(tr).find('.qt-kq4').first().html();
            var kq5 = $(tr).find('.qt-kq5').first().html();
            var kq6 = $(tr).find('.qt-kq6').first().html();
            var kq7 = $(tr).find('.qt-kq7').first().html();
            var kq8 = $(tr).find('.qt-kq8').first().html();

            var data_template = $('#template-edit-qt').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                DonViId: donviId,
                DonVi: donvi,
                PL1: pl1,
                PL2: pl2,
                PL3: pl3,
                PL4: pl4,
                PL5: pl5,
                PL6: pl6,
                PL7: pl7,
                PL8: pl8,
                PL9: pl9,
                PL10: pl10,
                PL11: pl11,
                PL12: pl12,
                KQ1: kq1,
                KQ2: kq2,
                KQ3: kq3,
                KQ4: kq4,
                KQ5: kq5,
                KQ6: kq6,
                KQ7: kq7,
                KQ8: kq8,
            });

            $(tr).after(render);
            $(tr).remove();

        })

        $('body').on('click', '.btn-cancel-qt', function (e) {
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
                    url: "/Admin/vsld_QuanTrac/ChuyenNPC",
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
                    action: "/Admin/vsld_QuanTrac/Export",
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
            url: '/admin/vsld_QuanTrac/GetInfo',
            data: {
                nam: $('#ddlYear').val(),
                keyword: $('#txtKeyword').val()
            },
            dataType: 'json',
            success: function (response) {
                $('#checkall').prop('checked', false);
                var data_template = $('#template-qt').html();
                var render = "";

                var data = response.data;

                if (data.length > 0) {
                    var d = 1;
                    $.each(data, function (i3, item3) {

                        render += Mustache.render(data_template, {
                            STT: d++,
                            Id: item3.Id,
                            DonViId: item3.DonViId,
                            DonVi: item3.DonVi,
                            IsNPC: item3.IsChuyenNPC ? 'imgNoiDung' : '',
                            hiddenCheck: item3.IsChuyenNPC ? 'display: none' : '',
                            hiddenCheck2: item3.IsChuyenNPC ? 'disabled readonly' : '',
                            hidden: item3.IsChuyenNPC ? 'display: none' : '',
                            PL1: item3.PLSK_Tong_Nam,
                            PL2: item3.PLSK_Tong_Nu,
                            PL3: item3.PLSK_Loai1_Nam,
                            PL4: item3.PLSK_Loai1_Nu,
                            PL5: item3.PLSK_Loai2_Nam,
                            PL6: item3.PLSK_Loai2_Nu,
                            PL7: item3.PLSK_Loai3_Nam,
                            PL8: item3.PLSK_Loai3_Nu,
                            PL9: item3.PLSK_Loai4_Nam,
                            PL10: item3.PLSK_Loai4_Nu,
                            PL11: item3.PLSK_Loai5_Nam,
                            PL12: item3.PLSK_Loai5_Nu,
                            KQ1: item3.KQDK_TongMau,
                            KQ2: item3.KQDK_VuotMuc,
                            KQ3: item3.KQDK_BD1,
                            KQ4: item3.KQDK_BD2,
                            KQ5: item3.KQDK_BD3,
                            KQ6: item3.KQDK_BD4,
                            KQ7: item3.KQDK_ChiPhiDK,
                            KQ8: item3.KQDK_DonViThucHien,
                        });
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="24">Không có dữ liệu</td></tr>');
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