using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class FlashcardUserProgressRepository : IFlashcardUserProgressRepository
    {
        private readonly LearnitDbContext _context;
        public FlashcardUserProgressRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<FlashcardUserProgress> AddFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            _context.FlashcardUserProgress.Add(flashcardUserProgress);
            await _context.SaveChangesAsync();
            return flashcardUserProgress;
        }

        public async Task<FlashcardUserProgress?> GetFlashcardUserProgressByFlashcardIdAndUserId(int flashcardId, int userId)
        {
           return await _context.FlashcardUserProgress.Where(f => f.Flashcard.FlashcardId == flashcardId && f.User.UserId == userId)
                .Include(f => f.Flashcard)
                .Include(f => f.User)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FlashcardUserProgress>> GetFlashcardUserProgressesByFlashcardId(int flashcardId)
        {
            return await _context.FlashcardUserProgress.Where(f => f.Flashcard.FlashcardId == flashcardId)
                .Include(f => f.Flashcard)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<FlashcardUserProgress>> GetFlashcardUserProgressesByUserId(int userId)
        {
            return await _context.FlashcardUserProgress.Where(f => f.User.UserId == userId)
                .Include(f => f.Flashcard)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<FlashcardUserProgress> UpdateFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            _context.FlashcardUserProgress.Update(flashcardUserProgress);
            await _context.SaveChangesAsync();
            return flashcardUserProgress;
        }

        public async Task RemoveFlashcardUserProgress(FlashcardUserProgress flashcardUserProgress)
        {
            _context.FlashcardUserProgress.Remove(flashcardUserProgress);
            await _context.SaveChangesAsync();
        }
    }
}
