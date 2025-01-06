var currentInputRuning = null;
var sourceIdName = null;


function initMultiSelect(isFilter) {
    $('select.multiselect-add').multiselect({
        includeSelectAllOption: false,
        selectAllValue: 'select-all-value',
        enableFiltering: isFilter,
        enableCaseInsensitiveFiltering: isFilter,
        filterPlaceholder: 'Tìm kiếm ...',
        nonSelectedText: 'Check an option!',
        numberDisplayed: 1,
        maxHeight: 400,
    });
}

function SetSelectionPosition(forParent) {
    $("select.multiselect-add").html($("#" + sourceIdName).html());
    $('select.multiselect-add').multiselect('destroy');

    var isFilter = true;
    if ($(forParent).find(".selectbox-control").attr("searchbox") != "1") {
        isFilter = false;
    }
    initMultiSelect(isFilter);

    $("select.multiselect-add").multiselect("clearSelection");

    $(".empdata").appendTo($(forParent).find(".multiselect-wap"));
    setTimeout(function () {
        var attr = $(currentInputRuning).attr("data-id");
        if (typeof attr !== typeof undefined && attr !== false) {
            var splitValue = $(currentInputRuning).attr("data-id");
            $("select.multiselect-add").val(splitValue.split(','));
            $("select.multiselect-add").multiselect("refresh");
        }
        $(".multiselect-wap .btn-group").addClass("open");
        $(".multiselect-wap .btn-default").attr("aria-expanded", "true");
    }, 300);
}

$(document).on("chosen", ".multiselect-add", function () {
    //console.log("chosen");
    //console.log($(this));
});

$(document).on("change", ".multiselect-add", function () {
    var oldSelected = "";
    var arrayOld = null;
    if ($(currentInputRuning).attr("data-id")) {
        console.log($(currentInputRuning).attr("data-id"));
        oldSelected = $(currentInputRuning).attr("data-id");
        arrayOld = oldSelected.split(",");
    } else {
        arrayOld = [];
    }
    $(currentInputRuning).val('');
    $(currentInputRuning).attr("data-id", '');

    if ($(this).parents(".selectboxv1").attr("onlyone") == "1") {
        //console.log("onlyone");
        if ($(this).closest('select').find('option').filter(':selected').length > 0) {
            var arrayNew = $(this).val();
            var latest_value = "";
            var latest_text = "";

            var diffItem = [];

            jQuery.grep(arrayNew, function (el) {
                if (jQuery.inArray(el, arrayOld) == -1) diffItem.push(el);
            });

            //console.log("arrayOld " + arrayOld);
            //console.log("arrayNew " + arrayNew);
            //console.log("diff " + diffItem);

            var latest_value = "";
            var latest_text = "";
            if (diffItem != null && diffItem.length > 0) {
                latest_value = diffItem[0];
                latest_text = $(".multiselect-add").find("option[value = '" + latest_value + "']").text();
            }

            $(currentInputRuning).attr("value", "");
            $(currentInputRuning).val("");
            //console.log("check ID " + $(currentInputRuning).attr("id"));

            if ($(currentInputRuning).attr("id") == "txtS13_phongban") {
                console.log("t1");
                $(".tentodoithuchien").text("");
            }
            if ($(currentInputRuning).attr("id") == "txtS12") {
                console.log("txtS12");
                $(".chihuytructiep_name").each(function () {
                    $(this).text("");
                });
            }

            if (latest_value != "undefined") {
                console.log("set onlyone " + latest_text + " " + $(currentInputRuning).attr("id"));
                if ($(currentInputRuning).attr("type") === "text") {
                    $(currentInputRuning).attr("value", latest_text);
                    $(currentInputRuning).val(latest_text);
                }
                else {
                    $(currentInputRuning).val(latest_text);
                }

                //$(".tentodoithuchien").text(latest_text);
                if ($(currentInputRuning).attr("id") === "txtS13_phongban") {
                    console.log("t2");
                    $(".tentodoithuchien").text(latest_text);
                }

                if ($(currentInputRuning).attr("id") == "txtS12") {
                    console.log("1. txtS12" + latest_text);
                    $(".chihuytructiep_name").each(function () {
                        $(this).text(latest_text);
                    });
                }

                if ($(currentInputRuning).attr("id") == "txtS13_soluongnguoi") {
                    console.log("txtS13_soluongnguoi");
                    addRow2();
                    addRow3();
                }

                $(currentInputRuning).attr("data-id", latest_value);

                $("select.multiselect-add").val(latest_value);
                $("select.multiselect-add").multiselect("refresh");
            }
        }
    } else {
        if ($(currentInputRuning).attr("id") == "txtS13_phongban") {
            console.log("t3");
            $(".tentodoithuchien").text("");
        }
        if ($(currentInputRuning).attr("id") == "txtS12") {
            console.log("txtS12");
            $(".chihuytructiep_name").each(function () {
                $(this).text("");
            });
        }
        if ($(currentInputRuning).attr("id") == "txtS13_soluongnguoi") {
            console.log("txtS13_soluongnguoi");
            addRow2();
            addRow3();
        }

        var newSelect = $(this).val();
        if (newSelect != null) {
            var indexEmp = 0;
            $(newSelect).each(function (eachIndex, curVal) {
                var textSelect = $(".multiselect-add").find("option[value = '" + curVal + "']").text();
                console.log(textSelect);
                if (indexEmp == 0) {
                    console.log(textSelect);
                    $(currentInputRuning).val(textSelect);
                    $(currentInputRuning).attr("data-id", curVal);
                } else {
                    var oldValue = $(currentInputRuning).val();
                    $(currentInputRuning).val(oldValue + ", " + textSelect);
                    $(currentInputRuning).attr("data-id", $(currentInputRuning).attr("data-id") + "," + curVal);
                }

                console.log("trigger " + $(currentInputRuning).attr("id"));
                if ($(currentInputRuning).attr("id") == "txtS13_phongban") {
                    console.log("t4");
                    $(".tentodoithuchien").text(textSelect);
                }
                if ($(currentInputRuning).attr("id") == "txtS12") {
                    console.log("2. txtS12" + textSelect);
                    $(".chihuytructiep_name").each(function () {
                        $(this).text(textSelect);
                    });
                }

                indexEmp++;
            });
        }
    }

});

$(document).on("focus", ".LanhDaoCongViec_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".LanhDaoCongViec_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".loaiBienBan_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NguoiChiHuy_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".loaithoigianbc_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".ChiHuyTrucTiep_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".PhongBan_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".LoaiCongViec_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".SoLuongNguoi_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".GiamSatAnToan_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".GiamSatAnToan_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NguoiDuyet_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NguoiDuyet_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NguoiCapPhieu_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".SoPhieuThang_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".SoPhieuNam_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NhanVienDonViCongTac_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NhanVienDonViCongTac_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".Tram_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

//$(document).on("focus", ".DieuKienAnToan_SelectEmp", function () {
//    sourceIdName = $(this).attr("datasourceidname");
//    currentInputRuning = $(this);
//    SetSelectionPosition($(this).parent());
//});

//$(document).on("focus", ".DonViLienQuan_SelectEmp", function () {
//    sourceIdName = $(this).attr("datasourceidname");
//    currentInputRuning = $(this);
//    SetSelectionPosition($(this).parent());
//});

function setPhongBan() {
    //console.log($(".PhongBan_SelectEmp").val());
    //$(".tentodoithuchien").text($(".PhongBan_SelectEmp").val());
}

$(document).ready(function () {
    $(".tentodoithuchien").text($("#PhongBan_Id").val());
});

/* Add here all your JS customizations */
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

function showError(str) {
    $('#alertMessage').addClass('error').html(str).stop(true, true).show().animate({ opacity: 1, right: '10' }, 500);
}

function showSuccess(str) {
    $('#alertMessage').removeClass('error').addClass("success").html(str).stop(true, true).show().animate({ opacity: 1, right: '10' }, 500);
}
function hideTop() {
    $('#alertMessage').animate({ opacity: 0, right: '-20' }, 500, function () { $(this).hide(); });
}

function stringIsEmpty(value) {

    return value ? value.trim().length === 0 : true;

}

function UpdatePhieuCongTac() {
    console.log("UpdatePhieuCongTac()");
    loading('Đang xử lý...', 1);

    var phieucongtacid = $("#phieucongtacid").val();
    var phienlvid = $("#phienlvid").val();
    console.log(phieucongtacid + " - " + phienlvid);

    var loaiCviecId = $("#txtTop_LoaiCviec").attr("data-id");
    console.log("LCV:" + loaiCviecId);
    if (stringIsEmpty(loaiCviecId)) {
        //console.log("0");
        setTimeout(function () {
            unloading();
        }, 500);

        new PNotify({
            title: 'Thông báo!',
            text: 'Bạn phải chưa chọn loại công việc!',
            type: 'error'
        });
        return;
    }

    var NhanVienDonViCongTac_SelectEmp = "";
    $(".NhanVienDonViCongTac_SelectEmp").each(function () {
        if (NhanVienDonViCongTac_SelectEmp != "") {
            NhanVienDonViCongTac_SelectEmp = NhanVienDonViCongTac_SelectEmp + "*" + $(this).val() + "|" + $(this).attr("data-id");
            console.log("TenNV: " + $(this).val());
        } else {
            NhanVienDonViCongTac_SelectEmp = $(this).val() + "|" + $(this).attr("data-id");
            console.log("TenNV: " + $(this).val());
        }
    });

    if (phieucongtacid > 0 && phienlvid > 0) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Admin/PhienLV/UpdatePhieuCongTac',
            dataType: "json",
            data:
                JSON.stringify({
                    model:
                    {
                        "phieucongtacid": phieucongtacid,
                        "phienlvid": phienlvid,
                        //"phieucongtac": $("#txtsophieu").val().trim() + "/" + $("#txtsophieu_thang").val() + "/" + $("#txtsophieu_nam").val(),
                        "phieucongtac": $("#txtsophieu").val().trim(),

                        "LanhDaoCongViec": $("#txtS11").val(),
                        "LanhDaoCongViec_Id": $("#txtS11").attr("data-id"),

                        "NguoiChiHuy": $("#txtS12").val(),
                        "NguoiChiHuy_Id": $("#txtS12").attr("data-id"),

                        "GiamSatVien": $("#txtS19").val(),
                        "GiamSatVien_Id": $("#txtS19").attr("data-id"),

                        "NguoiCapPhieu": $("#txtS110_nguoicapphieu").val(),
                        "NguoiCapPhieu_Id": $("#txtS110_nguoicapphieu").attr("data-id"),

                        "NguoiKiemSoat": $("#txtS110_nguoichophep").val(),
                        "NguoiKiemSoat_Id": $("#txtS110_nguoichophep").attr("data-id"),

                        "NguoiKiemTraPhieu": $("#txtS62_nguoicapphieu").val(),
                        "NguoiKiemTraPhieu_Id": $("#txtS62_nguoicapphieu").attr("data-id"),

                        "PhongBanID": $("#txtS13_phongban").attr("data-id"),
                        "DiaDiem": "", // $("#txtS14").val(),
                        "NoiDung": "", //$("#txtS15").val(),
                        "GioBd": $("#txtS16_giobd").val() + ":" + $("#txtS16_phutbd").val() + " " + $("#txtS16_ngaybd").val() + "/" + $("#txtS16_thangbd").val() + "/" + $("#txtS16_nambd").val(),
                        "GioKt": $("#txtS16_giokt").val() + ":" + $("#txtS16_phutkt").val() + " " + $("#txtS16_ngaykt").val() + "/" + $("#txtS16_thangkt").val() + "/" + $("#txtS16_namkt").val(),

                        "DieuKienAnToan": $("#txtS17").val(),
                        "DonViQLVHLienQuan": $("#txtS18").val(),

                        "ChiTietCatDien": $("#txtS22").val(),
                        "ChiTietNoiDat": $("#txtS221").val(),
                        "ChiTietRaoChan": $("#txtS222").val(),
                        "ChiTietBienBao": $("#txtS223").val(),
                        "PhamViLamViec": $("#txtS23").val(),
                        "NoiDatTai": $("#txtS31").val(),
                        "AnToanKhac": $("#txtS32").val(),
                        "NhanVienDonViCongTac_SelectEmp": NhanVienDonViCongTac_SelectEmp,
                        "NgayGioKT_B2": $("#txtS25_gio").val() + ":" + $("#txtS25_phut").val() + " " + $("#txtS25_ngay").val() + "/" + $("#txtS25_thang").val() + "/" + $("#txtS25_nam").val(),
                        "NgayGioKT_B3": $("#txtS33_gio").val() + ":" + $("#txtS33_phut").val() + " " + $("#txtS33_ngay").val() + "/" + $("#txtS33_thang").val() + "/" + $("#txtS33_nam").val(),

                        "DonViLienQuanQLVH": $("#txtS18").val(),
                        "ChiTietCatDien": $("#txtS22").val(),
                        "ChiTietNoiDat": $("#txtS221").val(),
                        "ChiTietRaoChan": $("#txtS222").val(),
                        "ChiTietBienBao": $("#txtS223").val(),
                        "PhamViLamViec": $("#txtS23").val(),
                        "CanhBaoNguyHiem": $("#txtS24").val(),
                        "SoNguoiThamGia": $("#txtS13_soluongnguoi").val(),
                        "PhongBanID_PCT": $("#txtTop_phongban").attr("data-id"),
                        "PhongBan_PCT": $("#txtTop_phongban").val(),
                        "LoaiCongViec": loaiCviecId,
                        "MaYeuCauCRM": $("#txtMaYeuCauKH").val(),
                        "TramId": $("#txtTop_Tram").attr("data-id"),
                        //"TT_Phien": TT_Phien,
                        //"TrangThai": $("#TrangThai_" + id).val(),

                        //"NgayLamViec": NgayLamViec,

                        //"NgayKt": $("#NgayKt_" + id).val(),
                        //"NguoiDuyet_SoPa": $("#NguoiDuyet_SoPa_" + id).val(),

                        //"GiamSatVien": $("#GiamSatVien_" + id).val(),
                        //"NguoiKiemSoat": $("#NguoiKiemSoat_" + id).val(),
                        //"NguoiKiemTraPhieu": $("#NguoiKiemTraPhieu_" + id).val(),
                        //"LanhDaoTrucBan": $("#LanhDaoTrucBan_" + id).val(),
                        //"LyDoThayDoi": $("#LyDoThayDoi_" + id).val(),

                        //"NguoiDuyet_SoPa_Id": $("#NguoiDuyet_SoPa_" + id).attr("data-id"),

                        //"GiamSatVien_Id": $("#GiamSatVien_" + id).attr("data-id"),
                        //"NguoiKiemSoat_Id": $("#NguoiKiemSoat_" + id).attr("data-id"),
                        //"NguoiKiemTraPhieu_Id": $("#NguoiKiemTraPhieu_" + id).attr("data-id"),
                        //"LanhDaoTrucBan_Id": $("#LanhDaoTrucBan_" + id).attr("data-id"),
                        //"PhieuLenh": $("#PhieuLenh_" + id).attr("data-id"),
                        //"CatDien": $("#CatDien_" + id).attr("data-id") || 0,
                        //"TiepDia": $("#TiepDia_" + id).attr("data-id") || 0,
                        //"TinhChat": $("#TinhChat_" + id).attr("data-id") || 0
                    }
                }),
            dataType: "json",
            beforeSend: function () {//alert(id);
            },
            success: function (data) {
                console.log(data.success);

                setTimeout(function () {
                    unloading();
                }, 500);

                if (data.success) {

                    if (data.responseText == 'Số phiếu công tác đã tồn tại.') {
                        new PNotify({
                            title: 'Thông báo!',
                            text: data.responseText,
                            type: 'error'
                        });
                    } else {
                        new PNotify({
                            title: 'Thông báo!',
                            text: 'Sửa phiếu công tác thành công',
                            type: 'success'
                        });
                    }

                }
                else {

                    new PNotify({
                        title: 'Thông báo!',
                        text: 'Lỗi : ' + data.responseText,
                        type: 'error'
                    });


                }


            }

        });
    } else {
        console.log("0");
        setTimeout(function () {
            unloading();
        }, 500);

        new PNotify({
            title: 'Thông báo!',
            text: 'Dữ liệu chưa đầy đủ',
            type: 'error'
        });
    }
}

function EditLengthTime(obj, min, max) {
    if ($(obj).val()) {
        var timeVal = parseInt($(obj).val());
        if (timeVal < min) {
            $(obj).val("0" + min);
        } else if (timeVal > max) {
            $(obj).val(max);
        }
        else if (timeVal < 10) {
            $(obj).val("0" + timeVal);
        }
    }
}

function AddRow1() {
    $("#tb4").append($(".trtemp1").html());
}

function AddRowTiepDia() {
    console.log($(".trtemp_tiepdia tbody").html());
    $(".tbtiepdia tbody").append($(".trtemp_tiepdia tbody").html());
    var indexOrder = 0;
    $(".tiepdia_stt").each(function () {
        indexOrder++;
        $(this).val(indexOrder);
    });
}

function SaveTiepDia() {
    var phieucongtacid = $("#phieucongtacid").val();
    var props = { "model": [], "pctId": phieucongtacid };
    $(".trtiepdia").each(function () {
        props.model.push({ "Id": "0", "MaPCT": $(this).attr("data-mapct"), "STT": $(this).find(".tiepdia_stt").val(), "Ten": $(this).find(".tiepdia_ten").val().trim(), "NoiDung": $(this).find(".tiepdia_noidung").val().trim() });
    });
    console.log(props.model);
    $.ajax({
        url: '/Admin/PhienLV/UpdateTiepDia',
        type: 'POST', // add this
        data: JSON.stringify(props),
        contentType: "application/json; charset=utf-8",
        success: function (dt) {
            console.log(dt);
        }
    });
}

function RemoveTiepDia(obj) {
    $(obj).parent().parent().remove();
    var indexOrder = 0;
    $(".tiepdia_stt").each(function () {
        indexOrder++;
        $(this).val(indexOrder);
    });
}


function AddRowQLVH() {
    console.log($(".trtemp_qlvh tbody").html());
    $(".tbqlvh tbody").append($(".trtemp_qlvh tbody").html());
    var indexOrder = 0;
    $(".qlvh_stt").each(function () {
        indexOrder++;
        $(this).val(indexOrder);
    });
}

function SaveQLVH() {
    var phieucongtacid = $("#phieucongtacid").val();
    var props = { "model": [], "pctId": phieucongtacid };
    $(".trqlvh").each(function () {
        props.model.push({ "Id": "0", "MaPCT": $(this).attr("data-mapct"), "STT": $(this).find(".qlvh_stt").val(), "Ten": $(this).find(".qlvh_ten").val().trim(), "NoiDung": $(this).find(".qlvh_noidung").val().trim() });
    });
    console.log(props.model);
    $.ajax({
        url: '/Admin/PhienLV/UpdateQLVH',
        type: 'POST', // add this
        data: JSON.stringify(props),
        contentType: "application/json; charset=utf-8",
        success: function (dt) {
            console.log(dt);
        }
    });
}

function RemoveQLVH(obj) {
    $(obj).parent().parent().remove();
    var indexOrder = 0;
    $(".qlvh_stt").each(function () {
        indexOrder++;
        $(this).val(indexOrder);
    });
}
function addRow2() {
    $("#tb4 .rtb").remove();

    var soluong = parseInt($('#txtS13_soluongnguoi').val().trim());
    console.log(soluong);
    for (var i = 1; i <= soluong; i++) {
        $("#tb4 tbody").append($(".trtemp4 tbody").html());
    }
    var rIndex = 1;
    $("#tb4 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb4").css("height", $("#tb4").height());
}

function addRow3() {
    $("#tb5 .rtb").remove();

    var soluong = parseInt($('#txtS13_soluongnguoi').val().trim());
    console.log(soluong);
    for (var i = 1; i <= soluong; i++) {
        $("#tb5 tbody").append($(".trtemp5 tbody").html());
    }
    var rIndex = 1;
    $("#tb5 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb5").css("height", $("#tb5").height());
}

$('#txtS13_soluongnguoi').change(function () {
    addRow2();
    addRow3();
});

function addRowTB4() {
    $("#tb4 tbody").append($(".trtemp4 tbody").html());
    var rIndex = 1;
    $("#tb4 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb4").css("height", $("#tb4").height());
}

function removeRowTB4() {
    $("#tb4 tbody .rtb:last").remove();
    var rIndex = 1;
    $("#tb4 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
}

function addRowTB5() {
    $("#tb5 tbody").append($(".trtemp5 tbody").html());
    var rIndex = 1;
    $("#tb5 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb5").css("height", $("#tb5").height());
}
function removeRowTB5() {
    $("#tb5 tbody .rtb:last").remove();
    var rIndex = 1;
    $("#tb5 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
}

$('#txtTop_phongban').change(function () {
    $(this).attr("data-id", "");
});