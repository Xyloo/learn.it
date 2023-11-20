using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Services.Interfaces
{
    public interface IFlashcardsService
    {
        Task<IEnumerable<FlashcardDto>> GetFlashcardsInSet(int studySetId);
        Task<FlashcardDto> GetFlashcard(int id);
        Task<Flashcard> AddFlashcard(Flashcard flashcard);
        Task<Flashcard> UpdateFlashcard(Flashcard flashcard);
        Task RemoveFlashcard(int id);
    }
}
