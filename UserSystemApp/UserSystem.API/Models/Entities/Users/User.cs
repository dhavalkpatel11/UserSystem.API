using System.ComponentModel.DataAnnotations;

namespace UserSystem.API.Models.Entities.Users
{
    public class User
    {
        [Required()]
        [Key()]
        public int Id { get; set; }
        [Required()]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required()]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required()]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required()]
        public string Password { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}