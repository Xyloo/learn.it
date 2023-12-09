namespace learn.it.Exceptions.Conflict
{
    public class UserAchievementExistsException : AlreadyExistsException
    {
        public UserAchievementExistsException(int userId, int achievementId) : base($"Użytkownik o id [{userId}] zdobył już osiągnięcie o id [{achievementId}].")
        {
        }
    }
}
