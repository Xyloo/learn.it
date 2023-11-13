using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos
{
    public partial class UpdateUserDto : IValidatableObject
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Username is not null)
            {
                if (Username.Length < 3 || Username.Length > 32)
                {
                    yield return new ValidationResult("Username cannot be shorter than 3 and longer than 32 characters.", new[] { nameof(Username) });
                }
                else if (!UsernameRegex().IsMatch(Username))
                {
                    yield return new ValidationResult("Username cannot start with a number and can only contain letters, numbers and underscores.", new[] { nameof(Username) });
                }
            }

            if (Email is not null)
            {
                if (Email.Length > 255)
                {
                    yield return new ValidationResult("Email cannot be longer than 255 characters.", new[] { nameof(Email) });
                }
                else if (!new EmailAddressAttribute().IsValid(Email))
                {
                    yield return new ValidationResult("Email is not valid.", new[] { nameof(Email) });
                }
            }

            if (Password is not null)
            {
                if (Password.Length < 8 || Password.Length > 255)
                {
                    yield return new ValidationResult("Password cannot be shorter than 8 and longer than 255 characters.", new[] { nameof(Password) });
                }
                else if (!PasswordRegex().IsMatch(Password))
                {
                    yield return new ValidationResult("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter and one number.", new[] { nameof(Password) });
                }
            }
        }

        [System.Text.RegularExpressions.GeneratedRegex("^[A-Za-z][A-Za-z0-9_]*$")]
        private static partial System.Text.RegularExpressions.Regex UsernameRegex();
        [System.Text.RegularExpressions.GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]
        private static partial System.Text.RegularExpressions.Regex PasswordRegex();
    }
}
