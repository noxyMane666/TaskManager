using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название задачи обязательно")]
        public string TaskTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Текст задачи обязателен")]
        public string TaskDescription { get; set; } = string.Empty;
        public bool IsClosed { get; set; } = false;
    }
}
