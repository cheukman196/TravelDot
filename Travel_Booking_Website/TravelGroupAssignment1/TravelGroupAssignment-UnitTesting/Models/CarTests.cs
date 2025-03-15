using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Reflection;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using Xunit.Sdk;

namespace TravelGroupAssignment_UnitTesting.Models
{
    public class CarTests
    {

        // === Helper functions ===
        public Car createFakeCar()
        {
            return new Car
            {
                CarId = 1,
                Make = "Toyota",
                Model = "Corolla",
                Type = "Sedan",
                PricePerDay = 0,
                MaxPassengers = 5,
                Transmission = "Auto",
                HasAirConditioning = true,
                HasUnlimitedMileage = false,
                CompanyId = 1,
                Bookings = new List<CarBooking>()
            };
        }

        [Fact]
        public void createFakeCar_returnCarIsValid()
        {
            var car = createFakeCar();

            var validationContext = new ValidationContext(car);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(car, validationContext, validationResults);

            Assert.Equal(true, objectIsValid);
        }


        // === Testing Get & Set for attributes ===

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void CarId_SetId_ReturnSameId(int carId)
        {
            var mockCar = new Car();

            mockCar.CarId = carId;

            Assert.Equal(carId, mockCar.CarId);
        }


        [Theory]
        [InlineData("")]
        [InlineData("Forester")]
        [InlineData("Civic")]
        [InlineData("Fiesta")]
        [InlineData("4Runner")]
        public void CarModel_SetModel_ReturnSameModel(string model)
        {
            var mockCar = new Car();

            mockCar.Model = model;

            Assert.Equal(model, mockCar.Model);
        }


        [Theory]
        [InlineData("")]
        [InlineData("Toyota")]
        [InlineData("Honda")]
        [InlineData("Ford")]
        [InlineData("Mitsubishi")]
        public void CarMake_SetMake_ReturnSameMake(string make)
        {
            var mockCar = new Car();

            mockCar.Make = make;

            Assert.Equal(make, mockCar.Make);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Sedan")]
        [InlineData("Hatchback")]
        [InlineData("SUV")]
        [InlineData("Sport")]
        public void CarType_SetType_ReturnSameType(string type)
        {
            var mockCar = new Car();

            mockCar.Type = type;

            Assert.Equal(type, mockCar.Type);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(10000.50)]
        [InlineData(1000000.99)]
        public void CarPricePerDay_SetPrice_ReturnSamePrice(double price)
        {
            var mockCar = new Car();

            mockCar.PricePerDay = price;

            Assert.Equal(price, mockCar.PricePerDay);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(4)]
        [InlineData(9)]
        public void CarMaxPassengers_SetMaxPassengers_ReturnSameMaxPassengers(int maxPassengers)
        {
            var mockCar = new Car();

            mockCar.MaxPassengers = maxPassengers;

            Assert.Equal(maxPassengers, mockCar.MaxPassengers);
        }

        [Theory]
        [InlineData("Manual")]
        [InlineData("Automatic")]
        [InlineData("Dual Clutch")]
        public void CarTransmission_SetTransmission_ReturnSameTransmission(string transmission)
        {
            var mockCar = new Car();

            mockCar.Transmission = transmission;

            Assert.Equal(transmission, mockCar.Transmission);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CarHasAirCon_SetHasAirCon_ReturnSameHasAirCon(bool hasAirCon)
        {
            var mockCar = new Car();

            mockCar.HasAirConditioning = hasAirCon;

            Assert.Equal(hasAirCon, mockCar.HasAirConditioning);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CarHasUnlimitedMileage_SetHasUnlimitedMileage_ReturnSameHasUnlimitedMileage(bool hasUnlimitedMileage)
        {
            var mockCar = new Car();

            mockCar.HasUnlimitedMileage = hasUnlimitedMileage;

            Assert.Equal(hasUnlimitedMileage, mockCar.HasUnlimitedMileage);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(500)]
        [InlineData(10000)]
        public void CarCompanyId_SetCompanyId_ReturnSameCompanyId(int companyId)
        {
            var mockCar = new Car();

            mockCar.CompanyId = companyId;

            Assert.Equal(companyId, mockCar.CompanyId);
        }

        // testing navigation properties
        [Fact]
        public void CarCompany_SetCompany_ReturnSameCompany()
        {
            var mockCar = new Car();

            mockCar.Company = new CarRentalCompany();

            Assert.True(mockCar.Company.GetType() == typeof(CarRentalCompany));
        }

        // testing nullable references
        [Fact]
        public void CarCompany_SetNull_ReturnNull()
        {
            var mockCar = new Car();

            mockCar.Company = null;

            Assert.Null(mockCar.Company);
        }

        [Fact]
        public void CarBookings_SetTwoBookings_ReturnTwoBookings()
        {
            var mockCar = createFakeCar();

            var stubBooking = new CarBooking();
            mockCar.Bookings.Add(stubBooking);
            mockCar.Bookings.Add(stubBooking);

            Assert.Equal(2, mockCar.Bookings.Count);
        }

        [Fact]
        public void CarBookings_SetNull_ReturnNull()
        {
            var mockCar = createFakeCar();

            mockCar.Bookings = null;

            Assert.Null(mockCar.Bookings);
        }


        // === Testing Data Annotations ===

        // [Required] exists
        [Fact]
        public void CarMake_TestRequiredAttribute_Exists()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Make));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);

        }

        // [Required] validation works
        [Theory]
        [InlineData("", false)] // case: not provided
        [InlineData("1", true)]
        [InlineData("Toyota", true)]
        [InlineData("GMC", true)]
        [InlineData("Subaru", true)]
        public void CarMake_TestRequiredAttribute_ValidationSuccess(string make, bool resultValid)
        {
            var car = createFakeCar();
            car.Make = make;

            var validationContext = new ValidationContext(car);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(car, validationContext, validationResults);

            Assert.Equal(objectIsValid, resultValid);
        }

        // [Display(Name = ...)] name matches
        [Fact]
        public void CarMake_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Make));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Make", displayNameAttribute.Name);
        }

        // [StringLength(100, ErrorMessage = ...)] value and error message matches
        [Fact]
        public void CarMake_TestStringLengthAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Make));

            var stringLengthAttribute = carProperties.GetCustomAttribute<StringLengthAttribute>();

            Assert.Equal(100, stringLengthAttribute.MaximumLength);
            Assert.Equal("Car make must not exceed 100 characters.", stringLengthAttribute.ErrorMessage);
        }


        [Fact]
        public void CarModel_TestRequiredAttribute_Exists()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Model));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);

        }

        [Theory]
        [InlineData("", false)] // case: not provided
        [InlineData("1", true)]
        [InlineData("Corolla", true)]
        [InlineData("Model Y", true)]
        [InlineData("CX9", true)]
        public void CarModel_TestRequiredAttribute_ValidationSuccess(string model, bool resultValid)
        {
            var car = createFakeCar();
            car.Model = model;

            var validationContext = new ValidationContext(car);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(car, validationContext, validationResults);

            Assert.Equal(objectIsValid, resultValid);
        }

        [Fact]
        public void CarModel_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Model));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Model", displayNameAttribute.Name);
        }


        [Fact]
        public void CarModel_TestStringLengthAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Model));

            var stringLengthAttribute = carProperties.GetCustomAttribute<StringLengthAttribute>();

            Assert.Equal(100, stringLengthAttribute.MaximumLength);
            Assert.Equal("Car model must not exceed 100 characters.", stringLengthAttribute.ErrorMessage);
        }


        [Fact]
        public void CarType_TestRequiredAttribute_Exists()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Type));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);

        }

        [Theory]
        [InlineData("", false)] // case: not provided
        [InlineData("Sedan", true)]
        [InlineData("Hatchback", true)]
        [InlineData("SUV", true)]
        [InlineData("Minivan", true)]
        public void CarType_TestRequiredAttribute_ValidationSuccess(string type, bool resultValid)
        {
            var car = createFakeCar();
            car.Type = type;

            var validationContext = new ValidationContext(car);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(car, validationContext, validationResults);

            Assert.Equal(objectIsValid, resultValid);
        }

        [Fact]
        public void CarType_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Type));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Type", displayNameAttribute.Name);
        }


        [Fact]
        public void CarType_TestStringLengthAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.Type));

            var stringLengthAttribute = carProperties.GetCustomAttribute<StringLengthAttribute>();

            Assert.Equal(100, stringLengthAttribute.MaximumLength);
            Assert.Equal("Car type must not exceed 100 characters.", stringLengthAttribute.ErrorMessage);
        }


        [Fact]
        public void CarPricePerDay_TestRequiredAttribute_Exists()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.PricePerDay));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }


        [Fact]
        public void CarPricePerDay_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.PricePerDay));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Price Per Day", displayNameAttribute.Name);
        }

        // [Range(min, max, errorMessage = ...)] Range attribute test min, max values
        [Fact]
        public void CarPricePerDay_TestRangeAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.PricePerDay));

            var rangeAttribute = carProperties.GetCustomAttribute<RangeAttribute>();

            Assert.Equal(double.MaxValue, rangeAttribute.Maximum);
            Assert.Equal(0.00, rangeAttribute.Minimum);
        }


        [Fact]
        public void CarMaxPassengers_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.MaxPassengers));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Capacity", displayNameAttribute.Name);
        }

        // [Range(min, max, errorMessage = ...)] Range attribute test min, max values
        [Fact]
        public void CarMaxPassengers_TestRangeAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.MaxPassengers));

            var rangeAttribute = carProperties.GetCustomAttribute<RangeAttribute>();

            Assert.Equal(1000, rangeAttribute.Maximum);
            Assert.Equal(1, rangeAttribute.Minimum);
        }

        [Fact]
        public void CarHasAirConditioning_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.HasAirConditioning));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Has Air Conditioning", displayNameAttribute.Name);
        }

        [Fact]
        public void CarHasUnlimitedMileage_TestDisplayNameAttribute_ValueMatch()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.HasUnlimitedMileage));

            var displayNameAttribute = carProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Has Unlimited Mileage", displayNameAttribute.Name);
        }

        [Fact]
        public void CarCompanyId_TestRequiredAttribute_Exists()
        {
            var carProperties = typeof(Car).GetProperty(nameof(Car.CompanyId));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }
    }
}