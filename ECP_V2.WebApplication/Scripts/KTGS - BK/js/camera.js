
var camera = {
    urlcam: "https://cmsv9.mdvronline.vn/",
    urlfile: "http://115.146.126.228",
    plvid: null,
    madvi: null,
    camp: [],
    scale: 1,
    current_camid:null,
    init: function () {        
     	camera.madvi = $('#DonViID').val();
        var table = $('#tblCamera').DataTable(this.optiontable);
        url = "api/v1/Camera/GeturlCam";
        service.ExcuteAjaxtGet(url, null, (d) => {
            var data = d.Data;
            camera.urlcam = data.liveurl;
            camera.urlfile = data.storeurl;
        });
    },

    getUrl: function () {
        service.ExcuteAjaxtGet(url, null, (d) => {
            console.log(d);
            camera.camp = d.Data
        });
    },

    viewCamera: function (plvid) {
        camera.plvid = plvid;
        $('#modal_ShowCamera').modal('show');
        camera.loadcamplv();
    },

    addCamera: function (plvid) {
        console.log('addCamera' + plvid);
        camera.plvid = plvid;      
        $('#modal_AddCamera').modal('show');
        this.loadcamdv();

    },
    // xem camera lịch sử
    viewHisCamera: function (plvid) {
        camera.plvid = plvid;
        camera.camp = [];
        $('#current_his_cam').html('');
        $('#modal_ShowHisCamera').modal('show');
        url = "api/v1/Camera/GetHisCam?madvi=" + camera.madvi + "&idplv=" + camera.plvid;
        service.ExcuteAjaxtGet(url, null, (d) => {
            console.log(d);
            camera.camp = d.Data
            for (i = 0; i < d.Data.length; ++i) {
                var html = '';
                for (i = 0; i < d.Data.length; ++i) {
                    html = html + '<button type="button" class="btn btn-outline-primary" onclick="camera.getmediacam(' + d.Data[i].CAM_ID + ')" style:margin: 5px;">Xem ' + d.Data[i].CAM_DESC + '</button>';
                }
                $('#viewhiscam').html(html);
            };
            if (camera.camp.length > 0) {
                camera.getmediacam(d.Data[0].CAM_ID);
            }
        });

    },
    getmediacam: function (camid) {
        $('#current_his_cam').html('Bạn đang xem cam ' + camid);
        camera.getvideocam(camid);
        camera.getpicturecam(camid)
    },

    getvideocam: function (camid) {
        $('#ithump_cam').html('');
        var index = camera.camp.findIndex(e => e.CAM_ID == camid);
        if (index >= 0) {
            var medias = camera.camp[index].MediaData;
            var html = '';
            if (medias.length <= 0)
                html="<p>Không có dữ liệu video trong khoảng thời gian này</p>"
            for (i = 0; i < medias.length; ++i) {
                //html = html + '<a href=\'' + camera.urlfile + medias[i].file_url +'\' download><img src=\'' + camera.urlfile + medias[i].thumbnail_url + '\' style="margin: 10px;"/> </a> ';
                
                html = html + '<img src=\'' + camera.urlfile + medias[i].thumbnail_url + '\' style="margin: 10px; " onclick="camera.viewmp4camera(\'' + medias[i].file_url +'\')" />';
            }
            $('#ithump_cam').html(html);
          
        }       
    },

    getpicturecam: function (camid) {
        $('#ithump_pic').html('');
        var index = camera.camp.findIndex(e => e.CAM_ID == camid);
        if (index >= 0) {
            var medias = camera.camp[index].MediaDataPic;
            var html = '';
            if (medias.length <= 0)
                html = "<p>Không có dữ liệu picture trong khoảng thời gian này</p>"
            for (i = 0; i < medias.length; ++i) {
                //html = html + '<a href=\'' + camera.urlfile + medias[i].file_url +'\' download><img src=\'' + camera.urlfile + medias[i].thumbnail_url + '\' style="margin: 10px;"/> </a> ';

                html = html + '<img src=\'' + medias[i].downloadUrl + '\' style="margin: 10px;height: 180px;" onclick="camera.viewmp4picture(\'' + medias[i].downloadUrl + '\')" />';
            }
            $('#ithump_pic').html(html);
        }

    },

    viewmp4picture: function (url) {
        window.open(url);
    },
    viewmp4camera: function (url) {
        window.open(camera.urlfile + url);       

        //$('#modal_HisCameramp4').modal('show');
        //html = `<video width="400" height="400" controls>
        //                        <source src = "`+ camera.urlfile + url + `" type = "video/mp4">
                     
        //                </video>`
        //$('#objmp4').html(html);
    },

    loadcamdv:function() {
        url = "api/v1/Camera/GetCameraDvi?madvi=" + camera.madvi + "&idplv=" + camera.plvid;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            oTable = $("#tblCamera").dataTable();
            oTable.fnClearTable();           
            if (d.Data.length > 0)
                oTable.fnAddData(d.Data);
        });
    },

    // xem camera online
    loadcamplv: function () {
        url = "api/v1/Camera/GetCameraplv?madvi=" + camera.madvi + "&idplv=" + camera.plvid;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {            
            var html = '';
            for (i = 0; i < d.Data.length; ++i) {
                html = html + '<button type="button" class="btn btn-outline-primary" onclick="camera.viewcam(' + d.Data[i].CAM_ID + ')" style:margin: 5px;">Xem ' + d.Data[i].CAM_DESC+'</button>';
            }
            $('#viewcam').html(html);
            if (d.Data.length > 0) {
                camera.viewcam(d.Data[0].CAM_ID);
            }
        });


    },

    viewcam: function (camid) {
        camera.current_camid = camid;

        console.log('viewcam' + camid);
        url = "api/v1/Camera/GetUpcamera";
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            var jtoken = d.Data;
            //new url
            //
            var urlcam = camera.urlcam + "808gps/open/player/talkvideo.html?device_no=" + camid + "&jsession=" + jtoken + "&chn=0&close=3000";
            // tao url viewcam
            //var urlcam = camera.urlcam + "/808gps/open/player/video.html?lang=en&vehiIdno=" + camid + "&jsession=" + jtoken + "&channel=1&chns=0&close=100";
            document.getElementById('ifCamera').src = urlcam;
        });
    },

    viewimageonline:function() {
        camera.camp = [];
        url = "api/v1/Camera/GetHisCam?madvi=" + camera.madvi + "&idplv=" + camera.plvid;
        service.ExcuteAjaxtGet(url, null, (d) => {
            console.log(d);
            camera.camp = d.Data

            $('#ithump_pic_online').html('');
            var index = camera.camp.findIndex(e => e.CAM_ID == camera.current_camid);
            if (index >= 0) {
                var medias = camera.camp[index].MediaDataPic;
                var html = '';
                if (medias.length <= 0)
                    html = "<p>Không có dữ liệu picture trong khoảng thời gian này</p>"
                for (i = 0; i < medias.length; ++i) {
                    //html = html + '<a href=\'' + camera.urlfile + medias[i].file_url +'\' download><img src=\'' + camera.urlfile + medias[i].thumbnail_url + '\' style="margin: 10px;"/> </a> ';

                    html = html + '<img src=\'' + medias[i].downloadUrl + '\' style="margin: 10px;height: 180px;" onclick="camera.viewmp4picture(\'' + medias[i].downloadUrl + '\')" />';
                }
                $('#ithump_pic_online').html(html);
            }
        });
    },

    fullscreen: function (x) {
        //var w = $(window).width();
        //var h = $(window).height();        
        //if (x == -1) {
        //    camera.scale = camera.scale * 1.1;
        //    //w = w * 0.9;
        //    //h = h * 0.9;
        //    //$("#ifCamera").width(w + "px");
        //    //$("#ifCamera").height(h + "px")
        //} else {
        //    camera.scale = camera.scale * 0.9;
        //    //w = w * 1.1;
        //    //h = h * 1.1;
        //    //$("#ifCamera").width(w + "px");
        //    //$("#ifCamera").height(h + "px")
        //}
        //$('#ifCamera').css('transform', `scale(${camera.scale})`);
        window.open(document.getElementById('ifCamera').src);
    },

    optiontable: {
        "bLengthChange": true,
        "searching": true,
        "paging": true,
        "info": true,
        "scrollY": '70vh',
        "scrollCollapse": true,
        "pageLength": 50,
        "lengthMenu": [[50, 100, 500, 1000], [50, 100, 500, 1000]],
        "language": {
            "emptyTable": "Bảng chưa có dữ liệu",
            "info": "<b>Tổng số: _START_-_END_/_TOTAL_ </b>",
            "lengthMenu": "Xem _MENU_ bản ghi",
            "search": "Tìm nội dung:",
            "paginate": {
                "first": "Đầu",
                "last": "Cuối",
                "next": "Trang tiếp",
                "previous": "Trang trước"
            },
        },
        //"order": [[2, "desc"]],
        "columns": [
            {
                "data": "CAM_ID"
            },
            {
                "data": "CAM_DESC", "width": "15%"
            },
            {
                "data": "check", render: function (data, type, row) {
                    var html = '';
                    if(row.check!=true)
                        html = '<button type="button" class="btn btn-outline-primary" onclick="camera.sudung('+row.CAM_ID+')">Sử dụng</button>';
                    else
                        html = '<button type="button" class="btn btn-outline-danger" onclick="camera.huysudung(' + row.CAM_ID +')">Hủy</button>';
                    return html;
                }, "width": "20%"
            },
        ],

    },

    sudung: function (camid) {
        console.log('sudung' + camid);
        url = "api/v1/Camera/ActiveCam?madvi=" + camera.madvi + "&idplv=" + camera.plvid + "&camid=" + camid;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            camera.loadcamdv();
        });
    },

    huysudung: function (camid) {
        console.log('huysudung' + camid);
        url = "api/v1/Camera/DeActiveCam?madvi=" + camera.madvi + "&idplv=" + camera.plvid + "&camid=" + camid;
        //string madvi, string madvics, int thang, int nam
        service.ExcuteAjaxtGet(url, null, (d) => {
            camera.loadcamdv();
        });
    }
   
};


camera.init();