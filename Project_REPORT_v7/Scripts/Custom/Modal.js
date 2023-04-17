$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {
        $('#ModalPopupContent').load(this.href, function () {
            $('#ModalPopup').modal({
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#ModalPopup').modal('hide');
                    $('#progress').hide();
                    location.reload();
                } else {
                    $('#progress').hide();
                    $('#error').show();
                    $('#ModalPopup').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}