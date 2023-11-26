namespace learn.it.Exceptions.NotFound
{
    public class StudySetNotFoundException : NotFoundException
    {
        public StudySetNotFoundException(string idOrName) : base($"Study set with id or name: [{idOrName}] was not found.")
        {
        }

        public StudySetNotFoundException(int id) : base($"Study set with id: [{id}] was not found.")
        {
        }
    }
}
