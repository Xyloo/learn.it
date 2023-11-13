using learn.it.Models.Dtos.Response;

namespace learn.it.Models.Dtos.Request
{
    public class BasicGroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;

        public BasicGroupDto(Group group)
        {
            GroupId = group.GroupId;
            Name = group.Name;
        }
    }
}
