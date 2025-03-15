function loadComments(hotelId) {

    $.ajax({
        url: '/HotelComment/GetComments/'+ hotelId,
        method: 'GET',
        success: (data) => {
            var commentHtml = '';
            for (var i = 0; i < data.length; i++) {
                console.log(data[i])
                commentHtml += '<div class="comment container mt-2 card">'
                commentHtml += '<span><b>Author</b>: ' + data[i].author + '&nbsp&nbsp <b>Rating</b>: ' + data[i].rating +'</span>'
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

    var hotelId = $('#hotelComments input[name="HotelId"]').val();
    var author = $('#hotelComments input[name="Username"]').val();

    loadComments(hotelId);

    $('#addCommentForm').on("submit", function (e) {

        e.preventDefault(); // prevent reload
        var formData = {
            HotelId: hotelId,
            Author: author,
            Content: $('#hotelComments textarea[name="Content"]').val(),
            Rating: $('#hotelComments Select[name="Rating"]').val()
        };

        $.ajax({
            url: '/HotelComment/AddComment',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: (response) => {
                // handling success and failure status
                if (response.success) {

                    // reset form values
                    $('#hotelComments textarea[name="Content"]').val(''); 
                    $('#hotelComments select[name="Rating"]').val('5');
                    loadComments(hotelId);
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