#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace UserRoles.ViewModels
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<RoleCheckBox> Roles { get; set; } = new List<RoleCheckBox>();
    }

    public class RoleCheckBox
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
