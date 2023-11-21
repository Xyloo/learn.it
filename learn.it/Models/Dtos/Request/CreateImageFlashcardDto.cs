using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateImageFlashcardDto
    {
        [Required(ErrorMessage = "Flashcard's definition cannot be blank.")]
        [StringLength(500, ErrorMessage = "Flashcard's defintion cannot be longer than 500 characters.")]
        public string Definition { get; set; } = null!;

        [Required(ErrorMessage = "Flashcard must belong to a study set.")]
        public int StudySetId { get; set; }
    }
}