//Load danh sách tồn tại của đơn vị:
function GetExistsFromPMIS(loaitb, loaitt) {   
    var soap = "at_type_info_JSON";
    var key = "?";
    //var ds = '{"lst":[{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKD_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKD_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKD_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKDP_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKDP_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKDP_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKSC_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKSC_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKSC_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKN_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKN_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKN_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKDX_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKDX_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKDX_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000001","LOAI_KTR":"DKKT_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0000813","LOAI_KTR":"DKKT_DZ"},{"ASSETID":"PNYM00_DZ_DZ_0001536","LOAI_KTR":"DKKT_DZ"},{"ASSETID":"PN.5947","LOAI_KTR":"DKKT_TBA_TG"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKN_TBA"},{"ASSETID":"PN.5947","LOAI_KTR":"DKN_TBA_TG"},{"ASSETID":"PN.5947","LOAI_KTR":"DKPN_TBA_TG"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKSC_TBA"},{"ASSETID":"PN.5947","LOAI_KTR":"DKSC_TBA_TG"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKTN_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKTT_TBA"},{"ASSETID":"PN.5947","LOAI_KTR":"DKDP_TBA_TG"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKBT_TBA"},{"ASSETID":"PN.5947","LOAI_KTR":"DKBT_TBA_TG"},{"ASSETID":"PNYM00_TRAM_TRAM_1057","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_1064","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_635","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_636","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_637","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PNYM00_TRAM_TRAM_638","LOAI_KTR":"DKD_TBA"},{"ASSETID":"PN.5947","LOAI_KTR":"DKD_TBA_TG"}]}';
    $.ajax({
        type: 'POST',
        url: '/Admin/PhienXLSC/GetExistsProblem',
        data: JSON.stringify({
            loaitb: loaitb,
            loaitt: loaitt
        }),
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //Do something
            var outString= response.replace("{\"lst\":[", "").replace("}]}]","}]");
            //Cắt 6 ký tự đầu, 1 ký tự cuối.
            var jsonData = JSON.parse(response);            
            //var r = JSON.stringify(response);
            fillDatatable(JSON.parse(outString));
        },
        error(jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
            //Do something
        }
    })
}

//Ham fill data to Table
function fillDatatable(r) {
    var table = $('#tblTontai').DataTable({
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
            "processing": "ĐANG XỬ LÝ..."
        },
        "destroy": true,
        "data": r,
        "columns": [{
            'data': 'TENTB', render: function (data, type, row) {
                return '<a href="#" onclick="fnDisplayDlgAsset(event,'+data+');">'+data+'</a>';
            }
            },
            { 'data': 'TENKIEUTT' },
            {
                'data': 'NGAYTT', render: function (data, type, row) {
                    if (type === "sort" || type === "type") {
                        return data;
                    }
                    return moment(data).format("DD-MM-YYYY HH:mm");
                }
                },
            { 'data': 'GPHAP' }],
        "columnDefs": [{
            'targets': 4,
            'searchable': false,
            'orderable': false,
            'render': function (data, type, full, meta) {
                return '<input type="checkbox" name="id[]" value="' + full.MATT + '">';
            },            
        }, {
            "className": "dt-center", "targets": 2
            }, {
                "className": "dt-center", "targets": 4
            }],
        'order': [[0, 'asc']]
    });
}

function fnDisplayDlgAsset(e,assetid) {
    //set value to input hidden    
    e.preventDefault();
    getLinkReport();
    $('#mdViewAsset').modal('show');

    var iFrame = document.getElementById("ifrReport");

    var hWindow = $(window).height();
    iFrame.height = hWindow - 110;
}