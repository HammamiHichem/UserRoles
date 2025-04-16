#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace UserRoles.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

    }
}
