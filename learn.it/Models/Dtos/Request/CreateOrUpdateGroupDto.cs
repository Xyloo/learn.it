using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateOrUpdateGroupDto : IValidatableObject
    {
        [Required(ErrorMessage = "Group's name cannot be blank.")]
        [StringLength(150, ErrorMessage = "Group's name cannot be shorter than 5 and longer than 150 characters.", MinimumLength = 5)]
        public string? Name { get; set; }

        public CreateOrUpdateGroupDto()
        {
        }

        public CreateOrUpdateGroupDto(string? name)
        {
            Name = name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name is null) yield break;
            if (Name.Length < 5 || Name.Length > 150)
            {
                yield return new ValidationResult("Group's name cannot be shorter than 5 and longer than 150 characters.", new[] { nameof(Name) });
            }
        }
    }
}
