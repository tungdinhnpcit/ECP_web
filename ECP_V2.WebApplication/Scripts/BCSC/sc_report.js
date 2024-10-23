var sc_thop = {
    currrent_row: null,
    data: null,
    init: function () {
        //this.tables = $('#tblktgs').DataTable(this.optiontable);
        // tao cbo thang
        var html = "";
        for (i = 1; i < 13; ++i) {
            if (i == new Date().getMonth() + 1) {
                html += "<option selected='selected' value=" + i + ">" + i + "</option>";
            } else
                html += "<option value=" + i + ">" + i + "</option>";
        }
        $('#cboThang').html(html);

        //Lấy danh mục đơn vị
        $.ajax({
            type: 'POST',
            url: '/Admin/BCKTGS/getDmDvql',
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                //if type=tba then build select tba, dz build dz
                if (response.length > 0) {
                    $.each(response, function (i, value) {
                        $('#cboDonvi')
                            .append($('<option></option>', { text: value["TenDonVi"] })
                                .attr('value', value["Id"]));
                    });
                }
            },
            error: function (ex) {
                alert('Không thể load danh mục Đơn vị');
            }
        })

    },
    loadData: function () {
        var vthang = $('#cboThang').val();
        var vnam = $('#cboNam').val();
        var madvi = $('#cboDonvi').val();

        $.ajax({
            type: 'POST',
            url: '/Admin/BCSuco_Tonghop/GetSucoThop',
            dataType: 'json',
            data: JSON.stringify({ donvi: madvi, thang: vthang, nam: vnam }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                //if type=tba then build select tba, dz build dz
                $('#tblSucos').dataTable().fnClearTable();
                if (response.length > 0) {
                    $('#tblSucos').DataTable({
                        "language": {
                            "lengthMenu": "Hiển thị _MENU_ dòng mỗi trang",
                            "zeroRecords": "Không có dữ liệu",
                            "info": "Hiển thị _START_ đến _END_ trong _TOTAL_ dòng",
                            "infoEmpty": "",
                            "paginate": {
                                "next": ">>",
                                "previous": "<<"
                            },
                            search: "",
                            searchPlaceholder: "Tìm kiếm...",
                            "processing": "ĐANG XỮ LÝ..."
                        },
                        "destroy": true,
                        "data": response,
                        "columns": [
                            { 'data': 'Stt' },
                            { 'data': 'TenDonVi' },
                            { 'data': 'Dz_tq' },
                            { 'data': 'Dz_kd1' },
                            { 'data': 'Dz_kd2' },
                            { 'data': 'Tba' },
                            { 'data': 'Dz04' },
                            { 'data': 'Tong' },
                            { 'data': 'Mba_bt' },
                            { 'data': 'Mba_tt' },
                            { 'data': 'Mba_tong' }
                        ],
                    });
                }
            },
            error: function (ex) {
                alert('Không thể lấy được danh sách');
            }
        })
    },
    exportData: function () {
        loading('Đang tải dữ liệu...', 1);
        $("#preloader").unbind("click");
        $('#preloader').click(function () {
            unloading();
        })

        var vthang = $('#cboThang').val();
        var vnam = $('#cboNam').val();
        var madvi = $('#cboDonvi').val();

        $.UnifiedExportFile(
            {
                action: "/Admin/BCSuco_Tonghop/Export",
                data: {
                    donvi: madvi,
                    thang: vthang,
                    nam: vnam
                },
                downloadType: 'Progress',
                ajaxLoadingSelector: '#loading'
            });
        unloading();
    }
}

sc_thop.init();