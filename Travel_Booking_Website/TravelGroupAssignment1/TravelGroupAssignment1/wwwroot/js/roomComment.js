function loadComments(roomId) {

    $.ajax({
        url: '/RoomComment/GetComments/'+ roomId,
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

    var roomId = $('#roomComments input[name="RoomId"]').val();
    var author = $('#roomComments input[name="Username"]').val();

    loadComments(roomId);

    $('#addCommentForm').on("submit", function (e) {

        e.preventDefault(); // prevent reload
        var formData = {
            RoomId: roomId,
            Author: author,
            Content: $('#roomComments textarea[name="Content"]').val(),
            Rating: $('#roomComments Select[name="Rating"]').val()
        };

        $.ajax({
            url: '/RoomComment/AddComment',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: (response) => {
                // handling success and failure status
                if (response.success) {

                    // reset form values
                    $('#roomComments textarea[name="Content"]').val(''); 
                    $('#roomComments select[name="Rating"]').val('5');
                    loadComments(roomId);
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