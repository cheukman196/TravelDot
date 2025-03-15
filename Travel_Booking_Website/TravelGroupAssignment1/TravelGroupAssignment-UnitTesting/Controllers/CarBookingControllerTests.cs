using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TravelGroupAssignment1.Areas.CarManagement.Controllers;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Services;

namespace TravelGroupAssignment_UnitTesting.Controllers
{
    public class CarBookingControllerTests
    {
        public ApplicationDbContext GetApplicationDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        public async void SetupCarsAndRentalCompanyAsync(ApplicationDbContext context)
        {
            var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

            var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
            var stubCar2 = new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany };
            var stubCar3 = new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany };

            await context.Cars.AddRangeAsync(stubCar1, stubCar2, stubCar3);

            await context.CarBookings.AddRangeAsync(
                new CarBooking { BookingId = 1, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) },
                new CarBooking { BookingId = 2, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 2), EndDate = new DateTime(2020, 1, 3) },
                new CarBooking { BookingId = 3, CarId = 2, Car = stubCar2, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) }
            );
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_AndListOfCarBookings()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                SetupCarsAndRentalCompanyAsync(context);

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Index(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<List<CarBooking>>(viewResult.ViewData.Model);
                Assert.Equal(2, model.Count);

                // Assert: returned models match created bookings
                Assert.Equal(1, model[0].BookingId);
                Assert.Equal(2, model[1].BookingId);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Index_SearchCarBookingWithNoCarReference_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var stubCar2 = new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany };
                var stubCar3 = new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany };

                await context.Cars.AddRangeAsync(stubCar1, stubCar2, stubCar3);

                await context.CarBookings.AddAsync(
                    new CarBooking { BookingId = 1, CarId = 1, Car = null, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) }
                );
                await context.SaveChangesAsync();

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Index(4);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Details_ReturnsViewResult_AndCarBooking()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var stubCar2 = new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany };
                var stubCar3 = new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany };

                await context.Cars.AddRangeAsync(stubCar1, stubCar2, stubCar3);

                await context.CarBookings.AddRangeAsync(
                    new CarBooking { BookingId = 1, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) },
                    new CarBooking { BookingId = 2, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 2), EndDate = new DateTime(2020, 1, 3) },
                    new CarBooking { BookingId = 3, CarId = 2, Car = stubCar2, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) }
                );
                await context.SaveChangesAsync();

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action: call Index()
                var result = await controller.Details(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); 

                var model = Assert.IsAssignableFrom<CarBooking>(viewResult.ViewData.Model);
                Assert.Equal(1, model.BookingId);
                Assert.Equal(1, model.CarId);
                Assert.Equivalent(stubCar1, model.Car);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Details_NonExistentBookingId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Details(1);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreateGet_ReturnsViewResult_AndCarBookingModel()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                SetupCarsAndRentalCompanyAsync(context);

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action: call Index()
                var result = await controller.Create(1, new DateTime(2020, 1, 1), new DateTime(2020, 1, 5));

                // Assert: returned ViewResult, model returned is List<Car>, with 2 bookings
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<CarBooking>(viewResult.ViewData.Model);

                // Assert: returned models match created bookings
                // *** MAY NEED UPDATE AFTER TRIP MANAGEMENT IMPLEMENTED ***
                Assert.Equal(1, model.CarId);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreateGet_NonExistentCarId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Create(1, new DateTime(2020, 1, 1), new DateTime(2020, 1, 5));

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_AdminReturnsViewResultToTrip_AndCarBookingCreated()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                SetupCarsAndRentalCompanyAsync(context);
                var mockCarBooking = new CarBooking { BookingId = 4, CarId = 2, TripId = 2, StartDate = new DateTime(2020, 1, 10), EndDate = new DateTime(2020, 1, 11) };

                var mockContext = new Mock<ApplicationDbContext>();
                var userStore = new Mock<IUserStore<ApplicationUser>>();
                var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
                var mockHttpContext = new Mock<HttpContext>();
                var mockUser = new Mock<ClaimsPrincipal>();
                var mockSessionService = new Mock<ISessionService>();

                mockUser.Setup(user => user.IsInRole("Admin")).Returns(true);
                mockUser.Setup(user => user.IsInRole("SuperAdmin")).Returns(true);
                mockHttpContext.Setup(context => context.User).Returns(mockUser.Object);
                mockSessionService.Setup(service => service.GetSessionData<List<int>>("CarBookingIds")).Returns(new List<int>());

                var controller = new CarBookingController(context, mockSessionService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = mockHttpContext.Object
                    }
                };

                // Action
                var result = await controller.CreateBooking(mockCarBooking);

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);
                Assert.Equal("CarBooking", viewResult.ControllerName);

                var model = await context.CarBookings.FindAsync(4);
                Assert.Equal(4, model.BookingId);
                Assert.Equal(2, model.CarId);
                Assert.Equal(2, model.TripId);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_BasicUserReturnsViewResultToTrip_AndCarBookingCreated()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                SetupCarsAndRentalCompanyAsync(context);
                var mockCarBooking = new CarBooking { BookingId = 4, CarId = 2, TripId = 2, StartDate = new DateTime(2020, 1, 10), EndDate = new DateTime(2020, 1, 11) };

                var mockContext = new Mock<ApplicationDbContext>();
                var userStore = new Mock<IUserStore<ApplicationUser>>();
                var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
                var mockHttpContext = new Mock<HttpContext>();
                var mockUser = new Mock<ClaimsPrincipal>();
                var mockSessionService = new Mock<ISessionService>();

                mockUser.Setup(user => user.IsInRole("Admin")).Returns(false);
                mockUser.Setup(user => user.IsInRole("SuperAdmin")).Returns(false);
                mockHttpContext.Setup(context => context.User).Returns(mockUser.Object);
                mockSessionService.Setup(service => service.GetSessionData<List<int>>("CarBookingIds")).Returns(new List<int>());


                var controller = new CarBookingController(context, mockSessionService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = mockHttpContext.Object
                    }
                };


                // Action
                var result = await controller.CreateBooking(mockCarBooking);

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);
                Assert.Equal("Trip", viewResult.ControllerName);

                var model = await context.CarBookings.FindAsync(4);
                Assert.Equal(4, model.BookingId);
                Assert.Equal(2, model.CarId);
                Assert.Equal(2, model.TripId);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_CarBookingDateClash_ReturnViewWithModelError()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                SetupCarsAndRentalCompanyAsync(context);
                // setup: car with CarId 1 already is booked on 2020-1-1 to 2020-1-3
                var mockCarBooking = new CarBooking { BookingId = 4, CarId = 1, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 10) };

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.CreateBooking(mockCarBooking);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("Create", viewResult.ViewName); // returns default view i.e. Create
                Assert.IsAssignableFrom<CarBooking>(viewResult.Model); // returns incomplete carBooking obj
                var modelState = viewResult.ViewData.ModelState;

                // match custom error message when booking dates clash
                Assert.Contains(modelState.Values, x => x.Errors.Any(e => e.ErrorMessage == "Car is not available for booking on given date range."));

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_NonExistentCarId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCarBooking = new CarBooking { BookingId = 4, CarId = 99, TripId = 2, StartDate = new DateTime(2020, 1, 10), EndDate = new DateTime(2020, 1, 11) };
                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.CreateBooking(stubCarBooking);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_AndCarBooking()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                var stubCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var stubCar2 = new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany };
                var stubCar3 = new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany };

                await context.Cars.AddRangeAsync(stubCar1, stubCar2, stubCar3);

                await context.CarBookings.AddRangeAsync(
                    new CarBooking { BookingId = 1, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) },
                    new CarBooking { BookingId = 2, CarId = 1, Car = stubCar1, TripId = 1, StartDate = new DateTime(2020, 1, 2), EndDate = new DateTime(2020, 1, 3) },
                    new CarBooking { BookingId = 3, CarId = 2, Car = stubCar2, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) }
                );
                await context.SaveChangesAsync();

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action: call Index()
                var result = await controller.Edit(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                var model = Assert.IsAssignableFrom<CarBooking>(viewResult.ViewData.Model);
                Assert.Equal(1, model.BookingId);
                Assert.Equal(1, model.CarId);
                Assert.Equivalent(stubCar1, model.Car);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_NonExistentBookingId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Edit(1);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_NonExistentCarId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCarBooking = new CarBooking { BookingId = 1, CarId = 99, TripId = 1, StartDate = new DateTime(2020, 1, 10), EndDate = new DateTime(2020, 1, 11) };
                await context.CarBookings.AddAsync(stubCarBooking);

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Edit(1);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditPost_ReturnRedirectResult_AndUpdatedCarBooking()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                var stubCar = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var mockCarBooking = new CarBooking { BookingId = 1, CarId = 1, Car = stubCar, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) };

                await context.Cars.AddRangeAsync(stubCar);
                await context.CarBookings.AddRangeAsync(mockCarBooking);
                await context.SaveChangesAsync();

                mockCarBooking.TripId = 2;
                mockCarBooking.StartDate = new DateTime(2050, 1, 1);
                mockCarBooking.EndDate = new DateTime(2050, 1, 1);

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Edit(1, mockCarBooking);

                // Assert
                Assert.IsType<RedirectToActionResult>(result);
                var actualCarBooking = await context.CarBookings.FindAsync(1);
                Assert.Equivalent(mockCarBooking, actualCarBooking);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditPost_CarBookingDateClash_ReturnViewWithModelError()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                SetupCarsAndRentalCompanyAsync(context);
                var mockCarBooking = new CarBooking { BookingId = 4, CarId = 1, TripId = 1, StartDate = new DateTime(2050, 1, 1), EndDate = new DateTime(2050, 1, 2) };

                await context.CarBookings.AddRangeAsync(mockCarBooking);
                await context.SaveChangesAsync();

                mockCarBooking.TripId = 2;
                mockCarBooking.StartDate = new DateTime(2020, 1, 1);
                mockCarBooking.EndDate = new DateTime(2020, 1, 10);

                                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Edit(4, mockCarBooking);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // returns default view i.e. Create
                Assert.IsAssignableFrom<CarBooking>(viewResult.Model); // returns incomplete carBooking obj
                var modelState = viewResult.ViewData.ModelState;

                // match custom error message when booking dates clash
                Assert.Contains(modelState.Values, x => x.Errors.Any(e => e.ErrorMessage == "Car is not available for booking on given date range."));

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_AndCarBooking()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                var stubCar = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var mockCarBooking = new CarBooking { BookingId = 1, CarId = 1, Car = stubCar, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) };

                await context.Cars.AddRangeAsync(stubCar);
                await context.CarBookings.AddRangeAsync(mockCarBooking);

                await context.SaveChangesAsync();

                                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Delete(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                var model = Assert.IsAssignableFrom<CarBooking>(viewResult.ViewData.Model);
                Assert.Equal(1, model.BookingId);
                Assert.Equal(1, model.CarId);
                Assert.Equivalent(stubCar, model.Car);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_NonExistentBookingId_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.Delete(1);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectResult_AndCarBookingDeleted()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };
                var stubCar = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var mockCarBooking = new CarBooking { BookingId = 1, CarId = 1, Car = stubCar, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) };

                await context.Cars.AddRangeAsync(stubCar);
                await context.CarBookings.AddRangeAsync(mockCarBooking);
                await context.SaveChangesAsync();

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                mockSessionService.Setup(service => service.GetSessionData<List<int>>("CarBookingIds")).Returns(new List<int>());

                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.DeleteConfirmed(1, "");

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);

                var model = Assert.IsAssignableFrom<int>(viewResult.RouteValues["carId"]);
                Assert.Equal(1, model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsToController_AndCarBookingDeleted()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };
                var stubCar = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var mockCarBooking = new CarBooking { BookingId = 1, CarId = 1, Car = stubCar, TripId = 1, StartDate = new DateTime(2020, 1, 1), EndDate = new DateTime(2020, 1, 2) };

                await context.Cars.AddRangeAsync(stubCar);
                await context.CarBookings.AddRangeAsync(mockCarBooking);
                await context.SaveChangesAsync();

                Mock<ISessionService> mockSessionService = new Mock<ISessionService>();
                mockSessionService.Setup(service => service.GetSessionData<List<int>>("CarBookingIds")).Returns(new List<int>());
                var controller = new CarBookingController(context, mockSessionService.Object);

                // Action
                var result = await controller.DeleteConfirmed(1);

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);
                Assert.Equal("Trip", viewResult.ControllerName);

                context.Database.EnsureDeleted();
            }
        }

    }
}
