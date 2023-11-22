using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class FlashcardsRepository : IFlashcardsRepository
    {
        private readonly LearnitDbContext _context;

        public FlashcardsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardsInSet(int studySetId)
        {
            return await _context.Flashcards.Where(f => f.StudySet.StudySetId == studySetId).ToListAsync();
        }

        public async Task<Flashcard?> GetFlashcard(int id)
        {
            return await _context.Flashcards.Include(f => f.StudySet).FirstOrDefaultAsync(f => f.FlashcardId == id);
        }

        public async Task<Flashcard> AddFlashcard(Flashcard flashcard)
        {
            _context.Flashcards.Add(flashcard);
            await _context.SaveChangesAsync();
            return flashcard;
        }

        public async Task<Flashcard> UpdateFlashcard(Flashcard flashcard)
        {
            _context.Flashcards.Update(flashcard);
            await _context.SaveChangesAsync();
            return flashcard;
        }

        public async Task RemoveFlashcard(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id) ?? throw new FlashcardNotFoundException(id);
            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();
        }
    }
}
