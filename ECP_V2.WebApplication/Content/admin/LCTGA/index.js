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
            MoTa: 'Chờ duyệt'
        },
        {
            Id: 1,
            MoTa: 'Đã duyệt/Chờ nhận lệnh'
        },
        {
            Id: 2,
            MoTa: 'Nhận lệnh'
        },
        {
            Id: 3,
            MoTa: 'Đã hoàn thành'
        },
        {
            Id: 10,
            MoTa: 'Hủy duyệt'
        },
        {
            Id: 11,
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
                        if (item.GhiChu == null) item.GhiChu = '';
                        if (item.GCNhanLenh == null) item.GCNhanLenh = '';
                        if (item.GCDuyet == null) item.GCDuyet = '';
                        let hienthi = '';
                        var ghichu = (item.GhiChu ?? '') + (item.GCNhanLenh ?? '') + (item.GCDuyet ?? '');
                        if (item.IdPhienLV != undefined && item.IdPhienLV > 0) hienthi = 'hide';
                        render += Mustache.render(template, {
                            STT: d,
                            Id: item.Id,
                            DViCPhieu: item.DViCPhieu,
                            SoPhieu: item.SoPhieu,
                            TenNguoiNhap: item.TenNguoiNhap,
                            TenNhanLenh: item.TenNhanLenh,
                            TenNguoiDuyet: item.TenNguoiDuyet,
                            TrangThai: dataTinhTrang.find(x => x.Id == item.TrangThai)?.MoTa,
                            NgayNhap: item.NgayNhap,
                            NgayDuyet: item.NgayDuyet,
                            NgayNhanLenh: item.NgayNhanLenh,
                            TGHoanThanh: item.TGHoanThanh,
                            GhiChu: ghichu,
                            hienthi: hienthi,
                            LinkFile: item.LinkFile
                        });
                        d++;
                    });

                    if (render !== '') {
                        $('#tbl-content').html(render);
                    }
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="12">Không có dữ liệu</td></tr>');
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
    let idlct = 0;
    $('body').on('click', '.btn-view-chitiet', function (e) {
        e.preventDefault();
        idlct = $(this).data('id');
        let donviId = $(this).data('iddv');
        $('#ddPCT').empty();
        $.ajax({
            type: "GET",
            url: "/Admin/LCTGA/GetDsPhieuLV",
            data: { donviId: donviId },
            dataType: "json",
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                response.data.forEach((e) => {
                    $('#ddPCT').append(` 
                    <option value="${e.ID}">${e.SoPhieu} - ${e.NoiDung}</option>
                    `);
                });
               

                $('#modal-history').modal('show');

            },
            error: function (status) {
                javi.notify('Có lỗi xảy ra', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    })

    $('body').on('click', '#btnCapNhatPLV', function (e) {

        $.ajax({
            type: "GET",
            url: "/Admin/LCTGA/UpdatePhienLvLctga",
            data: { Id: idlct, IdPhienLV: $('#ddPCT').val()  },
            dataType: "json",
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                loadData();
                javi.notify('Cập nhật thành công', 'success');

            },
            error: function (status) {
                javi.notify('Có lỗi xảy ra', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });

    });


    var currentSound = null;

    $(document).on("click", ".play-btn", function () {
        var songSrc = $(this).data('linkfile');
        var idvoice = $(this).data('id');


        if (currentSound) {
         
            currentSound.stop();
        }

        currentSound = new Howl({
            src: [songSrc],
            volume: 1.0
        });

        if ($('#playvoice-' + idvoice).hasClass('fa-play')) {
            $('#playvoice-' + idvoice).removeClass('fa-play');
            $('#playvoice-' + idvoice).addClass('fa-stop');
            if (currentSound) {

                currentSound.play();
            }
        } else {
            $('#playvoice-' + idvoice).addClass('fa-play');
            $('#playvoice-' + idvoice).removeClass('fa-stop');
            if (currentSound) {

                currentSound.stop();
            }

        }
      
    });
}