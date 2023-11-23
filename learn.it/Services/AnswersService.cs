using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class AnswersService : IAnswersService
    {
        private readonly IAnswersRepository _answersRepository;

        public AnswersService(IAnswersRepository answersRepository)
        {
            _answersRepository = answersRepository;
        }

        public async Task<Answer> AddAnswer(Answer answer)
        {
            return await _answersRepository.AddAnswer(answer);
        }

        public async Task<Answer> GetAnswerById(int answerId)
        {
            var answer = await _answersRepository.GetAnswerById(answerId) ?? throw new AnswerNotFoundException(answerId);
            return answer;
        }

        public async Task<AnswerDto> GetAnswerDtoById(int answerId)
        {
            var answer = await _answersRepository.GetAnswerById(answerId) ?? throw new AnswerNotFoundException(answerId);
            return new AnswerDto(answer);
        }

        public async Task<IEnumerable<Answer>> GetAnswersByFlashcardId(int flashcardId)
        {
            return await _answersRepository.GetAnswersByFlashcardId(flashcardId);
        }

        public async Task<IEnumerable<Answer>> GetAnswersByUserId(int userId)
        {
            return await _answersRepository.GetAnswersByUserId(userId);
        }

        public async Task RemoveAnswer(Answer answer)
        {
            await _answersRepository.RemoveAnswer(answer);
        }
    }
}
