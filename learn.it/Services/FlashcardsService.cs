using learn.it.Models;
using learn.it.Models.Dtos.Response;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;
using learn.it.Exceptions.NotFound;
using learn.it.Utils;

namespace learn.it.Services
{
    public class FlashcardsService : IFlashcardsService
    {
        private readonly IFlashcardsRepository _flashcardsRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageHandler _imageHandler;
        private const string FlashcardImagesFolder = "FlashcardImages";

        public FlashcardsService(IFlashcardsRepository flashcardsRepository, IWebHostEnvironment webHostEnvironment, IImageHandler imageHandler)
        {
            _flashcardsRepository = flashcardsRepository;
            _webHostEnvironment = webHostEnvironment;
            _imageHandler = imageHandler;
        }

        public async Task<IEnumerable<FlashcardDto>> GetFlashcardsInSet(int studySetId)
        {
            var flashcards = await _flashcardsRepository.GetFlashcardsInSet(studySetId);
            return flashcards.Select(f => new FlashcardDto(f)).ToList();
        }

        public async Task<FlashcardDto> GetFlashcardDto(int id)
        {
            var flashcard = await _flashcardsRepository.GetFlashcard(id) ?? throw new FlashcardNotFoundException(id);
            return new FlashcardDto(flashcard);
        }

        public async Task<Flashcard> GetFlashcard(int id)
        {
            return await _flashcardsRepository.GetFlashcard(id) ?? throw new FlashcardNotFoundException(id);
        }

        public async Task<Flashcard> AddFlashcard(Flashcard flashcard)
        {
            return await _flashcardsRepository.AddFlashcard(flashcard);
        }

        public async Task<Flashcard> AddImageFlashcard(Flashcard flashcard, IFormFile image)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, FlashcardImagesFolder);
            var filename = await _imageHandler.AddImage(image, path);
            flashcard.Term = filename;
            return await _flashcardsRepository.AddFlashcard(flashcard);
        }

        public async Task<Flashcard> UpdateFlashcard(Flashcard flashcard)
        {
            return await _flashcardsRepository.UpdateFlashcard(flashcard);
        }

        public async Task<Flashcard> UpdateToImageFlashcard(Flashcard flashcard, IFormFile image)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, FlashcardImagesFolder);
            var filename = await _imageHandler.AddImage(image, path);
            flashcard.Term = filename;
            return await _flashcardsRepository.UpdateFlashcard(flashcard);
        }

        public async Task RemoveImage(Flashcard flashcard)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, FlashcardImagesFolder, flashcard.Term);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            flashcard.Term = string.Empty;
            await _flashcardsRepository.UpdateFlashcard(flashcard);
        }

        public async Task RemoveFlashcard(int id)
        {
            await _flashcardsRepository.RemoveFlashcard(id);
        }
    }
}
