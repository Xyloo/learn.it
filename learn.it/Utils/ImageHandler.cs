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

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(directoryPath, fileName);
            while (File.Exists(filePath))
            {
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                filePath = Path.Combine(directoryPath, fileName);
            }
            await using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);
            return fileName;
        }
    }
}
