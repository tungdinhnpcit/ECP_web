function getLinkReport() {    
    $.ajax({
        type: 'POST',
        url: '/Admin/BCKTDK/GetUrlReportQLTA',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                //$('ifrReport').attr('src', 'http://10.1.117.35:8080/qlta/shared/report/S_ReportOu.jsf?pgid=' + response);

                //$('ifrReport').attr('src', 'https://vnexpress.net');
                document.getElementById("ifrReport").setAttribute("src", "http://10.1.117.35:8080/PMIS_Web/shared/report/S_ReportOu.jsf?pgid=" + JSON.parse(response));
            }       
                
            },
        error: function (ex) {
            alert('Không lấy được Token');
        }
    })
}
///Lấy link xử lý tồn tại view
function getLinkProcess() {
    $.ajax({
        type: 'POST',
        url: '/Admin/XLTT/GetUrlProcessQLTA',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {                
                document.getElementById("ifrReport").setAttribute("src", "http://10.1.117.35:8080/PMIS_Web/eam/qlta/managementExists.jsf?pgid=" + response);
            }

        },
        error: function (ex) {
            alert('Không lấy được Token');
        }
    })
}

///Lấy link Điều chỉnh thông tin thiết bị
function getLinkChangeAsset() {
    $.ajax({
        type: 'POST',
        url: '/Admin/DCTB/GetUrlChangeAsset',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //if type=tba then build select tba, dz build dz
            if (response.length > 0) {
                document.getElementById("ifrReport").setAttribute("src", "http://10.1.117.35:8080/PMIS_Web/eam/asset/AssetCorrection.jsf?pgid=" + response);
            }

        },
        error: function (ex) {
            alert('Không lấy được Token');
        }
    })
}