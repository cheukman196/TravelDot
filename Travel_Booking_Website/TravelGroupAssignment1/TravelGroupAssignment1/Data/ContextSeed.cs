using Azure.Identity;
using TravelGroupAssignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace TravelGroupAssignment1.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {


           await  roleManager.CreateAsync(new IdentityRole(Enum.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Admin.ToString()));
           await  roleManager.CreateAsync(new IdentityRole(Enum.Roles.Traveler.ToString()));



        }
        public static async Task SuperSeedRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            System.Diagnostics.Debug.WriteLine("seeding the user");

            //superadmin is associated with all the other groups
            var superUser = new ApplicationUser
            {
                UserName = "superAdmin",
                Email = "adminsupport@domain.com",
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true


            };
           if (userManager.Users.All(u => u.Id != superUser.Id))
            {
                System.Diagnostics.Debug.WriteLine("no super users");
            try
            {

                    var user = await userManager.FindByEmailAsync(superUser.Email);
                    if (user == null)
                    {
                    System.Diagnostics.Debug.WriteLine("Super user creating");
                    await userManager.CreateAsync(superUser, "P@ssword123"); //foist administration to users when you can 
                    await userManager.AddToRoleAsync(superUser, Enum.Roles.SuperAdmin.ToString());
                    await userManager.AddToRoleAsync(superUser, Enum.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(superUser, Enum.Roles.Traveler.ToString());

               }
               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error "+ex.Message);

            }
            
            }
        }
    }
}
