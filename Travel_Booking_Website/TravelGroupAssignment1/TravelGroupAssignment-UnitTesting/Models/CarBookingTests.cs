using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Validation;
using Xunit;

namespace TravelGroupAssignment_UnitTesting.Models
{
    public class CarBookingTests
    {
        public CarBooking createFakeCarBooking()
        {
            CarBooking booking = new CarBooking();
            booking.CarId = 1;
            booking.Car = new Car();
            booking.StartDate = DateTime.Now;
            booking.EndDate = DateTime.Now.AddDays(2);

            Console.WriteLine(booking.BookingReference);

            return booking;
        }

        // === Attribute Get and Set ===

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void CarBookingId_SetCarId_ReturnSameCarId(int carId)
        {
            var mockCarBooking = new CarBooking();

            mockCarBooking.CarId = carId;

            Assert.Equal(carId, mockCarBooking.CarId);
        }

        [Fact]
        public void CarBookingCar_SetCar_ReturnSameCar()
        {
            var mockCarBooking = new CarBooking();
            Car mockCar = new Car();

            mockCarBooking.Car = mockCar;

            Assert.Equal(mockCar, mockCarBooking.Car);
        }

        [Fact]
        public void CarBookingCar_SetNull_ReturnNull()
        {
            var mockCarBooking = new CarBooking();

            mockCarBooking.Car = null;

            Assert.Null(mockCarBooking.Car);
        }

        [Fact]
        public void CarBookingStartDate_SetStartDate_ReturnSameDate()
        {
            var mockCarBooking = new CarBooking();

            DateTime timestamp = DateTime.UtcNow;
            mockCarBooking.StartDate = timestamp;

            Assert.Equal(timestamp, mockCarBooking.StartDate);
        }

        [Fact]
        public void CarBookingEndDate_SetEndDate_ReturnSameDate()
        {
            var mockCarBooking = new CarBooking();

            DateTime timestamp = DateTime.UtcNow.AddDays(2);
            mockCarBooking.EndDate = timestamp;

            Assert.Equal(timestamp, mockCarBooking.EndDate);
        }


        // === Data Annotations ===

        [Fact]
        public void CarBookingCarId_TestRequiredAttribute_Exists()
        {
            var carBookingProperties = typeof(CarBooking).GetProperty(nameof(CarBooking.CarId));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carBookingProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(50, true)]
        [InlineData(100, true)]
        [InlineData(5000, true)]
        [InlineData(10000, true)]
        public void CarBookingCarId_TestRequiredAttribute_ValidationSuccess(int carId, bool resultValid)
        {
            var stubCarBooking = createFakeCarBooking();
            stubCarBooking.CarId = carId;

            var validationContext = new ValidationContext(stubCarBooking);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(stubCarBooking, validationContext, validationResults);

            Assert.Equal(resultValid, objectIsValid);
        }

        [Fact]
        public void CarBookingStartDate_TestRequiredAttribute_Exists()
        {
            var carBookingProperties = typeof(CarBooking).GetProperty(nameof(CarBooking.StartDate));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carBookingProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CarBookingStartDate_AssignNull_ReturnObjectInvalid()
        {
            var stubCarBooking = createFakeCarBooking();
            stubCarBooking.StartDate = null;

            var validationContext = new ValidationContext(stubCarBooking);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(stubCarBooking, validationContext, validationResults);

            Assert.Equal(false, objectIsValid);
        }

        [Fact]
        public void CarBookingEndDate_TestRequiredAttribute_Exists()
        {
            var carBookingProperties = typeof(CarBooking).GetProperty(nameof(CarBooking.EndDate));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carBookingProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CarBookingEndDate_AssignNull_ReturnObjectInvalid()
        {
            var stubCarBooking = createFakeCarBooking();
            stubCarBooking.EndDate = null;

            var validationContext = new ValidationContext(stubCarBooking);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(stubCarBooking, validationContext, validationResults);

            Assert.Equal(false, objectIsValid);
        }


    }
}
