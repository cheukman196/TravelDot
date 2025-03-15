// Ajax Search funcionalities for Flights (only on the Index page)
// This script is only run on the Index.cshtml page for FlightController

// function to call controller to retrieve data in JSON format
function loadRoomSearch(hotelId, capacity, checkInDate, checkOutDate) {

    $.ajax({
        url: '/Room/SearchAjax?hotelId=' + hotelId + '&capacity=' + capacity + '&checkInDate=' + checkInDate + '&checkOutDate=' + checkOutDate,
        method: 'GET',
        // on successfully retrieving data, generate cards for each item
        success: (data) => {
            console.log(data)
            var roomListHtml = '';
            for (var i = 0; i < data.length; i++) {
                console.log(data)
                room = data[i];
                roomListHtml += '<div class="col-12 col-sm-4">'
                roomListHtml += '<div class="card shadow" style="width: 18rem;">'
                roomListHtml += '<a href="/Room/Details/' + room.roomId +'" class="card-link">'
                roomListHtml += '<img id="' + room.roomId + '" src="https://assets-global.website-files.com/5c6d6c45eaa55f57c6367749/65045f093c166fdddb4a94a5_x-65045f0266217.webp" class="card-img-top" alt="Hotel Room" style="width: 100%; height: 100%;">'
                roomListHtml += '<div class="card-body">'
                roomListHtml += '<h4 class="card-title">' + room.name + '</h4>'
                roomListHtml += '</div>'
                roomListHtml += '<ul class="list-group list-group-flush">'
                roomListHtml += '<li class="list-group-item"><i class="fa-solid fa-user-group" ></i > ' + room.capacity + ' </li>'
                roomListHtml += '<li class="list-group-item"><i class="fa-solid fa-bed" ></i > ' + room.bedDescription + ' </li>'
                roomListHtml += '<li class="list-group-item"><i class="fa-solid fa-expand" ></i > ' + room.roomSize + ' sq. ft.</li>'
                roomListHtml += '<li class="list-group-item"><i class="fa-solid fa-tag" ></i > ' + room.pricePerNight + ' / night</li>'
                roomListHtml += '</ul>'
                roomListHtml += '</a>'
                roomListHtml += '<a href="/RoomBooking/Create/' + room.roomId + '/' + checkInDate + '/' + checkOutDate +
                                '" class="btn btn.sm btn-primary" role="button">Reserve Room</a>'
                roomListHtml += '</div>'
                roomListHtml += '</div>'

            }

            if (data.length == 0) {
                roomListHtml = '<p>Sorry, no available rooms matches your search criteria</p>'
            }

            // append to the list of items
            $('#roomList').html(roomListHtml);
        }
    });

}
        
        
// include and run partial script on DOM ready (entering car Index page)
$(() => {

    $('#roomSearchForm').on("submit", (e) => {
        e.preventDefault(); // prevent reload and calling default search

        var hotelId = $('#hotelId').val();
        var capacity = $('#capacity').val();
        var checkInDate = $('#checkInDate').val();
        var checkOutDate = $('#checkOutDate').val();

        // display loading icon 
        // not very obvious because it replaces the submit button quickly (still a better alternative than replacing search results bc visual stutter)
        $('#roomSearchButton').html('<div class="w-50 h-50 d-flex justify-content-center"><img src="../images/loading-icon.gif"></div>')

        // call Ajax search function, populate grid with items
        loadRoomSearch(hotelId, capacity, checkInDate, checkOutDate);

        $('#roomSearchButton').html('<button type="submit" class="btn btn-primary">Search</button>')
    })
});

