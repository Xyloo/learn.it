namespace learn.it.Models.Dtos.Response
{
    public class SelfUserResponseDto : AnonymousUserResponseDto
    {
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Permissions { get; set; }

        public SelfUserResponseDto(User user) : base(user)
        {
            Email = user.Email;
            CreateTime = user.CreateTime;
            LastLogin = user.LastLogin;
            Permissions = user.Permissions.Name;
        }
    }
}
