using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateImageFlashcardDto
    {
        [Required(ErrorMessage = "Definicja musi być podana.")]
        [StringLength(500, ErrorMessage = "Definicja nie może być dłuższa niż 500 znaków.")]
        public string Definition { get; set; }

        [Required(ErrorMessage = "Fiszka musi należeć do jakiegoś zestawu (niepoprawne ID).")]
        public int StudySetId { get; set; }
    }
}