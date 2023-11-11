namespace learn.it.Models.Dtos.Response
{
    public class SelfUserResponseDto : AnonymousUserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Permissions { get; set; }

        public SelfUserResponseDto(User user) : base(user)
        {
            Id = user.UserId;
            Email = user.Email;
            CreateTime = user.CreateTime;
            LastLogin = user.LastLogin;
            Permissions = user.Permissions.Name;
        }
    }
}
