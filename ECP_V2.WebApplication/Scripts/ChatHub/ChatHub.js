var chatHub;
$(function () {
    chatHub = $.connection.chatHub;
    registerClientMethods(chatHub);
    // Start Hub
    $.connection.hub.start().done(function () {

        registerEvents(chatHub)

    });
});

function registerClientMethods(chatHub) {


    // Calls when user successfully logged in
    chatHub.client.onConnected = function (id, userName, allUsers) {

        for (i = 0; i < allUsers.length; i++) {
            $("#li_" + allUsers[i].UserName).addClass('status-online');
        }

    }

    // On New User Connected
    chatHub.client.onNewUserConnected = function (id, name, UserImage, loginDate, allUsers) {

        for (i = 0; i < allUsers.length; i++) {
            $("#li_" + allUsers[i].UserName).addClass('status-online');
        }

    }

    // On User Disconnected
    chatHub.client.onUserDisconnected = function (id, userName) {

        $("#li_" + userName).removeClass('status-online');

    }

    chatHub.client.messageReceived = function (userName, message, time, userimg) {

        //alert(message);


    }


    chatHub.client.sendPrivateMessage = function (name, fromUserName, message, userimg, CurrentDateTime) {
        $.playSound('/Scripts/PlaySound/ting_ting.mp3')
        //alert(message);
        register_popup("chatbox_" + fromUserName, name);

        $("#chatbox_" + fromUserName + "_body").append('<li class="pl-2 pr-2 text-center timereceive mb-1">' + CurrentDateTime + '</li>');
        $("#chatbox_" + fromUserName + "_body").append('<li class="p-1 rounded mb-1"><div class="receive-msg"><img src="' + userimg + '"><div class="receive-msg-desc  text-center mt-1 ml-1 pl-2 pr-2"><p class="pl-2 pr-2 rounded">' + message + '</p></div></div></li>');
    }



}

