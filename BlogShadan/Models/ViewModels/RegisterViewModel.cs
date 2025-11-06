using System.ComponentModel.DataAnnotations;

namespace BlogShadan.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email is Required")]
        [EmailAddress(ErrorMessage ="Email must be in proper format ex abc@abc.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password should Match compare Password")]
        public string ConfirmPassword { get; set; }
    }
}
