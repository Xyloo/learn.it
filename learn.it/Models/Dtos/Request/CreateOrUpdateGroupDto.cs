using System.ComponentModel.DataAnnotations;

namespace learn.it.Models.Dtos.Request
{
    public class CreateOrUpdateGroupDto
    {
        [Required(ErrorMessage = "Group's name cannot be blank.")]
        [StringLength(150, ErrorMessage = "Group's name cannot be shorter than 5 and longer than 150 characters.", MinimumLength = 5)]
        public string Name { get; set; }
    }
}
