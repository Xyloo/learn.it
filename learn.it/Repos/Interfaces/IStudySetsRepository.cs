using learn.it.Models;
using learn.it.Models.Dtos.Response;

namespace learn.it.Repos.Interfaces
{
    public interface IStudySetsRepository
    {
        Task<StudySet> CreateStudySet(StudySet studySet);
        Task DeleteStudySet(StudySet studySet);
        Task<StudySet?> GetStudySetById(int studySetId);
        Task<StudySet?> GetStudySetByName(string studySetName);
        Task<StudySetDto?> GetStudySetDtoById(int studySetId);
        Task<StudySetDto?> GetStudySetDtoByName(string studySetName);
        Task<IEnumerable<BasicStudySetDto>> GetStudySetsContainingName(string studySetName);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsByCreator(int creatorId);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsForGroup(int groupId);
        Task<IEnumerable<BasicStudySetDto>> GetAllStudySets();
        Task<StudySet> UpdateStudySet(StudySet studySet);
    }
}
