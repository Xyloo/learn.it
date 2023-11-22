using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Services.Interfaces
{
    public interface IFlashcardsService
    {
        Task<IEnumerable<FlashcardDto>> GetFlashcardsInSet(int studySetId);
        Task<FlashcardDto> GetFlashcardDto(int id);
        Task<Flashcard> GetFlashcard(int id);
        Task<Flashcard> AddFlashcard(Flashcard flashcard);
        Task<Flashcard> AddImageFlashcard(Flashcard flashcard, IFormFile image);
        Task<Flashcard> UpdateFlashcard(Flashcard flashcard);
        Task RemoveImage(Flashcard flashcard);
        Task<string> AddImage(IFormFile image);
        Task RemoveFlashcard(int id);
    }
}
