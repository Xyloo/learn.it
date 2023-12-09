using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class StudySetDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Visibility Visibility { get; set; }
        public AnonymousUserResponseDto Creator { get; set; }
        public ICollection<FlashcardDto> Flashcards { get; set; }
        public BasicGroupDto? Group { get; set; }

        public StudySetDto(StudySet studySet)
        {
            Name = studySet.Name;
            Description = studySet.Description;
            Visibility = studySet.Visibility;
            Creator = new AnonymousUserResponseDto(studySet.Creator);
            Flashcards = studySet.Flashcards.Select(flashcard => new FlashcardDto(flashcard)).ToList();
            Group = studySet.Group != null ? new BasicGroupDto(studySet.Group) : null;
        }

        [JsonConstructor]
        public StudySetDto()
        {
        }
    }
}
