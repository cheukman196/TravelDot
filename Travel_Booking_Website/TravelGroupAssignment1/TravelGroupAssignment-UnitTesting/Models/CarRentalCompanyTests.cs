using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using Xunit;

namespace TravelGroupAssignment_UnitTesting.Models
{
    public class CarRentalCompanyTests
    {
        // === Helper Function ===
        public CarRentalCompany createFakeCompany()
        {
            return new CarRentalCompany
            {
                CarRentalCompanyId = 1,
                CompanyName = "Company",
                Location = "Toronto",
                Rating = 0,
                Cars = new List<Car>()
            };
        }

        // === Attributes get and set ===

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void CompanyId_SetId_ReturnSameId(int companyId)
        {
            var mockCompany = new CarRentalCompany();

            mockCompany.CarRentalCompanyId = companyId;

            Assert.Equal(companyId, mockCompany.CarRentalCompanyId);
        }

        [Theory]
        [InlineData("Autobreeze")]
        [InlineData("Highway 06")]
        [InlineData("Ferguson and Son's Limited")]
        [InlineData("")]
        [InlineData(null)] // nullable despite [Required] tag form validation
        public void CompanyName_SetName_ReturnSameName(string name)
        {
            var mockCompany = new CarRentalCompany();

            mockCompany.CompanyName = name;

            Assert.Equal(name, mockCompany.CompanyName);
        }

        [Theory]
        [InlineData("18 Redwood Drive")]
        [InlineData("Downtown Toronto")]
        [InlineData("Jacksonville West")]
        [InlineData("")]
        [InlineData(null)] // nullable despite [Required] tag form validation
        public void CompanyLocation_SetLocation_ReturnSameLocation(string location)
        {
            var mockCompany = new CarRentalCompany();

            mockCompany.Location = location;

            Assert.Equal(location, mockCompany.Location);
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(3.55)]
        [InlineData(5)]
        [InlineData(4.21)]
        [InlineData(1.21)]
        public void CompanyRating_SetRating_ReturnSameRating(double rating)
        {
            var mockCompany = new CarRentalCompany();

            mockCompany.Rating = rating;

            Assert.Equal(rating, mockCompany.Rating);
        }

        [Fact]
        public void CompanyCars_SetTwoCars_ReturnTwo()
        {
            var mockCompany = new CarRentalCompany();
            mockCompany.Cars = new List<Car>();
            var stubCar = new Car();

            mockCompany.Cars.Add(stubCar);
            mockCompany.Cars.Add(stubCar);

            Assert.Equal(2, mockCompany.Cars.Count);
        }

        [Fact]
        public void CompanyCars_SetNull_ReturnNull()
        {
            var mockCompany = new CarRentalCompany();

            mockCompany.Cars = null;

            Assert.Null(mockCompany.Cars);
        }

        // === Test Data Annotations ===

        // [Key] exists
        [Fact]
        public void CompanyId_TestKeyAttribute_Exists()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.CarRentalCompanyId));

            var keyAttribute = Assert.IsType<KeyAttribute>(companyProperties.GetCustomAttribute(typeof(KeyAttribute)));

            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void CompanyNameRequired_AssignNullValue_ReturnsObjectInvalid()
        {
            var stubCompany = createFakeCompany();
            stubCompany.CompanyName = null;
            var validationContext = new ValidationContext(stubCompany);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(stubCompany, validationContext, validationResults);

            Assert.False(objectIsValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(CarRentalCompany.CompanyName)));
        }

        [Fact]
        public void CompanyName_TestDisplayNameAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.CompanyName));

            var displayNameAttribute = companyProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Company Name", displayNameAttribute.Name);
        }

        [Fact]
        public void CompanyName_TestStringLengthAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.CompanyName));

            var stringLengthAttribute = companyProperties.GetCustomAttribute<StringLengthAttribute>();

            Assert.Equal(200, stringLengthAttribute.MaximumLength);
            Assert.Equal("Company name must not exceed 200 characters.", stringLengthAttribute.ErrorMessage);
        }

        [Fact]
        public void CompanyLocationRequired_AssignNullValue_ReturnsObjectInvalid()
        {
            var stubCompany = createFakeCompany();
            stubCompany.Location = null;
            var validationContext = new ValidationContext(stubCompany);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(stubCompany, validationContext, validationResults);

            Assert.False(objectIsValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(CarRentalCompany.Location)));
        }

        [Fact]
        public void CompanyLocation_TestDisplayNameAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.Location));

            var displayNameAttribute = companyProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Location", displayNameAttribute.Name);
        }

        [Fact]
        public void CompanyLocation_TestStringLengthAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.Location));

            var stringLengthAttribute = companyProperties.GetCustomAttribute<StringLengthAttribute>();

            Assert.Equal(200, stringLengthAttribute.MaximumLength);
            Assert.Equal("Location must not exceed 200 characters.", stringLengthAttribute.ErrorMessage);
        }

        [Fact]
        public void CompanyRating_TestDisplayNameAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.Rating));

            var displayNameAttribute = companyProperties.GetCustomAttribute<DisplayAttribute>();

            Assert.Equal("Rating", displayNameAttribute.Name);
        }

        [Fact]
        public void CompanyRating_TestRangeAttribute_ValueMatch()
        {
            var companyProperties = typeof(CarRentalCompany).GetProperty(nameof(CarRentalCompany.Rating));

            var rangeAttribute = companyProperties.GetCustomAttribute<RangeAttribute>();

            Assert.Equal(5.00, rangeAttribute.Maximum);
            Assert.Equal(0.00, rangeAttribute.Minimum);
            Assert.Equal("Rating must be between 0 and 5", rangeAttribute.ErrorMessage);
        }
    }
}
