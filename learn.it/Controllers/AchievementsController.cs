using System.ComponentModel.DataAnnotations;
using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementsService _achievementsService;
        public AchievementsController(IAchievementsService achievementsService)
        {
            _achievementsService = achievementsService;
        }

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var achievements = (await _achievementsService.GetAchievements()).Select(a => new AchievementDto(a)).ToList();
            return Ok(achievements);
        }

        [HttpGet("{achievementId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetAchievementDetails([FromRoute] int achievementId)
        {
            var achievement = await _achievementsService.GetAchievement(achievementId);
            return Ok(achievement);
        }

        [HttpPost]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> AddAchievement(IFormFile achievementImage,
            [FromForm] CreateAchievementDto achievement)
        {
            ControllerUtils.CheckIfValidImage(achievementImage);

            var newAchievement = new Achievement
            {
                Name = achievement.Name,
                Description = achievement.Description,
                Predicate = achievement.Predicate
            };

            var addedAchievement = await _achievementsService.AddAchievement(newAchievement, achievementImage);
            return Ok(addedAchievement);
        }

        [HttpDelete("{achievementId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> RemoveAchievement([FromRoute] int achievementId)
        {
            var achievement = await _achievementsService.GetAchievement(achievementId);
            await _achievementsService.RemoveAchievement(achievementId);
            return Ok();
        }

        [HttpPut("{achievementId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> UpdateAchievement([FromRoute] int achievementId,
            [FromBody] UpdateAchievementDto updateData)
        {
            var achievement = await _achievementsService.GetAchievement(achievementId);
            var validationContext = new ValidationContext(updateData);
            var validationResult = updateData.Validate(validationContext);
            if (validationResult.Any())
            {
                throw new InvalidInputDataException(validationResult.ToString());
            }

            achievement.Name = updateData.Name ?? achievement.Name;
            achievement.Description = updateData.Description ?? achievement.Description;
            achievement.Predicate = updateData.Predicate ?? achievement.Predicate;
            var updatedAchievement = await _achievementsService.UpdateAchievement(achievement);
            return Ok(updatedAchievement);
        }

        [HttpPut("{achievementId}/image")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> UpdateAchievementImage([FromRoute] int achievementId, IFormFile image)
        {
            ControllerUtils.CheckIfValidImage(image);

            var achievement = await _achievementsService.GetAchievement(achievementId);
            var updatedAchievement = await _achievementsService.UpdateAchievementImage(achievement, image);
            return Ok(updatedAchievement);
        }
    }
}
