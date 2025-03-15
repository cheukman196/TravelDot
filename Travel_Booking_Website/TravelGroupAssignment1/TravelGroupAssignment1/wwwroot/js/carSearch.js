// Ajax Search funcionalities for Cars (only on the Index page)
// This script is only run on the Index.cshtml page for CarController

// function to call controller to retrieve data in JSON format
function loadCarSearch(carLocation, startDate, endDate) {
    $.ajax({
        url: '/Car/SearchAjax?location=' + carLocation + '&startDate=' + startDate + '&endDate=' + endDate + '&tab=car',
        method: 'GET',
        // on successfully retrieving data, generate cards for each item
        success: (data) => {
            var carsListHtml = '';
            console.log(data)
            for (var i = 0; i < data.length; i++) {
                car = data[i];
                carsListHtml += '<div class="col-12 col-sm-4" >'
                carsListHtml += '<div class="card shadow" style="width: 18rem; margin-bottom: 20px;">'
                carsListHtml += '<a href="/Car/Details/' + car.carId + '" class="card-link">'
                carsListHtml += '<img id ="' + car.carId + '" src="https://www.kbb.com/wp-content/uploads/2022/10/2023-toyota-rav4-prime-frt-3qtr.jpg?w=918"' +
                                    'class="card-img-top" alt = "Car" style = "width: 100%; height: 100%;" >'
                carsListHtml += '<div class="card-body">'
                carsListHtml += '<h5 class="card-title">' + car.make + ' ' + car.model + '</h5>'
                carsListHtml += '</div>'
                carsListHtml += '<ul class="list-group list-group-flush">'
                carsListHtml += '<li class="list-group-item">'
                carsListHtml += '<i class="fa-solid fa-user-group"></i> ' + car.maxPassengers + '</li>'
                carsListHtml += '<li class="list-group-item"><i class="fa-solid fa-code-branch"></i> ' + car.transmission + '</li>'
                carsListHtml += '<li class="list-group-item">'

                carsListHtml += '<i class="fa-solid fa-fan"></i> <span>' + (car.hasAirConditioning ? '' : 'No') + ' Air Conditioner</span></li>'
                carsListHtml += '<li class="list-group-item"><i class="fa-solid fa-globe" ></i> Model ' + car.model + '</li>'
                carsListHtml += '<li class="list-group-item"><i class="fa-solid fa-gauge-high"></i> <span>' + (car.hasUnlimitedMileage? 'Unlimited ' : 'Limited ') + 
                                    'Mileage</span></li>'
                carsListHtml += '<li class="list-group-item"><i class="fa-solid fa-building"></i> ' + (car.companyName == null ? "None" : car.companyName) + '</li>'
                carsListHtml += '<li class="list-group-item"><i class="fa-solid fa-tag"></i> $' + car.pricePerDay + ' / day</li>'
                carsListHtml += '</ul>'
                carsListHtml += '</a>'    
                carsListHtml += '<a href="/CarBooking/Create/' + car.carId + '/' + startDate + '/' + endDate + '" class="btn btn.sm btn-primary" role="button">Reserve Car</a>'
                carsListHtml += '</div>'    
                carsListHtml += '</div>'    
            }

            if (data.length == 0) {
                carsListHtml = '<p>Sorry, no available Car that matches your search criteria</p>'
            }

            // append to the list of items
            $('#carList').html(carsListHtml);
        }
    });

}
// include and run partial script on DOM ready (entering car Index page)
$(() => {
    var carLocation = $('#carLocation').val();
    var carStartDate = $('#carStartDate').val();
    var carEndDate = $('#carEndDate').val();

    $('#carLocation').val(carLocation)
    $('#carStartDate').val(carStartDate)
    $('#carEndDate').val(carEndDate)

    console.log(carLocation, carStartDate, carEndDate)
    console.log($('#carLocation').val(), $('#carStartDate').val(), $('#carEndDate').val())

    if (carLocation && carStartDate && carEndDate) {
        loadCarSearch(carLocation, carStartDate, carEndDate);
    }

    $('#carSearchForm').on("submit", (e) => {
        
        e.preventDefault(); // prevent reload and calling default search
        var carLocation = $('#carLocation').val();
        var carStartDate = $('#carStartDate').val();
        var carEndDate = $('#carEndDate').val();

        // display loading icon 
        // not very obvious because it replaces the submit button quickly (still a better alternative than replacing search results bc visual stutter)
        $('#carSearchButton').html('<div class="w-50 h-50 d-flex justify-content-center"><img src="../images/loading-icon.gif"></div>')

        // call Ajax search function, populate grid with items
        loadCarSearch(carLocation, carStartDate, carEndDate);


        $('#carSearchButton').html('<button type="submit" class="btn btn-primary">Search</button>')
    })
});

