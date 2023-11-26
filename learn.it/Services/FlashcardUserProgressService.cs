using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class FlashcardUserProgressService : IFlashcardUserProgressService
    {
        private readonly IFlashcardUserProgressRepository _flashcardUserProgressRepository;

        public FlashcardUserProgressService(IFlashcardUserProgressRepository flashcardUserProgressRepository)
        {
            _flashcardUserProgressRepository = flashcardUserProgressRepository;
        }

        public async Task<FlashcardUserProgress> AddFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            return await _flashcardUserProgressRepository.AddFlashcardUserProgress(flashcardUserProgress);
        }

        public async Task<FlashcardUserProgress> GetFlashcardUserProgressByFlashcardIdAndUserId(int flashcardId, int userId)
        {
            var progress = await _flashcardUserProgressRepository.GetFlashcardUserProgressByFlashcardIdAndUserId(flashcardId, userId) ?? throw new FlashcardUserProgressNotFoundException(flashcardId, userId);
            return progress;
        }

        public async Task<FlashcardUserProgressDto> GetFlashcardUserProgressDtoByFlashcardIdAndUserId(int flashcardId, int userId)
        {
            var progress = await _flashcardUserProgressRepository.GetFlashcardUserProgressByFlashcardIdAndUserId(flashcardId, userId) ?? throw new FlashcardUserProgressNotFoundException(flashcardId, userId);
            return new FlashcardUserProgressDto(progress);
        }

        public async Task<IEnumerable<FlashcardUserProgressDto>> GetFlashcardUserProgressesByFlashcardId(int flashcardId)
        {
            var progresses = await _flashcardUserProgressRepository.GetFlashcardUserProgressesByFlashcardId(flashcardId);
            return progresses.Select(p => new FlashcardUserProgressDto(p));
        }

        public async Task<IEnumerable<FlashcardUserProgressDto>> GetFlashcardUserProgressesByUserId(int userId)
        {
            var progresses = await _flashcardUserProgressRepository.GetFlashcardUserProgressesByUserId(userId);
            return progresses.Select(p => new FlashcardUserProgressDto(p));
        }

        public async Task RemoveFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            var progress = await _flashcardUserProgressRepository.GetFlashcardUserProgressByFlashcardIdAndUserId(flashcardUserProgress.FlashcardId, flashcardUserProgress.UserId) ?? throw new FlashcardUserProgressNotFoundException(flashcardUserProgress.FlashcardId, flashcardUserProgress.UserId);
            await _flashcardUserProgressRepository.RemoveFlashcardUserProgress(flashcardUserProgress);
        }

        public async Task<FlashcardUserProgress> UpdateFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            var progress = await _flashcardUserProgressRepository.GetFlashcardUserProgressByFlashcardIdAndUserId(flashcardUserProgress.FlashcardId, flashcardUserProgress.UserId) ?? throw new FlashcardUserProgressNotFoundException(flashcardUserProgress.FlashcardId, flashcardUserProgress.UserId);
            return await _flashcardUserProgressRepository.UpdateFlashcardUserProgress(flashcardUserProgress);
        }
    }
}
