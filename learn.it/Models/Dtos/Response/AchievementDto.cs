﻿using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class AchievementDto
    {
        public int AchievementId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public AchievementDto(Achievement achievement)
        {
            AchievementId = achievement.AchievementId;
            Name = achievement.Name;
            Description = achievement.Description;
            ImagePath = achievement.ImagePath;
        }

        [JsonConstructor]
        public AchievementDto()
        {
        }
    }
}
