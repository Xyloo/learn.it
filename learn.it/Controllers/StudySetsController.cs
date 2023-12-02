using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using learn.it.Exceptions;

namespace learn.it.Controllers
{
    [ApiController]
    [Route("api/studysets")]
    public class StudySetsController : ControllerBase
    {
        private readonly IStudySetsService _studySetsService;

        private readonly IUsersService _usersService;

        private readonly IGroupsService _groupsService;
        private readonly IFlashcardsService _flashcardsService;
        private readonly IFlashcardUserProgressService _flashcardProgressService;

        public StudySetsController(IStudySetsService studySetsService, IUsersService usersService,
            IGroupsService groupsService, IFlashcardsService flashcardsService, IFlashcardUserProgressService flashcardUserProgressService)
        {
            _studySetsService = studySetsService;
            _usersService = usersService;
            _groupsService = groupsService;
            _flashcardsService = flashcardsService;
            _flashcardProgressService = flashcardUserProgressService;
        }

        [HttpGet]
        [Authorize(Policy = "Admins")]
        public async Task<IActionResult> GetAllStudySets()
        {
            var studySets = await _studySetsService.GetAllStudySets();
            return Ok(studySets);
        }

        [HttpGet("{studySetId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> GetStudySetDetails([FromRoute] int studySetId)
        {
            var studySetDto = await _studySetsService.GetStudySetDtoById(studySetId);
            var studySet = await _studySetsService.GetStudySetById(studySetId);
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            //if user is an admin, or:
            //the set is private - but the user is the creator, return the set
            //the set belongs to a group - but the user is in the group, return the set
            //the set is public - return the set
            if (ControllerUtils.CanUserAccessStudySet(user, studySet))
            {
                return Ok(studySetDto);
            }
            //this is a 404 because we don't want to leak the existence of the study set
            //actually recommended by HTTP Specification
            throw new StudySetNotFoundException(studySetId);
        }

        [HttpGet("find/{name}")]
        public async Task<IActionResult> GetStudySetsContainingName([FromRoute] string name)
        {
            var studySets = (await _studySetsService.GetStudySetsContainingName(name)).ToList();
            if (studySets.Count == 0)
                throw new InvalidInputDataException($"No study sets containing [{name}] found.");

            User? user;
            try
            {
                var userId = ControllerUtils.GetUserIdFromClaims(User); //throws InvalidCredentialException
                user = await _usersService.GetUserByIdOrUsername(userId.ToString());
            }
            catch (InvalidCredentialException)
            {
                user = null;
            }

            if (user is null)
                return Ok(studySets.Where(s => s.Visibility == Visibility.Public));

            if (ControllerUtils.IsUserAdmin(User))
                return Ok(studySets);

            var studySetsToReturn = new List<BasicStudySetDto>();
            foreach (var studySet in studySets)
            {
                if (studySet.Visibility == Visibility.Public)
                    studySetsToReturn.Add(studySet);
                else if (studySet.Visibility == Visibility.Group)
                {
                    var group = await _groupsService.GetGroupById((int)studySet.Group?.GroupId!);
                    if (await _groupsService.IsUserInGroup(user.UserId, group.GroupId))
                        studySetsToReturn.Add(studySet);
                }
                else if (studySet.Creator.Username == user.Username)
                    studySetsToReturn.Add(studySet);
            }

            return Ok(studySetsToReturn);
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> CreateStudySet([FromBody] CreateStudySetDto studySet)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            Group? group;
            if (studySet.GroupId == null)
                group = null;
            else
            {
                try
                {
                    group = await _groupsService.GetGroupById(studySet.GroupId.Value);
                    if (!await _groupsService.IsUserInGroup(user.UserId, group.GroupId))
                        throw new InvalidInputDataException("User is not a member of the group.");
                }
                catch (GroupNotFoundException)
                {
                    group = null;
                }
            }

            if (group is null && studySet.Visibility == Visibility.Group)
                throw new InvalidInputDataException("Cannot create a group study set without a group.");

            var newStudySet = new StudySet()
            {
                Name = studySet.Name,
                Description = studySet.Description,
                Visibility = studySet.Visibility,
                Creator = user,
                Group = group
            };
            var createdStudySet = await _studySetsService.CreateStudySet(newStudySet);
            //this technically isn't necessary, but EF Core might not update the group's study sets
            //it ensures consistency
            group?.StudySets.Add(createdStudySet);

            user.UserStats.SetsAdded++;
            await _usersService.UpdateUser(user);

            return CreatedAtAction(nameof(GetStudySetDetails), new { studySetId = createdStudySet.StudySetId },
                               new StudySetDto(createdStudySet));
        }

        [HttpPut("{studySetId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> UpdateStudySet([FromRoute] int studySetId,
            [FromBody] UpdateStudySetDto studySet)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var studySetToUpdate = await _studySetsService.GetStudySetById(studySetId);

            if (ControllerUtils.IsUserAdmin(User) || studySetToUpdate.Creator.Username == user.Username)
            {
                var validationContext = new ValidationContext(studySet);
                var validationResults = studySet.Validate(validationContext);
                if (validationResults.Any())
                    throw new InvalidInputDataException(validationResults.ToString());

                studySetToUpdate.Name = studySet.Name ?? studySetToUpdate.Name;
                studySetToUpdate.Description = studySet.Description ?? studySetToUpdate.Description;
                studySetToUpdate.Visibility = studySet.Visibility ?? studySetToUpdate.Visibility;
                studySetToUpdate.Group = studySet.GroupId == null
                    ? null
                    : await _groupsService.GetGroupById(studySet.GroupId.Value);
                var updatedStudySet = await _studySetsService.UpdateStudySet(studySetToUpdate);
                return Ok(new StudySetDto(updatedStudySet));
            }

            throw new StudySetNotFoundException(studySetId);
        }

        [HttpDelete("{studySetId}")]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> DeleteStudySet([FromRoute] int studySetId)
        {
            var user = await _usersService.GetUserByIdOrUsername(ControllerUtils.GetUserIdFromClaims(User).ToString());
            var studySet = await _studySetsService.GetStudySetById(studySetId);
            var creator = await _usersService.GetUserByIdOrUsername(studySet.Creator.UserId.ToString());

            if (ControllerUtils.IsUserAdmin(User) || studySet.Creator.Username == user.Username)
            {
                await _studySetsService.DeleteStudySet(studySet.StudySetId);
                creator.UserStats.SetsAdded--;
                var masteredSetUsers = await ControllerUtils.GetUsersWhoMasteredStudySet(studySet, _flashcardsService,
                    _flashcardProgressService, _usersService);
                foreach (var masteredSetUser in masteredSetUsers)
                {
                    masteredSetUser.UserStats.SetsCompleted--;
                    await _usersService.UpdateUser(masteredSetUser);
                }
                await _usersService.UpdateUser(creator);
                return NoContent();
            }
            throw new StudySetNotFoundException(studySetId);
        }
    }
}