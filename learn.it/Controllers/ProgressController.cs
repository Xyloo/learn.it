using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Controllers
{
    [ApiController]
    [Route("api/flashcard_progress")]
    public class ProgressController : ControllerBase
    {
        private readonly IFlashcardsService _flashcardsService;
        private readonly IAnswersService _answersService;
        private readonly IStudySetsService _studySetsService;
        private readonly IUsersService _usersService;
        private readonly IFlashcardUserProgressService _flashcardProgressService;

        public ProgressController(IFlashcardsService flashcardsService, IAnswersService answersService,
            IStudySetsService studySetsService, IUsersService usersService,
            IFlashcardUserProgressService flashcardProgressService)
        {
            _flashcardsService = flashcardsService;
            _answersService = answersService;
            _studySetsService = studySetsService;
            _usersService = usersService;
            _flashcardProgressService = flashcardProgressService;
        }

        [HttpGet("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetFlashcardProgress([FromRoute] int flashcardId)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            var progress = await _flashcardProgressService.GetFlashcardUserProgressDtoByFlashcardIdAndUserId(flashcard.FlashcardId, user.UserId);
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                return Ok(progress);
            }

            throw new FlashcardUserProgressNotFoundException(user.UserId, flashcardId);
        }

        [HttpGet("{flashcardId}/{userId}")]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetFlashcardProgress([FromRoute] int flashcardId, [FromRoute] int userId)
        {
            var user = await _usersService.GetUserByIdOrUsername(userId.ToString());
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var progress = await _flashcardProgressService.GetFlashcardUserProgressDtoByFlashcardIdAndUserId(flashcard.FlashcardId, user.UserId);
            return Ok(progress);
        }

        [HttpGet("{studySetId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetStudySetProgress([FromRoute] int studySetId)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var studySet = await _studySetsService.GetStudySetById(studySetId);
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                var progress = await _flashcardProgressService.GetFlashcardUserProgressesByUserId(user.UserId);
                var flashcards = await _flashcardsService.GetFlashcardsInSet(studySetId);
                progress = progress.Where(p => flashcards.Any(f => f.Id == p.Flashcard.Id));
                return Ok(progress);
            }

            throw new FlashcardUserProgressNotFoundException($"No user progress was found for study set id: [{studySetId}]");
        }

        [HttpPut("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateFlashcardProgress([FromRoute] int flashcardId,
            [FromBody] FlashcardUserProgressDto progressDto)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                var progress = await _flashcardProgressService.GetFlashcardUserProgressByFlashcardIdAndUserId(flashcard.FlashcardId, user.UserId);
                progress.NeedsMoreRepetitions = progressDto.NeedsMoreRepetitions;
                if (progressDto.NeedsMoreRepetitions && progress.IsMastered && progress.ConsecutiveCorrectAnswers < 7)
                {
                    progress.IsMastered = false;
                    progress.MasteredTimestamp = null;
                }

                if (!progressDto.NeedsMoreRepetitions && !progress.IsMastered &&
                    progress.ConsecutiveCorrectAnswers >= 5)
                {
                    progress.IsMastered = true;
                    progress.MasteredTimestamp = DateTime.UtcNow;
                }
                return Ok(progressDto);
            }
            throw new FlashcardUserProgressNotFoundException(user.UserId, flashcardId);
        }
    }
}
