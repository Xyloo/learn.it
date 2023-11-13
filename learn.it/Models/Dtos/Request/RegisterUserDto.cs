using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class RegisterUserDto : LoginUserDto
    {
        [Required(ErrorMessage = "Email cannot be blank.")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
