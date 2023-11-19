using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Response
{
    public class FlashcardDto
    {
        public int Id { get; set; }

        public string Term { get; set; }

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
