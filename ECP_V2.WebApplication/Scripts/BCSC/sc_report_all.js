var sc_all = {
    currrent_row: null,
    data: null,
    init: function () {
        //this.tables = $('#tblktgs').DataTable(this.optiontable);
        $('#cboLoai').val('1');
        var now = new Date();

        var day = ("0" + now.getDate()).slice(-2);
        var month = ("0" + (now.getMonth() + 1)).slice(-2);

        var today = now.getFullYear() + "-" + (month) + "-" + (day);

        $('#NgayBc').val(today);
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
        var vloai = $('#cboLoai').val();
        var vngay = $('#NgayBc').val();
        var vtuan = $('#cboWeek').val();
        var vthang = $('#cboThang').val();
        var vnam = $('#cboNam').val();
        var madvi = $('#cboDonvi').val();

        $.ajax({
            type: 'POST',
            url: '/Admin/BCSuco_ThNam/GetSucoXtuyen',
            dataType: 'json',
            data: JSON.stringify({ donvi: madvi, loai: vloai, ngay: vngay, tuan:vtuan, thang: vthang, nam: vnam}),
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
                        columnDefs: [
                            { width: 200, targets: 1 },
                            { width: 200, targets: 2 }
                        ],
                        "data": response,
                        "columns": [
                            { 'data': 'Stt' },
                            { 'data': 'TenLo' },
                            { 'data': 'TenDonVi' },
                            { 'data': 'sc_t1' },
                            { 'data': 'sc_t2' },
                            { 'data': 'sc_t3' },
                            { 'data': 'sc_t4' },
                            { 'data': 'sc_t5' },
                            { 'data': 'sc_t6' },
                            { 'data': 'sc_t7' },
                            { 'data': 'sc_t8' },
                            { 'data': 'sc_t9' },
                            { 'data': 'sc_t10' },
                            { 'data': 'sc_t11' },
                            { 'data': 'sc_t12' },
                            { 'data': 'sc_tong' },
                            { 'data': 'sc_cky' },
                            { 'data': 'sc_ss_cky' }
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
        var vloai = $('#cboLoai').val();
        var vngay = $('#NgayBc').val();
        var vtuan = $('#cboWeek').val();

        $.UnifiedExportFile(
            {
                action: "/Admin/BCSuco_ThNam/Export",
                data: {
                    donvi: madvi,
                    loai: vloai,
                    ngay: vngay,
                    tuan: vtuan,
                    thang: vthang,
                    nam: vnam
                },
                downloadType: 'Progress',
                ajaxLoadingSelector: '#loading'
            });
        unloading();
    },
    hideInput: function () {
        var vLoai = $('#cboLoai').val();
        var cbThang = document.getElementById('cboThang');
        var lbThang = document.getElementById('lbThang');
        var cbWeek = document.getElementById('cboWeek');
        var lbWeek = document.getElementById('lbWeek');
        var cbNam = document.getElementById('cboNam');
        var lbNam = document.getElementById('lbNam');
        var NgayBc = document.getElementById('NgayBc');
        var lbDay = document.getElementById('lbNgay');

        if (vLoai == '1') {
            cbWeek.style.display = 'none';
            lbWeek.style.display = 'none';

            cbThang.style.display = 'none';
            lbThang.style.display = 'none';

            cbNam.style.display = 'none';
            lbNam.style.display = 'none';

            NgayBc.style.display = 'block';
            lbDay.style.display = 'block';
        } else if (vLoai == '2') {
            cbWeek.style.display = 'block';
            lbWeek.style.display = 'block';

            cbThang.style.display = 'none';
            lbThang.style.display = 'none';

            cbNam.style.display = 'block';
            lbNam.style.display = 'block';

            NgayBc.style.display = 'none';
            lbDay.style.display = 'none';
        } else if (vLoai == '3') {
            cbWeek.style.display = 'none';
            lbWeek.style.display = 'none';

            cbThang.style.display = 'block';
            lbThang.style.display = 'block';

            cbNam.style.display = 'block';
            lbNam.style.display = 'block';

            NgayBc.style.display = 'none';
            lbDay.style.display = 'none';
        } else {
            cbWeek.style.display = 'none';
            lbWeek.style.display = 'none';

            cbThang.style.display = 'none';
            lbThang.style.display = 'none';

            cbNam.style.display = 'block';
            lbNam.style.display = 'block';

            NgayBc.style.display = 'none';
            lbDay.style.display = 'none';
        }


    },
}
$('#cboLoai').change(sc_all.hideInput);
sc_all.init();
sc_all.hideInput();