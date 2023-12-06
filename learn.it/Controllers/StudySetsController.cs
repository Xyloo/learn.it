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
        private readonly IAchievementsService _achievementsService;

        public StudySetsController(IStudySetsService studySetsService, IUsersService usersService,
            IGroupsService groupsService, IFlashcardsService flashcardsService, IFlashcardUserProgressService flashcardUserProgressService, IAchievementsService achievementsService)
        {
            _studySetsService = studySetsService;
            _usersService = usersService;
            _groupsService = groupsService;
            _flashcardsService = flashcardsService;
            _flashcardProgressService = flashcardUserProgressService;
            _achievementsService = achievementsService;
        }

        [HttpGet("allSets")]
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
                throw new InvalidInputDataException($"Nie odnaleziono zestawów zawierających frazę [{name}].");

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
                        throw new InvalidInputDataException("Użytkownik nie należy do tej grupy.");
                }
                catch (GroupNotFoundException)
                {
                    group = null;
                }
            }

            if (group is null && studySet.Visibility == Visibility.Group)
                throw new InvalidInputDataException("Nie można utworzyć zestawu o widoczności grupy bez uzupełnienia pola grupy.");

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

            user.UserStats.TotalSetsAdded++;
            await _usersService.UpdateUser(user);

            await _achievementsService.GrantAchievementsContainingPredicate(nameof(UserStats.TotalSetsAdded), user);

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

                var masteredSetUsers = (await ControllerUtils.GetUsersWhoMasteredStudySet(studySetToUpdate,
                    _flashcardsService,
                    _flashcardProgressService, _usersService)).ToList();
                var masteredSetUsersBeforeVisibilityChange = masteredSetUsers.Where(u => ControllerUtils.CanUserAccessStudySet(u, studySetToUpdate)).ToList();

                studySetToUpdate.Name = studySet.Name ?? studySetToUpdate.Name;
                studySetToUpdate.Description = studySet.Description ?? studySetToUpdate.Description;
                studySetToUpdate.Visibility = studySet.Visibility ?? studySetToUpdate.Visibility;
                studySetToUpdate.Group = studySet.GroupId == null
                    ? null
                    : await _groupsService.GetGroupById(studySet.GroupId.Value);
                var updatedStudySet = await _studySetsService.UpdateStudySet(studySetToUpdate);

                var masteredSetUsersAfterVisibilityChange = masteredSetUsers.Where(u => ControllerUtils.CanUserAccessStudySet(u, updatedStudySet)).ToList();

                foreach (var userToDecrement in masteredSetUsersBeforeVisibilityChange.Except(
                             masteredSetUsersAfterVisibilityChange))
                {
                    var userProgresses =
                        await _flashcardProgressService.GetFlashcardUserProgressesByUserIdAndStudySetId(
                            userToDecrement.UserId, updatedStudySet.StudySetId);
                    var masteredFlashcards = userProgresses.Count(p => p.IsMastered);
                    userToDecrement.UserStats.TotalSetsMastered--;
                    userToDecrement.UserStats.TotalFlashcardsMastered -= masteredFlashcards;
                    await _usersService.UpdateUser(userToDecrement);
                }

                foreach (var userToIncrement in masteredSetUsersAfterVisibilityChange.Except(
                             masteredSetUsersBeforeVisibilityChange))
                {
                    var userProgresses =
                        await _flashcardProgressService.GetFlashcardUserProgressesByUserIdAndStudySetId(
                            userToIncrement.UserId, updatedStudySet.StudySetId);
                    var masteredFlashcards = userProgresses.Count(p => p.IsMastered);
                    userToIncrement.UserStats.TotalFlashcardsMastered += masteredFlashcards;
                    userToIncrement.UserStats.TotalSetsMastered++;
                    await _usersService.UpdateUser(userToIncrement);
                    await _achievementsService.GrantAchievementsContainingPredicate(nameof(UserStats.TotalSetsMastered), user);
                    await _achievementsService.GrantAchievementsContainingPredicate(nameof(UserStats.TotalFlashcardsMastered), user);
                }

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
                creator.UserStats.TotalSetsAdded--;
                var masteredSetUsers = await ControllerUtils.GetUsersWhoMasteredStudySet(studySet, _flashcardsService,
                    _flashcardProgressService, _usersService);
                foreach (var masteredSetUser in masteredSetUsers)
                {
                    masteredSetUser.UserStats.TotalSetsMastered--;
                    await _usersService.UpdateUser(masteredSetUser);
                }

                foreach (var flashcard in studySet.Flashcards)
                {
                    var flashcardProgresses =
                        (await _flashcardProgressService.GetFlashcardUserProgressesByFlashcardId(flashcard.FlashcardId)).Where(p => p.IsMastered);
                    foreach (var flashcardProgress in flashcardProgresses)
                    {
                        var userToDecrement = await _usersService.GetUserByIdOrUsername(flashcardProgress.User.Username);
                        userToDecrement.UserStats.TotalFlashcardsMastered--;
                        await _usersService.UpdateUser(userToDecrement);
                    }
                }
                await _usersService.UpdateUser(creator);
                return NoContent();
            }
            throw new StudySetNotFoundException(studySetId);
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicStudySets()
        {
            var sets = await _studySetsService.GetAllStudySets();
            sets = sets.Where(s => s.Visibility == Visibility.Public).Take(6);
            return Ok(sets);
        }
    }
}