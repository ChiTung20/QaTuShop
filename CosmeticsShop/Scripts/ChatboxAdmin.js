
$(function () {
    var chat = $.connection.chatHubs
    chat.client.displayMessage = function () {
        getMessageNotiData();
    }
    $.connection.hub.start();
    getMessageNotiData();
})
function getMessageNotiData() {
    var $noti = $('#messageNoti')
    var $totalNofi = $('#messTotalNoti')
    $.ajax({
        url: location.origin + "/Chat/GetNotiMessage",
        type: 'GET',
        dataType: 'JSON',
        success: function (data) {
            $noti.empty()
            $totalNofi.empty()
            if (data.length > 0) {
                $totalNofi.append(
                    data.length
                )
                $.each(data, function (i, model) {
                    $noti.append(
                        '<a href = "/Chat/Chating?WithUserID=' + model.FromUserID + '&MessageID=' + model.ID + '" class= "dropdown-item" id="' + model.FromUserID + '">' +
                        '<div class="media">' +
                        '<img src="/Content/images/' + model.FromUserAvatar + '" alt="User Avatar" class="img-size-50 mr-3 img-circle">' +
                        '<div class="media-body">' +
                        '<h3 class="dropdown-item-title">'
                        + model.FromUserName +
                        '<span class="float-right text-sm text-danger"><i class="fas fa-star"></i></span>' +
                        '</h3>' +
                        '<p class="text-sm text-muted"><i class="far fa-clock mr-1"></i>' + model.CreatedDate + ' phút trước</p>' +
                        '</div>' +
                        '</div>' +
                        '</a>'
                    );
                })
            } else {
                $totalNofi.append(
                    0
                )
                $noti.append(
                    '<a href="/Chat/" class="dropdown-item dropdown-footer">Xem tất cả tin nhắn</a>'
                );
            }
        }
    })
}
$('#btn-chat').click(function send() {
    var fromId = $('#user-id').val();
    var content = $('#btn-input').val();
    $.ajax({
        url: location.origin + "/Chat/Send",
        data: { FromUserID: fromId, ToUserID: 1, Content: content, Side: "Admin" },
        type: 'POST',
        dataType: 'JSON',
        success: function (data) {
            if (data.status) {
                getLastData();
                $('#btn-input').val("");
                $(".chatbox__body").animate({ scrollTop: 2000000 });
                console.log("yes");
            } else {
                alert("Có lỗi xảy ra");
                return;
            }
        }
    })
})