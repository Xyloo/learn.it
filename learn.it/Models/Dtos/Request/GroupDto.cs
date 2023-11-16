using learn.it.Models.Dtos.Response;

namespace learn.it.Models.Dtos.Request
{
    public class GroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public AnonymousUserResponseDto Owner { get; set; } = null!;
        public ICollection<AnonymousUserResponseDto> Users { get; set; } = null!;
        public ICollection<StudySet> StudySets { get; set; } = null!;

        public GroupDto(Group group)
        {
            GroupId = group.GroupId;
            Name = group.Name;
            Owner = new AnonymousUserResponseDto(group.Creator);
            Users = group.Users.Select(user => new AnonymousUserResponseDto(user)).ToList();
            StudySets = group.StudySets;
        }
    }
}
