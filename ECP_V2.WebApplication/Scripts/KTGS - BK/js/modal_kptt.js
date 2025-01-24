var modal_kptt = {
    parent: null,
    data:null,
    init: function () {
        fileupload.onchange = evt => {
            const [file] = fileupload.files;
            if (file) {
                var html = '';
                for (i = 0; i < fileupload.files.length; ++i) {
                    html += '<img src=' + URL.createObjectURL(fileupload.files[i]) +' style="height:110px;width:110px;margin:10px;border: 2px darkgray solid"/>'
                }
                $('#imgpreview').html(html);
               // blah.src = URL.createObjectURL(file)
            }
        }
    },

    show: function () {
        $('#modal_kptt').show();
    },

    submit: function () {
        // submit kieu gì đây
        var url = "api/v1/PLV/UploadImage?IDConect=PA&iddv=" + util.iddv;
        //for (i = 0; i < fileupload.files.length; ++i) {
        var para = {
            "IDConect": GlobalData.IDConnect,
            "iddv": GlobalData.tblDonVi_current.id,
            "id_phong": idpb
        };
        for (i = 0; i < fileupload.files.length; ++i) {
            var formData = new FormData();
            formData.append('phienLamViecId', -1);
            formData.append('ID_KTGS', modal_kptt.data.ID);
            formData.append('groupId', '22');
            formData.append('files', fileupload.files[0]);
            service.ExcuteAjaxtUpload(url, formData, (d) => {
                console.log(d);
            });
        }
            //var f = dio.FormData.fromMap(
            //    {
            //        'phienLamViecId': idplv,
            //        'ID_KTGS': img.id_ktgs,
            //        "groupId": img.groupId,
            //        "UserUp": GlobalData.tblNhanVien.id,
            //        "ngayChup": DateTime.now(),
            //        "kinhDo": _locationData == null ? 0 : _locationData.longitude,
            //        "viDo": _locationData == null ? 0 : _locationData.latitude,
            //        'files': [
            //            dio.MultipartFile.fromFileSync(img.localFile.path, filename: basename(img.localFile.path)),
            //        ]
            //    }
        //}

    },

    cancel: function () {
        $('#modal_kptt').hide();
    }
}

modal_kptt.init();