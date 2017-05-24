using System.ComponentModel.DataAnnotations;

namespace BSUIR.ManagerQueue.Infrastructure.Models
{
    public class RegisterBindingModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string Middlename { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
}
