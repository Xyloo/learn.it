using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class FlashcardsService : IFlashcardsService
    {
        private readonly IFlashcardsRepository _flashcardsRepository;

        public FlashcardsService(IFlashcardsRepository flashcardsRepository)
        {
            _flashcardsRepository = flashcardsRepository;
        }

        public async Task<IEnumerable<FlashcardDto>> GetFlashcardsInSet(int studySetId)
        {
            var flashcards = await _flashcardsRepository.GetFlashcardsInSet(studySetId);
            return flashcards.Select(f => new FlashcardDto(f)).ToList();
        }

        public async Task<FlashcardDto> GetFlashcard(int id)
        {
            var flashcard = await _flashcardsRepository.GetFlashcard(id) ?? throw new FlashcardNotFoundException(id);
            return new FlashcardDto(flashcard);
        }

        public async Task<Flashcard> AddFlashcard(Flashcard flashcard)
        {
            return await _flashcardsRepository.AddFlashcard(flashcard);
        }

        public async Task<Flashcard> UpdateFlashcard(Flashcard flashcard)
        {
            return await _flashcardsRepository.UpdateFlashcard(flashcard);
        }

        public async Task RemoveFlashcard(int id)
        {
            await _flashcardsRepository.RemoveFlashcard(id);
        }
    }
}
