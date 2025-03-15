using TravelGroupAssignment1.Data;
using Moq;
using Moq.Protected;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.CarManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace TravelGroupAssignment_UnitTesting.Controllers
{
    public class CarControllerTests
    {
        public ApplicationDbContext GetApplicationDbContext()
        {
            // use Guid to generate different instances of Db for all the different texts
            // else, DbContext persists throughout the scope (between tests)
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            return new ApplicationDbContext(options);
        }
                
        public Car GetFakeCarWithoutCompanyReference()
        {
            return new Car
            {
                CarId = 1,
                Make = "test make",
                Model = "test model",
                Type = "test type",
                PricePerDay = 150,
                MaxPassengers = 5,
                CompanyId = 1,
                Company = null, // no reference
                Transmission = "Automatic",
                HasAirConditioning = true,
                HasUnlimitedMileage = true
            };
        }


        [Fact]
        public async Task Index_ReturnsViewResult_AndListOfCars()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation"};

                await context.Cars.AddRangeAsync(
                    new Car { Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany },
                    new Car { Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany },
                    new Car { Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany }
                );
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);

                // Action: call Index()
                var result = await controller.Index();

                // Assert: returned ViewResult, model returned is List<Car>, with 3 cars
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<List<Car>>(viewResult.ViewData.Model);
                Assert.Equal(3, model.Count);

                // Assert: returned models match created cars
                Assert.Equal("Corolla", model[0].Model);
                Assert.Equal("Yaris", model[1].Model);
                Assert.Equal("RAV4", model[2].Model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Details_ReturnsViewResult_OneCar()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                await context.Cars.AddRangeAsync(
                    new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany },
                    new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany },
                    new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany }
                );
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.Details(3);

                // Assert: returned ViewResult, model returned is type Car
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                // Assert: returned models match specific car
                var model = Assert.IsAssignableFrom<Car>(viewResult.ViewData.Model);
                Assert.Equal("RAV4", model.Model);

                context.Database.EnsureDeleted();

            }
        }

        [Fact]
        public async Task Details_EmptyDbContext_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: empty context
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action
                var result = await controller.Details(1);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreateGet_ReturnsViewResult_ViewBagWith2Companies()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany1", Location = "stubLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "stubCompany2", Location = "stubLocation2" }
                );

                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call create
                var result = await controller.Create();

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                // Assert: returned ViewBag items are of the right type
                var selectList = Assert.IsAssignableFrom<SelectList>(viewResult.ViewData["Companies"]).AsQueryable();
                Assert.IsAssignableFrom<Dictionary<int, CarRentalCompany>>(viewResult.ViewData["CompanyObjects"]);

                Assert.Equal(2, selectList.Count());
                Assert.True(selectList.Any(x => x.Text == "stubCompany1"));
                Assert.True(selectList.Any(x => x.Text == "stubCompany2"));

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreateGet_ReturnsViewResult_EmptyViewBagItems()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call create
                var result = await controller.Create();

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                // Assert: returned ViewBag items are of the right type
                var selectList = Assert.IsAssignableFrom<SelectList>(viewResult.ViewData["Companies"]).AsQueryable();
                Assert.IsAssignableFrom<Dictionary<int, CarRentalCompany>>(viewResult.ViewData["CompanyObjects"]);

                Assert.Empty(selectList);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_ReturnsViewResult_AndCarObject()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { 
                    CarRentalCompanyId = 1, 
                    CompanyName = "stubCompany1", 
                    Location = "stubLocation1" 
                };

                await context.CarRentalCompanies.AddAsync(stubCompany);

                await context.SaveChangesAsync();

                var mockCar = GetFakeCarWithoutCompanyReference();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call create
                var result = await controller.Create(mockCar);

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName); // redirect to Index on success

                // Assert: returned ViewBag items are of the right type
                var actualCar = await context.Cars.FirstOrDefaultAsync(c => c.CarId == 1);
                mockCar.Company = stubCompany;
                Assert.Equivalent(mockCar, actualCar, strict: true); // Assert.Equivalent checks object contents

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_AndOneCarObject()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany1", Location = "stubLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "stubCompany2", Location = "stubLocation2" }
                );

                var mockCar = GetFakeCarWithoutCompanyReference(); // CarId = 1
                await context.Cars.AddAsync(mockCar);

                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call create
                var result = await controller.Edit(1);

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                // Assert: returned ViewBag items are of the right type
                var model = Assert.IsAssignableFrom<Car>(viewResult.Model);

                // returned same Car object
                Assert.Equal(1, model.CarId);
                Assert.Equal("test model", model.Model);
                Assert.Equal("test make", model.Make);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_AndCompanySelectList()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany1", Location = "stubLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "stubCompany2", Location = "stubLocation2" }
                );

                var mockCar = GetFakeCarWithoutCompanyReference();
                await context.Cars.AddAsync(mockCar);

                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call create
                var result = await controller.Edit(1);

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName);

                // Assert: returned ViewBag items are of the right type
                var selectList = Assert.IsAssignableFrom<SelectList>(viewResult.ViewData["CompanyList"]).AsQueryable();

                Assert.Equal(2, selectList.Count());
                Assert.True(selectList.Any(x => x.Text == "stubCompany1"));
                Assert.True(selectList.Any(x => x.Text == "stubCompany2"));

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditPost_ReturnsViewResult_AndUpdatedCarObject()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany
                {
                    CarRentalCompanyId = 1,
                    CompanyName = "stubCompany1",
                    Location = "stubLocation1"
                };

                var mockCar = GetFakeCarWithoutCompanyReference();
                await context.Cars.AddAsync(mockCar);
                await context.CarRentalCompanies.AddAsync(stubCompany);

                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // update car with new values

                mockCar.Make = "test make 2";
                mockCar.Model = "test model 2";
                mockCar.Type = "test type 2";
                mockCar.PricePerDay = 200;
                mockCar.MaxPassengers = 2;
                mockCar.CompanyId = 1;
                mockCar.Company = stubCompany; 
                mockCar.Transmission = "Manual";
                mockCar.HasAirConditioning = false;
                mockCar.HasUnlimitedMileage = false;


                // Action: call create
                var result = await controller.Edit(1, mockCar);

                // Assert: returned ViewResult
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName); // redirect to Index on success

                // Assert: returned ViewBag items are of the right type
                var carReturned = await context.Cars.FirstOrDefaultAsync(c => c.CarId == 1);
                Assert.Equivalent(mockCar, carReturned, strict: true); // Assert.Equivalent checks object contents

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditPost_WrongCarIdParameter_ReturnsNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany
                {
                    CarRentalCompanyId = 1,
                    CompanyName = "stubCompany1",
                    Location = "stubLocation1"
                };

                var mockCar = GetFakeCarWithoutCompanyReference();
                await context.Cars.AddAsync(mockCar);
                await context.CarRentalCompanies.AddAsync(stubCompany);

                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                var updatedMockCar = new Car
                {
                    CarId = 1,
                    Make = "test make 2",
                    Model = "test model 2",
                    Type = "test type 2",
                    PricePerDay = 200,
                    MaxPassengers = 2,
                    CompanyId = 1,
                    Company = stubCompany,
                    Transmission = "Manual",
                    HasAirConditioning = false,
                    HasUnlimitedMileage = false
                };

                // Action: call create
                var result = await controller.Edit(2, updatedMockCar);

                // Assert: return NotFound()
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }


        [Fact]
        public async Task Delete_ReturnsViewResult_OneCar()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                await context.Cars.AddRangeAsync(
                    new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany },
                    new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, Company = stubCompany },
                    new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, Company = stubCompany }
                );
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.Delete(3);

                // Assert: returned ViewResult, model returned is type Car
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                // Assert: returned models match specific car
                var model = Assert.IsAssignableFrom<Car>(viewResult.ViewData.Model);
                Assert.Equal("RAV4", model.Model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_EmptyDbContext_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: empty context
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action
                var result = await controller.Delete(3);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsViewResult_AndHasDeletedCar()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                await context.Cars.AddRangeAsync(
                    new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1 },
                    new Car { CarId = 2, Make = "Toyota", Model = "Yaris", Type = "Hatchback", CompanyId = 1, },
                    new Car { CarId = 3, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, }
                );
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.DeleteConfirmed(3);

                // Assert: returned Redirect (to Index)
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);

                // Assert: car with CarId 3 no longer exists in database
                var model = await context.Cars.FirstOrDefaultAsync(x => x.CarId == 3);
                Assert.Null(model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_EmptyDbContext_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: empty context
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action
                var result = await controller.DeleteConfirmed(3);

                // Assert
                var viewResult = Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Search_ThreeCarsWithOneBooked_ReturnsTwoUnbookedCars()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };

                await context.Cars.AddRangeAsync(
                    new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany },
                    new Car { CarId = 2, Make = "Toyota", Model = "RAV4", Type = "SUV", CompanyId = 1, },
                    new Car
                    {
                        CarId = 3,
                        Make = "Toyota",
                        Model = "Yaris",
                        Type = "Hatchback",
                        CompanyId = 1,
                        Company = stubCompany,
                        Bookings = new List<CarBooking> { 
                            new CarBooking { 
                                BookingId = 1, 
                                TripId = 1, 
                                CarId = 2, 
                                StartDate = new DateTime(2020, 1, 1),
                                EndDate = new DateTime(2020, 1, 10)
                            } 
                        }
                    }
                );
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.Search("stubLocation", new DateTime(2020, 1, 1), new DateTime(2020, 1, 3)); 

                // Assert: returned ViewResult, model returned is type Car
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("Index", viewResult.ViewName);

                // Assert: returned models match specific car
                var model = Assert.IsType<List<Car>>(viewResult.ViewData.Model);
                Assert.Equal(2, model.Count);
                Assert.Equal(1, model[0].CarId); // match the exact car
                Assert.Equal(2, model[1].CarId);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Search_EmptyDatabase_ReturnsRedirectResultAndEmptyList()
        {

            using (var context = GetApplicationDbContext())
            {
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.Search("stubLocation", new DateTime(2020, 1, 1), new DateTime(2020, 1, 3));

                // Assert: returned ViewResult, model returned is type Car
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("Index", viewResult.ViewName);

                // Assert: returned models match specific car
                var model = Assert.IsType<List<Car>>(viewResult.ViewData.Model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Search_EmptySearchString_ReturnsIndexAndNoModel()
        {

            using (var context = GetApplicationDbContext())
            {
                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.Search("", new DateTime(2020, 1, 1), new DateTime(2020, 1, 3));

                // Assert: returned ViewResult, model returned is type Car
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task SearchAjax_TwoCarsWithOneBooked_ReturnsOneUnbookedCarJson()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var stubCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "stubCompany", Location = "stubLocation" };
                var mockCar1 = new Car { CarId = 1, Make = "Toyota", Model = "Corolla", Type = "Sedan", CompanyId = 1, Company = stubCompany };
                var mockCar2 = new Car
                {
                    CarId = 2,
                    Make = "Toyota",
                    Model = "Yaris",
                    Type = "Hatchback",
                    CompanyId = 1,
                    Company = stubCompany,
                    Bookings = new List<CarBooking> {
                            new CarBooking {
                                BookingId = 1,
                                TripId = 1,
                                CarId = 2,
                                StartDate = new DateTime(2020, 1, 1),
                                EndDate = new DateTime(2020, 1, 10)
                            }
                        }
                };
                await context.Cars.AddRangeAsync(mockCar1, mockCar2);
                await context.SaveChangesAsync();

                var mockLogger = new Mock<ILogger<CarController>>();
                var controller = new CarController(context, mockLogger.Object);
                // Action: call Details
                var result = await controller.SearchAjax("stubLocation", new DateTime(2020, 1, 1), new DateTime(2020, 1, 3));

                // Assert: returned ViewResult, model returned is type Car
                var jsonResult = Assert.IsType<JsonResult>(result);
                var carList = Assert.IsAssignableFrom<IEnumerable<dynamic>>(jsonResult.Value).AsQueryable();

                // match the cars
                Assert.Equal(1, carList.Count()); // Assert that only one car is returned

                context.Database.EnsureDeleted();
            }
        }

    }


}
