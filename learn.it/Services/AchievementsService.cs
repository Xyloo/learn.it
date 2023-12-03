using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly IAchievementsRepository _achievementsRepository;
        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            _achievementsRepository = achievementsRepository;
        }

        public async Task<Achievement> AddAchievement(Achievement achievement)
        {
            await _achievementsRepository.AddAchievement(achievement);
            return achievement;
        }

        public async Task<Achievement> GetAchievement(int id)
        {
            return await _achievementsRepository.GetAchievement(id);
        }

        public async Task<IEnumerable<Achievement>> GetAchievements()
        {
            return await _achievementsRepository.GetAchievements();
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsContainingInPredicate(string predicate)
        {
            return await _achievementsRepository.GetAchievementsContainingInPredicate(predicate);
        }

        public async Task<Achievement> UpdateAchievement(Achievement achievement)
        {
            await _achievementsRepository.UpdateAchievement(achievement);
            return achievement;
        }

        public async Task<IEnumerable<UserAchievements>> GetUserAchievements(int userId)
        {
            return await _achievementsRepository.GetUserAchievementsByUserId(userId);
        }

        public async Task<UserAchievements> GrantAchievement(int userId, int achievementId)
        {
            var achievements = await _achievementsRepository.GetUserAchievementsByUserId(userId);
            var userAchievement = achievements.FirstOrDefault(u => u.AchievementId == achievementId);
            if (userAchievement != null)
            {
                throw new UserAchievementExistsException(userId, achievementId);
            }
            var newAchievement = new UserAchievements
            {
                UserId = userId,
                AchievementId = achievementId,
                Timestamp = DateTime.UtcNow
            };
            await _achievementsRepository.GrantAchievementToUser(newAchievement);
            return newAchievement;
        }

        public async Task RevokeAchievement(int userId, int achievementId)
        {
            var achievements = await _achievementsRepository.GetUserAchievementsByUserId(userId);
            var userAchievement = achievements.FirstOrDefault(u => u.AchievementId == achievementId);
            if (userAchievement == null)
            {
                throw new UserAchievementNotFoundException(userId, achievementId);
            }
            await _achievementsRepository.RemoveAchievementFromUser(userAchievement);
        }

        public async Task RemoveAchievement(int id)
        {
            var achievement = await _achievementsRepository.GetAchievement(id);
            await _achievementsRepository.RemoveAchievement(achievement);
        }
    }
}
