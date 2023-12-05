﻿using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using learn.it.Exceptions;
using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;
using Microsoft.OpenApi.Extensions;

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
            if (!CheckPredicateValid(achievement.Predicate))
                throw new InvalidInputDataException(
                    "Predykat nie spełnia wymogów walidacji. Oczekiwana postać: pole operator liczba, np. TotalLoginDays > 5. Dopuszczalne wartości: pola UserStats, operatory >= == <= > <, liczba int.");
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
            if (!CheckPredicateValid(achievement.Predicate))
                throw new InvalidInputDataException(
                    "Predykat nie spełnia wymogów walidacji. Oczekiwana postać: pole operator liczba, np. TotalLoginDays > 5. Dopuszczalne wartości: pola UserStats, operatory >= == <= > <, liczba int.");
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

        //wanted form: x.SomeUserStatsField operator number, eg. x.TotalLoginDays >= 5
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
