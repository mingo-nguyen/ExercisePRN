using System.ComponentModel.DataAnnotations;

namespace SP25_PRN222.WebApp.Models.AuthModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
