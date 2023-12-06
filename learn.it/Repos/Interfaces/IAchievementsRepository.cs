using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IAchievementsRepository
    {
        Task<Achievement> AddAchievement(Achievement achievement);
        Task<UserAchievements> GrantAchievementToUser(UserAchievements userAchievements);
        Task RemoveAchievementFromUser(UserAchievements userAchievements);
        Task<Achievement> GetAchievement(int id);
        Task<IEnumerable<Achievement>> GetAchievements();
        Task<IEnumerable<UserAchievements> > GetUserAchievementsByUserId(int userId);
        Task<IEnumerable<Achievement>> GetAchievementsContainingInPredicate(string predicate);
        Task<Achievement> UpdateAchievement(Achievement achievement);
        Task RemoveAchievement(Achievement achievement);
    }
}
