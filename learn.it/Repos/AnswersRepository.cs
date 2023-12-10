using learn.it.Models;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class AnswersRepository : IAnswersRepository
    {
        private readonly LearnitDbContext _context;
        public AnswersRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<Answer> AddAnswer(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<Answer?> GetAnswerById(int answerId)
        {
            return await _context.Answers.Where(a => a.AnswerId == answerId).Include(a => a.User).ThenInclude(u => u.Permissions).Include(a => a.Flashcard).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Answer>> GetAnswersByFlashcardId(int flashcardId)
        {
            return await _context.Answers.Where(a => a.Flashcard.FlashcardId == flashcardId).Include(a => a.User).ThenInclude(u => u.Permissions).Include(a => a.Flashcard).ToListAsync();
        }

        public async Task<IEnumerable<Answer>> GetAnswersByUserId(int userId)
        {
            return await _context.Answers.Where(a => a.User.UserId == userId).Include(a => a.User).ThenInclude(u => u.Permissions).Include(a => a.Flashcard).ToListAsync();
        }

        public async Task RemoveAnswer(Answer answer)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }
}
