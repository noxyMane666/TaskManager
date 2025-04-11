using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.DTO;
using TaskManager.Mappers;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController(ILogger<TasksController> logger, AppDbContext baseContext) : Controller
    {
        private readonly ILogger<TasksController> _logger = logger;
        private readonly AppDbContext _baseContext = baseContext;

        [HttpGet]
        public async Task<IActionResult> MyTasks(bool isClosed)
        {
            try
            {
                var userTasks = await _baseContext.Tasks
                    .Where(t => t.IsClosed == isClosed)
                    .Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        TaskTitle = t.TaskTitle,
                        TaskDescription = t.TaskDescription,
                        IsClosed = t.IsClosed
                    })
                    .ToListAsync();

                return View(userTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, "Данная страница сейчас недосупна. ");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskItemDto requestTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _baseContext.Tasks.AnyAsync(t => t.Id == requestTask.Id))
                    {
                        return Json((
                            success: false, 
                            error: $"Задача с ID {requestTask.Id} уже есть"));
                    }

                    var taskModel = TaskItemMapper.ToModel(requestTask);
                    _baseContext.Tasks.Add(taskModel);
                    await _baseContext.SaveChangesAsync();

                    return Ok(new { 
                        success = true, 
                        updatedTask = requestTask
                    });
                }
                else
                {
                    var errors = ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    _logger.LogWarning("Не удалось валидировать параметры: {@Errors}", errors);
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При создании задачи произошла ошибка: {ex.Message})");
                return StatusCode(500, $"При создании задачи произошла ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskState([FromBody] UpdateTaskStateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                                    .Where(e => e.Value?.Errors.Count > 0)
                                    .ToDictionary(
                                        kvp => kvp.Key,
                                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                                    );

                    _logger.LogError("Ошибка валидации при добавлении задачи: {@Errors}", errors);
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var taskModel = await GetTaskById(dto.Id);

                if (taskModel is null)
                {
                    return StatusCode(404, $"Задача с id {dto.Id} не найдена");
                }

                taskModel.ChangeTaskState(dto.IsClosed);
                await _baseContext.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    updatedTask = dto 
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления статуса задачи {dto.Id}", (dto.Id));
                return StatusCode(500, $"При обновлении статуса задачи произошла ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    _logger.LogError("Ошибка валидации при добавлении задачи: {@Errors}", errors);
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var taskModel = await GetTaskById(dto.Id);

                if (taskModel is null)
                {
                    return StatusCode(404, $"Задача с id {dto.Id} не найдена");
                }

                TaskItemMapper.MapUpdates(taskModel, dto);
                await _baseContext.SaveChangesAsync();
                
                return Ok(new { 
                    success = true, 
                    updatedTask = dto 
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления задачи {dto.Id}", (dto.Id));
                return StatusCode(500, $"При обновлении задачи произошла ошибка: {ex.Message}");
            }
        }

        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var taskModel = await GetTaskById(id);

                if (taskModel is null)
                {
                    return StatusCode(404, $"Задача с id {id} не найдена");
                }
                
                _baseContext.Tasks.Remove(taskModel);
                await _baseContext.SaveChangesAsync();
                
                return Ok(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления задачи {id}", (id));
                return StatusCode(500, $"При удалении задачи произошла ошибкаЖ: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        private async Task<TaskItem?> GetTaskById(int id)
        {
            return await _baseContext.Tasks.FindAsync(id);
        }
    }
}
