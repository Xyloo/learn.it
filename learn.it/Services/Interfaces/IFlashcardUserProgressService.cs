using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Services.Interfaces
{
    public interface IFlashcardUserProgressService
    {
        Task<FlashcardUserProgress> AddFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
        Task<FlashcardUserProgress> GetFlashcardUserProgressByFlashcardIdAndUserId(int flashcardId, int userId);
        Task<FlashcardUserProgressDto> GetFlashcardUserProgressDtoByFlashcardIdAndUserId(int flashcardId, int userId);
        Task<IEnumerable<FlashcardUserProgressDto>> GetFlashcardUserProgressesByFlashcardId(int flashcardId);
        Task<IEnumerable<FlashcardUserProgressDto>> GetFlashcardUserProgressesByUserId(int userId);
        Task RemoveFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
        Task<FlashcardUserProgress> UpdateFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress);
        Task<IEnumerable<FlashcardUserProgress>> GetFlashcardUserProgressesByUserIdAndStudySetId(int userId, int studySetId);
    }
}
