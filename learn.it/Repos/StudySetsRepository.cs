using learn.it.Models;
using learn.it.Models.Dtos.Response;
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

        public async Task<IEnumerable<BasicStudySetDto>> GetAllStudySets()
        {
            return await _context.StudySets
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Select(g => new BasicStudySetDto(g))
                .ToListAsync();
        }

        public async Task<IEnumerable<BasicStudySetDto>> GetStudySetsContainingName(string studySetName)
        {
            return await _context.StudySets.Where(g => g.Name.Contains(studySetName))
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Select(g => new BasicStudySetDto(g))
                .ToListAsync();
        }

        public async Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsByCreator(int creatorId)
        {
            return await _context.StudySets
                .Where(g => g.Creator.UserId == creatorId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Select(g => new BasicStudySetDto(g))
                .ToListAsync();
        }

        public async Task<IEnumerable<BasicStudySetDto>> GetAllStudySetsForGroup(int groupId)
        {
            return await _context.StudySets
                .Where(g => g.Group != null && g.Group.GroupId == groupId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Select(g => new BasicStudySetDto(g))
                .ToListAsync();
        }

        public async Task<StudySet?> GetStudySetById(int studySetId)
        {
            var set = await _context.StudySets
                .Where(g => g.StudySetId == studySetId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Include(g => g.Flashcards)
                .FirstOrDefaultAsync();
            if (set != null && set.Group != null)
            {
                set.Group = await _context.Groups
                    .Where(g => g.GroupId == set.Group.GroupId)
                    .Include(g => g.Creator)
                    .Include(g => g.Users)
                    .FirstOrDefaultAsync();
            }
            return set;
        }

        public async Task<StudySet?> GetStudySetByName(string studySetName)
        {
            return await _context.StudySets
                .Where(g => g.Name == studySetName)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Include(g => g.Flashcards)
                .FirstOrDefaultAsync();
        }

        public async Task<StudySetDto?> GetStudySetDtoById(int studySetId)
        {
            return await _context.StudySets
                .Where(g => g.StudySetId == studySetId)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Include(g => g.Flashcards)
                .Select(g => new StudySetDto(g))
                .FirstOrDefaultAsync();
        }

        public async Task<StudySetDto?> GetStudySetDtoByName(string studySetName)
        {
            return await _context.StudySets
                .Where(g => g.Name == studySetName)
                .Include(g => g.Creator)
                .Include(g => g.Group)
                .Include(g => g.Flashcards)
                .Select(g => new StudySetDto(g))
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
