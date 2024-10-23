function ChiTietDiemTru(id) {
    console.log("Chi tiet cham diem " + id);
    $('#chitietchamdiembody').load('/Admin/Report/ChiTietChamDiem?id=' + id);
    $.magnificPopup.open({
        items: {
            src: '#modalAnimDetail'
        },
        type: 'inline'
    });
}

function ModalDismissDetail() {
    $.magnificPopup.close();
}

$(document).on('click', ".rError", function () {
    console.log($(this).html());
    ChiTietDiemTru($(this).attr("data-id"));
});

$(document).ready(function () {
    var listDiemTru = [];

    $("td p.diemso").each(function () {
        if ($(this).attr("class") != "" && $(this).attr("class") != "undefined") {
            var nameClass = $(this).attr("class").replace("diemso ", "");
            var findRow = listDiemTru.find(x => x.class == nameClass);
            if (findRow && findRow.class) {
                console.log(findRow.class + "_" + findRow.diemso);
                $("." + findRow.class).text(findRow.diemso).parent().parent().addClass("rowError").attr("data-toggle", 'tooltip').attr("data-html", 'true').attr("title", "Click để xem chi tiết vi phạm");
            } else {
                $("." + nameClass).parent().parent().attr("style", "display:none");
            }
        }
    });
});

