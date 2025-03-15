//using TravelGroupAssignment1.Areas;
using TravelGroupAssignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TravelGroupAssignment1.Models.ViewModels;
///using COMP2139_Labs.Areas.ProjectManager.Models;

namespace TravelGroupAssignment1.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]

    public class UserRolesController : Controller //manages users and roles combined-Rolemanager is for creating roles-business reqs should be scrutinized
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) //don't need to extend idenitity role
        {
            this._roleManager = roleManager;
            this._userManager = userManager;

        }
        private async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRoleViewModel>();
            foreach (ApplicationUser u in users)
            {
                var thisViewModel = new UserRoleViewModel();
                thisViewModel.UserId = u.Id;
                thisViewModel.FirstName = u.FirstName;
                thisViewModel.LastName = u.LastName;
                thisViewModel.Email = u.Email;
                thisViewModel.Roles = await GetUserRolesAsync(u);
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Manage(string userId)
        {

            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id ={userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRoleViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,

                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.Selected = true;
                }
                else
                {
                    userRoleViewModel.Selected = false;

                }
                model.Add(userRoleViewModel);

            }
            return View(model);

        }
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles); ;
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user from roles");
                return View(model);

            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add user to roles");
                return View(model);

            }

            return RedirectToAction("Index");
        }
    }
}
