using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TravelGroupAssignment1.Areas.CustomerManagement.Models;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.CustomerManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var customer = _context.Customers.Find(id);

            return View(customer);

        }
        [HttpGet]
        public IActionResult Edit(int id)

        {
            var customer = _context.Customers.Find(id);
            return View(customer);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, [Bind("Username, Password, Email, FirstName, LastName")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();

            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Customers.Update(customer);                 //add new project - only in memory, nothing in database yet
                    _context.SaveChanges(); //commits changes to memory
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                { //for when two updates at the same time-rarely will happen with our form
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }

                    throw;
                }

            }
            return View(customer);

        }
        public IActionResult Delete(int id)

        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);


        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Hotels.Find(id);
            if (customer != null)
            {
                _context.Remove(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public bool CustomerExists(int id)
        {

            return _context.Customers.Any(e => e.CustomerId == id);
        }


    }
}
