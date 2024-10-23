var ndvsController = function () {

    var cachedObj = {
        donvi: []
    }


    var isUpload = true;

    this.initialize = function () {

        $.when(loadData())
            .done(function () {

                $('html').addClass('sidebar-left-collapsed');
                registerEvents();
            });
    };

    function loadDM() {
        var parent_template = $('#template-ds-parent').html();
        var children_template = $('#template-ds-children').html();
        var render = "";
        $.ajax({
            type: 'GET',
            url: '/admin/NoiDungVeSinh/GetAllLoaiVeSinh',
            dataType: 'json',
            success: function (response) {
                if (response.length > 0) {
                    var d = 1;
                    $.each(response, function (i, item) {
                        if (item.ParentId == null) {
                            render += Mustache.render(parent_template, {
                                STT: d,
                                Id: item.Id,
                                Name: d + ". " + item.Ten

                            });
                            d++;

                            $.each(response, function (i2, item2) {
                                if (item2.ParentId == item.Id) {
                                    render += Mustache.render(children_template, {
                                        STT: d,
                                        Id: item2.Id,
                                        Name: item2.Ten
                                    });
                                }
                            });
                        }
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="3">Không có dữ liệu</td></tr>');
                }
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu trang', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }



    function registerEvents() {

        $('body').on('click', '.checkall', function () {
            if ($(this).prop("checked") === true) {
                $(this).parent().parent().nextUntil('.ds-parent').find('input.ckChoose').not(":disabled").prop('checked', true);
            }

            else {
                $(this).parent().parent().nextUntil('.ds-parent').find('input.ckChoose').not(":disabled").prop('checked', false);
            }
           
        })

        $("#monthpicker").kendoDatePicker({
            // defines the start view
            start: "year",

            // defines when the calendar should return date
            depth: "year",

            // display month and year in the input
            format: "MM/yyyy",

            // specifies that DateInput is used for masking the input element
            dateInput: true
        });



        //todo: binding events to controls

        $('#ddlDonVi').on('change', function () {
            loadData();
        })

        $('#monthpicker').on('change', function () {
            loadData();
        })

        $('#btnAddImage').on('click', function (e) {
            e.preventDefault();
            $('#txtFile').val('');
            $('#txtFile').click();
        })

        $("#txtFile").on('change', function () {

            var fileUpload = $(this).get(0);
            var files = fileUpload.files;

            if (files.length > 0) {
                isUpload = false;

                var data = new FormData();

                for (i = 0; i < fileUpload.files.length; i++) {
                    //var sfilename = fileInput.files[i].name;
                    //let srandomid = Math.random().toString(36).substring(7);
                    data.append(fileUpload.files[i].name, fileUpload.files[i]);
                }

                //for (var i = 0; i < files.length; i++) {
                //    data.append(files[i].name, files[i]);
                //}

                var ndId = $('#hid-noidungId').val();
                var type = $('#hid-type').val();
                data.append("noidungId", ndId);
                data.append("type", type);

                //startUpdatingProgressIndicator();

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/UploadImage",
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (response) {
                        if (response.status) {
                            javi.notify(response.message, 'success');

                            loadImage();
                        }
                        else {
                            javi.notify(response.message, 'error');
                            return false;
                        }
                    },
                    error: function () {
                        javi.notify('Đã xảy ra lỗi khi tải tệp lên!', 'error');
                    },
                    complete: function () {
                        $('#txtAddFile').val('');
                        //setTimeout(stopUpdatingProgressIndicator(), 210);
                    }
                });

                isUpload = true;
            }
        });


        $('#tabTruoc').on('click', function () {
            $('#hid-type').val(1);
            loadImage();
        })

        $('#tabSau').on('click', function () {
            $('#hid-type').val(2);
            loadImage();
        })

        $('body').on('click', '.btn-image-nd', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();
            var id = $(tr).data('id');
            $('#hid-noidungId').val(id);
            $('#tabTruoc').click();

            $('#image-modal').modal('show');
        })

        $('body').on('click', '.btn-deleteImage', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            javi.confirm('Bạn có chắc chắn muốn xóa không?', function () {
                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/DeleteImage",
                    data: { id: that },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        javi.notify('Xóa thành công.', 'success');
                        loadImage();
                    },
                    error: function (status) {
                        javi.notify('Có lỗi xảy ra trong quá trình xóa.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            });
        })


        // comment
        $('body').on('click', '.btn-comment-nd', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();
            var id = $(tr).data('id');
            resetFormComment();

            loadComment(id);

            $('#modal-add-comment').modal('show');
            $('#hidNDId').val(id);
        });

        $('#btnSendComment').on('click', function (e) {
            e.preventDefault();
            if ($('#frmMaintainanceComment').valid()) {
                var id = $('#hidId').val();
                var ndId = $('#hidNDId').val();
                var content = $('#txtContent').val();

                if (content == '' || content == null || content == undefined) {
                    javi.notify("Chưa nhập nội dung", "error");
                    return false;
                }

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/SaveComment",
                    data: {
                        Id: id,
                        BaoCaoId: ndId,
                        NoiDung: content
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function () {
                        javi.notify('Gửi Bình Luận Thành Công', 'success');

                        $('#txtContent').val('');
                        loadComment(ndId);
                    },
                    error: function () {
                        javi.notify('Có Lỗi Xảy Ra', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            }
            return false;
        });


        $('body').on('click', '.btn-add-nd', function (e) {
            e.preventDefault();
            var template = $('#template-add-nd').html();
            var id = $(this).data('id');

            var last = $('.ndvs[data-parent-id="' + id + '"]').last().find('.nd-stt').first();
            var stt = 0;
            if (last.length > 0)
                stt = parseInt(last.html())
            else
                stt = 0;

            var render = Mustache.render(template, {
                STT: stt + 1,
                DsId: id,
                Id: 0,
                Name: "",
                Value: ""
            });

            if (stt > 0) {
                $('.ndvs[data-parent-id="' + id + '"]').last().after(render);
            }
            else {
                $(this).parent().parent().after(render);
            }

        });

        $('body').on('click', '.btn-save-nd', function (e) {
            e.preventDefault();
            var tr = $(this).parent().parent();

            var id = $(tr).data('id');
            var loaiId = $(tr).data('parent-id');
            var stt = $(tr).find('.nd-stt').first().html();
            var name = $(tr).find('.txt-nd-name').first().val();
            var value = $(tr).find('.txt-nd-value').first().val();

            if (loaiId == "") {
                javi.notify("Không tìm thấy loại danh mục", "error");
                return false;
            }

            if (name == "") {
                javi.notify("Chưa nhập nội dung", "error");
                return false;
            }

            if (value == "") {
                javi.notify("Chưa nhập giá trị", "error");
                return false;
            }

            var tempDate = $('#monthpicker').val().split("/");

            $.ajax({
                type: "POST",
                url: "/Admin/NoiDungVeSinh/SaveNoiDung",
                data: {
                    Id: id,
                    ThuTu: stt,
                    Ten: name,
                    MaLoai: loaiId,
                    GiaTri: value,
                    DonViId: $('#ddlDonVi').val(),
                    Thang: tempDate[0],
                    Nam: tempDate[1]

                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response.status) {
                        javi.notify(response.message, 'success');

                        var data_template = $('#template-nd').html();
                        var render = Mustache.render(data_template, {
                            STT: response.data.ThuTu,
                            Id: response.data.Id,
                            DsId: response.data.MaLoai,
                            Name: response.data.Ten,
                            Value: response.data.GiaTri
                        });

                        $(tr).after(render);
                        $(tr).remove();

                        var v = parseFloat($('.ds-children[data-id="' + response.data.MaLoai + '"]').find('.ds-children-value').first().html());
                        v += response.value;
                        $('.ds-children[data-id="' + response.data.MaLoai + '"]').find('.ds-children-value').first().html(v);
                    }
                    else {
                        javi.notify(response.message, "error");
                    }
                },
                error: function (err) {
                    javi.notify('Đã xảy ra lỗi khi cập nhật.', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });

        });

        $('body').on('click', '.btn-delete-add-nd', function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

        $('body').on('click', '.btn-delete-nd', function (e) {
            e.preventDefault();

            var id = $(this).data('id');
            var divtr = $(this).parent().parent();

            if (confirm('Bạn có chắc chắn muốn xóa không?')) {

                if (id != null && id != '' && id != 0 && id != undefined) {
                    $.ajax({
                        type: "POST",
                        url: "/Admin/NoiDungVeSinh/DeleteNoiDung",
                        data: {
                            Id: id
                        },
                        dataType: "json",
                        beforeSend: function () {
                            javi.startLoading();
                        },
                        success: function (response) {
                            if (response.status) {
                                javi.notify(response.message, 'success');
                                var v = parseFloat($('.ds-children[data-id="' + response.maloai + '"]').find('.ds-children-value').first().html());
                                v -= response.value;
                                $('.ds-children[data-id="' + response.maloai + '"]').find('.ds-children-value').first().html(v);

                                divtr.remove();
                            }
                            else {
                                javi.notify(response.message, "error");
                            }
                        },
                        error: function (err) {
                            javi.notify('Đã xảy ra lỗi khi xóa.', 'error');
                        },
                        complete: function () {
                            javi.stopLoading();
                        }
                    });
                }
                else {
                    javi.notify("Không tìm thấy bản ghi", "error");
                }
            }

        });

        $('body').on('click', '.btn-edit-nd', function (e) {
            e.preventDefault();

            var tr = $(this).parent().parent();
            var id = $(tr).data('id');
            var maLoai = $(tr).data('parent-id');
            var stt = $(tr).find('.nd-stt').html();
            var name = $(tr).find('.nd-name').html();
            var value = $(tr).find('.nd-value').html();

            var data_template = $('#template-edit-nd').html();
            var render = Mustache.render(data_template, {
                STT: stt,
                Id: id,
                DsId: maLoai,
                Name: name,
                Value: value
            });

            $(tr).after(render);
            $(tr).remove();
        })

        $('body').on('click', '.btn-cancel-nd', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var tr = $(this).parent().parent();

            $.ajax({
                type: "POST",
                url: "/Admin/NoiDungVeSinh/GetNoiDungById",
                data: {
                    Id: id
                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response != null) {

                        var data_template = $('#template-nd').html();
                        var render = Mustache.render(data_template, {
                            STT: response.ThuTu,
                            Id: response.Id,
                            DsId: response.MaLoai,
                            Name: response.Ten,
                            Value: response.GiaTri
                        });

                        $(tr).after(render);
                        $(tr).remove();
                    }
                    else {
                        javi.notify("Có lỗi xảy ra", "error");
                    }
                },
                error: function (err) {
                    javi.notify('Có lỗi xảy ra.', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });
        })

        $('body').on('click', '#btnChuyenNPC', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn Chuyển NPC?')) {

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/ChuyenNPC",
                    data: {
                        List: list
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            if (response.count > 0) {
                                alert("Có " + response.count + " bản ghi không hợp lệ!");
                            }
                            loadData();
                        }
                        else {
                            javi.notify(response.message, "error");
                        }
                    },
                    error: function (err) {
                        javi.notify('Có lỗi xảy ra.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            }
        })

        $('body').on('click', '#btnHuyChuyenNPC', function (e) {
            e.preventDefault();

            if (confirm('Bạn có chắc chắn muốn hủy Chuyển NPC?')) {
                var kybaocaoId = $('#hidKyBaoCao').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/HuyChuyenNPC",
                    data: {
                        kybaocaoId: kybaocaoId
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status) {
                            javi.notify(response.message, "success");
                            loadData();
                        }
                        else {
                            javi.notify(response.message, "error");
                        }
                    },
                    error: function (err) {
                        javi.notify('Có lỗi xảy ra.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            }
        })

        $('body').on('click', '#btnChotBaoCao', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn Chốt báo cáo. Sau khi chốt sẽ không thể chỉnh sửa báo cáo?')) {
               

                $.ajax({
                    type: "POST",
                    url: "/Admin/NoiDungVeSinh/ChotBaoCao",
                    data: {
                        List: list
                    },
                    dataType: "json",
                    beforeSend: function () {
                        javi.startLoading();
                    },
                    success: function (response) {
                        if (response.status != null) {
                            javi.notify(response.message, "success");
                            if (response.count > 0) {
                                alert("Có " + response.count + " bản ghi không hợp lệ!");
                            }
                            loadData();
                        }
                        else {
                            javi.notify(response.message, "error");
                        }
                    },
                    error: function (err) {
                        javi.notify('Có lỗi xảy ra.', 'error');
                    },
                    complete: function () {
                        javi.stopLoading();
                    }
                });
            }

        })

        $('body').on('click', '#btnHuyChotBaoCao', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });

            if (list.length < 1) {
                javi.notify("Chưa chọn nội dung", "error");
                return false;
            }

            if (confirm('Bạn có chắc chắn muốn hủy Chốt báo cáo?')) {
               
                $('#txtContent_HC').val('');
                $('#modal-huy-chot').modal('show');
            }

        })

        $('body').on('click', '#btnXacNhan_HuyChot', function (e) {
            e.preventDefault();

            var list = [];
            $.each($('#tbl-content tr .ckChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push(parseFloat($(item).val()));
                }
            });
            
            $.ajax({
                type: "POST",
                url: "/Admin/NoiDungVeSinh/HuyChotBaoCao",
                data: {
                    List: list,
                    lydo: $('#txtContent_HC').val()
                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response.status != null) {
                        javi.notify(response.message, "success");
                        if (response.count > 0) {
                            alert("Có " + response.count + " bản ghi không hợp lệ!");
                        }
                        loadData();
                    }
                    else {
                        javi.notify(response.message, "error");
                    }
                },
                error: function (err) {
                    javi.notify('Có lỗi xảy ra.', 'error');
                },
                complete: function () {
                    $('#modal-huy-chot').modal('hide');
                    javi.stopLoading();
                }
            });

        })

        $('body').on('click', '#btnExport', function (e) {
            e.preventDefault();
            var tempDate = $('#monthpicker').val().split("/");

            $.UnifiedExportFile(
                {
                    action: "/Admin/NoiDungVeSinh/Export",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        month: tempDate[0],
                        year: tempDate[1]
                    },
                    downloadType: 'Progress',
                    ajaxLoadingSelector: '#loading'
                });
        })


        $('body').on('click', '#btnExport2', function (e) {
            e.preventDefault();
            var tempDate = $('#monthpicker').val().split("/");

            $.UnifiedExportFile(
                {
                    action: "/Admin/NoiDungVeSinh/Export2",
                    data: {
                        donviId: $('#ddlDonVi').val(),
                        thang: tempDate[0],
                        nam: tempDate[1]
                    },
                    downloadType: 'Progress',
                    ajaxLoadingSelector: '#loading'
                });
        })
    }

    function resetFormComment() {
        $('#hidId').val(0);
        $('#hidNDId').val(0);
        $('#txtContent').val('');
    }

    function loadComment(NDid) {
        $.ajax({
            type: 'GET',
            url: '/Admin/NoiDungVeSinh/GetAllCommentByNoiDungId',
            data: { id: NDid },
            dataType: 'json',
            beforeSend: function () {
                javi.startLoading();
            },
            success: function (response) {
                var template = $('#comment-template').html();
                var render = "";
                if (response != null) {
                    $.each(response, function (i, item) {
                        render += Mustache.render(template, {
                            Id: item.Id,
                            Title: item.FullName + ' - ' + item.UserName,
                            FullName: item.FullName,
                            Content: item.NoiDung,
                            Image: item.Avatar != null ? item.Avatar : '/Content/images/user.jpg',
                            Date: javi.convertDateJS2(item.NgayTao),
                            HuyChot: item.Type == 2 ? "(Lý do huỷ chốt)" : ""
                        });
                    });
                    if (render != undefined) {
                        $('#listComments').html(render);
                    }
                }
                else {
                    $('#listComments').html('');
                }

            },
            error: function () {
                javi.notify('Có lỗi xảy ra', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }

    function loadImage() {
        var template = $('#image-template').html();
        var render = "";
        var ndId = $('#hid-noidungId').val();
        var type = $('#hid-type').val();
        $.ajax({
            type: 'GET',
            url: '/admin/NoiDungVeSinh/GetAllImageByOption',
            data: {
                noidungId: ndId,
                type: type
            },
            dataType: 'json',
            success: function (response) {
                if (response.length > 0) {
                    $.each(response, function (i, item) {
                        render += Mustache.render(template, {
                            Image: item.Url,
                            Id: item.Id,
                            Date: javi.convertDateJS2(item.DateCreated),
                            Name: item.Name
                        });
                    });

                    if (type == 1) {
                        $('.list-truoc-5s').html(render);
                    }
                    else if (type == 2) {
                        $('.list-sau-5s').html(render);
                    }
                }
                else {
                    if (type == 1) {
                        $('.list-truoc-5s').html('');
                    }
                    else if (type == 2) {
                        $('.list-sau-5s').html('');
                    }
                }
            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu trang', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }

    function downloadURL(url) {
        var hiddenIFrameID = 'hiddenDownloader',
            iframe = document.getElementById(hiddenIFrameID);
        if (iframe === null) {
            iframe = document.createElement('iframe');
            iframe.id = hiddenIFrameID;
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
        }
        iframe.src = url;
    };

    //load dữ liệu
    function loadData(isPageChanged) {
        loading('Đang tải dữ liệu...', 1);
        var tempDate = $('#monthpicker').val().split("/");

        $.ajax({
            type: 'GET',
            url: '/admin/NoiDungVeSinh/GetInfo',
            data: {
                donviId: $('#ddlDonVi').val(),
                month: tempDate[0],
                year: tempDate[1]
            },
            dataType: 'json',
            success: function (response) {

                var parent_template = $('#template-ds-parent').html();
                var children_template = $('#template-ds-children').html();
                var data_template = $('#template-nd').html();
                var render = "";

                var listLoai = response.listLoai;
                var data = response.data;

                if (listLoai.length > 0) {
                    var d = 1;
                    $.each(listLoai, function (i, item) {
                        if (item.ParentId == null) {
                            render += Mustache.render(parent_template, {
                                STT: d,
                                Id: item.Id,
                                Name: d + ". " + item.Ten,
                                TenND: item.TenNoiDung,
                                LoaiDVT: item.LoaiDonViTinh
                            });
                            d++;

                            $.each(listLoai, function (i2, item2) {
                                if (item2.ParentId == item.Id) {

                                    var render2 = "";
                                    var sum = 0;
                                    var d = 1;

                                    $.each(data, function (i3, item3) {
                                        if (item3.MaLoai == item2.Id) {

                                            var s = item3.TrangThai;
                                            var color = "#fff";
                                            if (s == 2)
                                                color = "#caffe7";
                                            else if (s == 3)
                                                color = "#c4c4c4"

                                            render2 += Mustache.render(data_template, {
                                                STT: d++,
                                                Id: item3.Id,
                                                DsId: item3.MaLoai,
                                                Name: item3.Ten,
                                                Value: item3.GiaTri,
                                                StatusColor: color,
                                                hiddenCheck: (item3.IsChuyenNPC || item3.TrangThai == 2) ? 'display: none' : '',
                                                hiddenCheck3: item3.IsChuyenNPC ? 'display: none' : '',
                                                hiddenCheck2: item3.IsChuyenNPC ? 'disabled readonly' : '',
                                                IsNPC: item3.IsChuyenNPC ? 'imgNoiDung' : '',
                                                Title: (item3.TrangThai == 3 && item3.LyDo != null) ? "Lý do huỷ chốt: " + item3.LyDo : ""
                                            });
                                            sum += item3.GiaTri;
                                        }
                                    });

                                    render += Mustache.render(children_template, {
                                        STT: d,
                                        Id: item2.Id,
                                        Name: item2.Ten,
                                        Value: sum
                                    });

                                    render += render2;
                                }
                            });
                        }
                    });

                    $('#tbl-content').html(render);
                }
                else {
                    $('#tbl-content').html('<tr style="text-align:center; font-size:18px; font-weight:bold; color:#DC0000; background-color:lightgoldenrodyellow" ><td colspan="3">Không có dữ liệu</td></tr>');
                }

                if (response.checkCT) {
                    $('.btn-add-nd').hide();
                    $('.btn-edit-nd').hide();
                    $('.btn-delete-nd').hide();
                 


                    //if (kybaocao.TrangThaiChot) {
                    //    $('#btnChotBaoCao').hide();
                    //    $('#btnHuyChotBaoCao').show();
                    //    $('.nd-function').html('<a href="#" class="btn btn-sm btn-image-nd" title="Hình ảnh" style="font-size:18px"><i class="fa fa-picture-o"></i></a>');
                    //}
                    //else {
                    //    $('#btnChotBaoCao').show();
                    //    $('#btnHuyChotBaoCao').hide();
                    //}

                    //if (kybaocao.IsChuyenNPC) {
                    //    $('#btnChuyenNPC').hide();
                    //    $('#btnChotBaoCao').hide();
                    //    $('#btnHuyChotBaoCao').hide();
                    //}
                    //else {
                    //    $('#btnChuyenNPC').show();
                    //}

                }
                else {
                 
                }

                //if (kybaocao.IsChuyenNPC) {
                //    $('#lblChuyenNPC').html("Đã chuyển NPC");
                //    $('#lblChuyenNPC').css('color', '#00a214')
                //}
                //else {
                //    $('#lblChuyenNPC').html("Chưa chuyển NPC");
                //    $('#lblChuyenNPC').css('color', '#DC0000')
                //}

                //if (kybaocao.TrangThaiChot) {
                //    $('#lblChotBC').html("Đã chốt báo cáo");
                //    $('#lblChotBC').css('color', '#00a214')

                //}
                //else {
                //    $('#lblChotBC').html("Chưa chốt báo cáo");
                //    $('#lblChotBC').css('color', '#DC0000')
                //}

            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu trang', 'error');
            },
            complete: function () {
                unloading();
                javi.stopLoading();
            }
        });
    }

}