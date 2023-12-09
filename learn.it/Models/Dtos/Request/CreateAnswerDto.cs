using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateAnswerDto
    {
        [Required(ErrorMessage = "Poprawność odpowiedzi musi być podana.")]
        public bool IsCorrect { get; set; }
        //I can't believe I can't just use int.MaxValue in the error string.
        [Range(0, int.MaxValue, ErrorMessage = "Czas odpowiedzi powinien być podany w milisekundach i być w zakresie od 0 do 2147483647.")]
        [Required(ErrorMessage = "Czas odpowiedzi musi być podany.")]
        public int AnswerTime { get; set; }
        [Required(ErrorMessage = "ID fiszki musi być podane.")]
        public int FlashcardId { get; set; }
    }
}
