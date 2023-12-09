namespace learn.it.Exceptions.NotFound
{
    public class FlashcardUserProgressNotFoundException : NotFoundException
    {
        public FlashcardUserProgressNotFoundException(string? message) : base(message) { }
        public FlashcardUserProgressNotFoundException(int userId, int flashcardId) : base($"Nie odnaleziono postępu użytkownika o id [{userId}] na fiszce o id [{flashcardId}].") { }

    }
}
