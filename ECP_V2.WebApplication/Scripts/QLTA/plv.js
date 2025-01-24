function loaddscty() {
    let cty = $("#cboCty").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/PhienLV/getDsDvi',
        dataType: 'json',
        data: JSON.stringify({ madvql: cty, kieudv: 'CTY' }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboCty').empty();     
            $('#cboCty').append($('<option>', {
                value: "",
                text: '-- Chọn công ty --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboCty')
                        .append($('<option></option>', { text: value["TenDonVi"] })
                            .attr('value', value["Id"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Công ty');
        }
    })
}

function loaddsdluc() {
    let cty = $("#cboCty").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/PhienLV/getDsDvi',
        dataType: 'json',
        data: JSON.stringify({ madvql: cty, kieudv: 'DLC' }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboDluc').empty();
            $('#cboDluc').append($('<option>', {
                value: "",
                text: '-- Chọn điện lực --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboDluc')
                        .append($('<option></option>', { text: value["TenDonVi"] })
                            .attr('value', value["Id"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Điện lực');
        }
    })
}

function loaddsnvien() {
    let cty = $("#cboCty").val();
    let dluc = $("#cboDluc").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/PhienLV/getDsNvien',
        dataType: 'json',
        data: JSON.stringify({ cty: cty, dluc: dluc }),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#cboNvien').empty();
            $('#cboNvien').append($('<option>', {
                value: "",
                text: '-- Chọn nhân viên --'
            }));
            if (response.length > 0) {
                $.each(response, function (i, value) {
                    $('#cboNvien')
                        .append($('<option></option>', { text: value["TenNhanVien"] })
                            .attr('value', value["Id"]));
                });
            }
        },
        error: function (ex) {
            alert('Không thể lấy danh sách Điện lực');
        }
    })
}