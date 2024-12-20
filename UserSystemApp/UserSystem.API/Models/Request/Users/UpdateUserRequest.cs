using System.ComponentModel.DataAnnotations;
namespace UserSystem.API.Models.Request.Users
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}