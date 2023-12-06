using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class RegisterUserDto : LoginUserDto
    {
        [Required(ErrorMessage = "Email musi być podany.")]
        [StringLength(255, ErrorMessage = "Email nie może być dłuższy niż 255 znaków.")]
        [EmailAddress(ErrorMessage = "Email jest niepoprawny.")]
        public required string Email { get; set; }
    }
}
