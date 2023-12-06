namespace learn.it.Exceptions.NotFound
{
    public class FlashcardNotFoundException : NotFoundException
    {
        public FlashcardNotFoundException(int flashcardId) : base($"Nie odnaleziono fiszki o id [{flashcardId}].")
        {
        }
    }
}
