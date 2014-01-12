$.fn.outerHTML = function () {
    $t = $(this);
    if ('outerHTML' in $t[0]) {
        return $t[0].outerHTML;
    } else {
        var content = $t.wrap('<div></div>').parent().html();
        $t.unwrap();
        return content;
    }
};

$.postJSON = function (url, data, success) {
    return $.ajax({
        'type': 'POST',
        'url': url,
        'contentType': 'application/json',
        'data': JSON.stringify(data),
        'dataType': 'json',
        'success': success
    });
};