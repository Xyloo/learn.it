using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateStudySetDto
    {
        [Required(ErrorMessage = "Nazwa zestawu musi być podana.")]
        [StringLength(100, ErrorMessage = "Nazwa zestawu nie może być krótsza niż 4 i dłuższa niż 100 znaków.", MinimumLength = 4)]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Opis zestawu nie może być dłuższy niż 250 znaków.")]
        public string? Description { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage="Widoczność musi być równa 0 (publiczny), 1 (prywatny) lub 2 (w zakresie grupy).")]
        public Visibility Visibility { get; set; }

        public int? GroupId { get; set; }
    }
}
