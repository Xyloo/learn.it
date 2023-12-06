using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateOrUpdateGroupDto : IValidatableObject
    {
        [Required(ErrorMessage = "Nazwa grupy musi być podana.")]
        [StringLength(150, ErrorMessage = "Nazwa grupy nie może być krótsza niż 5 i dłuższa niż 150 znaków.", MinimumLength = 5)]
        public string Name { get; set; }

        public CreateOrUpdateGroupDto(string name)
        {
            Name = name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.Length < 5 || Name.Length > 150)
            {
                yield return new ValidationResult("Nazwa grupy nie może być krótsza niż 5 i dłuższa niż 150 znaków.", new[] { nameof(Name) });
            }
        }
    }
}
