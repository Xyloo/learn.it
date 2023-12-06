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
        private readonly IFlashcardUserProgressService _flashcardUserProgressService;
        private readonly IAchievementsService _achievementsService;

        public AnswersController(IAnswersService answersService, IFlashcardsService flashcardsService,
            IUsersService usersService, IFlashcardUserProgressService flashcardUserProgressService, IAchievementsService achievementsService)
        {
            _answersService = answersService;
            _flashcardsService = flashcardsService;
            _usersService = usersService;
            _flashcardUserProgressService = flashcardUserProgressService;
            _achievementsService = achievementsService;
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> AddAnswer(CreateAnswerDto answer)
        {
            var flashcard = await _flashcardsService.GetFlashcard(answer.FlashcardId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
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

                FlashcardUserProgress progress;
                try
                {
                    progress = await _flashcardUserProgressService.GetFlashcardUserProgressByFlashcardIdAndUserId(
                            flashcard.FlashcardId, user.UserId);
                    if (answer.IsCorrect)
                    {
                        progress.ConsecutiveCorrectAnswers++;
                        if (progress is { ConsecutiveCorrectAnswers: >= 5, NeedsMoreRepetitions: false } or { ConsecutiveCorrectAnswers: >= 7, NeedsMoreRepetitions: true })
                        {
                            progress.IsMastered = true;
                            progress.MasteredTimestamp = DateTime.UtcNow;
                            user.UserStats.TotalFlashcardsMastered++;
                        }
                    }
                    else
                    {
                        progress.ConsecutiveCorrectAnswers = 0;
                    }
                    await _flashcardUserProgressService.UpdateFlashcardUserProgress(progress);
                    if (await ControllerUtils.IsStudySetMastered(flashcard.StudySet, user, _flashcardsService, _flashcardUserProgressService))
                    {
                        user.UserStats.TotalSetsMastered++;
                    }
                    await _usersService.UpdateUser(user);
                    await _achievementsService.GrantAchievementsContainingPredicate(nameof(UserStats.TotalFlashcardsMastered), user);
                    await _achievementsService.GrantAchievementsContainingPredicate(nameof(UserStats.TotalSetsMastered), user);
                }
                catch (FlashcardUserProgressNotFoundException)
                {
                    progress = new FlashcardUserProgress
                    {
                        Flashcard = flashcard,
                        User = user,
                        ConsecutiveCorrectAnswers = answer.IsCorrect ? 1 : 0,
                        IsMastered = false,
                        MasteredTimestamp = null,
                        NeedsMoreRepetitions = false
                    };
                    await _flashcardUserProgressService.AddFlashcardUserProgress(progress);
                }

                var addedAnswer = await _answersService.AddAnswer(newAnswer);
                return CreatedAtAction(nameof(GetAnswerById), new { answerId = addedAnswer.AnswerId }, new AnswerDto(addedAnswer));
            }
            throw new ForbiddenAccessException("Nie masz dostępu do tego zestawu.");
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
            throw new ForbiddenAccessException("Nie masz dostępu do tej odpowiedzi.");
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
            throw new ForbiddenAccessException("Nie masz dostępu do tego zestawu.");
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
            throw new ForbiddenAccessException("Nie masz dostępu do odpowiedzi tego użytkownika.");
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