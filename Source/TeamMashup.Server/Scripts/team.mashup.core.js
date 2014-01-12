$(document).ready(function () {
    $(".no-submit").each(function () {
        $(this).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
    });
});

function bindJsModals() {
    $('.js-load-modal').on('click', function (event) {

        event.preventDefault();

        // create the backdrop and wait for next modal to be triggered
        $('body').modalmanager('loading');

        var href;
        if (typeof ($(this).attr('href')) != 'undefined') {
            href = $(this).attr('href').replace('#', ' #');
        } else {
            href = $(this).find('A').attr('href').replace('#', ' #');
        }

        setTimeout(function () {
            $modal.load(href, '', function () {
                $modal.modal();
            });
        }, 1000);
    });
}

function applyUiErrorStylesIfAny() {

    $(".form-group").each(function () {
        if ($(this).find(".field-validation-error").length != 0) {
            $(this).addClass("has-error");
        }
    });

    $(".popover.validation").each(function () {

        var popover = $(this);
        var targetName = popover.data("for");
        var target = $("#" + targetName);

        target.click(function () {
            popover.toggle();
        });

        var position = target.position();
        var offset = target.outerHeight();

        popover.css({ top: position.top + offset, left: position.left, display: 'block' });
    });

}

function handleRowDelete(options) {

    options.modal = $('#' + options.modalId);
    options.modal.off('click', '.delete');

    options.modal.load(options.displayAction, function () {
        options.modal.modal();
    });

    options.modal.on('click', '.delete', function () {

        $.ajax({
            url: options.deleteAction,
            type: "POST",
            data: JSON.stringify({ ids: options.ids }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function () { }
        })
        .done(function (data) {
            if (data.Success == true) {
                options.modal.modal('hide');
                table.draw();
            }
            else {
                options.modal.find('.modal-body .alert')
                        .empty()
                        .append('<p>' + data.Error + '</p>')
                        .removeClass("hide");
            }
        });

    });
}

function onAjaxFormComplete(xhr, status, complete) {

    if (complete != null) {
        complete();
    }

}

function onAjaxFormSuccess(data, status, xhr, success) {

    if (success != null) {
        success(data);
    }

}

function onAjaxFormFailure(xhr, status, error, validationErrors) {

    var response = xhr.responseJSON;

    if (xhr.status == 422) {
        if (validationErrors != null) {
            return validationErrors(response);
        }
    }

    console.log(response.Message);
    window.location.href = response.RedirectUrl;

}

function setChatPopoverPosition(popover, trigger) {
    var position = trigger.position();
    var topOffset = trigger.outerHeight();
    var leftOffset = popover.outerWidth() - trigger.outerWidth();

    popover.css({ top: position.top + topOffset, left: position.left - leftOffset });
}

/* Delete Handlers */

function deleteComplete() {
}

function deleteSuccess() {
    modal.modal('hide');
    table.draw();
}

function deleteValidationErrors(data) {
    modal.html(data.View);
    modal.find('.modal-body .alert').removeClass("hide");
    applyUiErrorStylesIfAny();
}

/* End of Delete Handlers */