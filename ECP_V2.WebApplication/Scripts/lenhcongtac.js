var currentInputRuning = null;
var sourceIdName = null;

function validateOnlyNumber(evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

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
    console.log("Function() SetSelectionPosition");
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
            var latest_bat = "";

            var diffItem = [];

            jQuery.grep(arrayNew, function (el) {
                if (jQuery.inArray(el, arrayOld) == -1) diffItem.push(el);
            });

            //console.log("arrayOld " + arrayOld);
            //console.log("arrayNew " + arrayNew);
            //console.log("diff " + diffItem);

            if (diffItem != null && diffItem.length > 0) {
                latest_value = diffItem[0];
                latest_text = $(".multiselect-add").find("option[value = '" + latest_value + "']").text();
                if ($(".multiselect-add").find("option[value = '" + latest_value + "']").attr("data-bat") && $(".multiselect-add").find("option[value = '" + latest_value + "']").attr("data-bat").length) {
                    console.log("data-bat");
                    console.log("obj " + $(".multiselect-add").find("option[value = '" + latest_value + "']").html());
                    latest_bat = $(".multiselect-add").find("option[value = '" + latest_value + "']").attr("data-bat");
                }
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
                //console.log("set onlyone " + latest_text + " " + $(currentInputRuning).attr("id"));
                if ($(currentInputRuning).attr("type") == "text") {
                    $(currentInputRuning).attr("value", latest_text);
                    $(currentInputRuning).val(latest_text);
                }
                else {
                    $(currentInputRuning).val(latest_text);
                }

                if ($(currentInputRuning).parents(".parentbat").length && $(currentInputRuning).parents(".parentbat").find(".childbat").length) {
                    console.log("parentbat");
                    $(currentInputRuning).parents(".parentbat").find(".childbat").text(latest_bat + "/5");
                } else {
                    console.log("no parentbat");
                }

                //$(".tentodoithuchien").text(latest_text);
                if ($(currentInputRuning).attr("id") == "txtS13_phongban") {
                    console.log("t2");
                    $(".tentodoithuchien").text(latest_text);
                }

                if ($(currentInputRuning).attr("id") == "txtS12") {
                    console.log("1. txtS12" + latest_text);
                    $(".chihuytructiep_name").each(function () {
                        $(this).text(latest_text);
                    });
                }

                $(currentInputRuning).attr("data-id", latest_value);

                $("select.multiselect-add").val(latest_value);
                $("select.multiselect-add").multiselect("refresh");

                if ($(currentInputRuning).attr("id") == "txtS12_soluongnguoi") {
                    console.log("txtS12_soluongnguoi");
                    addRow2();
                }
            }
        } else {
            if ($(currentInputRuning).parents(".parentbat").length && $(currentInputRuning).parents(".parentbat").find(".childbat").length) {
                console.log("none parentbat");
                $(currentInputRuning).parents(".parentbat").find(".childbat").text("/5");
            }
            else {
                console.log("no parentbat");
            }
        }
    } else {
        if ($(currentInputRuning).parents(".parentbat").length && $(currentInputRuning).parents(".parentbat").find(".childbat").length) {
            console.log("none parentbat");
            $(currentInputRuning).parents(".parentbat").find(".childbat").text("/5");
        }
        else {
            console.log("no parentbat");
        }

        if ($(currentInputRuning).attr("id") == "txtS13_phongban") {
            console.log("txtS13_phongban");
            $(".tentodoithuchien").text("");
        }
        if ($(currentInputRuning).attr("id") == "txtS12_soluongnguoi") {
            console.log("txtS12_soluongnguoi");
            addRow2();
        }

        if ($(currentInputRuning).attr("id") == "txtS12") {
            console.log("txtS12");
            $(".chihuytructiep_name").each(function () {
                $(this).text("");
            });
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

$(document).on("focus", ".LanhDaoTrucBan_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".NguoiChiHuy_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".LanhDaoTrucBan_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".ChiHuyTrucTiep_BAT_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".LoaiCongViec_SelectEmp", function () {
    sourceIdName = $(this).attr("datasourceidname");
    currentInputRuning = $(this);
    SetSelectionPosition($(this).parent());
});

$(document).on("focus", ".PhongBan_SelectEmp", function () {
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

    console.log("NhanVienDonViCongTac");
    console.log(sourceIdName);
    console.log($(this));
    console.log("--------------------------");

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
    if (!$("#txtTenDonVi").val()) {
        alert("Tên đơn vị không được bỏ trống.");
        return;
    }
    console.log("UpdateLenhCongTac()");
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
    //console.log(NhanVienDonViCongTac_SelectEmp);
    if (phieucongtacid > 0 && phienlvid > 0) {
        console.log("POST");
        var jsonData = JSON.stringify(
            {
                "phieucongtacid": phieucongtacid,
                "phienlvid": phienlvid,
                "DiaDiem": $("#txtS13").val(),
                "NoiDung": $("#txtS14").val(),
                "DieuKienAnToan": $("#txtS15").val(),
                "SoNguoiThamGia": $("#txtS12_soluongnguoi").val(),
                "TenDonVi_LCT": $("#txtTenDonVi").val(),
                "NhanVienDonViCongTac_SelectEmp": NhanVienDonViCongTac_SelectEmp,
                "phieucongtac": $("#txtsophieu").val().trim(),
                "LoaiCongViec": loaiCviecId,
                "MaYeuCauCRM": $("#txtMaYeuCauKH").val(),
                "TramId": $("#txtTop_Tram").attr("data-id"),
            });
        console.log(jsonData);
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Admin/PhienLV/UpdateLenhCongTac',
            data: jsonData,
            dataType: "json",
            beforeSend: function () {
            },
            success: function (data) {
                console.log(data.success);

                setTimeout(function () {
                    unloading();
                }, 500);

                if (data.success) {

                    new PNotify({
                        title: 'Thông báo!',
                        text: 'Sửa lệnh làm việc thành công',
                        type: 'success'
                    });
                    location.reload();
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
    //if ($(".NhanVienDonViCongTac_SelectEmp").length < 2) {
    $("#txtS12_soluongnguoi2").val($('#txtS12_soluongnguoi').val());

    $(".tb1 .rtb").remove();
    $(".tb2 .rtb").remove();

    var soluong = parseInt($('#txtS12_soluongnguoi').val().trim());
    console.log(soluong);
    for (var i = 1; i <= soluong; i++) {
        $(".tb1 tbody").append($(".trtemp1 tbody").html());
    }
    var rIndex = 1;
    $(".tb1 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });

    rIndex = 1;
    for (var i = 1; i <= soluong; i++) {
        $(".tb2 tbody").append($(".trtemp2 tbody").html());
    }
    $(".tb2 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });

    $("#divtb1").css("height", $(".tb1").height());
    $("#divtb2").css("height", $(".tb2").height());
    //}
}

$('#txtS12_soluongnguoi').change(function () {
    addRow2();
});

function addRowTB1() {
    $(".tb1 tbody").append($(".trtemp1 tbody").html());
    var rIndex = 1;
    $(".tb1 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb1").css("height", $(".tb1").height());
    addRowTB2();
}
function removeRowTB1() {
    $(".tb1 tbody .rtb:last").remove();
    var rIndex = 1;
    $(".tb1 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    removeRowTB2();
}

function addRowTB2() {
    $(".tb2 tbody").append($(".trtemp2 tbody").html());
    var rIndex = 1;
    $(".tb2 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb2").css("height", $(".tb2").height());
}
function removeRowTB2() {
    $(".tb2 tbody .rtb:last").remove();
    var rIndex = 1;
    $(".tb2 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
}

function addRowTB3() {
    $(".tb3 tbody").append($(".trtemp3 tbody").html());
    var rIndex = 1;
    $(".tb3 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
    $("#divtb3").css("height", $(".tb3").height());
}
function removeRowTB3() {
    $(".tb3 tbody .rtb:last").remove();
    var rIndex = 1;
    $(".tb3 tbody .rtb .rstt").each(function () {
        $(this).text(rIndex);
        rIndex++;
    });
}



$(document)
    .one('focus.autoExpand1', 'textarea.autoExpand1', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;
        this.value = savedValue;
    })
    .on('input.autoExpand1', 'textarea.autoExpand1', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });
$(document)
    .one('focus.autoExpand2', 'textarea.autoExpand2', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;

        this.value = savedValue;
    })
    .on('input.autoExpand2', 'textarea.autoExpand2', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });

$(document)
    .one('focus.autoExpand3', 'textarea.autoExpand3', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;

        this.value = savedValue;
    })
    .on('input.autoExpand3', 'textarea.autoExpand3', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });
$(document)
    .one('focus.autoExpand11', 'textarea.autoExpand11', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;
        this.value = savedValue;
    })
    .on('input.autoExpand11', 'textarea.autoExpand11', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });
$(document)
    .one('focus.autoExpand22', 'textarea.autoExpand22', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;

        this.value = savedValue;
    })
    .on('input.autoExpand22', 'textarea.autoExpand22', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });

$(document)
    .one('focus.autoExpand33', 'textarea.autoExpand33', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;

        this.value = savedValue;
    })
    .on('input.autoExpand33', 'textarea.autoExpand33', function () {
        var countLine = $.countLines(this);
        this.rows = countLine.visual;
    });

$(document).ready(function () {
    $(".tentodoithuchien").text($("#PhongBan_Id").val());

    $("textarea").each(function () {
        if (!$(this).hasClass("noResz")) {
            var countLine = $.countLines(this);
            var minLine = $(this).attr("data-min-rows");
            this.rows = countLine.visual < minLine ? minLine : countLine.visual;
        }
    });
});

$("#txtTenDonVi").change(function () {
    $("#txtTenDonVi2").text($(this).val());
});

$('#txtS11').change(function () {
    $("#txtS12").val($(this).val());
});

$('#txtS13').change(function () {
    $("#txtS132").val($(this).val());
});

$('#txtS14').change(function () {
    $("#txtS142").val($(this).val());
});

$('#txtS15').change(function () {
    $("#txtS152").val($(this).val());
});

$('#txtsophieu').change(function () {
    $("#txtsophieu2").val($(this).val());
});

$('#txtS16_giobd').change(function () {
    $("#txtS16_giobd2").val($(this).val());
});
$('#txtS16_phutbd').change(function () {
    $("#txtS16_phutbd2").val($(this).val());
});
$('#txtS16_ngaybd').change(function () {
    $("#txtS16_ngaybd2").val($(this).val());
});
$('#txtS16_thangbd').change(function () {
    $("#txtS16_thangbd2").val($(this).val());
});
$('#txtS16_nambd').change(function () {
    $("#txtS16_nambd2").val($(this).val());
});

