using System.ComponentModel.DataAnnotations;

namespace UnicornProject.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [StringLength(50)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(13, MinimumLength = 6)]
        public string Password { get; set; }

        public int RoleId { get; set; }
    }
}
