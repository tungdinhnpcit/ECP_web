var bclsController = function () {

    this.initialize = function () {

        $.when()
            .done(function () {
                $('html').addClass('sidebar-left-collapsed');
                var moduleId = $("input[name='rdbModule']:checked").val();

                loadOptionByModuleId(moduleId);
            });
        registerEvents();
    };

    function loadOptionByModuleId(moduleId) {

    }


    function registerEvents() {

        $('#txtTuNgay').kendoDatePicker({
            start: "date",
            depth: "date",
            format: "dd/MM/yyyy",
            dateInput: true
        });

        $('#txtDenNgay').kendoDatePicker({
            start: "date",
            depth: "date",
            format: "dd/MM/yyyy",
            dateInput: true
        });

        $('body').on('change', '#txtTuNgay')

        $('body').on('change', 'input[name="rdbModule"]', function (e) {
            var moduleId = $("input[name='rdbModule']:checked").val();

            loadOptionByModuleId(moduleId);
        });

        $('body').on('click', '#btnConfirm', function (e) {
            e.preventDefault();
            var moduleId = $("input[name='rdbModule']:checked").val();
            var tungay = $('#txtTuNgay').val();
            var denngay = $('#txtDenNgay').val();

            if (moduleId == null || moduleId == "") {
                javi.notify("Chưa chọn phân hệ");
                $(".list").html("");
                return false;
            }

            if (tungay == "" || tungay == null || denngay == "" || denngay == null) {
                javi.notify("Chưa chọn thời gian");
                $(".list").html("");
                return false;
            }

            let temp1 = tungay.split('/');
            var tn = new Date(temp1[2], temp1[1] - 1, temp1[0]);

            let temp2 = denngay.split('/');
            var dn = new Date(temp2[2], temp2[1] - 1, temp2[0]);

            if (tn > dn) {
                javi.notify("Thời gian không hợp lệ");
                $(".list").html("");
                return false;
            }

            var tt = new Date();
            tt.setDate(1);
            tt.setMonth(tt.getMonth() - 3);

            if (dn > tt) {
                javi.notify("Thời gian không hợp lệ");
                $(".list").html("");
                return false;
            }


            if (moduleId == "SC") {
                Paging_SuCo(1, 50, "");
            }
            else if (moduleId == "CT") {
                Paging_PhieuCongTac(1, 500, "");
            }
            else {
                $(".list").html("");
            }
        })

        $('body').on('click', '#btnCancel', function (e) {
            e.preventDefault();
            var roleId = $("input[name='rdbRole']:checked").val();
            loadMenuByRole(roleId);
        })
    }


    function Paging_SuCo(page, pageSize, filter) {
        loading('Đang tải dữ liệu...', 1);
        $("#preloader").unbind("click");
        $('#preloader').click(function () {
            unloading();
        })

        var scriptUrl = "/Admin/BaoCaoLichSu/List_SuCo";
        $.ajax({
            url: scriptUrl,
            data: {
                page: page,
                pageSize: pageSize,
                filter: "",
                DateFrom: $('#txtTuNgay').val(),
                DateTo: $('#txtDenNgay').val(),
                DonViId: $('#cmbDonViID').val(),
                PhongBanId: "",
                LoaiSuCo: "",
                TinhChat: "",
                NguyenNhan: "",
                TrangThaiNhap: "",
                MienTru: "",
                KienNghi: "",
                TCTDuyetMT: "",
            },
            type: 'GET',
            dataType: 'html',
            async: false,
            success: function (data) {
                $(".list").html("");
                $(".list").html(data);
                unloading();
            },
            error: function () {
                unloading();
            }
        });
    }

    function Paging_PhieuCongTac(page, pageSize, filter, tcphien, catdien, tiepdia, khac, DateFrom, DateTo, DonViId, PhongBanId, chuyenNPC) {
        //if (filter.length) {
        //    if ($("#cmbLoaiTimKiemText").val() == "0") {
        //        filter = "0:" + filter;
        //    } else {
        //        filter = "1:" + filter;
        //    }
        //}
        filter = "0:" + filter;
        //console.log("filter: " + filter);
        loading('Đang tải dữ liệu...', 1);
        //$("#preloader").unbind("click");
        //$('#preloader').click(function () {
        //    ///unloading();
        //})
        var ttPhien = 0;
        var cbTTPhienSelect = 0;
        var userRoleDuyet = '@(Request.IsAuthenticated && User.IsInRole("DuyetViec")) ? "true" : "false")';
        if (userRoleDuyet && cbTTPhienSelect != "") {
            ttPhien = cbTTPhienSelect;
        }
        else {

            ttPhien = 0;
        }

        if (typeof (ttPhien) === 'undefined') {
            ttPhien = 0;
        }

        var lcv = null;

        var scriptUrl = "/Admin/BaoCaoLichSu/ListPhieuCongTac";
        $.ajax({
            url: scriptUrl,
            data: {
                page: page,
                pageSize: pageSize,
                filter: filter,
                tcphien: 0,
                catdien: 0,
                tiepdia: 0,
                khac: 0,
                DateFrom: $('#txtTuNgay').val(),
                DateTo: $('#txtDenNgay').val(),

                DonViId: $('#cmbDonViID').val(),
                PhongBanId: "",
                ttPhien: 0,
                chuyenNPC: -1,
                loaiPhieuLenh: 0,
                loaiCV: ""
            },
            type: 'GET',
            dataType: 'html',
            async: false,
            success: function (data) {
                $(".list").html("");
                $(".list").html(data);

                unloading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                unloading();
                alert("Có lỗi, hãy kiểm tra kết nối internet.");
            }
        });
    }

    function loading(name, overlay) {

        $('body').append('<div id="overlay"></div><div id="preloader" style="height: 40px;">' + name + '..</div>');

        if (overlay == 1) {
            $('#overlay').css('opacity', 0.4).fadeIn(400, function () { $('#preloader').fadeIn(400); });
            return false;
        }
        $('#preloader').fadeIn();
    }
    function unloading() {
        $('#preloader').fadeOut(400, function () { $('#overlay').fadeOut(); $(this).remove(); })
    }
}