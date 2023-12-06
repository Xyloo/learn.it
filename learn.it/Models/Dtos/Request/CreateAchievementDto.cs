using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateAchievementDto
    {
        [Required(ErrorMessage = "Nazwa osiągnięcia musi być podana.")]
        [StringLength(100, ErrorMessage = "Nazwa osiągnięcia nie może być dłuższa niż 100 znaków.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis osiągnięcia musi być podany.")]
        [StringLength(500, ErrorMessage = "Opis osiągnięcia nie może być dłuższy niż 500 znaków.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Predykat musi być podany.")]
        public string Predicate { get; set; }
    }
}