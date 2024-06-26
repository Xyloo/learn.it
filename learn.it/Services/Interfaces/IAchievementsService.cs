﻿using learn.it.Models;

namespace learn.it.Services.Interfaces
{
    public interface IAchievementsService
    {
        Task<Achievement> AddAchievement(Achievement achievement, IFormFile achievementImage);
        Task<Achievement> GetAchievement(int id);
        Task<IEnumerable<Achievement>> GetAchievements();
        Task<IEnumerable<Achievement>> GetAchievementsContainingInPredicate(string predicate);
        Task<Achievement> UpdateAchievement(Achievement achievement);
        Task<IEnumerable<UserAchievements>> GetUserAchievements(int userId);
        Task<UserAchievements> GrantAchievement(int userId, int achievementId);
        Task RevokeAchievement(int userId, int achievementId);
        Task RemoveAchievement(int id);
        bool GetPredicateResult(Achievement achievement, UserStats userStats);
        public Task<Achievement> UpdateAchievementImage(Achievement achievement, IFormFile newImage);

        public Task<IEnumerable<Achievement>> GrantAchievementsContainingPredicate(string predicate,
            User user);
    }
}
