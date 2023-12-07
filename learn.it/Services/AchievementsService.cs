using learn.it.Exceptions;
using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;
using learn.it.Utils;

namespace learn.it.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly IAchievementsRepository _achievementsRepository;
        private readonly IImageHandler _imageHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string AchievementImagesDirectory = "AchievementImages";
        public AchievementsService(IAchievementsRepository achievementsRepository, IImageHandler imageHandler, IWebHostEnvironment webHostEnvironment)
        {
            _achievementsRepository = achievementsRepository;
            _imageHandler = imageHandler;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Achievement> AddAchievement(Achievement achievement, IFormFile achievementImage)
        {
            if (!CheckPredicateValid(achievement.Predicate))
                throw new InvalidInputDataException(
                    "Predykat nie spełnia wymogów walidacji. Oczekiwana postać: pole operator liczba, np. TotalLoginDays > 5. Dopuszczalne wartości: pola UserStats, operatory >= == <= > <, liczba int.");
            var path = Path.Combine(_webHostEnvironment.WebRootPath, AchievementImagesDirectory);
            var imageFilename = await _imageHandler.AddImage(achievementImage, path);
            achievement.ImagePath = imageFilename;
            await _achievementsRepository.AddAchievement(achievement);
            return achievement;
        }

        public async Task<Achievement> GetAchievement(int id)
        {
            return await _achievementsRepository.GetAchievement(id) ?? throw new AchievementNotFoundException(id);
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
            if (!CheckPredicateValid(achievement.Predicate))
                throw new InvalidInputDataException(
                    "Predykat nie spełnia wymogów walidacji. Oczekiwana postać: pole operator liczba, np. TotalLoginDays > 5. Dopuszczalne wartości: pola UserStats, operatory >= == <= > <, liczba int.");
            await _achievementsRepository.UpdateAchievement(achievement);
            return achievement;
        }

        public async Task<Achievement> UpdateAchievementImage(Achievement achievement, IFormFile newImage)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, AchievementImagesDirectory);
            var imageFilename = await _imageHandler.AddImage(newImage, path);
            achievement.ImagePath = imageFilename;
            var updatedAchievement = await _achievementsRepository.UpdateAchievement(achievement);
            return updatedAchievement;
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

        public async Task<IEnumerable<Achievement>> GrantAchievementsContainingPredicate(string predicate,
            User user)
        {
            var matchingAchievements = await GetAchievementsContainingInPredicate(predicate);
            var userAchievements = (await GetUserAchievements(user.UserId)).Select(g => g.Achievement);
            var notGrantedAchievements = matchingAchievements.Where(a => userAchievements.All(x => x.Name != a.Name)).ToList();
            List<Achievement> grantedAchievements = new();
            foreach (var achievement in notGrantedAchievements)
            {
                if (GetPredicateResult(achievement, user.UserStats))
                {
                    await GrantAchievement(user.UserId, achievement.AchievementId);
                    grantedAchievements.Add(achievement);
                }
            }
            return grantedAchievements;
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

        public bool GetPredicateResult(Achievement achievement, UserStats userStats)
        {
            var predicate = achievement.Predicate;
            if (!CheckPredicateValid(predicate))
                throw new InvalidInputDataException(
                    "Predykat nie spełnia wymogów walidacji. Oczekiwana postać: pole operator liczba, np. TotalLoginDays > 5. Dopuszczalne wartości: pola UserStats, operatory >= == <= > <, liczba int.");
            
            var tokens = predicate.Split(' ');
            var stat = tokens[0];
            var comparisonOperator = tokens[1];
            var threshold =  int.Parse(tokens[2]);

            var userStatValue = (int)userStats.GetType().GetProperty(stat)?.GetValue(userStats)!;

            return comparisonOperator switch
            {
                ">=" => userStatValue >= threshold,
                "<=" => userStatValue <= threshold,
                "==" => userStatValue == threshold,
                "<" => userStatValue < threshold,
                ">" => userStatValue > threshold,
                _ => throw new InvalidInputDataException("Operator niewspierany. Ten wyjątek nigdy nie powinien się pojawić.")
            };

        }

        //wanted form: SomeUserStatsField operator number, eg. TotalLoginDays >= 5
        private bool CheckPredicateValid(string predicate)
        {
            var tokens = predicate.Split(" ");
            if(tokens.Length != 3) { return false; }

            var field = tokens[0];

            var properties = typeof(UserStats).GetProperties();
            var propertyNames = properties.Select(p => p.Name)
                .Where(p => p is not (nameof(UserStats.User) or nameof(UserStats.UserId))).ToList();
            if(propertyNames.All(p => p != field))
            { return false; }

            var predicateOperator = tokens[1];

            var operators = new List<string> { "<=", "==", ">=", ">", "<"};
            if (operators.All(p => p != predicateOperator))
            { return false; }

            var value = tokens[2];
            var isNumber = int.TryParse(value, out var parsedValue);
            if(!isNumber) { return false; }

            return true;
        }
    }
}
