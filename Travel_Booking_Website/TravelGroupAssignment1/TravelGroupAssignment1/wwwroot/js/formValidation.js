




// Client-side validation for search functions (start date <= end date)
// function requires 2 parameters (IDs of start date and end date input tags)
function updateCheckOutDateLimit(startDateId, EndDateId){
    var checkInDate = document.getElementById(startDateId).value;
    var checkOutDate = document.getElementById(EndDateId).value;
    document.getElementById(EndDateId).min = checkInDate; // set end date min value
    if (checkOutDate < checkInDate)
        document.getElementById(EndDateId).value = checkInDate;
}

