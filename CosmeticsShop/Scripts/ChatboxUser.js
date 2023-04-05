
$(document).ready(function () {
    $(function () {
        var chat = $.connection.chatHubs
        chat.client.displayMessage = function () {
            getAllData();
        }
        $.connection.hub.start();
        getAllData();
    })
    var lastMessID = 0;
    function getAllData() {
        var chatbox = $('.chatbox__body');
        var userID = $('#UserID').val();
        $.ajax({
            url: location.origin + "/Chat/GetAllMessageChating?UserID=" + userID,
            type: 'GET',
            dataType: 'JSON',
            success: function (data) {
                chatbox.empty()
                if (data.length > 0) {
                    $.each(data, function (i, model) {
                        var date = new Date(parseInt(model.CreatedDate.substr(6)));
                        if (model.FromUserID == userID) {
                            lastMessID = model.ID;
                            chatbox.append(
                                '<div class="chatbox__body__message chatbox__body__message--right" id="' + model.ID + '"' + '>' +
                                ' <div class="chatbox_timing">' +
                                '<ul>' +
                                '<li>' + date.getHours() + ':' + date.getMinutes() + '</li>' +
                                '</ul>' +
                                '</div>' +
                                '<img src="/Content/images/' + model.FromAvatarUser + '" alt="Picture">' +
                                '<div class="clearfix"></div>' +
                                ' <div class="ul_section_full">' +
                                ' <ul class="ul_msg">' +
                                '<li><strong>' + model.FromUserName + '</strong></li>' +
                                '<li>' + model.Content + '</li>' +
                                '</ul>' +
                                '<div class="clearfix"></div>' +
                                '</div>' +
                                '</div>'
                            );
                        } else {
                            chatbox.append(
                                '<div class="chatbox__body__message chatbox__body__message--left" id="' + model.ID + '"' + '>' +
                                ' <div class="chatbox_timing">' +
                                '<ul>' +
                                '<li>' + date.getHours() + ':' + date.getMinutes() + '</li>' +
                                '</ul>' +
                                '</div>' +
                                '<img src="/Content/images/' + model.FromAvatarUser + '" alt="Picture">' +
                                '<div class="clearfix"></div>' +
                                ' <div class="ul_section_full">' +
                                ' <ul class="ul_msg">' +
                                '<li><strong>' + model.FromUserName + '</strong></li>' +
                                '<li>' + model.Content + '</li>' +
                                '</ul>' +
                                '<div class="clearfix"></div>' +
                                '</div>' +
                                '</div>'
                            );
                        }
                    })
                } else {
                    chatbox.append(
                        '<p>Chưa có tin nhắn...</p>'
                    );
                }
            }
        })
        $(".chatbox__body").animate({ scrollTop: 2000000 });
    }
    function getLastData() {
        var chatbox = $('.chatbox__body');
        var userID = $('#UserID').val();
        $.ajax({
            url: location.origin + "/Chat/GetLastMessageClient?UserID=" + userID,
            type: 'GET',
            dataType: 'JSON',
            success: function (data) {
                var date = new Date(parseInt(data.CreatedDate.substr(6)));
                chatbox.append(
                    '<div class="chatbox__body__message chatbox__body__message--right">' +
                    ' <div class="chatbox_timing">' +
                    '<ul>' +
                    '<li>' + date.getHours() + ':' + date.getMinutes() + '</li>' +
                    '</ul>' +
                    '</div>' +
                    '<img src="/Content/images/' + data.FromAvatarUser + '" alt="Picture">' +
                    '<div class="clearfix"></div>' +
                    ' <div class="ul_section_full">' +
                    ' <ul class="ul_msg">' +
                    '<li><strong>' + data.FromUserName + '</strong></li>' +
                    '<li>' + data.Content + '</li>' +
                    '</ul>' +
                    '<div class="clearfix"></div>' +
                    '</div>' +
                    '</div>')
            }
        })
    }
    $('#form-chat').submit(function send(e) {
        e.preventDefault();
        var fromId = $('#UserID').val();
        var content = $('#btn-input').val();
        $.ajax({
            url: location.origin + "/Chat/Send",
            data: { FromUserID: fromId, ToUserID: 1, Content: content, Side: "Client" },
            type: 'POST',
            dataType: 'JSON',
            success: function (data) {
                if (data.status) {
                    $.notify("Đã gửi!", "success");
                    $("#btn-chat").show();
                    $(".loading-send").hide();
                    getLastData();
                    $('#btn-input').val("");
                    $(".chatbox__body").animate({ scrollTop: 2000000 });
                } else {
                    $.notify("Có lỗi xảy ra!", "error");
                    return;
                }
            }
        })
        $("#btn-chat").hide();
        $(".loading-send").show();
    })
});