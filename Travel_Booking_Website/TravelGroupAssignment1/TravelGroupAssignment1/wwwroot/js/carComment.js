function loadComments(carId) {

    $.ajax({
        url: '/CarComment/GetComments/'+ carId,
        method: 'GET',
        success: (data) => {
            var commentHtml = '';
            for (var i = 0; i < data.length; i++) {
                console.log(data[i])
                commentHtml += '<div class="comment container mt-2 card">'
                commentHtml += '<span><b>Author</b>: ' + data[i].author + '&nbsp&nbsp <b>Rating</b>: ' + data[i].rating + '</span>'
                commentHtml += '<p class="text-body">' + data[i].content + '</p>'

                commentHtml += '<span class="text-secondary">Posted on: ' + new Date(data[i].datePosted).toLocaleString() + '</span>'
                commentHtml += '</div>'
            }
            $('#commentList').html(commentHtml);
        }
    });

}


// on document ready run function
$(() => {

    var carId = $('#carComments input[name="CarId"]').val();
    var author = $('#carComments input[name="Username"]').val();

    loadComments(carId);

    $('#addCommentForm').on("submit", function (e) {

        e.preventDefault(); // prevent reload
        var formData = {
            CarId: carId,
            Author: author,
            Content: $('#carComments textarea[name="Content"]').val(),
            Rating: $('#carComments Select[name="Rating"]').val()
        };

        $.ajax({
            url: '/CarComment/AddComment',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: (response) => {
                // handling success and failure status
                if (response.success) {

                    // reset form values
                    $('#carComments textarea[name="Content"]').val(''); 
                    $('#carComments select[name="Rating"]').val('5');
                    loadComments(carId);
                } else {
                    alert("Please write something for the comment!\n" + response.message)
                }

            },
            // Handling exceptions
            error: function (xhr, status, error) {
                alert("Error: " + error)
            }

        });


    });

});