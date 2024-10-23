//this function can remove a array element.
Array.remove = function (array, from, to) {
    var rest = array.slice((to || from) + 1 || array.length);
    array.length = from < 0 ? array.length + from : from;
    return array.push.apply(array, rest);
};

//this variable represents the total number of popups can be displayed according to the viewport width
var total_popups = 0;

//arrays of popups ids
var popups = [];

//this is used to close a popup
function close_popup(id) {
    for (var iii = 0; iii < popups.length; iii++) {
        if (id == popups[iii]) {
            Array.remove(popups, iii);

            //document.getElementById(id).style.display = "none";
            $("#" + id).remove();

            calculate_popups();

            return;
        }
    }


}

//displays the popups. Displays based on the maximum number of popups that can be displayed on the current viewport width
function display_popups() {
    var right = 305;

    var iii = 0;
    for (iii; iii < total_popups; iii++) {
        if (popups[iii] != undefined) {
            var element = document.getElementById(popups[iii]);
            element.style.right = right + "px";
            right = right + 310;
            element.style.display = "block";
        }
    }

    for (var jjj = iii; jjj < popups.length; jjj++) {
        var element = document.getElementById(popups[jjj]);
        element.style.display = "none";
    }
}

function register_popup(id, name) {
    $("#" + id + "_input").focus();

    for (var iii = 0; iii < popups.length; iii++) {
        //already registered. Bring it to front.
        if (id == popups[iii]) {
            Array.remove(popups, iii);

            popups.unshift(id);

            calculate_popups();


            return;
        }
    }

    var element = '<div class="popup-box chat-popup" id="' + id + '">';
    element = element + '<div class="popup-head">';
    element = element + '<div class="popup-head-left">' + name + '</div>';
    element = element + '<div class="popup-head-right"><a href="javascript:close_popup(\'' + id + '\');">&#10005;</a></div>';
    element = element + '<div style="clear: both"></div></div>';
    element = element + '<div id="' + id + '_popup-messages" class="popup-messages">';
    element = element + '<div class="col-md-12 chats">';
    element = element + '<ul class="p-0" id="' + id + '_body">';
    //element = element + '<li class="pl-2 pr-2 text-center timesend mb-1">22:43, 9 Tháng 12, 2018</li>';
    //element = element + '<li class="p-1 rounded mb-1"><div class="send-msg"><div class="send-msg-desc  text-center mt-1 ml-1 pl-2 pr-2"><p class="pl-2 pr-2 rounded">hello</p></div><img src="/img/avatar/avatar.jpg"></div></li>';
    //element = element + '<li class="pl-2 pr-2 text-center timereceive mb-1">22:43, 9 Tháng 12, 2018</li>';
    //element = element + '<li class="p-1 rounded mb-1"><div class="receive-msg"><img src="/img/avatar/avatar.jpg"><div class="receive-msg-desc  text-center mt-1 ml-1 pl-2 pr-2"><p class="pl-2 pr-2 rounded">hello</p></div></div></li>';
    element = element + '</ul></div>';
    element = element + '</div>';
    element = element + '<div class="form-group">';
    element = element + '<input style="height:40px;border:none" id="' + id + '_input" autofocus  name="' + id + '_input" onkeypress="txtSearch_onKeyPress(event,this,\'' + id + '\')" placeholder="Nhập tin nhắn" class="form-control" type="text" />';
    element = element + '</div>';
    element = element + '</div>';


    $('body').append(element);

    popups.unshift(id);

    calculate_popups();

    $("#" + id + "_input").focus();

    //$("#" + id + "_popup-messages").loading({
    //    message: 'Đang tải...'
    //});
    $.ajax({
        type: "GET",
        url: "/Chat/GetListTinNhanHtml",
        data: {
            MaNhan: id.replace("chatbox_", ""),
            page: 1,
            Total: 0
        },
        success: function (data) {
            $("#" + id + "_body").html(data);
            //$("#" + id + "_popup-messages_loading-overlay").remove();


            $("#" + id + " .popup-messages").scrollTop($("#" + id + " .popup-messages")[0].scrollHeight)
            $("#" + id + " .popup-messages").bind('scroll', function () {
                if ($(this).scrollTop() == 0) {
                    var cn = $("#" + id + " .popup-messages li").length / 2;
                    cn = Math.round(cn / 10) + 1;
                    //load more
                    // $("#" + id + "_popup-messages").loading({
                    //    message: 'Đang tải...'
                    //});
                    $.ajax({
                        type: "GET",
                        url: "/Chat/GetListTinNhanHtml",
                        data: {
                            MaNhan: id.replace("chatbox_", ""),
                            page: cn,
                            Total: $("#" + id + " .popup-messages li").length / 2
                        },
                        success: function (data) {
                            if (data.length != 0) {
                                $("#" + id + "_body").prepend(data);
                                $("#" + id + " .popup-messages").scrollTop(300)
                            }
                            //$("#" + id + "_popup-messages_loading-overlay").remove();
                        },
                        error: function () {
                            //$("#" + id + "_popup-messages_loading-overlay").remove();
                        }
                    });


                }
            })
        },
        error: function () {
            //$("#" + id + "_popup-messages_loading-overlay").remove();
        }

    });

}


//calculate the total number of popups suitable and then populate the toatal_popups variable.
function calculate_popups() {
    var width = window.innerWidth;
    if (width < 540) {
        total_popups = 0;
    }
    else {
        width = width - 200;
        //320 is width of a single popup box
        total_popups = parseInt(width / 320);
    }

    display_popups();

}

//recalculate when window is loaded and also when window is resized.
window.addEventListener("resize", calculate_popups);
window.addEventListener("load", calculate_popups);



