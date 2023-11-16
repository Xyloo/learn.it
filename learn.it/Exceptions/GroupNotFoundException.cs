namespace learn.it.Exceptions
{
    public class GroupNotFoundException : Exception
    {
        public GroupNotFoundException(string id) : base($"Group with id: [{id}] was not found.")
        {
        }
    }
}
