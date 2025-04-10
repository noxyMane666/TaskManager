using System.ComponentModel.DataAnnotations;

namespace TaskManager.DTO
{
    public class UpdateTaskStateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Статус обязателен")]
        public bool IsClosed { get; set; }
    }
}
