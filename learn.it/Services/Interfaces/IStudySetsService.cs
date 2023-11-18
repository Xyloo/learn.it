using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Services.Interfaces
{
    public interface IStudySetsService
    {
        Task<StudySetDto> GetStudySetById(int id);
        Task<StudySetDto> GetStudySetByName(string name);
        Task<StudySet> AddFlashcardToStudySet(int studySetId, Flashcard flashcard);
        Task<StudySet> RemoveFlashcardFromStudySet(int studySetId, Flashcard flashcard);
        Task<IEnumerable<BasicStudySetDto>> GetStudySetsContainingName(string name);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsByCreator(int creatorId);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsForGroup(int groupId);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySets();
        Task<StudySet> CreateStudySet(StudySet studySet);
        Task<StudySet> UpdateStudySet(StudySet studySet);
        Task DeleteStudySet(int id);
    }
}
