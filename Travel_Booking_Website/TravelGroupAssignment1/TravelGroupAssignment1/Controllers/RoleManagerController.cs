using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TravelGroupAssignment1.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RoleManagerController : Controller
    {
        //mechanism to view all roles and add more roles-who can add roles is depending on the business
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleManagerController> _logger;

        public RoleManagerController(RoleManager<IdentityRole> roleManager, ILogger<RoleManagerController> logger) //don't need to extend idenitity role
        {
            this._roleManager = roleManager;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Calling RoleManager Index() Action");
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return View(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddRoles(string roleName)
        {
            _logger.LogInformation("Calling RoleManager AddRoles() Action");
            try
            {
                if (roleName != null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }

    }
}
