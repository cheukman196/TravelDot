// Ajax Search funcionalities for Flights (only on the Index page)
// This script is only run on the Index.cshtml page for FlightController

// function to call controller to retrieve data in JSON format
function loadFlightSearch(flightFrom, flightTo, flightStartDate, flightEndDate, passengerCount) {

    $.ajax({
        url: '/Flight/SearchAjax?locationFrom=' + flightFrom + '&location=' + flightTo + '&startDate=' + flightStartDate + '&endDate=' + flightEndDate + '&capacity=' + passengerCount + '&tab=flight',
        method: 'GET',
        // on successfully retrieving data, generate cards for each item
        success: (data) => {
            console.log(data)
            var flightListHtml = '';
            for (var i = 0; i < data.length; i++) {
                flight = data[i];
                flightListHtml += '<div class="col-md-6">'
                flightListHtml += '<div class="flight-card shadow custom-rounded">'
                flightListHtml += '<div class="flight-card-body">'
                flightListHtml += '<div class="flight-info">'
                flightListHtml += '<p><b>Departure Time:</b> ' + flight.departTime + ' </p>'
                flightListHtml += '<p><b>Arrival Time:</b> ' + flight.arrivalTime + ' </p>'
                flightListHtml += '<p><strong>From:</strong> ' + flight.from + ' <strong>To:</strong>  ' + flight.to + ' </p>'
                flightListHtml += '<p><b>Airline:</b>   ' + flight.airline + ' </p>'
                flightListHtml += '</div>'
                flightListHtml += '<div class="flight-price">$' + flight.price + '</div>'
                flightListHtml += '</div>'
                flightListHtml += '<div class="text-center custom-margin">'
                flightListHtml += '<a href="/FlightBooking/Create/' + flight.flightId + '" class="btn btn-primary btn-lg">Book</a>'
                flightListHtml += '</div>'
                flightListHtml += '</div>'
                flightListHtml += '</div>'
            }

            if (data.length == 0) {
                hotelListHtml = '<p>Sorry, no available flight matches your search criteria</p>'
            }

            // append to the list of items
            $('#flightList').html(flightListHtml);
        }
    });

}


// include and run partial script on DOM ready (entering car Index page)
$(() => {

    $('#flightSearchForm').on("submit", (e) => {
        e.preventDefault(); // prevent reload and calling default search
        var flightFrom = $('#flightLocationFrom').val();
        var flightTo = $('#flightLocation').val();
        var flightStartDate = $('#flightStartDate').val();
        var flightEndDate = $('#flightEndDate').val();
        var passengerCount = $('#flightCapacity').val();

        // display loading icon 
        // not very obvious because it replaces the submit button quickly (still a better alternative than replacing search results bc visual stutter)
        $('#flightSearchButton').html('<div class="w-50 h-50 d-flex justify-content-center"><img src="../images/loading-icon.gif"></div>')

        // call Ajax search function, populate grid with items
        loadFlightSearch(flightFrom, flightTo, flightStartDate, flightEndDate, passengerCount);


        $('#flightSearchButton').html('<button type="submit" class="btn btn-primary">Search</button>')
    })
});

