using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Core.Abstractions
{
    public interface ITaskMapper
    {
        public TaskItem ToModel(TaskItemDto dto, int userId);
        public TaskItemDto ToGetDto(TaskItem model);
        public TaskItem MapUpdates(TaskItem model, UpdateTaskItemDto dto);
        public IEnumerable<TaskItemDto> ToDtoList(IEnumerable<TaskItem> models);

    }
}
