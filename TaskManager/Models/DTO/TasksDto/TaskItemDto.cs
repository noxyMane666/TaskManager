using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTO
{
    public class TaskItemDto
    {
        [Required(ErrorMessage = "Номер задачи обязателен")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Описание задачи обязательно")]
        public string TaskTitle { get; set; } = string.Empty;
        [Required(ErrorMessage = "Название задачи обязательно")]
        public string TaskDescription { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
    }
}
