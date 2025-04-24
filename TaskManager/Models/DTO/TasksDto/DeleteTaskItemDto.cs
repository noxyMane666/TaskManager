using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTO
{
    public class DeleteTaskItemDto
    {
        [Required(ErrorMessage = "Номер задачи обязателен")]
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
