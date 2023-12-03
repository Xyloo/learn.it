namespace learn.it.Exceptions.NotFound
{
    public class UserAchievementNotFoundException : NotFoundException
    {
        public UserAchievementNotFoundException(int userId, int achievementId) : base(
            $"UserAchievement with userId: {userId} and achievementId: {achievementId} was not found.")
        {
        }
    }
}
