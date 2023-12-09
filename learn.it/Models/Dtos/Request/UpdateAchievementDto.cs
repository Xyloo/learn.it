using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class UpdateAchievementDto : IValidatableObject
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Predicate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == null && Description == null && Predicate == null)
            {
                yield return new ValidationResult("Przynajmniej jedno pole musi być uzupełnione.", new []{nameof(Name), nameof(Description), nameof(Predicate)});
            }

            if (Name != null && Name.Length > 100)
            {
                yield return new ValidationResult("Nazwa osiągnięcia nie może być dłuższa niż 100 znaków.",
                    new[] { nameof(Name) });
            }

            if (Description != null && Description.Length > 500)
            {
                yield return new ValidationResult("Opis osiągnięcia nie może być dłuższy niż 500 znaków.",
                    new[] { nameof(Description) });
            }
        }
    }
}