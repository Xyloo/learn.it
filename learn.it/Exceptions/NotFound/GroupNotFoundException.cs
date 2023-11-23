namespace learn.it.Exceptions.NotFound
{
    public class GroupNotFoundException : NotFoundException
    {
        public GroupNotFoundException(string id) : base($"Group with id: [{id}] was not found.")
        {
        }
    }
}
