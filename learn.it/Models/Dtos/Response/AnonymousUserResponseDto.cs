using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class AnonymousUserResponseDto
    {
        public string Username { get; set; }
        public string? Avatar { get; set; }

        public AnonymousUserResponseDto(User user)
        {
            Username = user.Username;
            Avatar = user.Avatar;
        }

        [JsonConstructor]
        public AnonymousUserResponseDto()
        {
        }
    }
}
