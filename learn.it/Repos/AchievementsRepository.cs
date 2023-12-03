using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly LearnitDbContext _dbContext;

        public AchievementsRepository(LearnitDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Achievement> AddAchievement(Achievement achievement)
        {
            _dbContext.Achievements.Add(achievement);
            await _dbContext.SaveChangesAsync();
            return achievement;
        }

        public async Task<UserAchievements> GrantAchievementToUser(UserAchievements userAchievements)
        {
            _dbContext.UserAchievements.Add(userAchievements);
            await _dbContext.SaveChangesAsync();
            return userAchievements;
        }

        public async Task RemoveAchievementFromUser(UserAchievements userAchievement)
        {
            _dbContext.UserAchievements.Remove(userAchievement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Achievement> GetAchievement(int id)
        {
            var achievement = await _dbContext.Achievements.FindAsync(id) ?? throw new AchievementNotFoundException(id);
            return achievement;
        }

        public async Task<IEnumerable<Achievement>> GetAchievements()
        {
            return await _dbContext.Achievements.ToListAsync();
        }

        public async Task<IEnumerable<UserAchievements>> GetUserAchievementsByUserId(int userId)
        {
            var achievements = await _dbContext.UserAchievements
                .Include(u => u.Achievement)
                .Where(u => u.User.UserId == userId)
                .ToListAsync();
            return achievements;
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsContainingInPredicate(string predicate)
        {
            return await _dbContext.Achievements.Where(a => a.Predicate.Contains(predicate)).ToListAsync();
        }

        public async Task<Achievement> UpdateAchievement(Achievement achievement)
        {
            _dbContext.Achievements.Update(achievement);
            await _dbContext.SaveChangesAsync();
            return achievement;
        }

        public async Task RemoveAchievement(Achievement achievement)
        {
            _dbContext.Achievements.Remove(achievement);
            await _dbContext.SaveChangesAsync();
        }
    }
}
