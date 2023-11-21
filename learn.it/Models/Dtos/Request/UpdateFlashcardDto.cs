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
                yield return new ValidationResult("At least one property must be provided.", new[] { nameof(Term), nameof(Definition), nameof(StudySetId) });
            }

            if (Term != null && Term.Length > 500)
            {
                yield return new ValidationResult("Flashcard's term cannot be longer than 500 characters.", new[] { nameof(Term) });
            }

            if (Definition != null && Definition.Length > 500)
            {
                yield return new ValidationResult("Flashcard's definition cannot be longer than 500 characters.", new[] { nameof(Definition) });
            }
        }
    }
}