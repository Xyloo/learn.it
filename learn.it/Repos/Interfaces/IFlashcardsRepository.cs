using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IFlashcardsRepository
    {
        Task<IEnumerable<Flashcard>> GetFlashcardsInSet(int studySetId);
        Task<Flashcard?> GetFlashcard(int id);
        Task<Flashcard> AddFlashcard(Flashcard flashcard);
        Task<Flashcard> UpdateFlashcard(Flashcard flashcard);
        Task RemoveFlashcard(int id);
    }
}
