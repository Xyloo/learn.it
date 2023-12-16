using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class BasicGroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public int UserCount { get; set; }
        public AnonymousUserResponseDto Creator { get; set; } = null!;

        public BasicGroupDto(Group group)
        {
            GroupId = group.GroupId;
            Name = group.Name;
            UserCount = group.Users.Count;
            Creator = new AnonymousUserResponseDto(group.Creator);
        }

        [JsonConstructor]
        public BasicGroupDto()
        {
        }
    }
}
