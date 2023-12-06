namespace learn.it.Models.Dtos.Response
{
    public class UserAchievementsDto
    {
        public AnonymousUserResponseDto User { get; set; }
        public AchievementDto Achievement { get; set; }
        public DateTime Timestamp { get; set; }

        public UserAchievementsDto(UserAchievements userAchievement)
        {
            User = new AnonymousUserResponseDto(userAchievement.User);
            Achievement = new AchievementDto(userAchievement.Achievement);
            Timestamp = userAchievement.Timestamp;
        }
    }
}
