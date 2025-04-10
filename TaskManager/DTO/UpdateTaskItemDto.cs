namespace TaskManager.DTO
{
    public class UpdateTaskItemDto
    {
        public int Id { get; set; }
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
    }
}
