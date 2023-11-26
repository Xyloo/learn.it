namespace learn.it.Models.Dtos.Response
{
    public class AnswerDto
    {
        public SelfUserResponseDto User { get; set; }
        public bool IsCorrect { get; set; }
        public int AnswerTime { get; set; }
        public FlashcardDto Flashcard { get; set; }
        public DateTime AnswerTimestamp { get; set; }

        public AnswerDto(Answer answer)
        {
            User = new SelfUserResponseDto(answer.User);
            IsCorrect = answer.IsCorrect;
            AnswerTime = answer.AnswerTime;
            Flashcard = new FlashcardDto(answer.Flashcard);
            AnswerTimestamp = answer.AnswerTimestamp;
        }
    }
}
