namespace learn.it.Exceptions.NotFound
{
    public class AnswerNotFoundException : NotFoundException
    {
        public AnswerNotFoundException(int id) : base($"Answer with id [{id}] was not found.")
        {
        }
    }
}
