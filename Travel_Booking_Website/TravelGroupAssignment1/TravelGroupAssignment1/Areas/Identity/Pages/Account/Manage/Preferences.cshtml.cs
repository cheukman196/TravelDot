// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.Identity.Pages.Account.Manage
{
    public class PreferencesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PreferencesModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>


            [Display(Name = "Birthday")]
            public DateOnly? Birthday { get; set; }

            public string? Passport { get; set; }
            public string? Gender { get; set; }

            //preferences
            [Display(Name = "Home Airport")]

            public string? HomeAirport { get; set; }
            [Display(Name = "Seat Preference")]

            public string? SeatPreference { get; set; }

            [Display(Name = "Reward Program Name")]

            public string? RewardProgramName { get; set; }
            [Display(Name = "Reward Program Number")]

            public string? RewardProgramNumber { get; set; }

        }

          private async Task LoadAsync(ApplicationUser user)
          {

              var bday = user.Birthday;
              var passport = user.Passport;
              var gender = user.Gender;
            var homeAirport = user.HomeAirport;
            var seatPref = user.SeatPreference;
            var rewardName = user.RewardProgramName;
            var rewardNum = user.RewardProgramNumber;


              Input = new InputModel
              {
                  Birthday = bday,
                  Passport = passport,
                  Gender = gender,
                  HomeAirport = homeAirport,
                  SeatPreference = seatPref,
                  RewardProgramName = rewardName,
                  RewardProgramNumber = rewardNum
              };
          }

          public async Task<IActionResult> OnGetAsync()
          {
              var user = await _userManager.GetUserAsync(User);
              if (user == null)
              {
                  return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
              }

              await LoadAsync(user);
              return Page();
          }

          public async Task<IActionResult> OnPostAsync()
          {
              var user = await _userManager.GetUserAsync(User);
              if (user == null)
              {
                  return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
              }

              if (!ModelState.IsValid)
              {
                  await LoadAsync(user);
                  return Page();
              }

            var bday = user.Birthday;
            if (Input.Birthday != bday)
            {
                user.Birthday = Input.Birthday;
                await _userManager.UpdateAsync(user);
            }
            var passport = user.Passport;
            if (Input.Passport != passport)
            {
                user.Passport = Input.Passport;
                await _userManager.UpdateAsync(user);
            }
            var gender = user.Gender;
              if (Input.Gender != gender)
              {
                  user.Gender = Input.Gender;
                  await _userManager.UpdateAsync(user);
              }
            var homeAirport = user.HomeAirport;
            if (Input.HomeAirport != homeAirport)
            {
                user.HomeAirport = Input.HomeAirport;
                await _userManager.UpdateAsync(user);
            }
            var seatPref = user.SeatPreference;
            if (Input.SeatPreference != seatPref)
            {
                user.SeatPreference = Input.SeatPreference;
                await _userManager.UpdateAsync(user);
            }
            var rewardName = user.RewardProgramName;
            if (Input.RewardProgramName != rewardName)
            {
                user.RewardProgramName = Input.RewardProgramName;
                await _userManager.UpdateAsync(user);
            }
            var rewardNum = user.RewardProgramNumber;
            if (Input.RewardProgramNumber != rewardNum)
            {
                user.RewardProgramNumber = Input.RewardProgramNumber;
                await _userManager.UpdateAsync(user);
            }





            await _signInManager.RefreshSignInAsync(user);
              StatusMessage = "Your profile has been updated";
              return RedirectToPage();
          }
      
    }
}