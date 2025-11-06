using System.ComponentModel.DataAnnotations;

namespace BlogShadan.Models.ViewModels
{
    public class LoginVIewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Email must be in proper format ex abc@abc.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
