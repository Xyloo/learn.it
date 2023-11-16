namespace learn.it.Exceptions
{
    public class StudySetNotFoundException : Exception
    {
        public StudySetNotFoundException(string idOrName) : base($"Study set with id or name: [{idOrName}] was not found.")
        {
        }
    }
}
