namespace TravelGroupAssignment1.Models.ViewModels
{
    public class ManageUserRolesViewModel //just checkboxes
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public Boolean Selected { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
