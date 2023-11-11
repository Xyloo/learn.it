using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Username cannot be blank.")]
        [StringLength(32, ErrorMessage = "Username cannot be shorter than 3 and longer than 32 characters.", MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "Username cannot start with a number and can only contain letters, numbers and underscores.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password cannot be blank.")]
        [StringLength(100, ErrorMessage = "Password cannot be shorter than 8 and longer than 255 characters.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter and one number.")]
        public required string Password { get; set; }
    }
}
