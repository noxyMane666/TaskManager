namespace TaskManager.Exceptions
{
    public class TaskNotFoundException(string message) : NullReferenceException(message)
    {
    }
}
