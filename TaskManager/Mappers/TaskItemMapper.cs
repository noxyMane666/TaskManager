using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Mappers
{
    public static class TaskItemMapper
    {
        public static TaskItem ToModel (TaskItemDto dto)
        {
            return new TaskItem
            {
                Id = dto.Id,
                TaskTitle = dto.TaskTitle,
                TaskDescription = dto.TaskDescription,
                IsClosed = dto.IsClosed
            };
        }

        public static TaskItemDto ToGetDto(TaskItem model)
        {
            return new TaskItemDto
            {
                Id = model.Id,
                TaskTitle = model.TaskTitle,
                TaskDescription = model.TaskDescription,
                IsClosed = model.IsClosed
            };
        }
    }
}
