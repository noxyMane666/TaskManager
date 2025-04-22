using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Core.Abstractions;

public interface ITaskRepository
{
    Task AddTaskAsync(TaskItem newTaskItem);
    Task UpdateTaskAsync(TaskItem taskItem);
    Task DeleteTaskAsync(TaskItem taskItem);
    Task<TaskItem?> GetTaskByIdAsync(int taskId);
    Task<IEnumerable<TaskItem>> GetTasksAsync(bool isClosed);
}