using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignWment1.Areas.CarManagement.Controllers
{
    [Area("CarManagement")]
    [Route("[controller]")]
    public class CarCommentController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CarCommentController(ApplicationDbContext context)
        {
            this._context = context;
        }

        // No views for comments, only return JSON
        [HttpGet("GetComments/{carId:int}")]
        public async Task<IActionResult> GetComments(int carId)
        {
            var comments = await _context.CarComments
                                .Where(c => c.CarId == carId)
                                .OrderByDescending(c => c.DatePosted)
                                .ToListAsync();
            if (comments.Count == 0)
                return Json(new { success = false, message = "no comments found" });
            return Json(comments); // return as JSON
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CarComment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DatePosted = DateTime.Now;
                _context.CarComments.Add(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Successfully created comment" });
            }

            var errors = ModelState.Values.SelectMany(e => e.Errors)
                            .Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid comment data", error = errors });
        }
        
    }
}
