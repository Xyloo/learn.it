using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateStudySetDto
    {
        [Required(ErrorMessage = "Study set's name cannot be blank.")]
        [StringLength(100, ErrorMessage = "Study set's name cannot be shorter than 4 and longer than 100 characters.", MinimumLength = 4)]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Study set's description cannot be longer than 250 characters.")]
        public string? Description { get; set; }

        [Required]
        [Range(0, 2, ErrorMessage="Visibility must be 0, 1 or 2.")]
        public Visibility Visibility { get; set; }

        public int? GroupId { get; set; }
    }
}
