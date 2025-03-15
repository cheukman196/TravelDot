using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TravelGroupAssignment1.Areas.CarManagement.Models;

namespace TravelGroupAssignment_UnitTesting.Models
{
    public class CarCommentTests
    {
        // Helper Functions
        public CarComment getFakeCarComment()
        {
            return new CarComment
            {
                CommentId = 1,
                Author = "mockAuthor",
                Content = "mockContent",
                Rating = 0,
                DatePosted = new DateTime(2020, 1, 1),
                CarId = 1,
                Car = new Car()
            };
        }


        // === Test Attribute Get & Set

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void CarCommentId_SetCommentId_ReturnSameCommentId(int commentId)
        {
            var mockCarComment = new CarComment();

            mockCarComment.CommentId = commentId;

            Assert.Equal(commentId, mockCarComment.CommentId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("user")]
        [InlineData("admin")]
        [InlineData("joe401")]
        [InlineData("14RunnerLeague")]
        public void CarCommentAuthor_SetAuthor_ReturnSameAuthor(string author)
        {
            var mockCarComment = new CarComment();

            mockCarComment.Author = author;

            Assert.Equal(author, mockCarComment.Author);
        }

        [Theory]
        [InlineData("")]
        [InlineData("This is a great product!")]
        [InlineData("Not a great car\nService is great though")]
        [InlineData("Would not use again")]
        [InlineData("Would definitely recommend 5/7")]
        public void CarCommentContent_SetContent_ReturnSameContent(string content)
        {
            var mockCarComment = new CarComment();

            mockCarComment.Content = content;

            Assert.Equal(content, mockCarComment.Content);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3.0)]
        [InlineData(5)]
        [InlineData(4.5)]
        [InlineData(1.5)]
        public void CarCommentRating_SetRating_ReturnSameRating(double rating)
        {
            var mockCarComment = new CarComment();

            mockCarComment.Rating = rating;

            Assert.Equal(rating, mockCarComment.Rating);
        }

        [Fact]
        public void CarCommentDatePosted_SetDatePosted_ReturnSameDatePosted()
        {
            var mockCarComment = new CarComment();

            var mockDateTime = DateTime.Now;
            mockCarComment.DatePosted = mockDateTime;

            Assert.Equal(mockDateTime, mockCarComment.DatePosted);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void CarCommentId_SetCarId_ReturnSameCarCommentId(int carId)
        {
            var mockCarComment = new CarComment();

            mockCarComment.CarId = carId;

            Assert.Equal(carId, mockCarComment.CarId);
        }

        [Fact]
        public void CarCommentCar_SetCar_ReturnSameCar()
        {
            var mockCarComment = new CarComment();
            Car mockCar = new Car();

            mockCarComment.Car = mockCar;

            Assert.Equal(mockCar, mockCarComment.Car);
        }

        [Fact]
        public void CarCommentCar_SetNull_ReturnNull()
        {
            var mockCarComment = new CarComment();

            mockCarComment.Car = null;

            Assert.Null(mockCarComment.Car);
        }


        // === test essential data annotations ===
        
        [Fact]
        public void CarCommentAuthor_TestRequiredAttribute_Exists()
        {
            var carCommentProperties = typeof(CarComment).GetProperty(nameof(CarComment.Author));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carCommentProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }


        [Theory]
        [InlineData("", false)] // case: not provided
        [InlineData("user", true)]
        [InlineData("admin", true)]
        [InlineData("joe401", true)]
        [InlineData("14RunnerLeague", true)]
        public void CarCommentAuthor_TestRequiredAttribute_ValidationSuccess(string author, bool resultValid)
        {
            var carComment = getFakeCarComment();
            carComment.Author = author;

            var validationContext = new ValidationContext(carComment);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(carComment, validationContext, validationResults);

            Assert.Equal(objectIsValid, resultValid);
        }

        [Fact]
        public void CarCommentContent_TestRequiredAttribute_Exists()
        {
            var carCommentProperties = typeof(CarComment).GetProperty(nameof(CarComment.Content));

            var requiredAttribute = Assert.IsType<RequiredAttribute>(carCommentProperties.GetCustomAttribute(typeof(RequiredAttribute)));

            Assert.NotNull(requiredAttribute);
        }


        [Theory]
        [InlineData("", false)]
        [InlineData("This is a great product!", true)]
        [InlineData("Not a great car\nService is great though", true)]
        [InlineData("Would not use again", true)]
        [InlineData("Would definitely recommend 5/7", true)]
        public void CarCommentContent_TestRequiredAttribute_ValidationSuccess(string content, bool resultValid)
        {
            var carComment = getFakeCarComment();
            carComment.Content = content;

            var validationContext = new ValidationContext(carComment);
            var validationResults = new List<ValidationResult>();

            var objectIsValid = Validator.TryValidateObject(carComment, validationContext, validationResults);

            Assert.Equal(objectIsValid, resultValid);
        }

    }
}
