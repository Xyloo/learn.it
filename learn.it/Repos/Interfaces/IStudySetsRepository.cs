using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IStudySetsRepository
    {
        Task<StudySet> CreateStudySet(StudySet studySet);
        Task DeleteStudySet(StudySet studySet);
        Task<StudySet?> GetStudySetById(int studySetId);
        Task<StudySet?> GetStudySetByName(string studySetName);
        Task<IEnumerable<StudySet>> GetAllStudySetsByCreator(int creatorId);
        Task<IEnumerable<StudySet>> GetAllStudySetsForGroup(int groupId);
        Task<IEnumerable<StudySet>> GetAllStudySets();
        Task<StudySet> UpdateStudySet(StudySet studySet);
    }
}
