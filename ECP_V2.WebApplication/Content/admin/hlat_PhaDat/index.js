var pdController = function () {
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
            var hl41 = $(tr).find('.txt-hl-41').first().val();
            var hl51 = $(tr).find('.txt-hl-51').first().val();
            var hl52 = $(tr).find('.txt-hl-52').first().val();
            var hl53 = $(tr).find('.txt-hl-53').first().val();
            var hl61 = $(tr).find('.txt-hl-61').first().val();
        
            var donviId = $(tr).data('donvi');
            var thang = $('#ddlMonth').val();
            var nam = $('#ddlYear').val();


            $.ajax({
                type: "POST",
                url: "/Admin/hlat_PhaDat/SaveEntity",
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
                    DoVong: hl31,
                    DanhGiaDoNguyHiem: hl41,
                    CayCoi_TrongHL: hl51,
                    CayCoi_NgoaiHL: hl52,
                    CayCoi_DiemHRN: hl53,
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
            var hl41 = $(tr).find('.hl-41').html();
            var hl51 = $(tr).find('.hl-51').html();
            var hl52 = $(tr).find('.hl-52').html();
            var hl53 = $(tr).find('.hl-53').html();
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
                HL41: hl41,
                HL51: hl51,
                HL52: hl52,
                HL53: hl53,
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
                    url: "/Admin/hlat_PhaDat/ChuyenNPC",
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
                    action: "/Admin/hlat_PhaDat/Export",
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
            url: '/admin/hlat_PhaDat/GetInfo',
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
                var sum21 = 0;
                var sum22 = 0;
                var sum23 = 0;
                var sum24 = 0;
                var sum25 = 0;
                var sum31 = 0;
                var sum32 = 0;
                var sum33 = 0;
                var sum34 = 0;
                var sum35 = 0;
                var sum41 = 0;
                var sum42 = 0;
                var sum43 = 0;
                var sum44 = 0;
                var sum45 = 0;
                var sum51 = 0;
                var sum52 = 0;
                var sum53 = 0;
                var sum54 = 0;
                var sum55 = 0;

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
                                HL31: item3.DoVong,
                                HL41: item3.DanhGiaDoNguyHiem,
                                HL51: item3.CayCoi_TrongHL,
                                HL52: item3.CayCoi_NgoaiHL,
                                HL53: item3.CayCoi_DiemHRN,
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
                                    break;
                                case 35:
                                    sum21 += item3.Tong_DauNam;
                                    sum22 += item3.Tong_LuyKe;
                                    sum23 += item3.TangGiam_PhatSinhMoi;
                                    sum24 += item3.TangGiam_GiamDoCaiTao;
                                    sum25 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    break;
                                case 22:
                                    sum31 += item3.Tong_DauNam;
                                    sum32 += item3.Tong_LuyKe;
                                    sum33 += item3.TangGiam_PhatSinhMoi;
                                    sum34 += item3.TangGiam_GiamDoCaiTao;
                                    sum35 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;
                                    break;
                                case 10:
                                    sum41 += item3.Tong_DauNam;
                                    sum42 += item3.Tong_LuyKe;
                                    sum43 += item3.TangGiam_PhatSinhMoi;
                                    sum44 += item3.TangGiam_GiamDoCaiTao;
                                    sum45 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;

                                    break;
                                case 6:
                                    sum51 += item3.Tong_DauNam;
                                    sum52 += item3.Tong_LuyKe;
                                    sum53 += item3.TangGiam_PhatSinhMoi;
                                    sum54 += item3.TangGiam_GiamDoCaiTao;
                                    sum55 += item3.TangGiam_GiamDoPhoiHopDiaPhuong;

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

                    $('#table-hl .lblTong_21').html(sum21);
                    $('#table-hl .lblTong_22').html(sum22);
                    $('#table-hl .lblTong_23').html(sum23);
                    $('#table-hl .lblTong_24').html(sum24);
                    $('#table-hl .lblTong_25').html(sum25);

                    $('#table-hl .lblTong_31').html(sum31);
                    $('#table-hl .lblTong_32').html(sum32);
                    $('#table-hl .lblTong_33').html(sum33);
                    $('#table-hl .lblTong_34').html(sum34);
                    $('#table-hl .lblTong_35').html(sum35);

                    $('#table-hl .lblTong_41').html(sum41);
                    $('#table-hl .lblTong_42').html(sum42);
                    $('#table-hl .lblTong_43').html(sum43);
                    $('#table-hl .lblTong_44').html(sum44);
                    $('#table-hl .lblTong_45').html(sum45);

                    $('#table-hl .lblTong_51').html(sum51);
                    $('#table-hl .lblTong_52').html(sum52);
                    $('#table-hl .lblTong_53').html(sum53);
                    $('#table-hl .lblTong_54').html(sum54);
                    $('#table-hl .lblTong_55').html(sum55);


                    $('#table-hl .lblTong_All_1').html(sum11 + sum21 + sum31 + sum41 + sum51);
                    $('#table-hl .lblTong_All_2').html(sum12 + sum22 + sum32 + sum42 + sum52);
                    $('#table-hl .lblTong_All_3').html(sum13 + sum23 + sum33 + sum43 + sum53);
                    $('#table-hl .lblTong_All_4').html(sum14 + sum24 + sum34 + sum44 + sum54);
                    $('#table-hl .lblTong_All_5').html(sum15 + sum25 + sum35 + sum45 + sum55);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="15">Không có dữ liệu</td></tr>');
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