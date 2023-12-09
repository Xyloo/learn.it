namespace learn.it.Utils
{
    public interface IImageHandler
    {
        public Task<string> AddImage(IFormFile image, string directoryPath);
    }
}
