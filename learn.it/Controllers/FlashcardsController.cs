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
    [ApiController]
    [Route("api/flashcards")]
    public class FlashcardsController : ControllerBase
    {
        private readonly IFlashcardsService _flashcardsService;
        private readonly IStudySetsService _studySetsService;
        private readonly IUsersService _usersService;

        public FlashcardsController(IFlashcardsService flashcardsService, IStudySetsService studySetsService, IUsersService usersService)
        {
            _flashcardsService = flashcardsService;
            _studySetsService = studySetsService;
            _usersService = usersService;
        }

        [HttpGet("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetFlashcard([FromRoute] int flashcardId)
        {
            var flashcardDto = await _flashcardsService.GetFlashcardDto(flashcardId);
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                return Ok(flashcardDto);
            }
            return NotFound();
        }

        [HttpGet("fromSet/{studySetId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetFlashcardsInSet([FromRoute] int studySetId)
        {
            var studySet = await _studySetsService.GetStudySetById(studySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                var flashcards = await _flashcardsService.GetFlashcardsInSet(studySetId);
                return Ok(flashcards);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateTextFlashcard([FromBody] CreateTextFlashcardDto newFlashcard)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var studySet = await _studySetsService.GetStudySetById(newFlashcard.StudySetId);
            if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User))
            {
                var flashcard = new Flashcard
                {
                    Term = newFlashcard.Term,
                    Definition = newFlashcard.Definition,
                    IsTermText = true,
                    StudySet = studySet
                };
                var createdFlashcard = await _flashcardsService.AddFlashcard(flashcard);
                return CreatedAtAction(nameof(GetFlashcard), new { flashcardId = createdFlashcard.FlashcardId }, new FlashcardDto(createdFlashcard));
            }
            return BadRequest();
        }

        [HttpPost("withImage")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateImageFlashcard([FromForm] CreateImageFlashcardDto newFlashcard, IFormFile image)
        {
            switch (image.Length)
            {
                case 0:
                    return BadRequest("No file was provided.");
                case > 10 * 1024 * 1024:
                    return BadRequest("The provided file is too large (max 10 MB).");
            }

            if (ControllerUtils.IsImage(image) is false)
            {
                return BadRequest("The provided file is not an image.");
            }


            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var studySet = await _studySetsService.GetStudySetById(newFlashcard.StudySetId);
            if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User))
            {
                var flashcard = new Flashcard
                {
                    Definition = newFlashcard.Definition,
                    IsTermText = false,
                    StudySet = studySet
                };
                var createdFlashcard = await _flashcardsService.AddImageFlashcard(flashcard, image);
                return CreatedAtAction(nameof(GetFlashcard), new { flashcardId = createdFlashcard.FlashcardId }, new FlashcardDto(createdFlashcard));
            }
            return BadRequest();
        }

        [HttpPut("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateToTextFlashcard([FromRoute] int flashcardId,
            [FromBody] UpdateFlashcardDto updatedFlashcard)
        {
            var validationContext = new ValidationContext(updatedFlashcard);
            var validationResult = updatedFlashcard.Validate(validationContext);
            if (validationResult.Any())
            {
                return BadRequest(validationResult);
            }

            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());

            StudySet? newStudySet;
            if (updatedFlashcard.StudySetId is not null)
            {
                newStudySet = await _studySetsService.GetStudySetById(updatedFlashcard.StudySetId.Value);
                if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User) is false)
                {
                    return NotFound();
                }
            }
            else
            {
                newStudySet = null;
            }

            if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User))
            {
                if (flashcard.IsTermText is false)
                {
                    if (updatedFlashcard.Term is null)
                    {
                        throw new InvalidInputDataException(
                            "Term cannot be null if changing from an image flashcard to a text flashcard.");
                    }
                    await _flashcardsService.RemoveImage(flashcard);
                    flashcard.IsTermText = true;
                }
                flashcard.Term = updatedFlashcard.Term ?? flashcard.Term;
                flashcard.Definition = updatedFlashcard.Definition ?? flashcard.Definition;
                flashcard.StudySet = newStudySet ?? flashcard.StudySet;
                var updated = await _flashcardsService.UpdateFlashcard(flashcard);
                return Ok(new FlashcardDto(updated));
            }
            return NotFound();
        }

        [HttpPut("{flashcardId}/withImage")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateToImageFlashcard([FromRoute] int flashcardId,
            [FromForm] UpdateFlashcardDto updatedFlashcard, IFormFile image)
        {
            //because it should be anyway
            updatedFlashcard.Term = null;

            switch (image.Length)
            {
                case 0:
                    return BadRequest("No file was provided.");
                case > 10 * 1024 * 1024:
                    return BadRequest("The provided file is too large (max 10 MB).");
            }

            if (ControllerUtils.IsImage(image) is false)
            {
                return BadRequest("The provided file is not an image.");
            }

            var validationContext = new ValidationContext(updatedFlashcard);
            var validationResult = updatedFlashcard.Validate(validationContext);
            if (validationResult.Any())
            {
                return BadRequest(validationResult);
            }

            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());

            StudySet? newStudySet;
            if (updatedFlashcard.StudySetId is not null)
            {
                newStudySet = await _studySetsService.GetStudySetById(updatedFlashcard.StudySetId.Value);
                if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User) is false)
                {
                    return NotFound();
                }
            }
            else
            {
                newStudySet = null;
            }

            if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User))
            {
                if (flashcard.IsTermText)
                {
                    flashcard.IsTermText = false;
                }
                else
                { 
                    await _flashcardsService.RemoveImage(flashcard);
                }
                var imageFileName = await _flashcardsService.AddImage(image);
                flashcard.Term = imageFileName;
                flashcard.Definition = updatedFlashcard.Definition ?? flashcard.Definition;
                flashcard.StudySet = newStudySet ?? flashcard.StudySet;
                var updated = await _flashcardsService.UpdateFlashcard(flashcard);
                return Ok(new FlashcardDto(updated));
            }
            return NotFound();
        }

        [HttpDelete("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteFlashcard([FromRoute] int flashcardId)
        {
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            var studySet = await _studySetsService.GetStudySetById(flashcard.StudySet.StudySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            if (ControllerUtils.IsUserAdminOrSelf(studySet.Creator, User))
            {
                await _flashcardsService.RemoveFlashcard(flashcardId);
                return NoContent();
            }
            return NotFound();
        }
    }
}
