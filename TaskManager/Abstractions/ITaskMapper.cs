using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Abstractions
{
    public interface ITaskMapper
    {
        public TaskItem ToModel(TaskItemDto dto);
        public TaskItemDto ToGetDto(TaskItem model);
        public void MapUpdates(TaskItem model, UpdateTaskItemDto dto);

    }
}
