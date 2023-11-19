using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class UpdateStudySetDto : IValidatableObject
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Visibility? Visibility { get; set; }

        public int? GroupId { get; set; }

        public UpdateStudySetDto(StudySet studySet)
        {
            Name = studySet.Name;
            Description = studySet.Description;
            Visibility = studySet.Visibility;
            GroupId = studySet.Group?.GroupId;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name != null && Name.Length < 4)
            {
                yield return new ValidationResult("Study set's name cannot be shorter than 4 characters.", new[] { nameof(Name) });
            }

            if (Name != null && Name.Length > 100)
            {
                yield return new ValidationResult("Study set's name cannot be longer than 100 characters.", new[] { nameof(Name) });
            }

            if (Description != null && Description.Length > 250)
            {
                yield return new ValidationResult("Study set's description cannot be longer than 250 characters.", new[] { nameof(Description) });
            }
        }

    }
}
