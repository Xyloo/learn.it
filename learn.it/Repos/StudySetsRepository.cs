using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Repos
{
    public class StudySetsRepository : IStudySetsRepository
    {
        private readonly LearnitDbContext _context;

        public StudySetsRepository(LearnitDbContext context)
        {
            _context = context;
        }

        public async Task<StudySet> CreateStudySet(StudySet studySet)
        {
            await _context.StudySets.AddAsync(studySet);
            await _context.SaveChangesAsync();
            return studySet;
        }

        public async Task DeleteStudySet(StudySet studySet)
        {
            _context.StudySets.Remove(studySet);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySets()
        {
            return await _context.StudySets.Include(g => g.Creator.ToAnonymousUserResponseDto()).ToListAsync();
        }

        public async Task<IEnumerable<StudySet>> GetStudySetsContainingName(string studySetName)
        {
            return await _context.StudySets.Where(g => g.Name.Contains(studySetName)).ToListAsync();
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySetsByCreator(int creatorId)
        {
            return await _context.StudySets.Where(g => g.Creator.UserId == creatorId).ToListAsync();
        }

        public async Task<IEnumerable<StudySet>> GetAllStudySetsForGroup(int groupId)
        {
            return await _context.StudySets.Where(g => g.Group.GroupId == groupId).Include(g => g.Creator.ToAnonymousUserResponseDto()).ToListAsync();
        }

        public async Task<StudySet?> GetStudySetById(int studySetId)
        {
            return await _context.StudySets
                .Where(g => g.StudySetId == studySetId)
                .Include(g => g.Creator.ToAnonymousUserResponseDto())
                .Include(g => new BasicGroupDto(g.Group))
                .Include(g => g.Flashcards)
                .FirstOrDefaultAsync();
        }

        public async Task<StudySet?> GetStudySetByName(string studySetName)
        {
            return await _context.StudySets
                .Where(g => g.Name == studySetName)
                .Include(g => g.Creator.ToAnonymousUserResponseDto())
                .Include(g => new BasicGroupDto(g.Group))
                .Include(g => g.Flashcards)
                .FirstOrDefaultAsync();
        }

        public async Task<StudySet> UpdateStudySet(StudySet studySet)
        {
            _context.StudySets.Update(studySet);
            await _context.SaveChangesAsync();
            return studySet;
        }
    }
}
