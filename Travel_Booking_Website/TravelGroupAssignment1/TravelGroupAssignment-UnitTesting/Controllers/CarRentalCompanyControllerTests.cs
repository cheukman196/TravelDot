using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.CarManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment_UnitTesting.Controllers
{
    public class CarRentalCompanyControllerTests
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


        [Fact]
        public async Task Index_ReturnsViewResult_AndListOfCarRentalCompanies()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Index()
                var result = await controller.Index();

                // Assert: returned ViewResult, model returned is List<Car>, with 3 cars
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<List<CarRentalCompany>>(viewResult.ViewData.Model);
                Assert.Equal(3, model.Count);

                // Assert: returned models match created cars
                Assert.Equal("mockCompany1", model[0].CompanyName);
                Assert.Equal("mockCompany2", model[1].CompanyName);
                Assert.Equal("mockCompany3", model[2].CompanyName);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Details_ReturnsViewResult_AndOneCarRentalCompany()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.Details(1);

                // Assert: returned ViewResult, model returned is List<Car>, with 3 cars
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<CarRentalCompany>(viewResult.ViewData.Model);

                // Assert: returned models match created cars
                Assert.Equal("mockCompany1", model.CompanyName);
                Assert.Equal("mockLocation1", model.Location);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Details_EmptyDatabase_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.Details(5);

                // Assert: returned ViewResult, model returned is List<Car>, with 3 cars
                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void CreateGet_EmptyDatabase_ReturnNotFound()
        {
            using (var context = GetApplicationDbContext())
            {
                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = controller.Create();

                // Assert: returned ViewResult, model returned is List<Car>, with 3 cars
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default View() = null ViewName

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CreatePost_ReturnsViewResult_AndFindsCompanyObject()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange: Cars, Company and Controller
                var mockCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" };

                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Create(mockCompany);

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName); // redirect to Index on success

                // Assert
                var actualCompany = await context.CarRentalCompanies.FirstOrDefaultAsync(c => c.CarRentalCompanyId == 1);
                Assert.Equivalent(mockCompany, actualCompany, strict: true); // Assert.Equivalent checks object contents

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_ReturnsViewResult_AndOneCarRentalCompany()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.Edit(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<CarRentalCompany>(viewResult.ViewData.Model);

                // Assert
                Assert.Equal("mockCompany1", model.CompanyName);
                Assert.Equal("mockLocation1", model.Location);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_SearchNonExistentCompanyId_ReturnNotFound()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Edit(5);

                // Assert
                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditGet_EmptyDatabase_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Edit(5);

                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task EditPost_NonExistentCompanyId_ReturnNotFound()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var mockCompany = new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" };

                await context.CarRentalCompanies.AddAsync(mockCompany);

                await context.SaveChangesAsync();

                mockCompany.CompanyName = "mockCompany2";
                mockCompany.Location = "mockLocation2";

                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Edit(5, mockCompany);

                // Assert: returned ViewResult
                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_AndOneCarRentalCompany()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.Delete(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // default value = null, if View Name not specified in return View()

                var model = Assert.IsAssignableFrom<CarRentalCompany>(viewResult.ViewData.Model);

                // Assert
                Assert.Equal("mockCompany1", model.CompanyName);
                Assert.Equal("mockLocation1", model.Location);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_SearchNonExistentCompanyId_ReturnNotFound()
        {
            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Delete(5);

                // Assert
                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Delete_EmptyDatabase_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                var controller = new CarRentalCompanyController(context);

                // Action
                var result = await controller.Delete(5);

                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsViewResult_RentalCompanyDeleted()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.DeleteConfirmed(1);

                // Assert
                var viewResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", viewResult.ActionName); // default value = null, if View Name not specified in return View()

                var model = await context.CarRentalCompanies.FirstOrDefaultAsync(c => c.CarRentalCompanyId == 1);
                Assert.Null(model);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task DeleteConfirmed_DeleteNonExistentCompany_ReturnNotFound()
        {

            using (var context = GetApplicationDbContext())
            {
                // Arrange
                await context.CarRentalCompanies.AddRangeAsync(
                    new CarRentalCompany { CarRentalCompanyId = 1, CompanyName = "mockCompany1", Location = "mockLocation1" },
                    new CarRentalCompany { CarRentalCompanyId = 2, CompanyName = "mockCompany2", Location = "mockLocation2" },
                    new CarRentalCompany { CarRentalCompanyId = 3, CompanyName = "mockCompany3", Location = "mockLocation3" }
                );

                await context.SaveChangesAsync();

                var controller = new CarRentalCompanyController(context);

                // Action: call Details()
                var result = await controller.DeleteConfirmed(5);

                // Assert
                Assert.IsType<NotFoundResult>(result);

                context.Database.EnsureDeleted();
            }
        }
    }



}
