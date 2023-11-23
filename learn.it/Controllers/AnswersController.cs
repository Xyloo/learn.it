using learn.it.Exceptions;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [ApiController]
    [Route("api/answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswersService _answersService;
        private readonly IFlashcardsService _flashcardsService;
        private readonly IUsersService _usersService;

        public AnswersController(IAnswersService answersService, IFlashcardsService flashcardsService,
            IUsersService usersService)
        {
            _answersService = answersService;
            _flashcardsService = flashcardsService;
            _usersService = usersService;
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AddAnswer(CreateAnswerDto answer)
        {
            var flashcard = await _flashcardsService.GetFlashcard(answer.FlashcardId);
            var user = await _usersService.GetUserByIdOrUsername(answer.UserId.ToString());
            if (ControllerUtils.CanUserAccessStudySet(user, flashcard.StudySet))
            {
                var newAnswer = new Answer
                {
                    Flashcard = flashcard,
                    User = user,
                    IsCorrect = answer.IsCorrect,
                    AnswerTime = answer.AnswerTime,
                    AnswerTimestamp = DateTime.UtcNow
                };
                //TODO: Update flashcard user progress
                var addedAnswer = await _answersService.AddAnswer(newAnswer);
                return CreatedAtAction(nameof(GetAnswerById), new { answerId = addedAnswer.AnswerId }, new AnswerDto(addedAnswer));
            }
            throw new ForbiddenAccessException("You cannot access this study set");
        }

        [HttpGet("{answerId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetAnswerById(int answerId)
        {
            var answer = await _answersService.GetAnswerById(answerId);
            if (ControllerUtils.IsUserAdminOrSelf(answer.User, User))
            {
                return Ok(new AnswerDto(answer));
            }
            throw new ForbiddenAccessException("You cannot access this answer");
        }

        [HttpGet("flashcard/{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetAnswersByFlashcardId(int flashcardId)
        {
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            if (ControllerUtils.IsUserAdmin(User))
            {
                return Ok((await _answersService.GetAnswersByFlashcardId(flashcardId)).Select(a => new AnswerDto(a)));
            }

            if (ControllerUtils.CanUserAccessStudySet(user, flashcard.StudySet))
            {
                var answers = (await _answersService.GetAnswersByFlashcardId(flashcardId)).ToList().Where(g => g.User.UserId == user.UserId).ToList();
                return Ok(answers.Select(a => new AnswerDto(a)));
            }
            throw new ForbiddenAccessException("You cannot access this study set");
        }

        [HttpGet("user/{userId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetAnswersByUserId(int userId)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            if (ControllerUtils.IsUserAdminOrSelf(user, User))
            {
                return Ok((await _answersService.GetAnswersByUserId(userId)).Select(a => new AnswerDto(a)));
            }
            throw new ForbiddenAccessException("You cannot access this user's answers");
        }

        [HttpDelete("{answerId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> RemoveAnswer(int answerId)
        {
            var answer = await _answersService.GetAnswerById(answerId);
            await _answersService.RemoveAnswer(answer);
            return NoContent();
        }
    }
}
