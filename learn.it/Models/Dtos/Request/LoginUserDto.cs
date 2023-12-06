using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Nazwa użytkownika musi być podana.")]
        [StringLength(32, ErrorMessage = "Nazwa użytkownika nie może być krótsza niż 3 i dłuższa niż 32 znaki.", MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "Nazwa użytkownika nie może zaczynać się od cyfry i może zawierać tylko litery, cyfry i podkreślniki.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Hasło musi być podane.")]
        [StringLength(100, ErrorMessage = "Hasło nie może być krótsze niż 8 i dłuższe niż 255 znaków.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Hasło musi składać się z przynajmniej 8 znaków i zawierać przynajmniej jedną wielką literę, jedną małą literę oraz jedną cyfrę.")]
        public required string Password { get; set; }
    }
}
