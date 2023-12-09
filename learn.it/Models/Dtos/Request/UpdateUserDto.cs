using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public partial class UpdateUserDto : IValidatableObject
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        [System.Text.RegularExpressions.GeneratedRegex("^[A-Za-z][A-Za-z0-9_]*$")]
        private static partial System.Text.RegularExpressions.Regex UsernameRegex();

        [System.Text.RegularExpressions.GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]
        private static partial System.Text.RegularExpressions.Regex PasswordRegex();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Username is null && Email is null && Password is null)
            {
                yield return new ValidationResult("Przynajmniej jedno pole musi być uzupełnione.", new[] { nameof(Username), nameof(Email), nameof(Password) });
            }

            if (Username is not null)
            {
                if (Username.Length < 3 || Username.Length > 32)
                {
                    yield return new ValidationResult("Nazwa użytkownika nie może być krótsza niż 3 i dłuższa niż 32 znaki.", new[] { nameof(Username) });
                }
                else if (!UsernameRegex().IsMatch(Username))
                {
                    yield return new ValidationResult("Nazwa użytkownika nie może zaczynać się od cyfry i może zawierać tylko litery, cyfry i podkreślniki.", new[] { nameof(Username) });
                }
            }

            if (Email is not null)
            {
                if (Email.Length > 255)
                {
                    yield return new ValidationResult("Email nie może być dłuższy niż 255 znaków.", new[] { nameof(Email) });
                }
                else if (!new EmailAddressAttribute().IsValid(Email))
                {
                    yield return new ValidationResult("Email jest niepoprawny.", new[] { nameof(Email) });
                }
            }

            if (Password is not null)
            {
                if (Password.Length < 8 || Password.Length > 255)
                {
                    yield return new ValidationResult("Hasło nie może być krótsze niż 8 i dłuższe niż 255 znaków.", new[] { nameof(Password) });
                }
                else if (!PasswordRegex().IsMatch(Password))
                {
                    yield return new ValidationResult("Hasło musi składać się z przynajmniej 8 znaków i zawierać przynajmniej jedną wielką literę, jedną małą literę oraz jedną cyfrę.", new[] { nameof(Password) });
                }
            }
        }
    }
}