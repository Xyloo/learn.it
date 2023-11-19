namespace learn.it.Models.Dtos.Response
{
    public class GroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public AnonymousUserResponseDto Creator { get; set; } = null!;
        public ICollection<AnonymousUserResponseDto> Users { get; set; } = null!;
        public ICollection<StudySet> StudySets { get; set; } = null!;

        public GroupDto(Group group)
        {
            GroupId = group.GroupId;
            Name = group.Name;
            Creator = new AnonymousUserResponseDto(group.Creator);
            Users = group.Users.Select(user => new AnonymousUserResponseDto(user)).ToList();
            StudySets = group.StudySets;
        }
    }
}
