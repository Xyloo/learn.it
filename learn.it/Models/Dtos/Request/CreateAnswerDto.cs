using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateAnswerDto
    {
        [Required(ErrorMessage = "Answer correctness must be provided.")]
        public bool IsCorrect { get; set; }
        //I can't believe I can't just use int.MaxValue in the error string.
        [Range(0, int.MaxValue, ErrorMessage = "Answer time should be provided in milliseconds and must be in the range between 0 and 2147483647.")]
        [Required(ErrorMessage = "Answer time must be provided.")]
        public int AnswerTime { get; set; }
        [Required(ErrorMessage = "Flashcard id must be provided.")]
        public int FlashcardId { get; set; }
    }
}
