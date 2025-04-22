using TaskManager.Core.Abstractions;
using TaskManager.DTO;

namespace TaskManager.Core.Services;

public class TaskService(ITaskMapper mapper, ITaskRepository taskRepository) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ITaskMapper _taskMapper = mapper;

    public async Task AddTask(TaskItemDto dto)
    {
        var model = _taskMapper.ToModel(dto);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        await _taskRepository.AddTaskAsync(model);
    }

    public async Task DeleteTask(DeleteTaskItemDto dto)
    {
        var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        await _taskRepository.DeleteTaskAsync(model);
    }

    public async Task UpdateTask(UpdateTaskItemDto dto)
    {
        var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        model = _taskMapper.MapUpdates(model, dto);
        await _taskRepository.UpdateTaskAsync(model);
    }

    public async Task UpdateTaskState(UpdateTaskStateDto dto)
    {
        var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        model.ChangeTaskState(dto.IsClosed);
        await _taskRepository.UpdateTaskAsync(model);
    }

    public async Task<IEnumerable<TaskItemDto>> GetUserTasks(bool isClosed)
    {
        var models = await _taskRepository.GetTasksAsync(isClosed);
        var dtoList = _taskMapper.ToDtoList(models);
        
        return dtoList;
    }
}