using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IFlashcardUserProgressRepository
    {
        Task<FlashcardUserProgress> AddFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
        Task<FlashcardUserProgress?> GetFlashcardUserProgressByFlashcardIdAndUserId(int flashcardId, int userId);
        Task<IEnumerable<FlashcardUserProgress>> GetFlashcardUserProgressesByFlashcardId(int flashcardId);
        Task<IEnumerable<FlashcardUserProgress>> GetFlashcardUserProgressesByUserId(int userId);
        Task<FlashcardUserProgress> UpdateFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
        Task RemoveFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
    }
}
