jQuery(function ($) {
    $("#dynamic").click(function () {
        $.ajax({
            url: '/Home/LoadPartialView',
            type: 'GET',
            success: function (result) {
                $("#partialViewContainer").append("<h2>hi</h2>");
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    });
});
