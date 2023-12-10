using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using learn.it.Models.Dtos.Response;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models
{
    [Table("GroupJoinRequest", Schema = "learnitdb")]
    public class GroupJoinRequest
    {
        //by using a compound key, we can ensure that a user can only request to join a group once
        [Key]
        public int GroupId { get; set; }
        [Key]
        public int UserId { get; set; }
        [Precision(0)]
        public DateTime CreatedAt { get; set; }
        [Precision(0)]
        public DateTime ExpiresAt { get; set; }
        [ForeignKey("CreatorId")]
        [InverseProperty("GroupJoinRequests")]
        // this might be a bit over-engineered, but using the creator's User object we can determine if this should be treated as an invitation to join or a request to join
        public virtual User Creator { get; set; }
        [InverseProperty("GroupJoinRequests")]
        public virtual Group Group { get; set; }

        public GroupJoinRequestDto ToGroupJoinRequestDto()
        {
            return new GroupJoinRequestDto(this);
        }
    }
}
