using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class PasswordValidationDto
    {
        [Required(ErrorMessage = "Hasło musi być podane.")]
        [StringLength(100, ErrorMessage = "Hasło nie może być krótsze niż 8 i dłuższe niż 255 znaków.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Hasło musi składać się z przynajmniej 8 znaków i zawierać przynajmniej jedną wielką literę, jedną małą literę oraz jedną cyfrę.")]
        public required string Password { get; set; }
    }
}
