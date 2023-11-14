using learn.it.Models;

namespace learn.it.Services.Interfaces
{
    public interface IStudySetsService
    {
        Task<StudySet> GetStudySetById(int id);
        Task<StudySet> GetStudySetByName(string name);
        Task<StudySet> AddFlashcardToStudySet(int studySetId, Flashcard flashcard);
        Task<IEnumerable<StudySet>> GetStudySetsContainingName(string name);
        Task<IEnumerable<StudySet>> GetAllStudySetsByCreator(int creatorId);
        Task<IEnumerable<StudySet>> GetAllStudySetsForGroup(int groupId);
        Task<IEnumerable<StudySet>> GetAllStudySets();
        Task<StudySet> CreateStudySet(StudySet studySet);
        Task<StudySet> UpdateStudySet(StudySet studySet);
        Task DeleteStudySet(int id);
    }
}
