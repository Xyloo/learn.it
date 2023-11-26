using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IAnswersRepository
    {
        Task<Answer> AddAnswer(Answer answer);
        Task<Answer?> GetAnswerById(int answerId);
        Task<IEnumerable<Answer>> GetAnswersByFlashcardId(int flashcardId);
        Task<IEnumerable<Answer>> GetAnswersByUserId(int userId);
        Task RemoveAnswer(Answer answer);
    }
}
