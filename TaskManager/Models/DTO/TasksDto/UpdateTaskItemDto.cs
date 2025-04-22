using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTO
{
    public class UpdateTaskItemDto
    {
        [Required(ErrorMessage = "Номер задачи обязателен")]
        public int Id { get; set; }
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
    }
}
