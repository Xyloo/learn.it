using learn.it.Services.Interfaces;
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

        public FlashcardsController(IFlashcardsService flashcardsService, IStudySetsService studySetsService)
        {
            _flashcardsService = flashcardsService;
            _studySetsService = studySetsService;
        }

        [HttpGet("{flashcardId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetFlashcard([FromRoute] int flashcardId)
        {
            var flashcard = await _flashcardsService.GetFlashcard(flashcardId);
            return Ok(flashcard);
        }
    }
}
