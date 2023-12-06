namespace learn.it.Exceptions.NotFound
{
    public class UserAchievementNotFoundException : NotFoundException
    {
        public UserAchievementNotFoundException(int userId, int achievementId) : base(
            $"Nie odnaleziono osiągnięcia użytkownika dla id użytkownika [{userId}] oraz id osiągnięcia [{achievementId}]")
        {
        }
    }
}
