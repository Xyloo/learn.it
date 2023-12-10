using System.Security.Authentication;
using learn.it.Models;
using System.Security.Claims;
using learn.it.Exceptions;
using learn.it.Models.Dtos.Response;
using learn.it.Services.Interfaces;
using SixLabors.ImageSharp;

namespace learn.it.Utils
{
    public static class ControllerUtils
    {
        public static void CheckIfValidImage(IFormFile file)
        {
            switch (file.Length)
            {
                case 0:
                    throw new InvalidInputDataException("Nie wysłano pliku.");
                case > 10 * 1024 * 1024:
                    throw new InvalidInputDataException("Wysłany plik jest zbyt duży (maks. 10 MB).");
            }
            try
            {
                var imageInfo = Image.Identify(file.OpenReadStream());
                if(imageInfo == null)
                    throw new InvalidInputDataException("Przesłany plik nie jest obrazem.");
            }
            catch (Exception)
            {
                throw new InvalidInputDataException("Przesłany plik nie jest obrazem.");
            }

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
                //should never happen
                throw new InvalidCredentialException("Uzyskanie id użytkownika z tokenu nie było możliwe.");
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
            if (flashcards.Count == 0)
                return new List<User>();

            var userDtos = new List<AnonymousUserResponseDto>();
            // Initialize users list with users who have mastered the first flashcard
            var firstFlashcardProgress = (await flashcardUserProgressService.GetFlashcardUserProgressesByFlashcardId(flashcards.First().FlashcardId)).ToList();
            if (firstFlashcardProgress.Count == 0)
                return new List<User>();
            userDtos.AddRange(firstFlashcardProgress.Where(p => p.IsMastered).Select(p => p.User));

            // Iterate through the remaining flashcards
            foreach (var flashcard in flashcards.Skip(1))
            {
                var progress = (await flashcardUserProgressService.GetFlashcardUserProgressesByFlashcardId(flashcard.FlashcardId)).ToList();
                if (progress.Count == 0)
                    continue;
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
