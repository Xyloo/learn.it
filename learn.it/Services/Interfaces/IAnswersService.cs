using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Services.Interfaces
{
    public interface IAnswersService
    {
        Task<Answer> AddAnswer(Answer answer);
        Task<Answer> GetAnswerById(int answerId);
        Task<AnswerDto> GetAnswerDtoById(int answerId);
        Task<IEnumerable<Answer>> GetAnswersByFlashcardId(int flashcardId);
        Task<IEnumerable<Answer>> GetAnswersByUserId(int userId);
        Task RemoveAnswer(Answer answer);
    }
}
