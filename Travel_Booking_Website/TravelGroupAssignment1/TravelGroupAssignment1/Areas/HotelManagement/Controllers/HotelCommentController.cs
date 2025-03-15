using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.HotelManagement.Models;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.HotelManagement.Controllers
{
    [Area("HotelManagement")]
    [Route("[controller]")]
    public class HotelCommentController : Controller
    {

        // required
        private readonly ApplicationDbContext _context;

        // required for DI 
        public HotelCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetComments/{hotelId:int}")]
        public async Task<IActionResult> GetComments(int hotelId)
        {
            var comments = await _context.HotelComments
                                .Where(c => c.HotelId == hotelId)
                                .OrderByDescending(c => c.DatePosted)
                                .ToListAsync();
            if (comments.Count == 0)
                return Json(new { success = false, message = "no comments found" });
            return Json(comments); // return as JSON
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] HotelComment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DatePosted = DateTime.Now;
                _context.HotelComments.Add(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Successfully created comment" });
            }

            var errors = ModelState.Values.SelectMany(e => e.Errors)
                            .Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid comment data", error = errors });
        }

    }
}
