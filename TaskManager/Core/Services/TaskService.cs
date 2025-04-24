using System.Security.Claims;
using TaskManager.Core.Abstractions;
using TaskManager.DTO;
using TaskManager.Exceptions;
using TaskManager.Models;

namespace TaskManager.Core.Services;

public class TaskService(
    ITaskMapper mapper, 
    ITaskRepository taskRepository,
    IAccountService accountService) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ITaskMapper _taskMapper = mapper;
    private readonly IAccountService _accountService = accountService;

    public async Task AddTask(TaskItemDto dto, ClaimsPrincipal claims)
    {
        
        var userId = await _accountService.GetUserIdAsync(claims);
        var model = _taskMapper.ToModel(dto, userId); 
        
        await _taskRepository.AddTaskAsync(model);
    }

    public async Task DeleteTask(DeleteTaskItemDto dto)
    {
         var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new TaskNotFoundException($"Задача {dto.Id }не найдена");
        }
        
        await _taskRepository.DeleteTaskAsync(model);
    }

    public async Task UpdateTask(UpdateTaskItemDto dto)
    {
        var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new TaskNotFoundException($"Задача {dto.Id}не найдена");
        }
        
        model = _taskMapper.MapUpdates(model, dto);
        await _taskRepository.UpdateTaskAsync(model);
    }

    public async Task UpdateTaskState(UpdateTaskStateDto dto)
    {
        var model = await _taskRepository.GetTaskByIdAsync(dto.Id);

        if (model is null)
        {
            throw new TaskNotFoundException($"Задача {dto.Id}не найдена");
        }
        
        model.ChangeTaskState(dto.IsClosed);
        await _taskRepository.UpdateTaskAsync(model);
    }

    public async Task<IEnumerable<TaskItemDto>> GetUserTasks(bool isClosed, ClaimsPrincipal claims)
    {
        var userId = await _accountService.GetUserIdAsync(claims);
        var models = await _taskRepository.GetTasksAsync(isClosed, userId);
        var dtoList = _taskMapper.ToDtoList(models);
        
        return dtoList;
    }
}