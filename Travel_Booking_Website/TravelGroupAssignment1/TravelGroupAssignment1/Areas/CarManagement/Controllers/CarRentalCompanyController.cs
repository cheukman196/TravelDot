using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.CarManagement.Controllers
{
    [Area("CarManagement")]
    [Route("[controller]")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class CarRentalCompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarRentalCompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarRentalCompanyController
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var carRentalCompanies = await _context.CarRentalCompanies.ToListAsync();
            return View(carRentalCompanies);
        }

        // GET: CarRentalCompanyController/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.CarRentalCompanies.FirstOrDefaultAsync(c => c.CarRentalCompanyId == id);
            if (company == null) return NotFound();
            return View(company);
        }

        // GET: CarRentalCompanyController/Create
        [HttpGet("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarRentalCompanyController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create(CarRentalCompany newCarRentalCompany)
        {
            if (ModelState.IsValid)
            {
                await _context.CarRentalCompanies.AddAsync(newCarRentalCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(newCarRentalCompany);
        }

        [HttpGet("Edit/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: CarRentalCompanyController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var company = await _context.CarRentalCompanies.FindAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        // POST: CarRentalCompanyController/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CarRentalCompanyId", "CompanyName", "Location", "Rating")] CarRentalCompany company)
        {
            if (id != company.CarRentalCompanyId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.CarRentalCompanies.Update(company);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CarRentalCompanyExists(id)) return NotFound();
                    else throw;
                }
            }
            return View(company);
        }

        [HttpGet("Delete/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        // GET: CarRentalCompanyController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _context.CarRentalCompanies.FirstOrDefaultAsync(c => c.CarRentalCompanyId == id);
            if (company == null) return NotFound();
            return View(company);
        }

        // POST: CarRentalCompanyController/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.CarRentalCompanies.FindAsync(id);
            if (company != null)
            {
                _context.Remove(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<bool> CarRentalCompanyExists(int id)
        {
            return await _context.CarRentalCompanies.AnyAsync(h => h.CarRentalCompanyId == id);
        }
    }
}
