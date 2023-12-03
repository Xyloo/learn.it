using System.Security.Authentication;
using learn.it.Models;
using System.Security.Claims;
using learn.it.Exceptions;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;

namespace learn.it.Utils
{
    public static class ControllerUtils
    {
        public static void CheckIfValidImage(IFormFile file)
        {
            switch (file.Length)
            {
                case 0:
                    throw new InvalidInputDataException("No file was provided.");
                case > 10 * 1024 * 1024:
                    throw new InvalidInputDataException("The provided file is too large (max 10 MB).");
            }

            // Check the file content type
            if (file.ContentType.ToLower().StartsWith("image/"))
            {
                return;
            }

            // Alternatively, check the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if(!allowedExtensions.Contains(extension))
                throw new InvalidInputDataException("The provided file is not an image.");
        }

        public static bool IsUserAdminOrSelf(User user, ClaimsPrincipal data)
        {
            return data.FindFirst(ClaimTypes.Role)?.Value == "Admin" || data.Identity?.Name == user.Username;
        }

        public static bool IsUserAdmin(ClaimsPrincipal data)
        {
            return data.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        }

        public static int GetUserIdFromClaims(ClaimsPrincipal data)
        {
            var parseSuccessful = int.TryParse(data.FindFirst(ClaimTypes.NameIdentifier)?.Value!, out int creatorId);
            if (!parseSuccessful)
            {
                throw new InvalidCredentialException("Could not parse user id from token");
            }
            return creatorId;
        }

        public static bool CanUserAccessStudySet(User user, StudySet studySet)
        {
            return user.Permissions.Name == "Admin" ||
                   (studySet.Group != null && studySet.Group.Users.Contains(user) && studySet.Visibility == Visibility.Group) ||
                   studySet.Creator.Username == user.Username ||
                   studySet.Visibility == Visibility.Public;
        }

        public static async Task<bool> IsStudySetMastered(
            StudySet studySet,
            User user,
            IFlashcardsService flashcardsService,
            IFlashcardUserProgressService flashcardUserProgressService)
        {
            var progress = (await flashcardUserProgressService
                    .GetFlashcardUserProgressesByUserIdAndStudySetId(user.UserId, studySet.StudySetId))
                .ToList();

            var flashcards = (await flashcardsService.GetFlashcardsInSet(studySet.StudySetId)).ToList();

            return progress.Count == flashcards.Count && progress.All(p => p.IsMastered);
        }

        public static async Task<IEnumerable<User>> GetUsersWhoMasteredStudySet(
            StudySet studySet,
            IFlashcardsService flashcardsService,
            IFlashcardUserProgressService flashcardUserProgressService,
            IUsersService usersService)
        {
            var flashcards = (await flashcardsService.GetFlashcardsInSet(studySet.StudySetId)).ToList();

            var userDtos = new List<AnonymousUserResponseDto>();
            // Initialize users list with users who have mastered the first flashcard
            var firstFlashcardProgress = (await flashcardUserProgressService.GetFlashcardUserProgressesByFlashcardId(flashcards.First().Id)).ToList();
            userDtos.AddRange(firstFlashcardProgress.Where(p => p.IsMastered).Select(p => p.User));

            // Iterate through the remaining flashcards
            foreach (var flashcard in flashcards.Skip(1))
            {
                var progress = (await flashcardUserProgressService.GetFlashcardUserProgressesByFlashcardId(flashcard.Id)).ToList();
                var usersWhoMasteredFlashcard = progress.Where(p => p.IsMastered).Select(p => p.User).ToList();

                // Update users list to only include users who have mastered the current flashcard
                userDtos = userDtos.Intersect(usersWhoMasteredFlashcard).ToList();
            }

            var users = new List<User>();
            foreach (var user in userDtos)
            {
                users.Add(await usersService.GetUserByIdOrUsername(user.Username));
            }
            // Return users who have mastered all flashcards
            return users;
        }

    }
}
