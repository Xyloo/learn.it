namespace learn.it.Models.Dtos.Response
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public AchievementDto(Achievement achievement)
        {
            Id = achievement.AchievementId;
            Name = achievement.Name;
            Description = achievement.Description;
            ImagePath = achievement.ImagePath;
        }
    }
}
