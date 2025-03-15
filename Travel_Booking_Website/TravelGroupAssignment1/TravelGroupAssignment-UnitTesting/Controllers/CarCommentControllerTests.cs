using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Data;
using TravelGroupAssignWment1.Areas.CarManagement.Controllers;

namespace TravelGroupAssignment_UnitTesting.Controllers
{
    public class CarCommentControllerTests
    {
        public ApplicationDbContext GetApplicationDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        public async void SetupCommentsAsync(ApplicationDbContext context)
        {

            var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1 };
            var mockCarComment1 = new CarComment { CommentId = 1, Author = "user1", Content = "mockComment1", DatePosted = new DateTime(2020, 1, 1), CarId = 1, Car = stubCar1 };
            var mockCarComment2 = new CarComment { CommentId = 2, Author = "user2", Content = "mockComment2", DatePosted = new DateTime(2020, 1, 1), CarId = 1, Car = stubCar1 };
            var mockCarComment3 = new CarComment { CommentId = 3, Author = "user3", Content = "mockComment3", DatePosted = new DateTime(2020, 1, 1), CarId = 1, Car = stubCar1 };

            await context.Cars.AddRangeAsync(stubCar1);
            await context.CarComments.AddRangeAsync(mockCarComment1, mockCarComment2, mockCarComment3);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetComments_ThreeComments_ReturnsThreeCommentsJson()
        {            
            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1 };
                var mockCarComment1 = new CarComment { CommentId = 1, Author = "user1", Content = "mockComment1", DatePosted = new DateTime(2020, 1, 1), CarId = 1 };
                var mockCarComment2 = new CarComment { CommentId = 2, Author = "user2", Content = "mockComment2", DatePosted = new DateTime(2020, 1, 1), CarId = 1 };
                var mockCarComment3 = new CarComment { CommentId = 3, Author = "user3", Content = "mockComment3", DatePosted = new DateTime(2020, 1, 1), CarId = 1 };

                await context.Cars.AddRangeAsync(stubCar1);
                await context.CarComments.AddRangeAsync(mockCarComment1, mockCarComment2, mockCarComment3);

                await context.SaveChangesAsync();

                var controller = new CarCommentController(context);

                // Act
                var result = await controller.GetComments(1);

                // Assert
                var jsonResult = Assert.IsType<JsonResult>(result);
                var commentList = Assert.IsAssignableFrom<IEnumerable<dynamic>>(jsonResult.Value).AsQueryable();

                // match the comments
                Assert.Equal(3, commentList.Count()); // Assert that only one car is returned
                Assert.Equal("mockComment1", commentList.ToList()[0].Content);
                Assert.Equal("mockComment2", commentList.ToList()[1].Content);
                Assert.Equal("mockComment3", commentList.ToList()[2].Content);

                context.Database.EnsureDeleted();
            }            
        }

        [Fact]
        public async Task AddComments_AddComment_DatabaseAddedComment()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1 };
                var mockCarComment1 = new CarComment { CommentId = 1, Author = "user1", Content = "mockComment1", DatePosted = new DateTime(2020, 1, 1), CarId = 1 };


                await context.Cars.AddRangeAsync(stubCar1);

                await context.SaveChangesAsync();

                var controller = new CarCommentController(context);

                // Act
                var result = await controller.AddComment(mockCarComment1);

                // Assert
                var jsonResult = Assert.IsType<JsonResult>(result);

                // find new comment
                var actualComment = await context.CarComments.FindAsync(1);
                Assert.Equivalent(mockCarComment1, actualComment);

                context.Database.EnsureDeleted();
            }
        }

    }
}
