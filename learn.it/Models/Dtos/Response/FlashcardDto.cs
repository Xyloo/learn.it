using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Response
{
    public class FlashcardDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Flashcard's term cannot be blank.")]
        [StringLength(500, ErrorMessage = "Flashcard's term cannot be longer than 500 characters.")]
        public string Term { get; set; }

        [Required(ErrorMessage = "Flashcard's definition cannot be blank.")]
        [StringLength(500, ErrorMessage = "Flashcard's defintion cannot be longer than 500 characters.")]
        public string Definition { get; set; }

        public bool IsTermText { get; set; }

        public FlashcardDto(Flashcard flashcard)
        {
            Id = flashcard.FlashcardId;
            Term = flashcard.Term;
            Definition = flashcard.Definition;
            IsTermText = flashcard.IsTermText;
        }
    }
}
