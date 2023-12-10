namespace learn.it.Models.Dtos.Response
{
    public class GroupJoinRequestDto
    {
        public int UserId { get; set; }
        public int CreatorId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public GroupJoinRequestDto(GroupJoinRequest groupJoinRequest)
        {
            UserId = groupJoinRequest.UserId;
            CreatorId = groupJoinRequest.Creator.UserId;
            GroupId = groupJoinRequest.GroupId;
            GroupName = groupJoinRequest.Group?.Name;
            CreatedAt = groupJoinRequest.CreatedAt;
            ExpiresAt = groupJoinRequest.ExpiresAt;
        }
    }
}
