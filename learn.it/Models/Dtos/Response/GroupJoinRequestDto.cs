using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class GroupJoinRequestDto
    {
        public int UserId { get; set; }
        public int CreatorId { get; set; }
        public int GroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public GroupJoinRequestDto(GroupJoinRequest groupJoinRequest)
        {
            UserId = groupJoinRequest.UserId;
            CreatorId = groupJoinRequest.Creator.UserId;
            GroupId = groupJoinRequest.GroupId;
            CreatedAt = groupJoinRequest.CreatedAt;
            ExpiresAt = groupJoinRequest.ExpiresAt;
        }

        [JsonConstructor]
        public GroupJoinRequestDto()
        {
        }
    }
}
