using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.RoomManagement.Controllers
{
    [Area("RoomManagement")]
    [Route("[controller]")]
    public class RoomCommentController : Controller
    {

        // required
        private readonly ApplicationDbContext _context;

        // required for DI 
        public RoomCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetComments/{roomId:int}")]
        public async Task<IActionResult> GetComments(int roomId)
        {
            var comments = await _context.RoomComments
                                .Where(c => c.RoomId == roomId)
                                .OrderByDescending(c => c.DatePosted)
                                .ToListAsync();
            if (comments.Count == 0)
                return Json(new { success = false, message = "no comments found" });
            return Json(comments); // return as JSON
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] RoomComment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DatePosted = DateTime.Now;
                _context.RoomComments.Add(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Successfully created comment" });
            }

            var errors = ModelState.Values.SelectMany(e => e.Errors)
                            .Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Invalid comment data", error = errors });
        }

    }
}
