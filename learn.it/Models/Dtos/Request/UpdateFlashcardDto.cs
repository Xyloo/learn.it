using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class UpdateFlashcardDto : IValidatableObject
    {
        public string? Term { get; set; } = null!;

        public string? Definition { get; set; } = null!;

        public int? StudySetId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Term == null && Definition == null && StudySetId == null)
            {
                yield return new ValidationResult("Przynajmniej jedno pole musi być uzupełnione.", new[] { nameof(Term), nameof(Definition), nameof(StudySetId) });
            }

            if (Term != null && Term.Length > 500)
            {
                yield return new ValidationResult("Pojęcie nie może być dłuższe niż 500 znaków.", new[] { nameof(Term) });
            }

            if (Definition != null && Definition.Length > 500)
            {
                yield return new ValidationResult("Definicja nie może być dłuższa niż 500 znaków.", new[] { nameof(Definition) });
            }
        }
    }
}