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
    var dsNV;
    loadDSNV();
    function loadDSNV() {
        $.ajax({
            type: "GET",
            url: "/Admin/LCTGA/GetDSNhanVien",
            dataType: "json",
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                if (response.data) {
                    dsNV = response.data;

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
    }
    function formatdate(datestring) {
        try {
            // Extract the milliseconds from the string
            const milliseconds = parseInt(datestring.match(/\d+/)[0], 10);

            // Create a new Date object
            const date = new Date(milliseconds);

            // Format the date to dd/MM/yyyy hh:mm:ss
            return formattedDate = date.toLocaleString('en-GB', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false // Use 24-hour format
            });
        }
        catch {
            return '';
        }

    }
    $('[data-gallery="photoviewer"]').click(function (e) {
        e.preventDefault();
        var items = [],
            options = {
                index: $(this).index(),
                appendTo: '.ChiTietLCTGA',
            };
        $('[data-gallery=photoviewer]').each(function () {
            items.push({
                src: $(this).attr('href'),
                title: $(this).attr('data-title'),
            });
        });
        new PhotoViewer(items, options);
    });
    $('body').on('click', '.btn-view-chitiet', function (e) {
        e.preventDefault();
        var that = $(this).data('id');
        $('#thuchien-b1').empty();
        $('#thuchien-b2').empty();
        $('#thuchien-b3').empty();
        $('#thuchien-b4').empty();
        $('#thuchien-b5').empty();
        $('#thuchien-b6').empty();
        $.ajax({
            type: "GET",
            url: "/Admin/LCTGA/GetChiTietLCT",
            data: { LCTGhiAmId: that },
            dataType: "json",
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                $('#thuchien-b1').empty();
                $('#thuchien-b2').empty();
                $('#thuchien-b3').empty();
                $('#thuchien-b4').empty();
                $('#thuchien-b5').empty();
                $('#thuchien-b6').empty();
                $('#thuchien-b1').removeClass('buoc-dangth-lctga');
                $('#thuchien-b2').removeClass('buoc-dangth-lctga');
                $('#thuchien-b3').removeClass('buoc-dangth-lctga');
                $('#thuchien-b4').removeClass('buoc-dangth-lctga');
                $('#thuchien-b5').removeClass('buoc-dangth-lctga');
                $('#thuchien-b6').removeClass('buoc-dangth-lctga');

                if (response.data.Id > 0) {

                    var template = $('#template-history').html();
                    var render = "";
                    var itemb1 = response.data;
                    var itemb2 = response.data.ThucHien?.find(x => x.Buoc == 2 && x.TrangThai == 1);
                    var itemb3 = response.data.ThucHien?.find(x => x.Buoc == 3 && x.TrangThai == 1);
                    var itemb4 = response.data.ThucHien?.find(x => x.Buoc == 4 && x.TrangThai == 1);
                    var itemb5 = response.data.ThucHien?.find(x => x.Buoc == 5 && x.TrangThai == 1);
                    $('#thuchien-b1').append(`
                          <div class="col-lg-12 css-tieudebuoc">
                               <b>Bước 1: </b> Tạo lệnh 
                            </div>
                            <div class="col-lg-12">
                               <b>Người ra lệnh:</b> ${dsNV.find(x => x.Id == itemb1.NguoiNhap)?.TenNhanVien}
                            </div>
                            <div class="col-lg-12">
                               <b>Người nhận lệnh: </b> ${dsNV.find(x => x.Id == itemb1.NguoiNhanLenh)?.TenNhanVien}
                            </div>
                            <div class="col-lg-12">
                             <b>  File Ghi Âm:</b>
                            <a href="${itemb1.LinkFile}">
                                <i class="fa fa-microphone mic-link-ghiam" aria-hidden="true"></i>
                                </a>
                            </div>
                    `)
                    if (itemb2) {
                        $('#thuchien-b2').append(`
                      <div class="col-lg-12 css-tieudebuoc">
                           <b>Bước 2:</b> Xác nhận lệnh
                        </div>
                        <div class="col-lg-12">
                            <b>Người nhận lệnh:</b> ${dsNV.find(x => x.Id == itemb2.NguoiNhap)?.TenNhanVien}
                        </div>
                        <div class="col-lg-12">
                            <b>Thời gian nhận lệnh:</b> ${formatdate(itemb2.NgayNhap)}
                        </div>
                     `)
                    } else {
                        $('#thuchien-b2').addClass('buoc-dangth-lctga');
                        $('#thuchien-b2').append(`
                        <div class="col-lg-12 css-tieudebuoc">
                            <b>Bước 2:</b> Xác nhận lệnh
                        </div>
                       
                        `)
                    }
                    if (itemb3) {
                        $('#thuchien-b3').append(`
                       <div class="col-lg-12 css-tieudebuoc">
                            <b>Bước 3:</b> Lấy tọa độ hiện trường
                        </div>
                        <div class="col-lg-12">
                            <b>Người thực hiện:</b> ${dsNV.find(x => x.Id == itemb3.AnhHT[0]?.NguoiNhap)?.TenNhanVien}
                        </div>
                        <div class="col-lg-12">
                            <b>Thời gian cập nhật:</b> ${formatdate(itemb3.AnhHT[0]?.NgayNhap)}
                        </div>
                        <div class="col-lg-12" style="display: flex;"  >
                            <b style="padding-right: 0.5rem;">Vị trí:</b>
                            <p> <a target="_blank" href="http://maps.google.com/maps?q=${itemb3.AnhHT[0]?.ToaDoLat},${itemb3.AnhHT[0]?.ToaDoLong}(My+Point)&z=14&ll=${itemb3.AnhHT[0]?.ToaDoLat},${itemb3.AnhHT[0]?.ToaDoLong}">Xem bản đồ vị trí</a></p>
                        </div>
                     `)
                    } else {
                        $('#thuchien-b3').addClass('buoc-dangth-lctga');
                        $('#thuchien-b3').append(`
                        <div class="col-lg-12 css-tieudebuoc">
                           <b>Bước 3:</b> Lấy tọa độ hiện trường
                        </div>
                       
                        `)
                    }
                    if (itemb4) {

                        $('#thuchien-b4').append(`
                       <div class="col-lg-12 css-tieudebuoc">
                            <b>Bước 4:</b> Ảnh CBNV thực hiện
                        </div>
                        <div class="col-lg-12">
                            <b>Thời gian cập nhật:</b> ${formatdate(itemb4.AnhHT[0]?.NgayNhap)}
                        </div>
                         <div class="col-lg-12">
                            <b>Mô tả:</b> ${itemb4.AnhHT[0]?.MoTa??''}
                        </div>
                        <div class="col-lg-12">
                            <div id="image-set-4" class="image-set">
                            </div>
                        </div>
                     `)
                    }
                    else {
                        $('#thuchien-b4').addClass('buoc-dangth-lctga');
                        $('#thuchien-b4').append(`
                        <div class="col-lg-12 css-tieudebuoc">
                        <b>Bước 4:</b> Ảnh CBNV thực hiện
                        </div>
                       
                        `)
                    }
                    if (itemb5) {
                        $('#thuchien-b5').append(`
                      <div class="col-lg-12 css-tieudebuoc">
                           <b>Bước 5:</b> Ảnh ảnh hiện trường
                        </div>
                        <div class="col-lg-12">
                            <b>Thời gian cập nhật:</b> ${formatdate(itemb5.AnhHT[0]?.NgayNhap)}
                        </div>
                        <div class="col-lg-12">
                            <b>Mô tả:</b> ${itemb5.AnhHT[0]?.MoTa ?? ''}
                        </div>
                        <div class="col-lg-12">
                            <div id="image-set-5" class="image-set">
                            </div>
                        </div>
                     `)
                    }
                    else {
                        $('#thuchien-b5').addClass('buoc-dangth-lctga');
                        $('#thuchien-b5').append(`
                        <div class="col-lg-12 css-tieudebuoc">
                          <b>Bước 5:</b> Ảnh ảnh hiện trường
                        </div>
                       
                        `)
                    }
                    if (itemb1.TGHoanThanh && itemb1.TrangThai == 3) {
                        $('#thuchien-b6').append(`
                      <div class="col-lg-12 css-tieudebuoc">
                           <b>Bước 6:</b> Xác nhận hoàn thành
                        </div>
                        <div class="col-lg-12">
                            <b>Người thực hiện:</b>  ${dsNV.find(x => x.Id == itemb1.NguoiNhanLenh)?.TenNhanVien}
                        </div>
                        <div class="col-lg-12">
                            <b>Thời gian cập nhật:</b>  ${formatdate(itemb1.TGHoanThanh)}
                        </div>
                     `)
                    } else {
                        $('#thuchien-b6').addClass('buoc-dangth-lctga');
                        $('#thuchien-b6').append(`
                        <div class="col-lg-12 css-tieudebuoc">
                          <b>Bước 6:</b> Xác nhận hoàn thành
                        </div>
                       
                        `)
                    }


                    if (itemb4?.AnhHT?.length > 0) {
                        $('#image-set-4').empty();
                        itemb4.AnhHT.map(el => {
                            var src = el.LinkFile + '';
                            src = src.replaceAll(' ', '%20');
                            $('#image-set-4').append(` <a data-gallery="photoviewer" data-title="Ảnh Hiện Trường" data-group="a"
                               href=${src}>
                                <img width = "10%" target="_blank" height = "10%" src=${src} alt="">
                            </a>`);
                        });
                        $('#image-set-4 [data-gallery="photoviewer"]').click(function (e) {
                            e.preventDefault();
                            var items = [],
                                options = {
                                    index: $(this).index(),
                                    appendTo: '.ChiTietLCTGA',
                                };
                            $('#image-set-4 [data-gallery=photoviewer]').each(function () {
                                items.push({
                                    src: $(this).attr('href'),
                                    title: $(this).attr('data-title'),
                                });
                            });
                            new PhotoViewer(items, options);
                        });
                    }
                    else {
                        $('#image-set-4').empty();
                    }

                    if (itemb5?.AnhHT?.length > 0) {
                        $('#image-set-5').empty();
                        itemb5.AnhHT.map(el => {
                            var src = el.LinkFile + '';
                            src = src.replaceAll(' ', '%20');
                            $('#image-set-5').append(` <a data-gallery="photoviewer" data-title="Ảnh Hiện Trường" data-group="b"
                               href=${src}>
                                <img width = "10%" target="_blank"  height = "10%" src=${src} alt="">
                            </a>`);
                        });
                        $('#image-set-5  [data-gallery="photoviewer"]').click(function (e) {
                            e.preventDefault();
                            var items = [],
                                options = {
                                    index: $(this).index(),
                                    appendTo: '.ChiTietLCTGA',
                                };
                            $('#image-set-5  [data-gallery=photoviewer]').each(function () {
                                items.push({
                                    src: $(this).attr('href'),
                                    title: $(this).attr('data-title'),
                                });
                            });
                            new PhotoViewer(items, options);
                        });
                     
                    }
                    else {
                        $('#image-set-5').empty();
                    }

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