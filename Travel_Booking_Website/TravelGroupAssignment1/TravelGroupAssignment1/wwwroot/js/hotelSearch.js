// Ajax Search funcionalities for Flights (only on the Index page)
// This script is only run on the Index.cshtml page for FlightController

// function to call controller to retrieve data in JSON format
function loadHotelSearch(location, checkInDate, checkOutDate, guestCount) {

    $.ajax({
        url: '/Hotel/SearchAjax?location=' + location + '&startDate=' + checkInDate + '&endDate=' + checkOutDate + '&capacity=' + guestCount + '&tab=hotel',
        method: 'GET',
        // on successfully retrieving data, generate cards for each item
        success: (data) => {
            console.log(data)
            var hotelListHtml = '';
            for (var i = 0; i < data.length; i++) {
                hotel = data[i];
                hotelListHtml += '<div class="col-12 col-sm-6 col-md-4 col-lg-3">'
                hotelListHtml += '<div class="card shadow" style="width: 18rem; margin-bottom: 20px;">'
                hotelListHtml += '<a class="card-link" href="/Room/Search?hotelId=' + hotel.hotelId + '&capacity=' + guestCount + '&checkInDate=' + checkInDate + '&checkOutDate=' + checkOutDate + '">'
                hotelListHtml += '<img id="' + hotel.hotelId + '" src="https://cdn.britannica.com/96/115096-050-5AFDAF5D/Bellagio-Hotel-Casino-Las-Vegas.jpg" class="card-img-top img-fluid" alt="Hotel Room">'
                hotelListHtml += '<div class="card-body" style="max-height: 150px; overflow: auto;">'
                hotelListHtml += '<h4 class="card-title">' + hotel.hotelName + '</h4>'
                hotelListHtml += '<p class="card-text">' + hotel.description + '</p>'
                hotelListHtml += '</div>'
                hotelListHtml += '<ul class="list-group list-group-flush">'
                hotelListHtml += '<li class="list-group-item">'
                hotelListHtml += '<i class="fa-solid fa-location-dot"></i> ' + hotel.location;
                hotelListHtml += '</li>'
                hotelListHtml += '</ul>'
                hotelListHtml += '</a>'
                hotelListHtml += '</div>'
                hotelListHtml += '</div>'

            }

            if (data.length == 0) {
                hotelListHtml = '<p>Sorry, no available hotel matches your search criteria</p>'
            }

            // append to the list of items
            $('#hotelList').html(hotelListHtml);
        }
    });

}


// include and run partial script on DOM ready (entering car Index page)
$(() => {

    $('#hotelSearchForm').on("submit", (e) => {
        e.preventDefault(); // prevent reload and calling default search
        var location = $('#hotelLocation').val();
        var checkInDate = $('#hotelStartDate').val();
        var checkOutDate = $('#hotelEndDate').val();
        var guestCount = $('#hotelCapacity').val();

        // display loading icon 
        // not very obvious because it replaces the submit button quickly (still a better alternative than replacing search results bc visual stutter)
        $('#hotelSearchButton').html('<div class="w-50 h-50 d-flex justify-content-center"><img src="../images/loading-icon.gif"></div>')

        // call Ajax search function, populate grid with items
        loadHotelSearch(location, checkInDate, checkOutDate, guestCount);

        $('#hotelSearchButton').html('<button type="submit" class="btn btn-primary">Search</button>')
    })
});

