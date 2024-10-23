var sc_chitiet = {
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
            url: '/Admin/BCSuco_Chitiet/GetSucoChitiet',
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
                        scrollX: true,
                        columnDefs: [
                            { width: 200, targets: 1 }
                        ],
                        "destroy": true,
                        "data": response,
                        "columns": [
                            {'data': 'Stt'},
                            { 'data': 'TenDvi' },
                            //{ 'data': 'Sc_hlang', render: $.fn.dataTable.render.number(',', '.', 3, '') },
                            { 'data': 'Sc_hlang' },
                            { 'data': 'Sc_tba_mba' },
                            { 'data': 'Sc_tba_cs' },
                            { 'data': 'Sc_tba_fco' },
                            { 'data': 'Sc_tba_cdao' },
                            { 'data': 'Sc_tba_tupp' },
                            { 'data': 'Sc_tba_su' },
                            { 'data': 'Sc_tba_khac' },
                            { 'data': 'Sc_dz_su' },
                            { 'data': 'Sc_dz_mc' },
                            { 'data': 'Sc_dz_dday' },
                            { 'data': 'Sc_dz_tuleo' },
                            { 'data': 'Sc_dz_dcat' },
                            { 'data': 'Sc_dz_cs' },
                            { 'data': 'Sc_dz_tu' },
                            { 'data': 'Sc_dz_khac' },
                            { 'data': 'Sc_dz_kxd' },
                            { 'data': 'Sc_tskh' },
                            { 'data': 'Sc_tong' }
                        ],                        
                    });
                }
            },
            error: function (ex) {
                alert('Không thể lấy được danh sách');
            }
        })
    },
    exportData:function () {
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
                action: "/Admin/BCSuco_Chitiet/Export",
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

sc_chitiet.init();