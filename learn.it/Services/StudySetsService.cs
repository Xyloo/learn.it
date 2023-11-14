using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class StudySetsService : IStudySetsService
    {
        private readonly IStudySetsRepository _studySetsRepository;

        public StudySetsService(IStudySetsRepository studySetsRepository)
        {
            _studySetsRepository = studySetsRepository;
        }

        public async Task<StudySet> GetStudySetById(int id)
        {
            var set = await _studySetsRepository.GetStudySetById(id) ?? throw new StudySetNotFoundException(id.ToString());
            return set;
        }

        public async Task<StudySet> GetStudySetByName(string name)
        {
            var set = await _studySetsRepository.GetStudySetByName(name) ?? throw new StudySetNotFoundException(name);
            return set;
        }

        public async Task<IEnumerable<StudySet>> GetStudySetsContainingName(string name)
        {
            return await _studySetsRepository.GetStudySetsContainingName(name);
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySetsByCreator(int creatorId)
        {
            return await _studySetsRepository.GetAllStudySetsByCreator(creatorId);
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySetsForGroup(int groupId)
        {
            return await _studySetsRepository.GetAllStudySetsForGroup(groupId);
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySets()
        { 
            return await _studySetsRepository.GetAllStudySets();
        }

        public async Task<StudySet> CreateStudySet(StudySet studySet)
        {
            return await _studySetsRepository.CreateStudySet(studySet);
        }

        public async Task<StudySet> UpdateStudySet(StudySet studySet)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteStudySet(int id)
        {
            var set = await _studySetsRepository.GetStudySetById(id) ?? throw new StudySetNotFoundException(id.ToString());
            await _studySetsRepository.DeleteStudySet(set);
        }

        public async Task<StudySet> AddFlashcardToStudySet(int studySetId, Flashcard flashcard)
        {
            var set = await _studySetsRepository.GetStudySetById(studySetId) ?? throw new StudySetNotFoundException(studySetId.ToString());
            set.Flashcards.Add(flashcard);
            await _studySetsRepository.UpdateStudySet(set);
            return set;
        }
    }
}
