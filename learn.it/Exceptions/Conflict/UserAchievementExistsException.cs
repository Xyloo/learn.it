namespace learn.it.Exceptions.Conflict
{
    public class UserAchievementExistsException : AlreadyExistsException
    {
        public UserAchievementExistsException(int userId, int achievementId) : base($"User with id {userId} already has achievement with id {achievementId}.")
        {
        }
    }
}
