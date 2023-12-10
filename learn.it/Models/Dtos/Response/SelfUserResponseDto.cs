using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class SelfUserResponseDto : AnonymousUserResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Permissions { get; set; }
        public UserPreferences UserPreferences { get; set; }
        public UserStats UserStats { get; set; }

        public SelfUserResponseDto(User user) : base(user)
        {
            UserId = user.UserId;
            Email = user.Email;
            CreateTime = user.CreateTime;
            LastLogin = user.LastLogin;
            Permissions = user.Permissions.Name;
            UserPreferences = user.UserPreferences;
            UserStats = user.UserStats;
        }

        [JsonConstructor]
        public SelfUserResponseDto()
        {
        }
    }
}
