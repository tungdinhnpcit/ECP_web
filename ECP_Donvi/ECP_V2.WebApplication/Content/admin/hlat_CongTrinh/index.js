var ctController = function () {
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


        $('#ddlDonVi').on('change', function () {
            loadData();
        })


        $('#ddlYear').on('change', function () {
            loadData();
        })

        $('#ddlMonth').on('change', function () {
            loadData();
        })


        $('body').on('click', '.btn-save-hl', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var cdaId = $(tr).find('.hl-capdienap').data('cdaid');
            var hl11 = $(tr).find('.txt-hl-11').first().val();
            var hl12 = $(tr).find('.txt-hl-12').first().val();
            var hl21 = $(tr).find('.txt-hl-21').first().val();
            var hl22 = $(tr).find('.txt-hl-22').first().val();
            var hl23 = $(tr).find('.txt-hl-23').first().val();
            var hl31 = $(tr).find('.txt-hl-31').first().val();
            var hl32 = $(tr).find('.txt-hl-32').first().val();
            var hl33 = $(tr).find('.txt-hl-33').first().val();
            var hl34 = $(tr).find('.txt-hl-34').first().val();
            var hl41 = $(tr).find('.txt-hl-41').first().val();
            var hl51 = $(tr).find('.txt-hl-51').first().val();
            var hl61 = $(tr).find('.txt-hl-61').first().val();

            var donviId = $(tr).data('donvi');
            var thang = $('#ddlMonth').val();
            var nam = $('#ddlYear').val();


            $.ajax({
                type: "POST",
                url: "/Admin/hlat_CongTrinh/SaveEntity",
                data: {
                    Id: id,
                    CapDienApId: cdaId,
                    DonViId: donviId,
                    Thang: thang,
                    Nam: nam,
                    Tong_DauNam: hl11,
                    Tong_LuyKe: hl12,
                    TangGiam_PhatSinhMoi: hl21,
                    TangGiam_GiamDoCaiTao: hl22,
                    TangGiam_GiamDoPhoiHopDiaPhuong: hl23,
                    PhanLoai_Khoan1: hl31,
                    PhanLoai_Khoan2: hl32,
                    PhanLoai_Khoan3: hl33,
                    PhanLoai_Khoan5: hl34,
                    CapNgam: hl41,
                    DanhGiaDoNguyHiemHRN: hl51,
                    GhiChu: hl61

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
            var donvi = $(tr).data('donvi');
            var cdaId = $(tr).find('.hl-capdienap').data('cdaid');
            var cda = $(tr).find('.hl-capdienap').html();
            var hl11 = $(tr).find('.hl-11').html();
            var hl12 = $(tr).find('.hl-12').html();
            var hl21 = $(tr).find('.hl-21').html();
            var hl22 = $(tr).find('.hl-22').html();
            var hl23 = $(tr).find('.hl-23').html();
            var hl31 = $(tr).find('.hl-31').html();
            var hl32 = $(tr).find('.hl-32').html();
            var hl33 = $(tr).find('.hl-33').html();
            var hl34 = $(tr).find('.hl-34').html();
            var hl41 = $(tr).find('.hl-41').html();
            var hl51 = $(tr).find('.hl-51').html();
            var hl61 = $(tr).find('.hl-61').html();

            var data_template = $('#template-edit-hl').html();
            var render = Mustache.render(data_template, {
                Id: id,
                DonViId: donvi,
                cdaId: cdaId,
                CapDienAp: cda,
                HL11: hl11,
                HL12: hl12,
                HL21: hl21,
                HL22: hl22,
                HL23: hl23,
                HL31: hl31,
                HL32: hl32,
                HL33: hl33,
                HL34: hl34,
                HL41: hl41,
                HL51: hl51,
                HL61: hl61,
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
                    url: "/Admin/hlat_CongTrinh/ChuyenNPC",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        month: $('#ddlMonth').val(),
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
                    action: "/Admin/hlat_CongTrinh/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        month: $('#ddlMonth').val(),
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
            url: '/admin/hlat_CongTrinh/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                month: $('#ddlMonth').val(),
                year: $('#ddlYear').val()
            },
            dataType: 'json',
            success: function (response) {

                var template_label = $('#template-hl-label').html();
                var template = $('#template-hl').html();
                var render = "";

                var listDV = response.listDV;
                var data = response.data;

                var sum11 = 0;
                var sum12 = 0;
                var sum13 = 0;
                var sum14 = 0;
                var sum15 = 0;
                var sum16 = 0;
                var sum17 = 0;
                var sum18 = 0;
                var sum19 = 0;
                var sum110 = 0;
                var sum111 = 0;
                var sum21 = 0;
                var sum22 = 0;
                var sum23 = 0;
                var sum24 = 0;
                var sum25 = 0;
                var sum26 = 0;
                var sum27 = 0;
                var sum28 = 0;
                var sum29 = 0;
                var sum210 = 0;
                var sum211 = 0;
                var sum31 = 0;
                var sum32 = 0;
                var sum33 = 0;
                var sum34 = 0;
                var sum35 = 0;
                var sum36 = 0;
                var sum37 = 0;
                var sum38 = 0;
                var sum39 = 0;
                var sum310 = 0;
                var sum311 = 0;
                var sum41 = 0;
                var sum42 = 0;
                var sum43 = 0;
                var sum44 = 0;
                var sum45 = 0;
                var sum46 = 0;
                var sum47 = 0;
                var sum48 = 0;
                var sum49 = 0;
                var sum410 = 0;
                var sum411 = 0;
                var sum51 = 0;
                var sum52 = 0;
                var sum53 = 0;
                var sum54 = 0;
                var sum55 = 0;
                var sum56 = 0;
                var sum57 = 0;
                var sum58 = 0;
                var sum59 = 0;
                var sum510 = 0;
                var sum511 = 0;

                var d = 1;

                if (listDV.length > 0) {
                    $.each(listDV, function (i, item) {

                        render += Mustache.render(template_label, {
                            STT: d++,
                            Id: item.Id,
                            TenDonVi: item.TenDonVi,
                        });

                        let tempData = jQuery.grep(data, function (item2, i2) {
                            return item2.DonViId == item.Id;
                        })

                        $.each(tempData, function (i3, item3) {

                            render += Mustache.render(template, {
                                Id: item3.Id,
                                DonViId: item3.DonViId,
                                cdaId: item3.CapDienApId,
                                CapDienAp: item3.TenCapDienAp,
                                HL11: item3.Tong_DauNam,
                                HL12: item3.Tong_LuyKe,
                                HL21: item3.TangGiam_PhatSinhMoi,
                                HL22: item3.TangGiam_GiamDoCaiTao,
                                HL23: item3.TangGiam_GiamDoPhoiHopDiaPhuong,
                                HL31: item3.PhanLoai_Khoan1,
                                HL32: item3.PhanLoai_Khoan2,
                                HL33: item3.PhanLoai_Khoan3,
                                HL34: item3.PhanLoai_Khoan5,
                                HL41: item3.CapNgam,
                                HL51: item3.DanhGiaDoNguyHiemHRN,
                                HL61: item3.GhiChu,
                                hidden: item3.IsChuyenNPC ? 'display: none' : '',
                                IsNPC: item3.IsChuyenNPC ? 'nd-isNPC' : '',
                            });

                            switch (item3.TenCapDienAp) {
                                case 110:
                                    sum11 += item3.Tong_DauNam;
                                    sum12 += item3.Tong_LuyKe;
                                    sum13 += item3.TangGiam_PhatSinhMoi;
                                    sum14 += item3.TangGiam_GiamDoCaiTao;
                                    sum15 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    sum16 += item3.PhanLoai_Khoan1;
                                    sum17 += item3.PhanLoai_Khoan2;
                                    sum18 += item3.PhanLoai_Khoan3;
                                    sum19 += item3.PhanLoai_Khoan5;
                                    sum110 += item3.CapNgam;
                                    sum111 += item3.DanhGiaDoNguyHiemHRN;
                                    break;
                                case 35:
                                    sum21 += item3.Tong_DauNam;
                                    sum22 += item3.Tong_LuyKe;
                                    sum23 += item3.TangGiam_PhatSinhMoi;
                                    sum24 += item3.TangGiam_GiamDoCaiTao;
                                    sum25 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    sum26 += item3.PhanLoai_Khoan1;
                                    sum27 += item3.PhanLoai_Khoan2;
                                    sum28 += item3.PhanLoai_Khoan3;
                                    sum29 += item3.PhanLoai_Khoan5;
                                    sum210 += item3.CapNgam;
                                    sum211 += item3.DanhGiaDoNguyHiemHRN;
                                    break;
                                case 22:
                                    sum31 += item3.Tong_DauNam;
                                    sum32 += item3.Tong_LuyKe;
                                    sum33 += item3.TangGiam_PhatSinhMoi;
                                    sum34 += item3.TangGiam_GiamDoCaiTao;
                                    sum35 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    sum36 += item3.PhanLoai_Khoan1;
                                    sum37 += item3.PhanLoai_Khoan2;
                                    sum38 += item3.PhanLoai_Khoan3;
                                    sum39 += item3.PhanLoai_Khoan5;
                                    sum310 += item3.CapNgam;
                                    sum311 += item3.DanhGiaDoNguyHiemHRN;
                                    break;
                                case 10:
                                    sum41 += item3.Tong_DauNam;
                                    sum42 += item3.Tong_LuyKe;
                                    sum43 += item3.TangGiam_PhatSinhMoi;
                                    sum44 += item3.TangGiam_GiamDoCaiTao;
                                    sum45 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    sum46 += item3.PhanLoai_Khoan1;
                                    sum47 += item3.PhanLoai_Khoan2;
                                    sum48 += item3.PhanLoai_Khoan3;
                                    sum49 += item3.PhanLoai_Khoan5;
                                    sum410 += item3.CapNgam;
                                    sum411 += item3.DanhGiaDoNguyHiemHRN;

                                    break;
                                case 6:
                                    sum51 += item3.Tong_DauNam;
                                    sum52 += item3.Tong_LuyKe;
                                    sum53 += item3.TangGiam_PhatSinhMoi;
                                    sum54 += item3.TangGiam_GiamDoCaiTao;
                                    sum55 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    sum56 += item3.PhanLoai_Khoan1;
                                    sum57 += item3.PhanLoai_Khoan2;
                                    sum58 += item3.PhanLoai_Khoan3;
                                    sum59 += item3.PhanLoai_Khoan5;
                                    sum510 += item3.CapNgam;
                                    sum511 += item3.DanhGiaDoNguyHiemHRN;

                                    break;
                                default:
                                // code block
                            }
                        });

                    });

                    $('#tbl-content').html(render);

                    $('#table-hl .lblTong_11').html(sum11);
                    $('#table-hl .lblTong_12').html(sum12);
                    $('#table-hl .lblTong_13').html(sum13);
                    $('#table-hl .lblTong_14').html(sum14);
                    $('#table-hl .lblTong_15').html(sum15);
                    $('#table-hl .lblTong_16').html(formatter.format(sum16));
                    $('#table-hl .lblTong_17').html(formatter.format(sum17));
                    $('#table-hl .lblTong_18').html(formatter.format(sum18));
                    $('#table-hl .lblTong_19').html(formatter.format(sum19));
                    $('#table-hl .lblTong_110').html(formatter.format(sum110));
                    $('#table-hl .lblTong_111').html(formatter.format(sum111));

                    $('#table-hl .lblTong_21').html(sum21);
                    $('#table-hl .lblTong_22').html(sum22);
                    $('#table-hl .lblTong_23').html(sum23);
                    $('#table-hl .lblTong_24').html(sum24);
                    $('#table-hl .lblTong_25').html(sum25);
                    $('#table-hl .lblTong_26').html(formatter.format(sum26));
                    $('#table-hl .lblTong_27').html(formatter.format(sum27));
                    $('#table-hl .lblTong_28').html(formatter.format(sum28));
                    $('#table-hl .lblTong_29').html(formatter.format(sum29));
                    $('#table-hl .lblTong_210').html(formatter.format(sum210));
                    $('#table-hl .lblTong_211').html(formatter.format(sum211));

                    $('#table-hl .lblTong_31').html(sum31);
                    $('#table-hl .lblTong_32').html(sum32);
                    $('#table-hl .lblTong_33').html(sum33);
                    $('#table-hl .lblTong_34').html(sum34);
                    $('#table-hl .lblTong_35').html(sum35);
                    $('#table-hl .lblTong_36').html(formatter.format(sum36));
                    $('#table-hl .lblTong_37').html(formatter.format(sum37));
                    $('#table-hl .lblTong_38').html(formatter.format(sum38));
                    $('#table-hl .lblTong_39').html(formatter.format(sum39));
                    $('#table-hl .lblTong_310').html(formatter.format(sum310));
                    $('#table-hl .lblTong_311').html(formatter.format(sum311));

                    $('#table-hl .lblTong_41').html(sum41);
                    $('#table-hl .lblTong_42').html(sum42);
                    $('#table-hl .lblTong_43').html(sum43);
                    $('#table-hl .lblTong_44').html(sum44);
                    $('#table-hl .lblTong_45').html(sum45);
                    $('#table-hl .lblTong_46').html(formatter.format(sum46));
                    $('#table-hl .lblTong_47').html(formatter.format(sum47));
                    $('#table-hl .lblTong_48').html(formatter.format(sum48));
                    $('#table-hl .lblTong_49').html(formatter.format(sum49));
                    $('#table-hl .lblTong_410').html(formatter.format(sum410));
                    $('#table-hl .lblTong_411').html(formatter.format(sum411));

                    $('#table-hl .lblTong_51').html(sum51);
                    $('#table-hl .lblTong_52').html(sum52);
                    $('#table-hl .lblTong_53').html(sum53);
                    $('#table-hl .lblTong_54').html(sum54);
                    $('#table-hl .lblTong_55').html(sum55);
                    $('#table-hl .lblTong_56').html(formatter.format(sum56));
                    $('#table-hl .lblTong_57').html(formatter.format(sum57));
                    $('#table-hl .lblTong_58').html(formatter.format(sum58));
                    $('#table-hl .lblTong_59').html(formatter.format(sum59));
                    $('#table-hl .lblTong_510').html(formatter.format(sum510));
                    $('#table-hl .lblTong_511').html(formatter.format(sum511));


                    $('#table-hl .lblTong_All_1').html(sum11 + sum21 + sum31 + sum41 + sum51);
                    $('#table-hl .lblTong_All_2').html(sum12 + sum22 + sum32 + sum42 + sum52);
                    $('#table-hl .lblTong_All_3').html(sum13 + sum23 + sum33 + sum43 + sum53);
                    $('#table-hl .lblTong_All_4').html(sum14 + sum24 + sum34 + sum44 + sum54);
                    $('#table-hl .lblTong_All_5').html(sum15 + sum25 + sum35 + sum45 + sum55);
                    $('#table-hl .lblTong_All_6').html(formatter.format(sum16 + sum26 + sum36 + sum46 + sum56));
                    $('#table-hl .lblTong_All_7').html(formatter.format(sum17 + sum27 + sum37 + sum47 + sum57));
                    $('#table-hl .lblTong_All_8').html(formatter.format(sum18 + sum28 + sum38 + sum48 + sum58));
                    $('#table-hl .lblTong_All_9').html(formatter.format(sum19 + sum29 + sum39 + sum49 + sum59));
                    $('#table-hl .lblTong_All_10').html(formatter.format(sum110 + sum210 + sum310 + sum410 + sum510));
                    $('#table-hl .lblTong_All_11').html(formatter.format(sum111 + sum211 + sum311 + sum411 + sum511));
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="16">Không có dữ liệu</td></tr>');
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