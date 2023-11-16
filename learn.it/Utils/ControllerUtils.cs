using System.Security.Authentication;
using learn.it.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Utils
{
    public static class ControllerUtils
    {
        public static bool IsImage(IFormFile file)
        {
            // Check the file content type
            if (file.ContentType.ToLower().StartsWith("image/"))
            {
                return true;
            }

            // Alternatively, check the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            return allowedExtensions.Contains(extension);
        }

        public static bool IsUserAdminOrSelf(User user, ClaimsPrincipal data)
        {
            return data.FindFirst(ClaimTypes.Role)?.Value == "Admin" || data.Identity?.Name == user.Username;
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
    }
}
