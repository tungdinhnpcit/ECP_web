var menuController = function () {
    
    this.initialize = function () {

        $.when()
            .done(function () {
                var roleId = $("input[name='rdbRole']:checked").val();

                loadMenuByRole(roleId);
            });
        registerEvents();
    };

    function loadMenuByRole(roleId) {
        $.ajax({
            type: 'GET',
            data: {
                roleId: roleId
            },
            url: '/admin/MenuOfRole/GetOptionSelectByRole',
            dataType: 'json',
            success: function (response) {
                if (response.listParent.length > 0) {

                    var render = "";
                    var parent_template = $('#template-parent').html();
                    var children_template = $('#template-children').html();

                    $.each(response.listParent, function (i, item) {

                        render += Mustache.render(parent_template, {
                            Id: item.Id,
                            Name: item.Text,
                            checked: item.Check ? 'checked="checked"' : ''
                        });

                        var children = jQuery.grep(response.data, function (item2, i2) {
                            return (item2.ParentId == item.Id);
                        })

                        $.each(children, function (i3, item3) {
                            render += Mustache.render(children_template, {
                                Id: item3.Id,
                                Name: item3.Text,
                                ParentId: item3.ParentId,
                                checked: item3.Check ? 'checked="checked"' : ''
                            });
                        });
                    });

                    $('#listMenu').html(render);
                }
                else {
                    $('#listMenu').html('<label style="width:100%; text-align:center; font-size:15px; font-weight:500; color:#DC0000">Không có dữ liệu</label>');
                }

            },
            error: function (status) {
                console.log(status);
                javi.notify('Không thể tải được dữ liệu', 'error');
            },
            complete: function () {
                javi.stopLoading();
            }
        });
    }


    function registerEvents() {

        $('body').on('click', '.ckbParent', function () {

            var id = $(this).val();

            if ($(this).prop("checked") === true) {
                $.each($('#listMenu input[data-parent="' + id + '"]'), function (i, item) {
                    $(item).first().prop('checked', 'checked')
                });
            }
            else {
                $.each($('#listMenu input[data-parent="' + id + '"]'), function (i, item) {
                    $(item).first().prop('checked', '')
                });
            }
        });

        $('body').on('change', 'input[name="rdbRole"]', function (e) {
            var roleId = $("input[name='rdbRole']:checked").val();

            loadMenuByRole(roleId);
        });

        $('body').on('click', '#btnSave', function (e) {
            e.preventDefault();
            var roleId = $("input[name='rdbRole']:checked").val();

            var list = [];
            $.each($('#listMenu .ckbChoose'), function (i, item) {
                if ($(item).prop('checked')) {
                    list.push($(item).val());
                }
            });

            if (roleId == '' || roleId == null || roleId == undefined) {
                javi.notify("Chưa chọn quyền", "error");
                return false;
            }

            $.ajax({
                type: "POST",
                url: "/Admin/MenuOfRole/Save",
                data: {
                    roleId: roleId,
                    listMenu: list
                },
                dataType: "json",
                beforeSend: function () {
                    javi.startLoading();
                },
                success: function (response) {
                    if (response.status != null) {
                        javi.notify(response.message, "success");
                    }
                    else {
                        javi.notify(response.message, "error");
                    }
                    loadMenuByRole(roleId);
                },
                error: function (err) {
                    javi.notify('Có lỗi xảy ra.', 'error');
                },
                complete: function () {
                    javi.stopLoading();
                }
            });
        })

        $('body').on('click', '#btnCancel', function (e) {
            e.preventDefault();
            var roleId = $("input[name='rdbRole']:checked").val();
            loadMenuByRole(roleId);
        })
    }

}