function Notifications(options) {

    var container = $('#' + options.containerId);
    container.hide();
    container.addClass("alert");

    this.showError = function (message) {
        container.addClass("alert-error");
        container.text(message);
        container.show();
        container.hide("fade", {}, 5000, function () {
            container.text("");
            container.removeClass("alert-error");
        });
    }

    return this;
}

function getIssueTypeCssClass(type) {

    switch (type) {
        case 'User Story':
            return "label-success";
            break;
        case 'Defect':
            return "label-danger";
            break;
        default:
            return "label-success";
    }

}

function renderComments(containerId, data) {

    var container = $("#" + containerId);
    var commentTemplate = '<li class="media">' +
                            '<a class="pull-left" href="#">' +
                                '<img class="media-object" src="{{user_image_url}}" alt="User Image">' +
                            '</a>' +
                            '<div class="media-body">' +
                                '<h4 class="media-heading">{{user_name}}</h4>' +
                                '<p>{{message}}</p>' +
                                '<div>{{replies}}</div>' +
                                '<input type="text" data-id="{{id}}" class="form-control reply" placeholder="{{reply_placeholder}}"></input>'
                            '</div>' +
                          '</li>';

    var replyTemplate = '<div class="media">' +
                            '<a class="pull-left" href="#">' +
                                '<img class="media-object" src="{{user_image_url}}" alt="User Image">' +
                            '</a>' +
                            '<div class="media-body">' +
                                '<h4 class="media-heading">{{user_name}}</h4>' +
                                '<p>{{reply_message}}</p>' +
                            '</div>' +
                         '</div>';

    var commentsHtml = '';

    //Render all comments.
    for (var key in data.Comments) {

        var comment = data.Comments[key];

        var repliesHtml = '';

        for (var key2 in comment.Replies) {
            var reply = comment.Replies[key2];

            var replyModel = {
                user_name: reply.UserName,
                user_image_url: "/Content/images/user-placeholder-50x50.jpg",
                reply_message: reply.Message
            };

            repliesHtml += Mustache.to_html(replyTemplate, replyModel);
        }

        var commentModel = {
            id: comment.Id,
            reply_placeholder: "write a comment...",
            user_name: comment.UserName,
            user_image_url: "/Content/images/user-placeholder-50x50.jpg",
            message: comment.Message,
            replies: repliesHtml == undefined ? '' : repliesHtml
        };

        commentsHtml += Mustache.to_html(commentTemplate, commentModel);
        
    }

    var html = $('<textarea />').html(commentsHtml).val();
    container.html(html);
}
