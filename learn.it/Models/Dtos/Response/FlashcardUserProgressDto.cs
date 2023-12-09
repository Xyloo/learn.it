using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class FlashcardUserProgressDto
    {
        public FlashcardDto Flashcard { get; set; }
        public AnonymousUserResponseDto User { get; set; }
        public bool NeedsMoreRepetitions { get; set; }
        public int ConsecutiveCorrectAnswers { get; set; }
        public bool IsMastered { get; set; }
        public DateTime? MasteredTimestamp { get; set; }

        public FlashcardUserProgressDto(FlashcardUserProgress flashcardUserProgress)
        {
            Flashcard = new FlashcardDto(flashcardUserProgress.Flashcard);
            User = new AnonymousUserResponseDto(flashcardUserProgress.User);
            NeedsMoreRepetitions = flashcardUserProgress.NeedsMoreRepetitions;
            ConsecutiveCorrectAnswers = flashcardUserProgress.ConsecutiveCorrectAnswers;
            IsMastered = flashcardUserProgress.IsMastered;
            MasteredTimestamp = flashcardUserProgress.MasteredTimestamp;
        }

        [JsonConstructor]
        public FlashcardUserProgressDto()
        {
        }
    }
}
