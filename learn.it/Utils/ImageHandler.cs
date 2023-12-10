using SixLabors.ImageSharp;

namespace learn.it.Utils
{
    public class ImageHandler : IImageHandler
    {
        public async Task<string> AddImage(IFormFile image, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var imageFile = await Image.LoadAsync(image.OpenReadStream());

            var fileName = $"{Guid.NewGuid()}.webp";
            var filePath = Path.Combine(directoryPath, fileName);
            while (File.Exists(filePath))
            {
                fileName = $"{Guid.NewGuid()}.webp";
                filePath = Path.Combine(directoryPath, fileName);
            }
            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.SaveAsWebpAsync(stream);
            return fileName;
        }
    }
}
