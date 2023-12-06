using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class UpdateStudySetDto : IValidatableObject
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Visibility? Visibility { get; set; }

        public int? GroupId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == null && Description == null && Visibility == null && GroupId == null)
            {
                yield return new ValidationResult("Przynajmniej jedno pole musi być uzupełnione.", new[] { nameof(Name), nameof(Description), nameof(Visibility), nameof(GroupId) });
            }

            if (Name != null)
            {
                if (Name.Length < 4 || Name.Length > 100)
                {
                    yield return new ValidationResult("Nazwa zestawu nie może być krótsza niż 4 i dłuższa niż 100 znaków.", new[] { nameof(Name) });
                }
            }

            if (Description != null && Description.Length > 250)
            {
                yield return new ValidationResult("Opis zestawu nie może być dłuższy niż 250 znaków.", new[] { nameof(Description) });
            }
        }

    }
}
