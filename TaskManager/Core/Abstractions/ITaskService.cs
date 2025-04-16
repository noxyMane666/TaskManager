using TaskManager.DTO;

namespace TaskManager.Core.Abstractions;

public interface ITaskService
{
    Task AddTask(TaskItemDto taskItem);
    Task DeleteTask(DeleteTaskItemDto taskItem);
    Task UpdateTask(UpdateTaskItemDto taskItem);
    Task UpdateTaskState(UpdateTaskStateDto dto);
    Task<IEnumerable<TaskItemDto>> GetUserTasks(bool isClosed);
}