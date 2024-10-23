function LoadDmNhomKtra() {
    //type: tba or dz
    $.ajax({
        type: 'POST',
        url: '/Admin/ApprovePlan/getDmNhomKtr',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    //if (value["typeid"] == 'DUONGDAY') {
                    //    $('#cbLoaiDZ')
                    //        .append($('<option></option>', { text: value["ten_loaiktr"] })
                    //            .attr('value', value["id_loaiktr"]));
                    //} else {

                    $('#cboLoaiKT')
                        .append($('<option></option>', { text: value["ten_loai_ktr"] })
                            .attr('value', value["ma_loai_ktr"]));
                    //}
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh mục Nhóm kiểm tra');
        }
    })
}

function LoadDmDonVi() {
    $.ajax({
        type: 'POST',
        url: '/Admin/ApprovePlan/GetDmDonvi',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cbDvvh')
                        .append($('<option></option>', { text: value["TenDonVi"] })
                            .attr('value', value["Id"]));
                });

                $('#cbDvvh option')[0].selected = true;
                //Load Dsach đội thuộc Đơn vị
                //loadDsDoi();
            }


        },
        error: function (ex) {
            alert('Không thể load danh mục Đội vận hành');
        }
    })
}

function loadDsDoi() {
    //let pdonvi = $('#cbDvvh').val();
    $.ajax({
        type: 'POST',
        url: '/Admin/ApprovePlan/getDmDoi',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                $('#cbDoi').empty();
                $.each(response, function (i, value) {
                    $('#cbDoi')
                        .append($('<option></option>', { text: value["TenPhongBan"] })
                            .attr('value', value["Id"]));
                });

                $('#cbDoi option')[0].selected = true;
                getLstAssetByTeamId();
            }


        },
        error: function (ex) {
            alert('Không thể load danh mục Đội vận hành');
        }
    })
}

//Hàm lấy danh sách asset đc phân công bởi đội
function getLstAssetByTeamId() {
    doiid = $('#cbDoi').val();
    thang = $('#cbThang').val();
    nam = $('#cbNam').val();
    manhom = $('#cboLoaiKT').val();
    $.ajax({
        type: "POST",
        url: "/Admin/ApprovePlan/getLstAssetByTeamId",
        data: JSON.stringify({
            doiid: doiid,
            thang: thang,
            nam: nam
        }),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var r = JSON.stringify(data);
            var lstAsset = "{\"lst\":" + r + "}";
        }
    });
}

function fnNote(assetid) {
    //set value to input hidden
    $('#assetSelect').val(assetid);
    $('#txtGhichu').val('');
    $('#mdAddLocation').modal('show');
}

function fnSetColumn() {
    var x = $('#assetSelect').val();
    var ghichu = $('#txtGhichu').val();
    var table = $('#tblSelected').DataTable();
    var data = table.rows().data();

    data.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        if (data[2] == x) {
            data[5] = ghichu;
            this.row(rowIdx).data(data);
            table.draw();
        }

    });
}